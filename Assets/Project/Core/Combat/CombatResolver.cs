using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatResolver
{
    CombatManager manager;

    private Dictionary<Unit, SimulatedUnit> simUnits;

    private bool isSimulation = false;

    public bool IsSimulation => isSimulation;
    public CombatManager Manager => manager;

    //True while a real round is being executed (not planning, not the end phase).
    //Unit.AddEffect reads it to decide whether an effect needs extending to cover the recipient's next turn
    public static bool ResolutionInProgress { get; private set; }

    //Planning only: speed bonus from this unit's earlier planned actions (e.g. Burst on self)
    public void AddSimulatedMoveSpeedBonus(Unit unit, int bonus)
    {
        if (isSimulation && simUnits.TryGetValue(unit, out var sim))
            sim.moveSpeedBonus += bonus;
    }

    public int GetMoveSpeed(Unit unit)
    {
        int planningBonus = 0;
        if (isSimulation && simUnits.TryGetValue(unit, out var sim))
            planningBonus = sim.moveSpeedBonus;

        return unit.GetMoveSpeed(planningBonus);
    }

    public Vector2Int GetPosition(Unit unit)
    {
        if (isSimulation)
        {
            return simUnits[unit].position;
        }

        return unit.position;
    }

    public bool IsUnitDead(Unit unit)
    {
        if (isSimulation)
        {
            return simUnits[unit].isDead;
        }

        return unit.IsDead();
    }

    public bool IsTileOccupied(Vector2Int pos, Unit ignoreUnit = null)
    {
        if (isSimulation)
        {
            foreach (var kvp in simUnits)
            {
                var unit = kvp.Key;
                var sim = kvp.Value;

                if (unit == ignoreUnit) continue;
                if (sim.isDead) continue;

                if (sim.position == pos) return true;
            }

            return false;
        }

        Unit unitAt = GridManager.Instance.GetUnitAt(pos);

        if (unitAt == null) return false;
        if (unitAt == ignoreUnit) return false;
        if (unitAt.IsDead()) return false;

        return true;
    }

    public CombatResolver(CombatManager manager, bool simulationMode)
    {
        this.manager = manager;
        this.isSimulation = simulationMode;

        if (isSimulation)
        {
            simUnits = new Dictionary<Unit, SimulatedUnit>();

            foreach (var unit in manager.GetAllUnits())
            {
                simUnits[unit] = new SimulatedUnit(unit);
            }
        }
    }

    int GetActionPriority(CombatAction action)
    {
        int baseInitiative = action.source.initiativeRoll;

        int actionPriority = GetActionSpeed(action.definition.actionID);

        int total = baseInitiative + actionPriority;

        return total;
    }

    //Defensive declarations (Total Defense, Fighting Defensively) take effect at the start of execution, whatever the
    //unit's initiative: planning is simultaneous, so the stance is known before anyone acts. Their bonuses last until
    //the end phase (duration 1), covering the whole round
    const int DefensiveStanceSpeed = 1000;

    int GetActionSpeed(string type)
    {
        switch (type)
        {
            case ActionIds.TotalDefense:
            case ActionIds.FightingDefensively:
            case ActionIds.PowerAttack:
            case ActionIds.ManifestDefensively:
            case ActionIds.DeadlyAim:
                return DefensiveStanceSpeed;

            default: return 0;
        }
    }

    public IEnumerator ResolveTurn(List<CombatAction> actions)
    {
        SortActions(actions);

        yield return new WaitForSeconds(0.5f);

        yield return ExecuteActions(actions);
    }

    void SortActions(List<CombatAction> actions)
    {
        Dictionary<CombatAction, int> tieBreakers = new Dictionary<CombatAction, int>();
        Dictionary<CombatAction, int> priorities = new Dictionary<CombatAction, int>();

        //Assign random tie-breakers and priorities once per turn (the sort compares each action many times)
        foreach (var action in actions)
        {
            tieBreakers[action] = Random.Range(0, 10000);
            priorities[action] = GetActionPriority(action);
        }

        actions.Sort((a, b) =>
        {
            int aPriority = priorities[a];
            int bPriority = priorities[b];

            if (aPriority != bPriority)
                return bPriority.CompareTo(aPriority);

            //Enforce per-unit sequence order
            if (a.source == b.source)
                return a.sequenceIndex.CompareTo(b.sequenceIndex);

            //Stable tie-breaker
            return tieBreakers[b].CompareTo(tieBreakers[a]);
        });

        //One line per action, in resolution order
        foreach (var action in actions)
            Debug.Log($"{action.source.unitName} action priority: initiative {action.source.initiativeRoll} " +
                      $"+ speed {GetActionSpeed(action.definition.actionID)} = {priorities[action]} ({action.definition.DisplayName})");
    }

    IEnumerator ExecuteActions(List<CombatAction> actions)
    {
        //Attack stances apply only when the unit also attacks this round: Fighting Defensively needs any attack,
        //Power Attack a melee attack. Drop them otherwise
        for (int i = actions.Count - 1; i >= 0; i--)
        {
            CombatAction stance = actions[i];
            string id = stance.definition.actionID;

            System.Predicate<CombatAction> needs;
            string missing;
            if (id == ActionIds.FightingDefensively)
            {
                needs = a => a.definition.attackType != ActionDefinition.AttackType.None;
                missing = "attacking";
            }
            else if (id == ActionIds.PowerAttack)
            {
                needs = a => a.definition.attackType == ActionDefinition.AttackType.Melee;
                missing = "making a melee attack";
            }
            else if (id == ActionIds.DeadlyAim)
            {
                needs = a => a.definition.attackType == ActionDefinition.AttackType.Ranged;
                missing = "making a ranged attack";
            }
            else if (id == ActionIds.ManifestDefensively)
            {
                needs = a => a.definition is PsionicPowerDefinition;
                missing = "manifesting a power";
            }
            else continue;

            if (actions.Exists(a => a.source == stance.source && needs(a))) continue;

            if (!isSimulation)
                CombatLog.Add($"{stance.source.unitName} isn't {missing} " +
                              $"this round: {stance.definition.DisplayName} has no effect.");
            actions.RemoveAt(i);
        }

        //Reactions are armed for the round and fire from their trigger, not in initiative order
        ArmReactions(actions);

        //Count each unit's planned actions so effects can tell whether the unit still acts this round.
        //Every unit starts at 0, so a unit with nothing planned counts as having no actions left
        foreach (Unit unit in manager.GetAllUnits()) unit.pendingActionCount = 0;
        foreach (CombatAction planned in actions)
            if (!planned.isReaction) planned.source.pendingActionCount++;

        if (!isSimulation) ResolutionInProgress = true;

        foreach (CombatAction action in actions)
        {
            // Run passive detection sweep once per unit at the start of their turn,
            // so detection state reflects current positions before the action validates.
            if (!isSimulation && action.sequenceIndex == 0)
            {
                List<Unit> allUnits = manager.GetAllUnits();
                List<Unit> playerUnits = allUnits.FindAll(u => !u.isEnemy);
                List<Unit> enemyUnits = allUnits.FindAll(u => u.isEnemy);

                if (action.source.isEnemy)
                    DetectionStateManager.RunPassiveSweep(enemyUnits, playerUnits);
                else
                    DetectionStateManager.RunPassiveSweep(playerUnits, enemyUnits);

                DetectionStateManager.ResetExposed(action.source);
            }

            if (action.source.CanAct() && !action.source.IsActionBlocked())
            {
                //Debug.Log("Executing action from: " + action.source.unitName);
                    
                manager.SetCurrentUnit(action.source);

                //Regen resource on first actionof this unit's turn
                if (action.sequenceIndex == 0)
                {
                    action.source.RegenResource();
                }

                //Armed reactions resolve from their trigger (regen above still runs for the unit's first slot)
                if (action.isReaction) continue;

                bool wasDisabled = action.source.IsDisabled();

                //Acting ends flat-footed: this unit's turn has come
                if (!isSimulation && action.source.IsFlatFooted)
                {
                    action.source.SetFlatFooted(false);
                    CombatLog.Add($"{action.source.unitName} is no longer flat-footed.");
                }

                ExecuteAction(action);

                if (!isSimulation && wasDisabled) ApplyDisabledExertion(action);

                //Refresh grid visuals after state changes
                GridHighlighter.Instance.RefreshUnitHighlights(manager);
                 
                yield return new WaitForSeconds(0.75f);
            }

            else if (!isSimulation && action.source.IsActionBlocked() && action.sequenceIndex == 0)
            {
                CombatLog.Add($"{action.source.unitName} is unable to act.");
            }

            if (!action.isReaction) action.source.pendingActionCount--;
        }

        //Resolution over: effects applied from here on (end phase, round start) already cover the coming round
        if (!isSimulation) ResolutionInProgress = false;
    }

    public void ExecuteAction(CombatAction action)
    {
        if (!action.source.CanAct()) return;

        ActionContext ctx = BuildContext(action);

        var def = ctx.definition;

        //Psionic manifestation cost: paid before effects resolve
        //(d20: power points are spent even if the power is disrupted)
        if (!isSimulation && def is PsionicPowerDefinition psionicPower)
        {
            if (ctx.attacker.IsManifestingBlocked())
            {
                CombatLog.Add($"{ctx.attacker.unitName} cannot manifest {psionicPower.DisplayName}.");
                return;
            }

            var cost = AugmentationProcessor.Validate(ctx.attacker, psionicPower, action.augmentations,
                maximized: action.maximize);

            if (!cost.isValid)
            {
                CombatLog.Add($"{ctx.attacker.unitName} fails to manifest {psionicPower.DisplayName}: {cost.failureReason}");
                return;
            }

            if (ctx.isMaximized)
                CombatLog.Add($"{ctx.attacker.unitName} maximizes {psionicPower.DisplayName}.");

            //Catapsi: inside an opposing field, a paid manifestation costs double unless resisted
            int powerPointCost = cost.totalPowerPointCost;
            if (powerPointCost > 0)
            {
                var field = CatapsiEffect.FindOpposingField(ctx.attacker, this);
                if (field != null && field.DoublesCost(ctx.attacker))
                {
                    powerPointCost *= 2;
                    if (!ctx.attacker.powerPoints.CanAfford(powerPointCost))
                    {
                        CombatLog.Add($"{ctx.attacker.unitName} fails to manifest {psionicPower.DisplayName}: " +
                                      $"not enough power points inside the catapsi field");
                        return;
                    }
                }
            }

            //Material component: paid alongside power points, so it is lost if the manifestation is disrupted
            if (!MaterialComponentResolver.TryPay(action, psionicPower)) return;

            //Trigger Power: a successful ability check manifests it for 0 PP (after catapsi doubling and the
            //material component; the material is still paid)
            if (powerPointCost > 0 && !cost.usesFreeZeroLevel && TriggerPower.TryTrigger(ctx.attacker, psionicPower))
                powerPointCost = 0;

            if (cost.usesFreeZeroLevel)
            {
                ctx.attacker.powerPoints.freeZeroLevelUsed++;
                CombatLog.Add($"{ctx.attacker.unitName}: free 0-level manifestation " +
                    $"({ctx.attacker.powerPoints.FreeZeroLevelRemaining} remaining)");
            }

            if (powerPointCost > 0)
                PowerPointManager.TrySpend(ctx.attacker, powerPointCost);
        }

        //Opportunity attacks and psionic concentration resolve BEFORE the action takes effect.
        //Skip in simulation mode — OA logic does not run during AI planning
        if (!isSimulation && !action.isReaction && !action.isFollowUp)
        {
            if (!ResolveProvocation(ctx, action)) return;
        }

        //Concentration powers: manifesting a new one ends any power already being maintained
        var concentrationPower = def as PsionicPowerDefinition;
        bool startsConcentration = !isSimulation
            && concentrationPower != null
            && concentrationPower.concentrationRequired;

        if (startsConcentration)
            ctx.attacker.EndConcentration("manifested another concentration power");

        BaseAction baseAction = new GenericAction(def);

        if (baseAction == null)
        {
            Debug.LogError($"Missing action: {ctx}");
            return;
        }

        //One line per resolved action, placeholders included (movement logs its own path line)
        if (!isSimulation && !def.IsMove())
        {
            string on = ctx.finalTarget != null && ctx.finalTarget != ctx.attacker
                ? $" on {ctx.finalTarget.unitName}"
                : def.IsGroundTargeted() && ctx.action.movePosition.HasValue
                    ? $" at {ctx.action.movePosition.Value}"
                    : "";
            CombatLog.Add($"{ctx.attacker.unitName} uses {def.DisplayName}{on}.");
        }

        //Killing Blow (Helix Berserker): melee attacks against a target at 25% HP or less deal maximum damage.
        //Set before the effects run so bonus dice (Brutal Strike, Devastating Cleave) are maximized too
        if (def.attackType == ActionDefinition.AttackType.Melee && ctx.finalTarget != null &&
            ctx.attacker.HasPassive(PassiveIds.HelixKillingBlow) &&
            GetCurrentHP(ctx.finalTarget) * 4 <= ctx.finalTarget.GetMaxHP())
        {
            ctx.isMaximized = true;
            if (!isSimulation)
                CombatLog.Add($"Killing Blow: {ctx.finalTarget.unitName} is at 25% HP or less — maximum damage.");
        }

        //Execute action (behavior only)
        baseAction.Execute(ctx, this);

        //Powers whose effect isn't authored yet: roll the save and hold concentration anyway
        if (!isSimulation && concentrationPower != null &&
            (def.effects == null || def.effects.Count == 0))
            ResolveDeferredPower(ctx, concentrationPower);

        //Class actions whose effect isn't authored yet (Vanish, Spectral Form, Group Suggestion): say so, like powers
        else if (!isSimulation && concentrationPower == null && !def.IsMove() &&
                 (def.effects == null || def.effects.Count == 0))
            CombatLog.Add($"{def.DisplayName}: effect not implemented yet.");

        //Link everything this power applied to a ConcentrationEffect on the manifester
        if (startsConcentration && ctx.appliedEffects.Count > 0)
            ctx.attacker.AddEffect(new ConcentrationEffect(concentrationPower,
                new List<StatusEffect>(ctx.appliedEffects),
                concentrationPower.GetConcentrationMaxRounds(ctx.attacker)));

        if (ctx.definition.resourceCost > 0)
            AudioEventBus.Fire(AudioEventIds.Combat.AbilityActivate);

        //Spend resource cost
        int effectiveCost = ctx.attacker.GetEffectiveResourceCost(ctx.definition);
        if (effectiveCost > 0)
        {
            ctx.attacker.SpendResource(effectiveCost);

            var resource = ctx.attacker.resource;
            if (!isSimulation && resource != null)
                CombatLog.Add($"{ctx.attacker.unitName} spends {effectiveCost} {resource.definition.displayName} " +
                              $"({resource.current}/{resource.max} remaining).");
        }

        if (ctx.isMovement)
        {
            ResolveMove(ctx);
        }
        else if (ctx.shouldApplyDamage)
        {
            TryFireReaction(ctx.finalTarget, ReactionTrigger.TargetedByAttack, ctx.attacker);

            ApplyPreAttackEffects(ctx);

            if (!ResolveHit(ctx))
            {
                CombatLog.Add($"{ctx.attacker.unitName} misses {ctx.finalTarget.unitName}");
                action.OnResolved();
                return;
            }

            action.hit = true;

            InitializeDamage(ctx);

            //Devastating Cleave: extra dice of the attack's base damage die (follow-up attacks only), rolled and logged
            if (action.extraDamageDice > 0 && ctx.damageDice.Count > 0)
            {
                int sides = ctx.damageDice[0].y;
                string label = action.extraDamageDiceLabel ?? "Bonus";
                int roll = PowerDice.Roll(ctx, action.extraDamageDice, sides, label);
                ctx.modifiedDamage += roll;
                ctx.AddDamagePart($"{label} {action.extraDamageDice}d{sides}", roll);
            }

            //Bloodlust (Helix Berserker): below half HP, +1 melee damage per 4 levels
            if (def.attackType == ActionDefinition.AttackType.Melee &&
                ctx.attacker.HasPassive(PassiveIds.HelixBloodlust) &&
                ctx.attacker.currentHP * 2 < ctx.attacker.GetMaxHP())
            {
                int bloodlust = Mathf.Max(1, (ctx.attacker.sourceCharacterData?.level ?? 4) / 4);
                ctx.modifiedDamage += bloodlust;
                ctx.AddDamagePart("Bloodlust", bloodlust);
            }

            ApplyPreActionEffects(ctx);
            LogDamageBreakdown(ctx);
            ApplyFinalResult(ctx);

            DetectionStateManager.EvaluateBreak(ctx.attacker, ctx.definition);

            //After-hit triggers (Knockdown): run once the damage has been applied
            ApplyPostActionEffects(ctx);

            //Savage Momentum: a melee hit that dropped its target adds +1 attack (before any follow-up attacks)
            if (!isSimulation && ctx.droppedTarget && def.attackType == ActionDefinition.AttackType.Melee &&
                ctx.attacker.HasPassive(PassiveIds.HelixSavageMomentum))
                ctx.attacker.GainSavageMomentum(manager.GetCurrentRound());

            //Follow-ups registered by the action's effects (Cleave), after this hit is fully resolved
            if (!isSimulation) ctx.onHitResolved?.Invoke(ctx);

            //Rampage: a melee hit that dropped its target grants a free attack (once per round)
            if (!isSimulation) TryRampage(ctx);
        }
        else if (ctx.isWindowBreak)
        {
            ResolveWindowBreak(ctx);
        }

        if (ctx.isForcedMovement && ctx.forcedMoveTarget != null)
        {
            Vector2Int dir = ctx.forcedMoveDelta;
            int mag = Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
            Vector2Int normalized = mag > 0
                ? new Vector2Int(dir.x / mag, dir.y / mag)
                : Vector2Int.zero;
            ResolveForcedMove(ctx.forcedMoveTarget, normalized, mag);
        }

        action.OnResolved();
    }

    //Extra melee attack inside another action (Cleave, Rampage): resolved at once through ExecuteAction, without
    //provoking and without action economy. Returns true if it hit
    public bool ExecuteFollowUpAttack(Unit attacker, Unit target, ActionDefinition attackDef,
                                      int extraDamageDice = 0, string extraDamageDiceLabel = null)
    {
        var followUp = new CombatAction(attacker, target, attackDef, manager)
        {
            isFollowUp = true,
            extraDamageDice = extraDamageDice,
            extraDamageDiceLabel = extraDamageDiceLabel
        };
        ExecuteAction(followUp);
        return followUp.hit;
    }

    //Rampage (Helix Berserker): once per round, a melee hit that drops its target grants a free melee attack against
    //another enemy adjacent to the attacker (lowest HP first). Not spent when no enemy is in reach
    void TryRampage(ActionContext ctx)
    {
        Unit attacker = ctx.attacker;
        if (!ctx.droppedTarget || ctx.definition.attackType != ActionDefinition.AttackType.Melee) return;
        if (!attacker.HasPassive(PassiveIds.HelixRampage) || !attacker.CanAct()) return;

        int round = manager.GetCurrentRound();
        if (attacker.rampageRound == round) return;

        Unit next = null;
        foreach (var unit in GetAllUnits())
        {
            if (unit == ctx.finalTarget || unit.isEnemy == attacker.isEnemy) continue;
            if (IsUnitDead(unit) || !unit.CanAct()) continue;
            if (!DistanceUtility.IsAdjacent(attacker.position, unit.position)) continue;

            if (next == null || unit.currentHP < next.currentHP) next = unit;
        }

        if (next == null)
        {
            CombatLog.Add($"{attacker.unitName}'s Rampage: no other enemy within reach.");
            return;
        }

        attacker.rampageRound = round;
        CombatLog.Add($"{attacker.unitName} rampages into {next.unitName}!");
        ExecuteFollowUpAttack(attacker, next, ActionDatabase.Get(ActionIds.MeleeAttack),
            CleaveEffect.DevastatingDice(attacker), "Devastating Cleave");
    }

    //Stand-in for psionic powers whose effect isn't authored yet (stubs and deferred powers: empty effects list).
    //Rolls power resistance and the save like an authored power, logs that the effect is deferred, and for
    //concentration powers places a placeholder condition so concentration starts and can be maintained
    void ResolveDeferredPower(ActionContext ctx, PsionicPowerDefinition power)
    {
        Unit target = ctx.finalTarget;
        bool affected = true;

        if (target != null && !IsUnitDead(target))
        {
            var outcome = PsionicSaveResolver.Resolve(ctx, target, this);
            affected = outcome == PsionicSaveOutcome.Failed || outcome == PsionicSaveOutcome.NoSave;
        }

        CombatLog.Add($"{power.DisplayName}: effect not implemented yet.");

        if (!affected || !power.concentrationRequired) return;

        //Untargeted concentration powers (Object Reading) hold the placeholder on the manifester
        Unit holder = target ?? ctx.attacker;
        var placeholder = new StatusEffect(power.DisplayName, 0)
        {
            isHarmful = holder.isEnemy != ctx.attacker.isEnemy
        };

        var applied = holder.AddEffect(placeholder);
        if (applied != null) ctx.appliedEffects.Add(applied);
    }

    public List<Unit> GetAllUnits()
    {
        return manager.GetAllUnits();
    }

    public void RemoveFromEncounter(Unit unit)
    {
        //Planning treats removal as defeat
        if (isSimulation)
        {
            simUnits[unit].currentHP = unit.GetDeathThreshold();
            return;
        }

        manager.RemoveFromEncounter(unit);
    }

    //Death effects: the target dies outright
    public void KillUnit(Unit target)
    {
        if (target == null || IsUnitDead(target)) return;

        if (isSimulation)
        {
            simUnits[target].currentHP = target.GetDeathThreshold();
            return;
        }

        target.Kill();
    }

    public int GetCurrentHP(Unit unit) => isSimulation ? simUnits[unit].currentHP : unit.currentHP;

    //Drops a living target straight to -1 HP and dying (Psychic Crush)
    public void DropToDying(Unit target)
    {
        if (target == null || IsUnitDead(target)) return;

        if (isSimulation)
        {
            simUnits[target].currentHP = Mathf.Min(simUnits[target].currentHP, -1);
            return;
        }

        if (target.isInvulnerableTarget)
        {
            CombatLog.Add($"[Target] {target.unitName}: would drop to -1 HP and dying — ignored, training target.");
            return;
        }

        if (target.currentHP <= -1) return;

        target.currentHP = -1;

        //Units without dying rules (general enemies) are dead at -1: Kill() runs OnDeath -> ProcessDefeat
        if (target.IsDead())
        {
            target.Kill();
            return;
        }

        if (!target.CanAct()) target.EndConcentration("incapacitated");
        target.OnHealthChanged?.Invoke(target);
        HandleUnitDying(target);
    }

    void HandleUnitDeath(Unit unit, Unit attacker = null)
    {
        // Frightful Presence trigger
        if (attacker != null)
        {
            foreach (var effect in attacker.activeEffects)
            {
                if (effect is FrightfulPresenceEffect fp)
                {
                    fp.TriggerOnKill(attacker, GetAllUnits(), this);
                    break;
                }
            }
        }

        //Removal, loot and incapacitation run from Unit.OnDeath (CombatManager.ProcessDefeat)
        manager.ProcessDefeat(unit);
    }

    void HandleUnitDying(Unit unit)
    {
        //OnlyApply DyingState once, a second hit on an already dying unit just deals HP loss
        if (!unit.IsDying()) return;

        if (!unit.activeEffects.Exists(e => e is DyingState))
        {
            unit.RemoveEffect<DisabledState>();
            unit.AddEffect(new DyingState());
        }
    }

    void HandleUnitDisabled(Unit unit)
    {
        if (!unit.activeEffects.Exists(e => e is DisabledState))
        {
            unit.AddEffect(new DisabledState());
        }
    }

    //d20 Disabled: a standard (strenuous) action deals 1 damage after it completes, dropping the unit to -1 and dying.
    //Moving alone is free. No damage if the unit is no longer at 0 HP (healed itself, or hit by an OA mid-action).
    //Direct HP loss, not TakeDamage: DR and temp HP don't stop exertion.
    void ApplyDisabledExertion(CombatAction action)
    {
        Unit unit = action.source;

        if (!unit.IsDisabled()) return;
        if (!action.definition.consumesCategories.Contains(ActionDefinition.ActionCategory.Standard)) return;

        unit.currentHP -= 1;
        CombatLog.Add($"{unit.unitName} strains while disabled and takes 1 damage. HP: {unit.currentHP}");

        if (!unit.CanAct()) unit.EndConcentration("incapacitated");
        unit.OnHealthChanged?.Invoke(unit);
        HandleUnitDying(unit);
    }

    public void ResolveMove(ActionContext ctx)
    {
        //Debug.Log($"Action: {ctx.actionId}, isMovement: {ctx.isMovement}");

        if (!ctx.movePosition.HasValue) return;

        Vector2Int newPos = ctx.movePosition.Value;

        if (!GridManager.Instance.IsWithinBounds(newPos))
        {
            CombatLog.Add("Invalid move (out of bounds)");
            return;
        }

        if (IsTileOccupied(newPos, ctx.action.source))
        {
            if (!isSimulation)
            {
                CombatLog.Add("Invalid move (tile occupied)");
            }
            return;
        }

        if (isSimulation)
        {
            var simUnit = simUnits[ctx.attacker];

            simUnit.position = newPos;

            return;
        }

        Unit mover = ctx.action.source;
        Vector2Int startPos = mover.position;
        var walked = new List<Vector2Int>();

        if (!MovementResolver.ExecuteMovePath(mover, newPos, this, walked))
            CombatLog.Add($"{mover.unitName} can't reach {newPos} and stays at {mover.position}.");
        else if (mover.position != newPos)
            CombatLog.Add($"{mover.unitName} moved from {startPos} to {mover.position}, stopped before reaching {newPos} [path {string.Join(" → ", walked)}]");
        else
            CombatLog.Add($"{mover.unitName} moved from {startPos} to {newPos} [path {string.Join(" → ", walked)}]");
    }

    void ApplyPreActionEffects(ActionContext ctx)
    {
        List<StatusEffect> effects = CollectEffects(ctx);

        effects.Sort((a, b) => b.Priority.CompareTo(a.Priority));

        foreach (var effect in effects)
        {
            int before = ctx.modifiedDamage;
            effect.OnBeforeAction(ctx);
            ctx.AddDamagePart(effect.effectName, ctx.modifiedDamage - before);
        }
    }

    //Before the attack roll: effects that change the roll itself (Coordinated Fire's +2). Execution phase only,
    //so one-shot effects are spent once, not in every planning replay
    void ApplyPreAttackEffects(ActionContext ctx)
    {
        if (isSimulation) return;

        //Shared passive instances can be collected from both attacked and target: run each once
        var effects = new List<StatusEffect>(new HashSet<StatusEffect>(CollectEffects(ctx)));
        effects.Sort((a, b) => b.Priority.CompareTo(a.Priority));

        foreach (var effect in effects)
            effect.OnBeforeAttackRoll(ctx);
    }

    void ApplyPostActionEffects(ActionContext ctx)
    {
        //Rolls happen once, in the execution phase only
        if (isSimulation) return;

        //Class passive effects are shared instances and can be collected from both attacker and target: run each once
        var effects = new List<StatusEffect>(new HashSet<StatusEffect>(CollectEffects(ctx)));

        effects.Sort((a, b) => b.Priority.CompareTo(a.Priority));

        foreach (var effect in effects)
        {
            effect.OnAfterAction(ctx);
        }
    }

    ActionContext BuildContext(CombatAction action)
    {
        return new ActionContext
        {
            attacker = action.source,
            originalTarget = action.target,
            finalTarget = action.target,

            baseDamage = 0,
            modifiedDamage = 0,

            action = action,
            temporaryEffects = action.temporaryEffects,
            definition = action.definition,
            isMaximized = action.maximize,
            logRolls = !isSimulation
        };
    }

    public void ApplyFinalResult(ActionContext ctx)
    {
        if (ctx.finalTarget == null) return;

        if (IsUnitDead(ctx.finalTarget)) return;

        int damage = Mathf.RoundToInt(ctx.modifiedDamage);

        if (isSimulation)
        {
            var simTarget = simUnits[ctx.finalTarget];

            ctx.simulatedDamage = damage;

            int prevHP = simTarget.currentHP;
            simTarget.currentHP -= damage;

            int deathThreshold = ctx.finalTarget.GetDeathThreshold();
            ctx.willKillTarget = prevHP > deathThreshold && simTarget.currentHP <= deathThreshold;
            return;
        }

        var target = ctx.finalTarget;

        int prev = target.currentHP;
        int damageReduction = target.GetDamageReduction(); //read before the hit (it can discharge Inertial Barrier)

        target.TakeDamage(damage);

        int actual = prev - target.currentHP;

        //Say why a hit did less than rolled: DR, Kinetic Absorption or temporary HP (temp HP also logs its own line).
        //Invulnerable training targets print their own breakdown.
        string reduction = "";
        if (actual < damage && !target.isInvulnerableTarget)
            reduction = damageReduction > 0
                ? $" ({damage} before DR {damageReduction})"
                : $" ({damage} before reduction)";

        CombatLog.Add($"{target.unitName} takes {actual} damage{reduction}. HP: {target.currentHP}");

        //Massive damage (d20 Modern): a single hit at or above the threshold that leaves the target above 0 HP
        //forces a Resolve (Fortitude) save, DC 15. Failure drops it straight to -1 HP and dying.
        //A hit that already took the target to 0 or lower skips the check
        if (actual >= target.GetMassiveDamageThreshold() && target.currentHP > 0)
        {
            var saveRoll = target.RollSave(Unit.SaveType.Resolve);
            CombatLog.Add($"{target.unitName} massive damage check: rolled {saveRoll} vs DC 15");

            if (saveRoll < 15)
            {
                if (target.isInvulnerableTarget)
                {
                    CombatLog.Add($"[Target] {target.unitName}: fails massive damage save — would drop to -1 HP and dying (ignored, training target).");
                }
                else
                {
                    target.currentHP = -1;
                    CombatLog.Add($"{target.unitName} fails massive damage save — drops to -1 HP!");

                    //General enemies (no dying rules) are dead at -1: Kill() fires OnDeath, which removes the
                    //unit's model and label and runs the defeat path (same as DropToDying)
                    if (target.IsDead())
                    {
                        target.Kill();
                    }
                    else
                    {
                        if (!target.CanAct()) target.EndConcentration("incapacitated");
                        target.OnHealthChanged?.Invoke(target);
                    }
                }
            }
        }

        //Rampage and Savage Momentum: this hit dropped the target (dead, or from above 0 HP to 0 or below)
        ctx.droppedTarget = target.IsDead() || (prev > 0 && target.currentHP <= 0);

        if (target.IsDead())
        {
            HandleUnitDeath(target, ctx.attacker);
        }
        else if (target.IsDying())
        {
            HandleUnitDying(target);
        }
        else if (target.IsDisabled())
        {
            HandleUnitDisabled(target);
        }
    }

    List<StatusEffect> CollectEffects(ActionContext ctx)
    {
        List<StatusEffect> allEffects = new List<StatusEffect>();

        //Runtime effects
        if (ctx.attacker != null)
            allEffects.AddRange(ctx.attacker.activeEffects);

        if (ctx.finalTarget != null)
            allEffects.AddRange(ctx.finalTarget.activeEffects);

        //Source-based effects
        if (ctx.attacker != null)
        {
            foreach (var source in ctx.attacker.effectSources)
            {
                allEffects.AddRange(source.GetEffects(ctx.attacker));
            }
        }

        if (ctx.finalTarget != null)
        {
            foreach (var source in ctx.finalTarget.effectSources)
            {
                allEffects.AddRange(source.GetEffects(ctx.finalTarget));
            }
        }

        //Temporary effects
        if (ctx.temporaryEffects != null)
            allEffects.AddRange(ctx.temporaryEffects);

        return allEffects;
    }

    bool ResolveHit(ActionContext ctx)
    {
        if (ctx.finalTarget == null) return false;

        int roll = Random.Range(1, 21);

        var attackType = ctx.definition.attackType;

        int accuracy = 0;

        switch (attackType)
        {
            case ActionDefinition.AttackType.Melee:
                accuracy = ctx.attacker.GetAccuracy(true);
                break;

            case ActionDefinition.AttackType.Ranged:
                accuracy = ctx.attacker.GetAccuracy(false);
                break;

            default:
                Debug.LogWarning($"ResolveHit called with non-attack action: {ctx.definition.actionID}");
                return false;
        }

        if (ctx.attacker.sourceCharacterData != null &&
            ctx.attacker.sourceCharacterData.HasItemEquipped(EquipmentSlot.MainHand))
        {
            ItemInstance weaponInstance = ctx.attacker.sourceCharacterData.GetEquippedItem(EquipmentSlot.MainHand);

            var weapon = weaponInstance != null ? ItemDatabase.Get(weaponInstance.definitionId) : null;
            if (weapon != null)
            {
                var classDef = ClassDatabase.Get(ctx.attacker.sourceCharacterData.classId);
                if (classDef != null)
                {
                    if (!classDef.IsProficientWith(weapon.weaponTag))
                    {
                        accuracy -= 4;
                        CombatLog.Add($"{ctx.attacker.unitName} is not proficient with {weapon.displayName} (-4 accuracy)");
                    }
                }
            }
        }

        //BAB from class progression (players) or progression profile + statBonuses (enemies), same source Volley Fire uses
        int bab = ctx.attacker.GetBAB();

        int attackTotal = roll + accuracy + bab + ctx.attackRollModifier;

        int defense = ctx.finalTarget.GetDefense();

        bool isMeleeAttack = attackType == ActionDefinition.AttackType.Melee;
        Debug.Log($"Attack Roll: {roll} + acc {accuracy} [{ctx.attacker.DescribeAccuracy(isMeleeAttack)}] + BAB {bab} " +
                  $"+ mod {ctx.attackRollModifier} = {attackTotal} vs {defense}" +
          (ctx.finalTarget.IsFlatFooted ? " (target flat-footed: no Agility bonus)" : "") +
          (roll == 20 ? " (natural 20: automatic hit)" : roll == 1 ? " (natural 1: automatic miss)" : ""));

        //d20: a natural 1 always misses, a natural 20 always hits (critical threats are deferred until criticals exist)
        bool hit = roll == 20 || (roll != 1 && attackTotal >= defense);

        //Miss chance: rolled only on a hit; the attacker's and target's sources don't stack, highest applies.
        //Every roll is logged with its source, whether it turns the hit into a miss or not
        if (hit)
        {
            int outgoing = ctx.attacker.GetMissChance(true, out string outgoingSource);
            int incoming = ctx.finalTarget.GetMissChance(false, out string incomingSource);

            bool fromAttacker = outgoing >= incoming;
            int missChance = fromAttacker ? outgoing : incoming;

            if (missChance > 0)
            {
                string source = fromAttacker
                    ? $"{ctx.attacker.unitName}'s {ActionDefinition.FormatActionName(outgoingSource)}"
                    : $"{ctx.finalTarget.unitName}'s {ActionDefinition.FormatActionName(incomingSource)}";

                int percentile = Random.Range(1, 101);
                hit = percentile > missChance;

                if (!isSimulation)
                    CombatLog.Add($"Miss chance ({source}, {missChance}%): rolled {percentile} — " +
                                  $"{(hit ? "the hit stands" : "miss")}");
            }
        }

        if (hit)
        {
            if (attackType == ActionDefinition.AttackType.Melee)
                AudioEventBus.Fire(AudioEventIds.Combat.MeleeHit);
            else if (attackType == ActionDefinition.AttackType.Ranged)
                AudioEventBus.Fire(AudioEventIds.Combat.RangedHit);
        }
        else
        {
            if (attackType == ActionDefinition.AttackType.Melee)
                AudioEventBus.Fire(AudioEventIds.Combat.MeleeMiss);
            else if (attackType == ActionDefinition.AttackType.Ranged)
                AudioEventBus.Fire(AudioEventIds.Combat.RangedMiss);
        }

        return hit;
    }

    //One line per hit: base roll, each labelled contributor and the total, before DR and temporary HP
    //(ApplyFinalResult logs those). Anything that changed damage without a label shows as "other"
    void LogDamageBreakdown(ActionContext ctx)
    {
        if (isSimulation) return;

        var parts = new List<string> { $"roll [ {ctx.baseDamage} ]" };
        parts.AddRange(ctx.damageParts);

        int other = ctx.modifiedDamage - ctx.baseDamage - ctx.itemizedDamage;
        if (other != 0) parts.Add($"other [ {other} ]");

        CombatLog.Add($"{ctx.attacker.unitName} damage: {string.Join(" + ", parts)} = {ctx.modifiedDamage}");
    }

    void InitializeDamage(ActionContext ctx)
    {
       int baseDamage = RollBaseDamage(ctx);

        ctx.baseDamage = baseDamage;
        ctx.modifiedDamage += baseDamage;
    }

    int RollBaseDamage(ActionContext ctx)
    {
        //Try to read damage from equipped weapon
        if (ctx.attacker.sourceCharacterData != null &&
            ctx.attacker.sourceCharacterData.HasItemEquipped(EquipmentSlot.MainHand))
        {
            ItemInstance weaponInstance = ctx.attacker.sourceCharacterData.GetEquippedItem(EquipmentSlot.MainHand);

            var weapon = weaponInstance != null ? ItemDatabase.Get(weaponInstance.definitionId) : null;
            if (weapon != null)
            {
                int total = 0;
                for (int i = 0; i < weapon.damageDiceCount; i++)
                    total += ctx.isMaximized ? weapon.damageDiceSides : Random.Range(1, weapon.damageDiceSides + 1);

                total += weapon.damageBonus;
                ctx.damageDice.Add(new Vector2Int(weapon.damageDiceCount, weapon.damageDiceSides));

                if (!isSimulation)
                    CombatLog.Add($"{ctx.attacker.unitName} rolls {weapon.displayName}{(ctx.isMaximized ? " (maximized)" : "")}: {total}");
                return total;
            }
        }

        //Fallback: unarmed strike, die from the unarmed feat chain
        int sides = ctx.attacker.GetUnarmedDamageDie();
        int unarmedRoll = ctx.isMaximized ? sides : Random.Range(1, sides + 1);
        ctx.damageDice.Add(new Vector2Int(1, sides));
        if (!isSimulation)
            CombatLog.Add($"{ctx.attacker.unitName} rolls unarmed (1d{sides}){(ctx.isMaximized ? " (maximized)" : "")}: {unarmedRoll}");
        return unarmedRoll;
    }

    public Unit GetUnitAt(Vector2Int pos)
    {
        if (isSimulation)
        {
            foreach (var kvp in simUnits)
            {
                if (kvp.Value.isDead) continue;
                if (kvp.Value.position == pos) return kvp.Key;
            }
            return null;
        }

        return GridManager.Instance.GetUnitAt(pos);
    }

    void ResolveWindowBreak(ActionContext ctx)
    {
        if (!ctx.action.movePosition.HasValue) return;

        Vector2Int attackerPos = ctx.attacker.position;
        Vector2Int targetPos = ctx.action.movePosition.Value;

        //Must be adjacent
        if (!DistanceUtility.IsAdjacent(attackerPos, targetPos)) return;

        Vector2Int diff = targetPos - attackerPos;

        // Determine edge ownership and which edge to break
        // North edge: owned by the southern tile
        // East edge: owned by the western tile
        if (diff == Vector2Int.up)
        {
            GridManager.Instance.SetWindowBroken(attackerPos, true);
        }
        else if (diff == Vector2Int.down)
        {
            GridManager.Instance.SetWindowBroken(targetPos, true);
        }
        else if (diff == Vector2Int.right)
        {
            GridManager.Instance.SetWindowBroken(attackerPos, false);
        }
        else if (diff == Vector2Int.left)
        {
            GridManager.Instance.SetWindowBroken(targetPos, false);
        }

        CombatLog.Add($"{ctx.attacker.unitName} breaks the window at {targetPos}");
    }

    public void ResolveInterruptOAs(List<OARecord> queue)
    {
        //Sort by initiative descending: highest acts first
        queue.Sort((a, b) => b.priority.CompareTo(a.priority));

        foreach (OARecord record in queue)
        {
            //Re-check conditions, earlier OAs in this queue may have changed state
            if (!record.attacker.CanAct()) continue;
            if (!record.attacker.oaState.CanTakeOA()) continue;
            if (record.target.IsDead()) continue;

            //Find the attacker's default melee action
            ActionDefinition meleeAction = record.attacker.GetAllActions()
                .Find(a => a.attackType == ActionDefinition.AttackType.Melee);

            if (meleeAction == null) continue;

            record.attacker.oaState.RecordOA();

            CombatAction oaAction = new CombatAction(
                record.attacker,
                record.target,
                meleeAction,
                manager
            );

            CombatLog.Add($"{record.attacker.unitName} takes an opportunity attack against {record.target.unitName}!");
            AudioEventBus.Fire(AudioEventIds.Combat.OATriggered);
            ExecuteAction(oaAction);
        }
    }

    //Marks planned reaction powers and arms them on their unit for this round
    //Marks planned reactions and arms them on their unit for this round
    void ArmReactions(List<CombatAction> actions)
    {
        foreach (var action in actions)
        {
            if (!action.definition.IsReaction) continue;

            action.isReaction = true;
            if (isSimulation) continue;

            action.source.armedReaction = action;
            if (!action.source.isEnemy)
                CombatLog.Add($"{action.source.unitName} readies {action.definition.DisplayName}.");
        }
    }

    //Fires the reactor's armed reaction if it matches the trigger. Shares the OA counter.
    //Resolves before the triggering attack or effect, so its protection applies to it.
    public void TryFireReaction(Unit reactor, ReactionTrigger trigger, Unit source)
    {
        if (isSimulation || reactor == null) return;

        CombatAction reaction = reactor.armedReaction;
        if (reaction == null || reaction.definition.reactionTrigger != trigger) return;

        if (source != null && source.isEnemy == reactor.isEnemy) return; //hostile sources only
        if (!reactor.CanAct() || reactor.IsActionBlocked()) return;
        if (reactor.oaState != null && !reactor.oaState.CanTakeOA()) return;

        reactor.armedReaction = null;
        reactor.oaState?.RecordOA();

        CombatLog.Add($"{reactor.unitName} reacts with {reaction.definition.DisplayName}!");
        if (reaction.definition is PsionicPowerDefinition)
            AudioEventBus.Fire(AudioEventIds.Combat.PsionicManifest);

        ExecuteAction(reaction);
    }

    //Threateners that would take an attack of opportunity against this action (empty if it doesn't provoke)
    private List<OARecord> CollectProvocationOAs(ActionContext ctx)
    {
        var oaQueue = new List<OARecord>();
        var def = ctx.definition;

        bool isRanged = def.attackType == ActionDefinition.AttackType.Ranged;
        if (!isRanged && !def.provokesOA) return oaQueue;

        //Collect all enemy units threatening the actor's current tile(s)
        List<Unit> threateners = ThreatZoneManager.GetThreateningUnits(
            ctx.attacker.position, isEnemy: !ctx.attacker.isEnemy);

        foreach (Unit threatener in threateners)
        {
            //A threatener that cannot detect the mover cannot take an OA against them.
            if (!DetectionStateManager.CanTarget(threatener, ctx.attacker)) continue;

            //Skip if the threatener is hidden from the target and has not opted
            //in to taking OAs while hidden
            if (DetectionStateManager.Get(ctx.attacker, threatener) == DetectionState.Hidden
                && !threatener.oaState.reactWhileHidden)
                continue;

            if (threatener.oaState.CanTakeOA())
                oaQueue.Add(new OARecord(threatener, ctx.attacker, ctx.attacker.position));
        }

        return oaQueue;
    }

    private void CheckActionProvocation(ActionContext ctx)
    {
        var oaQueue = CollectProvocationOAs(ctx);
        if (oaQueue.Count > 0)
            ResolveInterruptOAs(oaQueue);
    }

    public void ResolveForcedMove(Unit target, Vector2Int direction, int magnitude)
    {
        if (target == null || target.IsDead()) return;

        // Juggernaut check: reduce displacement if target has JuggernautEffect
        // and the source is not Large or larger (source is the attacker in context)
        foreach (var effect in target.activeEffects)
        {
            if (effect is JuggernautEffect)
            {
                magnitude -= 1; //1 tile = 5 feet per Juggernaut rank
                break;
            }
        }

        if (magnitude <= 0)
        {
            CombatLog.Add($"{target.unitName} resists forced movement (Juggernaut)");
            return;
        }

        Vector2Int destination = target.position + direction * magnitude;

        //Walk back along the displacement path until we find a valid tile
        while (magnitude > 0)
        {
            destination = target.position + direction * magnitude;

            if (GridManager.Instance.IsWithinBounds(destination) &&
                !IsTileOccupied(destination, target))
                break;

            magnitude--;
        }

        if (magnitude <=0)
        {
            CombatLog.Add($"{target.unitName} forced movement blocked");
            return;
        }

        if (isSimulation)
        {
            simUnits[target].position = destination;
            return;
        }

        GridManager.Instance.MoveUnit(target, destination);
        ThreatZoneManager.UpdateUnit(target);
        CombatLog.Add($"{target.unitName} is forced back to {destination}");
    }

    //Returns false if the action is lost: actor killed by an OA,
    //or a psionic concentration check failed (power points stay spent)
    private bool ResolveProvocation(ActionContext ctx, CombatAction action)
    {
        var power = ctx.definition as PsionicPowerDefinition;

        if (power != null)
            AudioEventBus.Fire(AudioEventIds.Combat.PsionicManifest);

        //Synesthesia and similar: manifesting at all requires a concentration check
        if (power != null)
        {
            int manifestDC = ctx.attacker.GetManifestConcentrationDC(power);
            if (manifestDC > 0)
            {
                bool held = false;
                ConcentrationManager.RequestConcentrationCheck(ctx.attacker, manifestDC,
                    success => held = success);
                if (!held) return false;
            }
        }

        //Defensive Manifesting stance: while threatened, a concentration check replaces provoking.
        //Not threatened: nothing to avoid, so no check
        if (power != null &&
            (ctx.attacker.activeEffects.Exists(e => e.effectName == ConcentrationManager.DefensiveManifestingEffectName) ||
             (ctx.attacker.sourceCharacterData != null && ctx.attacker.sourceCharacterData.defensiveManifesting)) &&
            CollectProvocationOAs(ctx).Count > 0)
        {
            bool defensiveSuccess = false;
            ConcentrationManager.RequestDefensiveCheck(ctx.attacker, power.powerLevel,
                success => defensiveSuccess = success);
            return defensiveSuccess;
        }

        int hpBefore = ctx.attacker.currentHP;

        CheckActionProvocation(ctx);

        //Killed or dropped to dying by an OA: the action is lost
        if (!ctx.attacker.CanAct())
        {
            if (!ctx.attacker.IsDead())
                CombatLog.Add($"{ctx.attacker.unitName} collapses before completing {ctx.definition.DisplayName}.");
            return false;
        }

        //Damaged while manifesting: concentration check or the power is lost
        int damageTaken = hpBefore - ctx.attacker.currentHP;
        if (power != null && damageTaken > 0)
        {
            bool held = false;
            ConcentrationManager.RequestDamagedCheck(ctx.attacker, damageTaken, power.powerLevel,
                success => held = success);
            return held;
        }

        return true;
    }
}