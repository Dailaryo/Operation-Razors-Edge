using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillCheckPreviewUI;

public class CombatManager : MonoBehaviour, ICombatInputHandler
{
    List<Unit> playerUnits;
    List<Unit> enemyUnits;

    ActionDefinition pendingAction = null;

    [SerializeField] private GameFlowManager gameFlowManager;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private MissionUIController missionUI;

    private ActionPlanner actionPlanner = new ActionPlanner();

    private TurnManager turnManager = new TurnManager();

    private ExtractionMissionManager extractionManager;

    private bool emergencyExtractionAvailable = false;
    private bool emergencyExtractionCalled = false;

    public Unit CurrentUnit => currentUnit;

    public bool IsPlayerTurn => turnManager.IsPlayerTurn();

    public System.Action<List<Unit>> OnCombatEnded;

    private EncounterData currentEncounter;
    private MissionState missionState;
    public CreditBalance enemyMaterialPool = new CreditBalance();

    private EscalationHandler escalationHandler;

    //Which side gained surprise. Its units act in the surprise round (Turn 0), along with any unit
    //that always acts in surprise rounds; everyone else is surprised and waits for Turn 1.
    public enum SurpriseSide { None, Squad, Enemies }

    public List<Unit> GetAllUnits()
    {
        List<Unit> all = new List<Unit>();
        all.AddRange(playerUnits);
        all.AddRange(enemyUnits);
        return all;
    }

    public int GetCurrentRound() => turnManager.turnCount;

    private void HandleUnitDeath(Unit unit)
    {
        ProcessDefeat(unit);

        if (unit.sourceCharacterData == null) return;
        if (unit.sourceCharacterData.deathRound >= 0) return; // Already stamped — guard against double-fire

        unit.sourceCharacterData.deathRound = turnManager.turnCount;
        CombatLog.Add($"{unit.unitName} died on round {turnManager.turnCount}.");
    }

    //Removes a defeated unit from the encounter: incapacitation and snapshot rollback for squad members,
    //loot for enemies. Runs once per unit, whatever caused the defeat (damage, bleed-out, Kill()).
    public void ProcessDefeat(Unit unit)
    {
        if (unit.defeatProcessed) return;
        unit.defeatProcessed = true;
        unit.EndConcentration("defeated"); //a dead manifester's maintained powers (Inflict Pain repeats) end with them

        if (!unit.isEnemy)
            SquadManager.Instance.SetIncapacitated(unit);
        else if (currentEncounter == null || !currentEncounter.isTraining)
            StageLoot(unit); //D8: no loot from drills

        ThreatZoneManager.UnregisterUnit(unit);
        GridManager.Instance.RemoveUnit(unit);

        CombatLog.Add($"{unit.unitName} has been defeated");
        AudioEventBus.Fire(AudioEventIds.Combat.UnitDeath);
    }

    //Out of the fight for good without dying (Microcosm). Squad members are unavailable until the Hub,
    //with no snapshot rollback. Enemies removed this way count as defeated.
    public void RemoveFromEncounter(Unit unit)
    {
        if (unit.isEnemy)
        {
            unit.Kill();
            return;
        }

        if (unit.defeatProcessed) return;
        unit.defeatProcessed = true;
        unit.removedFromEncounter = true;
        unit.EndConcentration("removed from the encounter");

        SquadManager.Instance.SetIncapacitated(unit, rollback: false);

        ThreatZoneManager.UnregisterUnit(unit);
        GridManager.Instance.RemoveUnit(unit);

        CombatLog.Add($"{unit.unitName} is unable to continue");
    }

    //Enemy drops: equipped items always, plus the loot table
    void StageLoot(Unit unit)
    {
        List<ItemInstance> lootQueue = new List<ItemInstance>();

        if (unit.sourceCharacterData != null)
        {
            foreach (var kvp in unit.sourceCharacterData.equippedItemIds)
                lootQueue.Add(kvp.Value);

            if (!string.IsNullOrEmpty(unit.sourceCharacterData.lootTableId))
                lootQueue.AddRange(LootResolver.ResolveLoot(unit.sourceCharacterData.lootTableId));
        }

        //Stage drops for post-combat loot screen
        LootQueue.Enqueue(lootQueue);

        if (lootQueue.Count > 0)
            CombatLog.Add($"[Loot] {unit.unitName} dropped {lootQueue.Count} item(s)");
    }

    public ActionDefinition GetPendingAction()
    {
        return pendingAction;
    }

    CombatResolver GetValidationResolver()
    {
        return new CombatResolver(this, false);
    }

    public event System.Action<List<Unit>> OnTargetSelectionRequested;
    public event System.Action<Unit> OnActionBarRefreshRequested;
    public event System.Action<SkillCheckPreviewUI.SkillCheckPreview> OnSkillCheckPreviewRequested;
    public event System.Action<ObjectiveDefinition> OnSecondaryObjectiveRevealed;

    private Unit currentUnit;

    bool isProcessingTurn = false;

    //Tiles enemies have claimed this round while planning, so later enemies plan around them. Cleared after resolution.
    private HashSet<Vector2Int> enemyPlannedDestinations = new HashSet<Vector2Int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SkillCheckExecutor.OnPreviewRequested += HandleExecutorPreview;
        CombatLog.Add("Starting test combat...");
    }

    IEnumerator RunTurn()
    {
        if (isProcessingTurn) yield break;

        isProcessingTurn = true;

        //Surprise round (Turn 0): only units flagged actsInSurpriseRound act, on either side
        bool isSurpriseRound = turnManager.CurrentState == TurnManager.CombatState.SurpriseRound;

        //Gather all actions (player and enemy)
        List<CombatAction> actions = new List<CombatAction>();

        actions.AddRange(actionPlanner.GetAllActionsFlat());

        CombatLog.Add(isSurpriseRound
            ? "=== RESOLVING SURPRISE ROUND (Turn 0) ==="
            : $"=== RESOLVING TURN {turnManager.turnCount} ===");

        enemyPlannedDestinations.Clear();

        foreach (Unit unit in enemyUnits)
        {
            if (!unit.CanAct()) continue;

            //Surprise round: only enemies on the surprising side (or that always act in surprise rounds) plan
            if (isSurpriseRound && !unit.actsInSurpriseRound) continue;

            currentUnit = unit;

            yield return new WaitForSeconds(0.4f);

            EnemyPlanner planner = new EnemyPlanner();

            var planned = planner.Plan(unit, playerUnits, this);

            Debug.Log($"Enemy {unit.unitName} planned {planned.Count} actions");
            foreach (var a in planned)
            {
                Debug.Log($" - {a.definition.actionID} targeting {a.target?.unitName}");
            }

            foreach (var act in planned)
            {
                actions.Add(act);

                if (act.definition.IsMove() && act.movePosition.HasValue)
                    enemyPlannedDestinations.Add(act.movePosition.Value);
            }
        }

        CombatResolver resolver = new CombatResolver(this, false);

        turnManager.SetState(TurnManager.CombatState.Resolving);

        yield return resolver.ResolveTurn(actions);
        enemyPlannedDestinations.Clear(); //stale after resolution; never shown to the squad's planning highlights

        //End of turn processing: effects, cooldowns, escalation (read turnCount = the turn just resolved)
        ProcessEndPhase();

        //Cards show this turn's damage, bleed-out and incapacitation
        if (PersistentChromeController.Instance != null)
            PersistentChromeController.Instance.RefreshPortraits();

        isProcessingTurn = false;

        //Objectives are evaluated against the turn just resolved; CheckCombatEnd advances the turn if combat continues
        CheckCombatEnd();
    }

    void CheckCombatEnd()
    {
        CombatContext context = new CombatContext(
            playerUnits,
            enemyUnits,
            turnManager.turnCount,
            gridManager,
            missionState
        );

        CombatEndState endState = MissionObjectiveEvaluator.EvaluateAll(
            currentEncounter,
            missionState,
            context,
            extractionManager
        );

        if (endState == CombatEndState.InProgress)
        {
            //Leaving the surprise round: everyone gets the full action economy from Turn 1
            bool leavingSurpriseRound = turnManager.turnCount == 0;

            //Next turn: surprise round (0) -> Turn 1, Turn N -> N + 1
            turnManager.IncrementTurn();

            if (leavingSurpriseRound)
            {
                foreach (Unit unit in GetAllUnits())
                    unit.limitedToSingleAction = false;
            }

            //Round start: Read the Room repeats Sweep; Empath Apotheosis refreshes
            SweepResolver.Run(GetAllUnits(), encounterStart: false);
            AwarenessManager.ApplyApotheosis(GetAllUnits());

            StartPlayerTurn();
            return;
        }

        FinishCombat(endState);
    }

    //End of combat: squad members still down sit out until the Hub, then clean up and hand off to GameFlowManager.
    //Shared by the objective check and the debug exit
    void FinishCombat(CombatEndState endState)
    {
        //Squad members still down at the end (Disabled, Dying or Stabilized) sit out until the Hub.
        //They survived, so no snapshot rollback (same path as removal by Microcosm)
        foreach (Unit unit in playerUnits)
        {
            if (unit.defeatProcessed || unit.currentHP > 0) continue;

            unit.defeatProcessed = true;
            SquadManager.Instance.SetIncapacitated(unit, rollback: false, downAtEncounterEnd: true);
        }

        CleanUpUnits();
        OnCombatEnded?.Invoke(playerUnits);
        gameFlowManager.EndCombat(endState);
    }

    void StartPlayerTurn()
    {
        turnManager.SetState(TurnManager.CombatState.PlayerTurn);

        turnManager.ResetTurn();
        actionPlanner.Clear();

        CombatLog.Add($"=== TURN {turnManager.turnCount}: PLAYER TURN ===");

        PromptNextUnit();
    }

    void StartSurpriseRound()
    {
        turnManager.SetState(TurnManager.CombatState.SurpriseRound);
        turnManager.ResetTurn();
        actionPlanner.Clear();

        // Sort player units by initiative descending so the surprise round
        // respects the same order that normal combat will use.
        playerUnits.Sort((a, b) => b.initiativeRoll.CompareTo(a.initiativeRoll));

        CombatLog.Add("=== SURPRISE ROUND (Turn 0) ===");
        CombatLog.Add("Only units aware of their opponents act this round.");
        AudioEventBus.Fire(AudioEventIds.Stealth.SurpriseRoundBegin);

        //d20: units acting in the surprise round get a single move or standard action (swift and free still allowed)
        foreach (Unit unit in GetAllUnits())
            unit.limitedToSingleAction = unit.actsInSurpriseRound;

        PromptNextUnit();
    }

    void PromptNextUnit()
    {
        if (isProcessingTurn) return;

        //if all units have chosen actions
        if (turnManager.GetCurrentIndex() >= playerUnits.Count)
        {
            GridHighlighter.Instance.RefreshUnitHighlights(this);

            if (turnManager.CurrentState == TurnManager.CombatState.SurpriseRound)
            {
                //Resolve the surprise round (RunTurn plans only flagged enemies), then Turn 1 begins
                StartCoroutine(RunTurn());
                return;
            }

            turnManager.SetState(TurnManager.CombatState.EnemyTurn);
            StartCoroutine(RunTurn());
            return;
        }

        Unit unit = playerUnits[turnManager.GetCurrentIndex()];
        currentUnit = unit;

        //Skip dead, dying and stabilized units, and units surprised in the surprise round
        bool surprised = turnManager.CurrentState == TurnManager.CombatState.SurpriseRound && !unit.actsInSurpriseRound;

        if (!unit.CanAct() || surprised)
        {
            turnManager.AdvanceUnit();
            PromptNextUnit();
            return;
        }

        OnActionBarRefreshRequested?.Invoke(unit);

        GridHighlighter.Instance.ClearAll();

        //Highlight valid moves for current unit
        if (currentUnit != null && !currentUnit.IsDead())
        {
            GridHighlighter.Instance.HighlightValidMoves(currentUnit, this);
        }

        CombatLog.Add(unit.unitName + ": Choose Action");

        var unitActions = actionPlanner.GetActions(unit);

        if (unitActions.Count > 0)
        {
            CombatLog.Add("Action selected. Choose another or right-click to cancel");
        }
    }

    void ProcessEndPhase()
    {
        //Apply end-of-turn effect logic, then reduce ability cooldowns. Dead units are skipped:
        //their effects (e.g. Inflict Pain on a target that already died) must not keep resolving
        foreach (Unit unit in playerUnits)
        {
            if (unit.IsDead()) continue;

            unit.ProcessEffects();
            unit.ReduceCooldowns();
            unit.CheckSavageMomentum(turnManager.turnCount);
        }

        foreach (Unit unit in enemyUnits)
        {
            if (unit.IsDead()) continue;

            unit.ProcessEffects();
            unit.ReduceCooldowns();
            unit.ResetInvulnerableTarget(); //training targets only; no-op for everything else
        }

        //Fast healing (Adaptive Physiology, Rapid Healing; Pathfinder): regain HP each round up to max HP at any HP above
        //the death threshold, so a downed unit climbs back up. FastHealingSuppressedEffect stops it for a round
        foreach (Unit unit in GetAllUnits())
        {
            if (unit.IsDead()) continue;

            //Fast healing stopped this end phase (FastHealingSuppressedEffect: e.g. acid or fire against a troll)
            if (unit.activeEffects.Find(e => e is FastHealingSuppressedEffect) is FastHealingSuppressedEffect suppressed)
            {
                if (unit.GetStat(StatType.FastHealing) > 0)
                    CombatLog.Add($"{unit.unitName}'s fast healing is suppressed this round.");
                suppressed.ConsumeRound(unit);
                continue;
            }

            int fastHealing = unit.GetStat(StatType.FastHealing);
            int maxHP = unit.GetMaxHP();
            if (fastHealing <= 0 || unit.currentHP >= maxHP) continue;

            bool wasDown = unit.currentHP <= 0;
            int healed = Mathf.Min(fastHealing, maxHP - unit.currentHP);
            unit.currentHP += healed;
            CombatLog.Add($"{unit.unitName} regains {healed} HP from fast healing. HP: {unit.currentHP}");

            //Back above 0 HP: Disabled, Dying and Stabilized end
            if (wasDown && unit.currentHP > 0)
            {
                unit.RemoveEffect<DisabledState>();
                unit.RemoveEffect<DyingState>();
                unit.RemoveEffect<StabilizedState>();
                CombatLog.Add($"{unit.unitName} is back on their feet.");
            }

            unit.OnHealthChanged?.Invoke(unit);
        }

        //Flat-footed ends once a unit's turn has come round, even if it planned nothing:
        //everyone after a full round, surprise-round actors after the surprise round
        foreach (Unit unit in GetAllUnits())
            if (turnManager.turnCount >= 1 || unit.actsInSurpriseRound)
                unit.SetFlatFooted(false);

        DetectionStateManager.TickAlertCountDowns();
        FieldZoneManager.ProcessEndPhase(GetAllUnits());

        //Auras: refreshed every end phase (the buffs last 1 turn, so they carry to the next one)
        ApplyAuras();

        //Escalation check
        int liveEnemyCount = enemyUnits.FindAll(u => !u.IsDead()).Count;
        EscalationHandler.EscalationResult escalationResult = escalationHandler.CheckEscalation(turnManager.turnCount, liveEnemyCount);
        PlaceReinforcements(escalationResult);

        // Check if emergency extraction should become available
        if (!emergencyExtractionAvailable && extractionManager == null)
            CheckEmergencyExtractionEligibility();

        foreach (Unit unit in GetAllUnits())
        {
            if (unit.oaState != null)
                unit.oaState.ResetRound();

            unit.armedReaction = null; //an unfired reaction expires with the round
            unit.triggerAttemptsThisRound.Clear();
        }

        CombatLog.Add($"--- End Phase Complete ---");
        CombatLog.Add($"Turn {turnManager.turnCount} complete");
    }

    //Auras (Tactical Uplink, Protective Aura): every living source refreshes its allies' buffs. Both effect lists
    //are checked, so an aura works as a shared passive or an instanced effect. activeEffects is copied because an
    //aura can buff its own source (Tactical Uplink includes Alexandra). Runs at combat start and every end phase
    void ApplyAuras()
    {
        foreach (Unit unit in GetAllUnits())
        {
            if (unit.IsDead()) continue;

            foreach (var effect in new List<StatusEffect>(unit.activeEffects))
                effect.OnEndPhaseAura(unit, GetAllUnits());

            foreach (var source in unit.effectSources)
                foreach (var effect in source.GetEffects(unit))
                    effect.OnEndPhaseAura(unit, GetAllUnits());
        }
    }

    public void StartCombat(EncounterData encounter, List<Unit> playerUnitsInput, List<Unit> enemyUnitsInput, SurpriseSide surpriseSide = SurpriseSide.None)
    {
        if (missionUI != null)
        {
            missionUI.Initialize(encounter, turnManager, this);
        }

        currentEncounter = encounter;
        //Encounter material pool: shared by every enemy (escalation waves included) for material components
        enemyMaterialPool = new CreditBalance();
        enemyMaterialPool.Add(encounter.materialPoolCredits);
        escalationHandler = new EscalationHandler(encounter);
        extractionManager = encounter.extractionConfig != null
            ? new ExtractionMissionManager(
                encounter.extractionConfig.extractionZoneIds,
                encounter.extractionConfig.extractionDeadline)
            : null;

        if (extractionManager != null)
            CombatLog.Add($"[Extraction] Deadline set: turn {encounter.extractionConfig.extractionDeadline}.");
        bool hasSurpriseRound = surpriseSide != SurpriseSide.None;
        turnManager.ResetTurnCount(hasSurpriseRound);
        missionState = new MissionState();

        CombatLog.Add("Intializing Combat...");

        //determine squads
        playerUnits = new List<Unit>(playerUnitsInput);
        enemyUnits = new List<Unit>(enemyUnitsInput);

        NumberStartingEnemies(enemyUnits);

        //Roll Initiative
        RollInitiative(playerUnits);
        RollInitiative(enemyUnits);

        GridManager grid = gridManager;

        //Player side: spawn from the map's entry edge, filling inward
        PlaceUnitsAtEdge(playerUnits, grid.squadEntryZone, "player");

        if (playerUnits.Count == 0)
        {
            Debug.LogError("No player units available for combat!");
        }

        //Enemy side: spawn from the edge opposite the squad's entry, filling inward
        PlaceUnitsAtEdge(enemyUnits, grid.EnemyEntryZone, "enemy",
            grid.GetZoneTiles(GridManager.EnemySpawnZoneId)); //authored spawn tiles first, if the map has any

        foreach (Unit unit in playerUnits)
        {
            UnitViewManager.Instance.CreateView(unit);
        }

        foreach (Unit unit in enemyUnits)
        {
            UnitViewManager.Instance.CreateView(unit);
        }

        // Refresh all positions now that GridVisualizer is fully initialized
        UnitViewManager.Instance.RefreshAllPositions();

        foreach (Unit unit in GetAllUnits())
        {
            unit.oaState = new OAState(1);
            unit.oaState.reactWhileHidden = unit.isEnemy;

            // Reset FullOnCombatStart resources
            if (unit.resource != null &&
                unit.resource.definition.regenMode ==
                ClassDefinition.ResourceDefinition.RegenMode.FullOnCombatStart)
            {
                unit.resource.current = unit.resource.max;
            }

            // Reset once-per-combat cooldowns
            List<ActionDefinition> keys = new List<ActionDefinition>(unit.cooldowns.Keys);
            foreach (var action in keys)
            {
                if (unit.cooldowns[action] == Unit.CombatCooldown)
                    unit.cooldowns.Remove(action);
            }

            unit.combatUses.Clear();
            unit.rampageRound = -1;
            unit.ResetSavageMomentum();

            // Reset per-combat effect flags
            foreach (var effect in unit.activeEffects)
            {
                if (effect is KineticAbsorptionEffect ka) ka.ResetForCombat();
                if (effect is ElectronicShroudEffect es) es.ResetForCombat();
                if (effect is ResilienceEffect res) res.ResetForCombat();
            }

            // Reset death round tracking for miracle actions
            if (unit.sourceCharacterData != null)
                unit.sourceCharacterData.deathRound = -1;

            // Subscribe to OnDeath for miracle window tracking
            unit.OnDeath += HandleUnitDeath;

            // Re-apply Combat Reflexes if the unit has it, since oaState
            // didn't exist when ApplyFeats() ran earlier
            if (unit.sourceCharacterData != null &&
                unit.sourceCharacterData.HasFeat(FeatIds.CombatReflexes))
            {
                int agilityMod = unit.GetModifier(StatType.Agility);
                unit.oaState.maxPerRound = 1 + Mathf.Max(0, agilityMod);
                unit.oaState.actsWhileFlatFooted = true; //Pathfinder Combat Reflexes
            }

            //Everyone starts combat flat-footed until their first action
            unit.SetFlatFooted(true);
        }

        ThreatZoneManager.Reset();

        foreach (Unit unit in GetAllUnits())
        {
            ThreatZoneManager.RegisterUnit(unit);
        }

        //Surprise round participants: the surprising side, plus anyone who always acts in surprise rounds
        List<Unit> surprisingSide = surpriseSide == SurpriseSide.Squad ? playerUnits
            : surpriseSide == SurpriseSide.Enemies ? enemyUnits
            : null;

        foreach (Unit unit in GetAllUnits())
            unit.actsInSurpriseRound = hasSurpriseRound &&
                (surprisingSide.Contains(unit) || unit.AlwaysActsInSurpriseRound());

        //The surprising side starts hidden from the other.
        //DEFERRED: a unit that always acts in surprise rounds is aware of its attackers, so it should start with them
        //Detected rather than Hidden. Wire this up with the first ability that grants ActsInSurpriseRound.
        DetectionStateManager.Initialize(GetAllUnits(), surprisingSide);

        AwarenessManager.Initialize(GetAllUnits());
        AwarenessManager.ApplyApotheosis(GetAllUnits());
        AuraOfCalm.Initialize(GetAllUnits());
        SweepResolver.Run(GetAllUnits(), encounterStart: true);
        //Passive auras are on from the start: the end phase refreshes them from round 1 on
        ApplyAuras();

        if (hasSurpriseRound)
            StartSurpriseRound();
        else
            StartPlayerTurn();
        AudioEventBus.Fire(AudioEventIds.GameFlow.CombatStart);

        if (PersistentChromeController.Instance != null)
            PersistentChromeController.Instance.RefreshPortraits();
    }

    //Fighting Defensively toggle (action bar): planned at most once per round, removed if pressed again.
    //Resolves at the start of execution (CombatResolver) and only applies if the unit also attacks
    void ToggleFightingDefensively(Unit unit)
    {
        if (RemovePlannedFightingDefensively(unit, "Fighting Defensively off"))
        {
            OnActionBarRefreshRequested?.Invoke(unit);
            return;
        }

        if (actionPlanner.GetActions(unit).Exists(a => a.definition.actionID == ActionIds.TotalDefense))
        {
            CombatLog.Add($"{unit.unitName} is in Total Defense: Fighting Defensively can't be combined with it.");
            return;
        }

        CombatLog.Add($"{unit.unitName}: Fighting Defensively on (-4 attack, +2 Defense this round if attacking).");
        AddPlannedAction(new CombatAction(unit, null, ActionDatabase.Definitions[ActionIds.FightingDefensively], this));
    }

    bool RemovePlannedFightingDefensively(Unit unit, string reason)
    {
        int removed = actionPlanner.GetActions(unit).RemoveAll(a => a.definition.actionID == ActionIds.FightingDefensively);
        if (removed > 0) CombatLog.Add($"{unit.unitName}: {reason}.");
        return removed > 0;
    }

    //Maximize Power (Telepath): armed from the Abilities tab, applies to the next power this unit plans.
    //Not a planned action: off once a power is planned, on right-click, or when the turn is confirmed
    Unit maximizeArmedFor;

    public bool IsMaximizeArmed(Unit unit) => unit != null && maximizeArmedFor == unit;

    void SetMaximize(Unit unit, bool on)
    {
        if (unit == null || on == IsMaximizeArmed(unit)) return;

        maximizeArmedFor = on ? unit : null;
        CombatLog.Add(on
            ? $"{unit.unitName}: Maximize Power on. Choose a power (+{AugmentationProcessor.MaximizePowerCost} PP); right-click to cancel."
            : $"{unit.unitName}: Maximize Power off.");
        OnActionBarRefreshRequested?.Invoke(unit);
    }

    //True if this power can be planned maximized now: it rolls variable dice, and its cost +6 PP fits both the
    //unit's remaining power points (after earlier planned powers) and the manifestation limit
    public bool CanMaximize(Unit unit, PsionicPowerDefinition power)
    {
        if (!power.hasVariableDice) return false;

        ActionValidator.GetCommittedPowerPoints(unit, actionPlanner.GetActions(unit), null,
            out int committedPP, out int committedFreeUses);

        return AugmentationProcessor.Validate(unit, power, null, committedPP, committedFreeUses, maximized: true).isValid;
    }

    //Manifest Defensively is a toggle for the round, like Power Attack: while threatened, this unit's powers this
    //round use a concentration check instead of provoking. Dropped at resolution if it manifests nothing
    void ToggleManifestDefensively(Unit unit)
    {
        int removed = actionPlanner.GetActions(unit).RemoveAll(a => a.definition.actionID == ActionIds.ManifestDefensively);
        if (removed > 0)
        {
            CombatLog.Add($"{unit.unitName}: Manifest Defensively off.");
            OnActionBarRefreshRequested?.Invoke(unit);
            return;
        }

        CombatLog.Add($"{unit.unitName}: Manifest Defensively on (concentration check instead of provoking this round).");
        AddPlannedAction(new CombatAction(unit, null, ActionDatabase.Definitions[ActionIds.ManifestDefensively], this));
    }

    //Power Attack (melee) and Deadly Aim (ranged) are toggles for the round, like Fighting Defensively (all can be
    //combined). Turning one on goes straight to that attack's targeting if no attack of that type is planned yet;
    //right-click cancels the targeting and leaves the stance on
    void ToggleAttackStance(Unit unit, string stanceId, bool ranged)
    {
        ActionDefinition stance = ActionDatabase.Definitions[stanceId];
        string attackWord = ranged ? "ranged" : "melee";

        int removed = actionPlanner.GetActions(unit).RemoveAll(a => a.definition.actionID == stanceId);
        if (removed > 0)
        {
            CombatLog.Add($"{unit.unitName}: {stance.DisplayName} off.");
            OnActionBarRefreshRequested?.Invoke(unit);
            return;
        }

        int penalty = PowerAttackEffect.GetPenalty(unit);
        CombatLog.Add($"{unit.unitName}: {stance.DisplayName} on (-{penalty} {attackWord} attack, " +
                      $"+{penalty * 2} {attackWord} damage this round).");
        AddPlannedAction(new CombatAction(unit, null, stance, this));

        var attackType = ranged ? ActionDefinition.AttackType.Ranged : ActionDefinition.AttackType.Melee;
        var planned = actionPlanner.GetActions(unit);
        bool stanceOn = planned.Exists(a => a.definition.actionID == stanceId);
        bool attackPlanned = planned.Exists(a => a.definition.attackType == attackType);
        ActionDefinition attack = unit.GetAllActions()
            .Find(a => a.actionID == (ranged ? ActionIds.RangedAttack : ActionIds.MeleeAttack));

        if (stanceOn && !attackPlanned && attack != null && currentUnit == unit && unit.IsActionAvailable(attack))
            PlayerChooseAction(attack);
    }


    public void PlayerChooseAction(ActionDefinition action)
    {
        //Get current unit
        Unit unit = CurrentUnit;

        if (!CanPlan || unit == null || unit.IsDead()) return;

        //Fighting Defensively is a stance for the round, not a separate action: the button toggles it
        if (action.actionID == ActionIds.FightingDefensively)
        {
            ToggleFightingDefensively(unit);
            return;
        }

        if (action.actionID == ActionIds.PowerAttack || action.actionID == ActionIds.DeadlyAim)
        {
            ToggleAttackStance(unit, action.actionID, ranged: action.actionID == ActionIds.DeadlyAim);
            return;
        }


        if (action.actionID == ActionIds.MaximizePower)
        {
            SetMaximize(unit, !IsMaximizeArmed(unit));
            return;
        }

        if (action.actionID == ActionIds.ManifestDefensively)
        {
            ToggleManifestDefensively(unit);
            return;
        }

        //Pathfinder: Total Defense can't be combined with Fighting Defensively
        if (action.actionID == ActionIds.TotalDefense)
            RemovePlannedFightingDefensively(unit, "Total Defense replaces Fighting Defensively");

        //Targeting actions
        var def = action;

        switch (def.actionID)
        {
            case ActionIds.Feint:
                SkillCheckExecutor.RequestPreview(SkillCheckIds.FeintOpposed, unit);
                break;

            case ActionIds.AcrobaticMovement:
                SkillCheckExecutor.RequestPreview(SkillCheckIds.AcrobaticMovementDC, unit);
                break;

            case ActionIds.Demoralize:
                SkillCheckExecutor.RequestPreview(SkillCheckIds.DemoralizeOpposed, unit);
                break;
        }

        //Clear old highlights
        GridHighlighter.Instance.ClearAll();

        //Targeted actions
        if (def.requiresTarget)
        {
            pendingAction = action;

            GridHighlighter.Instance.HighlightValidTargets(unit, action, this);

            switch (def.targetingMode)
            {
                case ActionDefinition.TargetingMode.UI:
                    OnTargetSelectionRequested?.Invoke(GetAllUnits());
                    break;

                case ActionDefinition.TargetingMode.Tile:
                    //UI will handle hiding itself if needed
                    break;
            }

            return;
        }

        //Ground-targeted actions (Deploy Field, Break Window): wait for a tile click in SelectTile
        if (def.IsGroundTargeted())
        {
            pendingAction = action;
            GridHighlighter.Instance.HighlightValidTargets(unit, action, this);
            return;
        }

        //Immediate self/non-target actions
        CombatAction newAction = new CombatAction(
            unit,
            null,
            action,
            this
        );

        if (!ActionValidator.IsValid(newAction, GetValidationResolver()))
        {
            //Debug.Log("Invalid self action");
            return;
        }

        AddPlannedAction(newAction);

        //Debug.Log($"Executed immediate action: {action}");
    }

    public void SetCurrentUnit(Unit unit)
    {
        currentUnit = unit;
    }

    //Drops any action waiting for a target and hides the target-selection UI.
    //Used when a planned action is rejected and when the unit confirms its turn
    void ClearTargeting()
    {
        pendingAction = null;
        OnTargetSelectionRequested?.Invoke(null);
    }

    public void AddPlannedAction(CombatAction action)
    {
        //Maximize Power armed: the next power planned is maximized (validated with the +6 PP)
        bool maximizing = IsMaximizeArmed(action.source) && action.definition is PsionicPowerDefinition;
        if (maximizing) action.maximize = true;

        if (!ActionValidator.IsValid(action, GetValidationResolver()))
        {
            CombatLog.Add("Invalid Action");
            ClearTargeting();

            GridHighlighter.Instance.ClearAll();

            if (currentUnit != null && !currentUnit.IsDead())
            {
                GridHighlighter.Instance.HighlightValidMoves(currentUnit, this);
            }

            return;
        }

        Unit unit = action.source;

        bool success = actionPlanner.TryAddAction(action);

        if (!success)
        {
            CombatLog.Add($"Not enough action resources for {action.definition.DisplayName}");
            ClearTargeting();

            GridHighlighter.Instance.ClearAll();

            if (currentUnit != null && !currentUnit.IsDead())
            {
                GridHighlighter.Instance.HighlightValidMoves(currentUnit, this);
            }

            return;
        }

        if (maximizing)
        {
            maximizeArmedFor = null;
            CombatLog.Add($"{unit.unitName}: {action.definition.DisplayName} will be maximized " +
                          $"(+{AugmentationProcessor.MaximizePowerCost} PP).");
        }

        var unitActions = actionPlanner.GetActions(unit);

        pendingAction = default;

        PromptNextUnit();
    }

    public void ClearPendingAction()
    {
        pendingAction = default;
    }

    public Vector2Int GetPlannedPosition(Unit unit)
    {
        var actions = actionPlanner.GetActions(unit);

        foreach (var act in actions)
        {
            if (act.definition.IsMove() && act.movePosition.HasValue)
                return act.movePosition.Value;
        }

        return unit.position;
    }

    public bool HasPlannedMove(Unit unit)
    {
        var actions = actionPlanner.GetActions(unit);

        return actions.Exists(a => a.definition.IsMove() && a.movePosition.HasValue);
    }

    public void CancelPlannedMove(Unit unit)
    {
        var actions = actionPlanner.GetActions(unit);
        actions.RemoveAll(a => a.definition.IsMove());

        pendingAction = null;

        if (currentUnit == unit && !unit.IsDead())
        {
            GridHighlighter.Instance.HighlightValidMoves(unit, this);
            CombatLog.Add(unit.unitName + " cleared planned move.");
        }
    }

    public void CancelCurrentUnitAction()
    {
        if (!CanPlan || currentUnit == null) return;

        var actions = actionPlanner.GetActions(currentUnit);

        //First handle targeting state
        if (pendingAction != null)
        {
            pendingAction = null;
            SetMaximize(currentUnit, false); //cancelling a maximized power's targeting ends Maximize too

            CombatLog.Add(currentUnit.unitName + " canceled targeting.");

            GridHighlighter.Instance.ClearAll();
            GridHighlighter.Instance.HighlightValidMoves(currentUnit, this);

            OnTargetSelectionRequested?.Invoke(null);
            return;
        }

        //Maximize Power armed with nothing targeted: right-click turns it off
        if (IsMaximizeArmed(currentUnit))
        {
            SetMaximize(currentUnit, false);
            return;
        }

        //Then handle planned actions
        if (actions.Count == 0)
        {
            CombatLog.Add("No actions to cancel.");
            return;
        }

        //Remove last action
        actions.RemoveAt(actions.Count - 1);

        CombatLog.Add(currentUnit.unitName + " canceled last action.");
        OnActionBarRefreshRequested?.Invoke(currentUnit); //stance labels ("Power Attack: On") and resource counts update

        pendingAction = null;

        GridHighlighter.Instance.ClearAll(); //drop overlays from the cancelled action before redrawing
        GridHighlighter.Instance.HighlightValidMoves(currentUnit, this);

        OnTargetSelectionRequested?.Invoke(null);
    }

    public bool IsTilePlannedOccupied(Vector2Int pos)
    {
        foreach (var kvp in actionPlanner.GetAllPlanned())
        {
            var actions = kvp.Value;

            foreach (var act in actions)
            {
                if (act.definition.IsMove() && act.movePosition.HasValue)
                {
                    if (act.movePosition.Value == pos) return true;
                }
            }
        }

        //Enemy destinations claimed earlier in this round's planning
        if (enemyPlannedDestinations.Contains(pos)) return true;

        return false;
    }

    public bool IsUnitLeavingTile(Unit unit, Vector2Int pos)
    {
        var actions = actionPlanner.GetActions(unit);

        foreach (var act in actions)
        {
            if (act.definition.actionID == ActionIds.Move && act.movePosition.HasValue)
            {
                return unit.position == pos;
            }
        }

        return false;
    }

    public List<CombatAction> GetPlannedActions(Unit unit)
    {
        return actionPlanner.GetActions(unit);
    }

    public void SelectAction(ActionDefinition action)
    {
        PlayerChooseAction(action);
    }

    public void SelectTarget(Unit target)
    {
        HandleTargetSelection(target);
    }

    public void Cancel()
    {
        CancelCurrentUnitAction();
    }

    public void HandleTargetSelection(Unit target)
    {
        if (!CanPlan) return;

        if (pendingAction == null)
        {
            OnTargetSelectionRequested?.Invoke(null);
            return;
        }

        if (target == null || target.IsDead())
        {
            CombatLog.Add("Invalid target!");
            OnTargetSelectionRequested?.Invoke(null);
            return;
        }

        Unit unit = CurrentUnit;

        CombatAction action = new CombatAction(
            unit,
            target,
            pendingAction,
            this
        );

        AddPlannedAction(action);
    }

    public void SelectTile(Vector2Int position)
    {
        Unit unit = CurrentUnit;

        if (!CanPlan || unit == null || unit.IsDead()) return;

        ActionDefinition pending = pendingAction;

        //If we are targeting an ability
        if (pending != null)
        {
            var def = pending;

            //Ground-targeted actions take the clicked tile itself, occupied or not
            if (def.IsGroundTargeted())
            {
                CombatAction tileAction = new CombatAction(unit, null, def, this);
                tileAction.SetMovePosition(position);
                AddPlannedAction(tileAction);
                return;
            }

            if (def.requiresTarget)
            {
                //Try to resolve unit on tile
                Unit target = GridManager.Instance.GetUnitAt(position);

                if (target != null)
                {
                    HandleTargetSelection(target);
                    return;
                }

                //FUTURE: tile-based abilities go here
                CombatLog.Add("Invalid target");
                return;
            }
        }

        //Otherwise treat as movement
        HandleMoveSelection(position);
    }

    private void HandleMoveSelection(Vector2Int position)
    {
        Unit unit = CurrentUnit;

        var moveDef = ActionDatabase.Definitions[ActionIds.Move];

        CombatAction moveAction = new CombatAction(
            unit,
            null,
            moveDef,
            this
        );

        moveAction.SetMovePosition(position);

        AddPlannedAction(moveAction);
    }

    void CleanUpUnits()
    {
        foreach (var unit in playerUnits)
        {
            GridManager.Instance.RemoveUnit(unit);
            UnitViewManager.Instance.ClearAll();
        }

        foreach (var unit in enemyUnits)
        {
            GridManager.Instance.RemoveUnit(unit);
            UnitViewManager.Instance.ClearAll();
        }

        ThreatZoneManager.Reset();
        FieldZoneManager.Reset();
        DetectionStateManager.Reset();
    }

    void RollInitiative(List<Unit> units)
    {
        foreach (var unit in units)
        {
            int roll = Random.Range(1, 21);
            int stat = unit.GetDerivedStat(StatType.Initiative);

            unit.initiativeRoll = roll + stat;

            Debug.Log($"{unit.unitName} initiative set: {roll} + {stat} = {unit.initiativeRoll}");
        }
    }

    public void ConfirmTurn()
    {
        if (!CanPlan || currentUnit == null || currentUnit.IsDead()) return;

        var unitActions = actionPlanner.GetActions(currentUnit);

        CombatLog.Add($"{currentUnit.unitName} confirmed turn.");

        //An action still waiting for a target (or one rejected mid-targeting) must not carry over to the next unit
        ClearTargeting();
        SetMaximize(currentUnit, false);

        turnManager.AdvanceUnit();
        PromptNextUnit();
    }

    public void RevealSecondaryObjective(ObjectiveDefinition objective)
    {
        if (objective == null || objective.visibleAtStart) return;

        objective.visibleAtStart = true;
        CombatLog.Add($"[Mission] New objective revealed: {objective.description}");
        OnSecondaryObjectiveRevealed?.Invoke(objective);
    }

    private void HandleExecutorPreview(SkillCheckPreviewUI.SkillCheckPreview preview)
    {
        OnSkillCheckPreviewRequested?.Invoke(preview);
    }

    private void OnDestroy()
    {
        SkillCheckExecutor.OnPreviewRequested -= HandleExecutorPreview;
    }

    public MissionState GetMissionState()
    {
        return missionState;
    }

    // Places each unit on the first free spawn tile along the given edge.
    // Units that cannot be placed are removed from the encounter with a warning,
    // so they never get a view, a turn, or become targetable off-grid.
    void PlaceUnitsAtEdge(List<Unit> units, MapDefinition.EntryZone edge, string side, List<Vector2Int> preferredTiles = null)
    {
        //Authored tiles first (e.g. the map's enemy spawn zone), then the edge fill for any overflow
        List<Vector2Int> spawnTiles = new List<Vector2Int>();
        if (preferredTiles != null) spawnTiles.AddRange(preferredTiles);
        spawnTiles.AddRange(gridManager.GetSpawnTiles(edge));
        List<Unit> unplaced = new List<Unit>();

        foreach (Unit unit in units)
        {
            bool placed = false;

            foreach (Vector2Int tile in spawnTiles)
            {
                if (gridManager.TryPlaceUnit(unit, tile))
                {
                    placed = true;
                    break;
                }
            }

            if (!placed) unplaced.Add(unit);
        }

        foreach (Unit unit in unplaced)
        {
            Debug.LogWarning($"[CombatManager] No free {side} spawn tile for '{unit.unitName}' on the {edge} edge. Unit removed from this encounter.");
            units.Remove(unit);
        }
    }

    //Log clarity: duplicate enemy types get numbered names ("Striker 1", "Striker 2").
    //Unique types keep their plain name; reinforcements continue their type's count
    Dictionary<string, int> enemyNameCounts = new Dictionary<string, int>();

    void NumberStartingEnemies(List<Unit> enemies)
    {
        enemyNameCounts.Clear();

        var totals = new Dictionary<string, int>();
        foreach (var enemy in enemies)
            totals[enemy.unitName] = totals.TryGetValue(enemy.unitName, out int n) ? n + 1 : 1;

        foreach (var enemy in enemies)
        {
            string baseName = enemy.unitName;
            int index = enemyNameCounts.TryGetValue(baseName, out int count) ? count + 1 : 1;
            enemyNameCounts[baseName] = index;

            if (totals[baseName] > 1)
                enemy.unitName = $"{baseName} {index}";
        }
    }

    void NumberReinforcement(Unit unit)
    {
        string baseName = unit.unitName;
        int index = enemyNameCounts.TryGetValue(baseName, out int count) ? count + 1 : 1;
        enemyNameCounts[baseName] = index;

        //The first of its type in this encounter keeps the plain name
        if (index > 1)
            unit.unitName = $"{baseName} {index}";
    }

    void PlaceReinforcements(EscalationHandler.EscalationResult result)
    {
        if (!result.HasReinforcements) return;

        for (int i = 0; i < result.units.Count; i++)
        {
            Unit unit = result.units[i];
            NumberReinforcement(unit);
            Vector2Int pos = result.positions[i];

            bool placed = gridManager.TryPlaceUnit(unit, pos);

            if (!placed)
            {
                // Position was occupied by the time we placed — shouldn't happen in
                // normal flow since positions are resolved against live grid state,
                // but log it rather than silently dropping the unit.
                Debug.LogWarning($"[Escalation] Could not place reinforcement '{unit.unitName}' at {pos} — tile occupied.");
                continue;
            }

            UnitViewManager.Instance.CreateView(unit);
            unit.oaState = new OAState(1);
            unit.oaState.reactWhileHidden = unit.isEnemy;
            //Reinforcements arrive already aware: not flat-footed
            unit.OnDeath += HandleUnitDeath; //same subscription StartCombat gives starting units; Kill() routes defeat through OnDeath
            ThreatZoneManager.RegisterUnit(unit);
            enemyUnits.Add(unit);
            AuraOfCalm.Register(unit);
        }
    }

    void CheckEmergencyExtractionEligibility()
    {
        // Condition 1: 3 or more turns have passed
        bool turnCondition = turnManager.turnCount >= 3;

        // Condition 2: 3 or more player units are dead or dying
        int casualties = 0;
        foreach (Unit unit in playerUnits)
        {
            if (unit.IsDead() || unit.IsDying()) casualties++;
        }
        bool casualtyCondition = casualties >= 3;

        if (turnCondition || casualtyCondition)
        {
            emergencyExtractionAvailable = true;
            OnEmergencyExtractionAvailable?.Invoke();
            CombatLog.Add("[Extraction] Emergency extraction now available.");
        }
    }

    public event System.Action OnEmergencyExtractionAvailable;

    public void CallEmergencyExtraction()
    {
        if (!emergencyExtractionAvailable || emergencyExtractionCalled) return;
        if (extractionManager != null) return;  // Already a configured extraction mission

        emergencyExtractionCalled = true;

        //Resolves at the end of the deadline turn: this turn plus two more to reach the zone
        int deadline = turnManager.turnCount + 2;

        string zoneId = GridManager.EmergencyExtractionZoneId;

        extractionManager = new ExtractionMissionManager(
            new List<string> { zoneId },
            deadline);

        extractionManager.isEmergencyExtraction = true;

        // Inject the extraction objective so the UI reflects it
        ObjectiveDefinition extractionObjective = new ObjectiveDefinition(
            "emergency_extraction",
            $"Reach the extraction zone by the end of turn {deadline}",
            new ExtractionCondition(zoneId),
            null,
            visibleAtStart: true,
            isPrimary: false
        );

        RevealSecondaryObjective(extractionObjective);

        OnEmergencyExtractionCalled?.Invoke(deadline);
        CombatLog.Add($"[Extraction] Emergency extraction called — deadline: turn {deadline}.");
    }

    public event System.Action<int> OnEmergencyExtractionCalled;

    public void RefreshCurrentUnitHighlights()
    {
        if (currentUnit == null || currentUnit.IsDead()) return;
        if (!IsPlayerTurn) return;

        GridHighlighter.Instance.ClearAll();
        GridHighlighter.Instance.HighlightValidMoves(currentUnit, this);
    }

    //Squad planning phase: a normal player turn, or the surprise round
    public bool IsPlanningPhase =>
        turnManager.CurrentState == TurnManager.CombatState.PlayerTurn ||
        turnManager.CurrentState == TurnManager.CombatState.SurpriseRound;

    //Squad input is accepted only while planning and before the round starts resolving
    //(the surprise round keeps its state while RunTurn plans the enemies, so the processing flag is needed too)
    public bool CanPlan => IsPlanningPhase && !isProcessingTurn;

    //DEBUG (dev builds): ends the current encounter at once, through the normal end-of-combat path as a Retreat
    //(no loot). Drills leave campaign state untouched; other encounters record the Retreat. Planning phase only.
    public void DebugExitCombat()
    {
        if (!Debug.isDebugBuild) return;
        if (!IsPlanningPhase || isProcessingTurn) return;

        CombatLog.Add($"[Debug] Exiting combat: {currentEncounter?.encounterName}.");
        pendingAction = null;
        actionPlanner.Clear();
        GridHighlighter.Instance.ClearAll();
        FinishCombat(CombatEndState.Retreat);
    }

    //DEBUG (dev builds): kills the given units through the normal defeat path (Kill -> OnDeath -> ProcessDefeat).
    //Squad members are incapacitated (Hub snapshot rollback); enemies are defeated and count toward escalation triggers.
    //Invulnerable training targets refuse (Kill logs it). Planning phase only.
    public void DebugKillUnits(List<Unit> units)
    {
        if (!Debug.isDebugBuild) return;
        if (!IsPlanningPhase || isProcessingTurn) return;
        if (units == null || units.Count == 0) return;

        List<Unit> killed = new List<Unit>();

        foreach (Unit unit in units)
        {
            if (unit == null || unit.IsDead()) continue;

            unit.Kill();
            if (unit.IsDead()) killed.Add(unit);
        }

        CombatLog.Add($"[Debug] Killed {killed.Count} unit(s): {string.Join(", ", killed.ConvertAll(u => u.unitName))}");

        if (killed.Count == 0) return;

        //Drop planned actions by or against the killed units, so nothing resolves for them
        foreach (var kvp in actionPlanner.GetAllPlanned())
            kvp.Value.RemoveAll(a => killed.Contains(a.source) || killed.Contains(a.target));

        if (PersistentChromeController.Instance != null)
            PersistentChromeController.Instance.RefreshPortraits();

        GridHighlighter.Instance.RefreshUnitHighlights(this);

        //The squad member being prompted was killed: move on to the next one
        if (currentUnit != null && currentUnit.IsDead())
        {
            pendingAction = null;
            turnManager.AdvanceUnit();
            PromptNextUnit();
            return;
        }

        RefreshCurrentUnitHighlights();
    }

    //DEBUG (dev builds): deals flat damage to the given units through CombatResolver.ApplyFinalResult, the same path an
    //attack uses after its roll: DR, temp HP, massive damage, concentration checks, Disabled/Dying/death. No attacker.
    public void DebugDamageUnits(List<Unit> units, int amount)
    {
        if (!Debug.isDebugBuild) return;
        if (!IsPlanningPhase || isProcessingTurn) return;
        if (units == null || units.Count == 0 ||amount <= 0) return;

        CombatResolver resolver = new CombatResolver(this, false);

        foreach (Unit unit in units)
        {
            if (unit == null || unit.IsDead()) continue;

            CombatLog.Add($"[Debug] {unit.unitName} takes {amount} debug damage.");

            ActionContext ctx = new ActionContext
            {
                originalTarget = unit,
                finalTarget = unit,
                baseDamage = amount,
                modifiedDamage = amount,
                damageSource = "Debug"
            };

            resolver.ApplyFinalResult(ctx);
        }

        if (PersistentChromeController.Instance != null)
            PersistentChromeController.Instance.RefreshPortraits();

        GridHighlighter.Instance.RefreshUnitHighlights(this);

        //The squad member being prompted can no longer act: move on to the next one
        if (currentUnit != null && !currentUnit.CanAct())
        {
            pendingAction = null;
            turnManager.AdvanceUnit();
            PromptNextUnit();
            return;
        }

        RefreshCurrentUnitHighlights();
    }

    //DEBUG (dev builds): empties the unit's regular power point pool, to test powers it can't afford.
    //Power Crystal charges and free 0-level uses are left alone. Planning phase only
    public void DebugDrainPowerPoints(Unit unit)
    {
        if (!Debug.isDebugBuild) return;
        if (!IsPlanningPhase || isProcessingTurn) return;
        if (unit == null || unit.powerPoints == null) return;

        var pp = unit.powerPoints;
        pp.current = 0;

        string crystal = pp.crystal != null ? $" (crystal still holds {pp.CrystalPoints})" : "";
        CombatLog.Add($"[Debug] {unit.unitName}: power points set to 0/{pp.max}{crystal}.");

        //Readout and button state, then the grid (any targeting in progress is re-validated)
        OnActionBarRefreshRequested?.Invoke(unit);
        RefreshCurrentUnitHighlights();
    }
}