using UnityEngine;
using System.Collections.Generic;

public static class DetectionStateManager
{
    // Per observer-target pair detection state.
    // Missing pairs are treated as Detected (open world assumption).
    private static Dictionary<(Unit observer, Unit target), DetectionState> detectionStates
        = new Dictionary<(Unit observer, Unit target), DetectionState>();

    // Last known position recorded by intelligent observers when a target
    // transitions from Detected to Hidden or Imprecise.
    private static Dictionary<(Unit observer, Unit target), LastKnownData> lastKnownPositions
        = new Dictionary<(Unit observer, Unit target), LastKnownData>();

    // Per-intelligent-enemy remaining alert turns after reaching last known
    // position without confirming the target.
    private static Dictionary<Unit, int> alertCountdowns
        = new Dictionary<Unit, int>();

    //Initialisation

    // Called by CombatManager.StartCombat() before the first turn.
    // Units in hiddenUnits are pre-initialised as Hidden vs all enemies
    // (used when entering combat from exploration stealth).
    public static void Initialize(List<Unit> allUnits, List<Unit> hiddenUnits = null)
    {
        Reset();

        if (hiddenUnits == null) return;

        foreach (Unit hidden in hiddenUnits)
        {
            foreach (Unit other in allUnits)
            {
                if (other == hidden) continue;
                if (other.isEnemy == hidden.isEnemy) continue;

                detectionStates[(other, hidden)] = DetectionState.Hidden;
            }
        }
    }

    //Core get/set
    public static DetectionState Get(Unit observer, Unit target)
    {
        if (detectionStates.TryGetValue((observer, target), out DetectionState state))
            return state;

        return DetectionState.Detected;
    }

    public static void Set(Unit observer, Unit target, DetectionState state)
    {
        DetectionState previous = Get(observer, target);

        detectionStates[(observer, target)] = state;

        //Record last known position when an intelligent observer loses a
        //confirmed target (Detected -> Hidden/Imprecise).
        if (previous == DetectionState.Detected &&
            (state == DetectionState.Hidden || state == DetectionState.Imprecise))
        {
            bool isIntelligent = observer.sourceCharacterData == null ||
                !observer.sourceCharacterData.IsMindless;

            if (isIntelligent)
            {
                lastKnownPositions[(observer, target)] = new LastKnownData(
                    target.position,
                    0 //turnRecorded: CombatManager turn counter hookup deferred to Step 11)
                );
                alertCountdowns[observer] = AlertDuration;
                AudioEventBus.Fire(AudioEventIds.Stealth.EnemyAlerted);
            }
        }
    }

    //Targeting gate

    //returns true if the attacker can directly target the given unit.
    //Hidden and Imprecise targets cannot be directly targeted.
    public static bool CanTarget(Unit attacker, Unit target)
    {
        DetectionState state = Get(attacker, target);
        return state == DetectionState.Detected || state == DetectionState.Exposed;
    }

    //LoS gate for re-stealth

    //Returns true if any unit in enemies list has LoS to the given unit.
    //Used as the positional gate before mid-combat Hide attempt
    public static bool HasAnyLoS(Unit unit, List<Unit> enemies)
    {
        foreach (Unit enemy in enemies)
        {
            if (enemy.IsDead()) continue;

            if (LoSResolver.HasLoS(enemy.position, unit.position, GridManager.Instance))
                return true;
        }

        return false;
    }

    // Called by HideEffect. Attempts to re-enter stealth during combat.
    // Requires the unit to be out of LoS of all enemies unless
    public static void AttemptRestealth(Unit unit, List<Unit> enemies)
    {
        //LoS gate: unit must not be visible to any enemy.
        if (HasAnyLoS(unit, enemies))
        {
            CombatLog.Add($"{unit.unitName} attempts to hide — still in sight!");
            return;
        }

        bool anySuccess = false;
        bool anyFailure = false;

        foreach (Unit enemy in enemies)
        {
            if (enemy.IsDead()) continue;

            int stealthTotal = SkillResolver.GetSkillTotal(unit, SkillType.Stealth);
            int passivePerception = GetPassivePerception(enemy);

            //Stealth is rolled opposed against each enemy's passive Perception.
            //Ties go to the defender (observer), consistent with ResolveOpposed().
            int stealthRoll = Random.Range(1, 21) + stealthTotal;
            bool hidden = stealthRoll > passivePerception;

            if (hidden)
            {
                Set(enemy, unit, DetectionState.Hidden);
                AudioEventBus.Fire(AudioEventIds.Stealth.EnterStealth);
                anySuccess = true;
            }
            else
            {
                int degreesOfFailure = passivePerception - stealthRoll;

                if (degreesOfFailure >= LoudFailureThreshold)
                {
                    ApplyLoudFailure(unit, enemies);
                    return;
                }

                Set(enemy, unit, DetectionState.Detected);
                anyFailure = true;
            }

            Debug.Log($"{unit.unitName} hide vs {enemy.unitName}: " +
                      $"roll {stealthRoll} vs passive {passivePerception} — " +
                      $"{(hidden ? "Hidden" : "Detected")}");
        }

        //Log result summary
        if (anySuccess && !anyFailure)
            CombatLog.Add($"{unit.unitName} slips into the shadows.");
        else if (anySuccess && anyFailure)
            CombatLog.Add($"{unit.unitName} partially hides — some enemies remain aware.");
        else
            CombatLog.Add($"{unit.unitName} fails to hide.");
    }

    //Last known position
    public static Vector2Int? GetLastKnownPosition(Unit observer, Unit target)
    {
        if (lastKnownPositions.TryGetValue((observer, target), out LastKnownData data))
            return data.position;

        return null;
    }

    //Passive sweep
    public static void RunPassiveSweep(List<Unit> observers, List<Unit> targets)
    {
        foreach (Unit observer in observers)
        {
            if (observer.IsDead()) continue;

            int passivePerception = GetPassivePerception(observer);

            foreach (Unit target in targets)
            {
                if (target.IsDead()) continue;

                DetectionState current = Get(observer, target);

                //Sweep only promotes Hidden/Imprecise -> Detected.
                //Transitions the other direction happen only through
                //explicit re-stealth attmepts
                if (current != DetectionState.Hidden && current != DetectionState.Imprecise) continue;

                //LoS is required for passive detection to succeed.
                if (!LoSResolver.HasLoS(observer.position, target.position, GridManager.Instance)) continue;

                int stealthTotal = SkillResolver.GetSkillTotal(target, SkillType.Stealth);

                if (passivePerception > stealthTotal)
                {
                    Set(observer, target, DetectionState.Detected);
                    CombatLog.Add($"{observer.unitName} spots {target.unitName}!");
                }
            }
        }
    }

    // Returns the passive Perception score: 10 + full Perception skill total.
    // Adds +5 when the observer is in Alert state (alertCountdowns entry is active).
    // Falls back to 10 for units with no CharacterData.
    private static int GetPassivePerception(Unit unit)
    {
        int base_ = 10;

        if (alertCountdowns.TryGetValue(unit, out int remaining) && remaining > 0)
            base_ += 5;

        if (unit.sourceCharacterData == null) return base_;

        return base_ + SkillResolver.GetSkillTotal(unit, SkillType.Perception);
    }

    //Stealth break evaluation
    public static void EvaluateBreak(Unit unit, ActionDefinition action)
    {
        //Only matters if the unit is currently hidden or imprecise from at least one observer
        //Check against all units on the opposing side.
        bool isHiddenFromAnyone = false;

        //We don't have direct reference to all units here, so we scan the
        //detectionStates table for any pair where this unit is the target
        //and the state os Hidden or Imprecise
        foreach (var kvp in detectionStates)
        {
            if (kvp.Key.target == unit &&
                (kvp.Value == DetectionState.Hidden || kvp.Value == DetectionState.Imprecise))
            {
                isHiddenFromAnyone = true;
                break;
            }
        }

        if (!isHiddenFromAnyone) return;

        //Determine which break condition this action triggers, if any.
        StealthBreakCondition? condition = GetBreakCondition(action);

        if (condition == null) return;

        //Check if the unit's equipped feats suppress this break condition.
        if (IsSuppressedByFeat(unit, condition.Value)) return;

        //Check if the action itself suppress its own break condition.
        if (action.stealthBreakProfile != null &&
            action.stealthBreakProfile.Suppresses(condition.Value)) return;

        //Break confirmed: set Exposed for all observers this unit was Hidden/Imprecise from/
        ExposeUnit(unit);

        CombatLog.Add($"{unit.unitName} breaks stealth!");
        AudioEventBus.Fire(AudioEventIds.Stealth.StealthBroken);
    }

    //Maps an ActionDefiniton to a StealthBreakCondition.
    //Returns null if the action has no default break condition.
    private static StealthBreakCondition? GetBreakCondition(ActionDefinition action)
    {
        switch (action.attackType)
        {
            case ActionDefinition.AttackType.Melee:
                return StealthBreakCondition.MeleeAttack;

            case ActionDefinition.AttackType.Ranged:
                return StealthBreakCondition.RangedAttack;
        }

        //Psionic actions are identified by type rather than attack type.
        if (action is PsionicPowerDefinition)
            return StealthBreakCondition.ManifestPower;

        return null;
    }

    //Returns true if any of the unit's equipped feats suppress the given condition.
    private static bool IsSuppressedByFeat(Unit unit, StealthBreakCondition condition)
    {
        if (unit.sourceCharacterData == null) return false;

        foreach (string featId in unit.sourceCharacterData.equippedFeatIds)
        {
            FeatDefinition feat = FeatDatabase.Get(featId);
            if (feat == null) continue;

            if (feat.stealthBreakProfile != null &&
                feat.stealthBreakProfile.Suppresses(condition))
                return true;
        }

        return false;
    }

    //Sets all Hidden/Imprecise states for this unit to Exposed
    private static void ExposeUnit(Unit unit)
    {
        //Collect keys first to avoid modifying the dictionary during iteration.
        List<(Unit observer, Unit target)> toUpdate = new List<(Unit observer, Unit target)> ();

        foreach (var kvp in detectionStates)
        {
            if (kvp.Key.target == unit &&
                (kvp.Value == DetectionState.Hidden || kvp.Value == DetectionState.Imprecise))
            {
                toUpdate.Add(kvp.Key);
            }
        }

        foreach (var key in toUpdate)
        {
            detectionStates[key] = DetectionState.Exposed;
        }
    }

    //Loud failure

    // Applied when a Stealth check fails by LoudFailureThreshold or more.
    public const int LoudFailureThreshold = 10;

    public static void ApplyLoudFailure(Unit unit, List<Unit> allEnemies)
    {
        //Immediately detected by all enemies regardless of LoS.
        foreach (Unit enemy in allEnemies)
        {
            if (enemy.IsDead()) continue;
            Set(enemy, unit, DetectionState.Detected);
        }

        CombatLog.Add($"{unit.unitName} makes a loud noise — everyone is alerted!");
    }

    //Reset

    public static void Reset()
    {
        detectionStates.Clear();
        lastKnownPositions.Clear();
        alertCountdowns.Clear();
    }

    //Called at the start of a unit's turn.  Clears Exposed state back to Detected,
    //allowing the unit to attempt re-stealth again this turn if conditions are met
    public static void ResetExposed(Unit unit)
    {
        List<(Unit observer, Unit target)> toUpdate = new List<(Unit observer, Unit target)>();

        foreach (var kvp in detectionStates)
        {
            if (kvp.Key.target == unit && kvp.Value == DetectionState.Exposed)
                toUpdate.Add(kvp.Key);
        }

        foreach (var key in toUpdate)
        {
            detectionStates[key] = DetectionState.Detected;
        }
    }

    // Called by CombatManager.ProcessEndPhase() once per turn.
    // Decrements alert countdowns for all intelligent enemies.
    // Default alert duration is 3 turns per the GDD.
    public const int AlertDuration = 3;

    public static void TickAlertCountDowns()
    {
        List<Unit> keys = new List<Unit>(alertCountdowns.Keys);

        foreach (Unit unit in keys)
        {
            if (alertCountdowns[unit] > 0)
                alertCountdowns[unit]--;
        }
    }
}
