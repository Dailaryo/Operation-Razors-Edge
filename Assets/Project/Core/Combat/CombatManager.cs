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
    public event System.Action<Unit> OnCommandOverrideTriggered;
    public event System.Action<SkillCheckPreviewUI.SkillCheckPreview> OnSkillCheckPreviewRequested;
    public event System.Action<ObjectiveDefinition> OnSecondaryObjectiveRevealed;

    private Unit currentUnit;

    bool isProcessingTurn = false;

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
        
        //Gather all actions (player and enemy)
        List<CombatAction> actions = new List<CombatAction>();

        actions.AddRange(actionPlanner.GetAllActionsFlat());

        CombatLog.Add("=== NEW TURN ===");

        foreach (Unit unit in enemyUnits)
        {
            if (unit.CanAct())
            {
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
                }
            }
        }

        CombatResolver resolver = new CombatResolver(this, false);

        turnManager.SetState(TurnManager.CombatState.Resolving);

        yield return resolver.ResolveTurn(actions);
        
        //End of turn Processing (effects, cooldowns handled later)
        ProcessEndPhase();

        //Increment turn after full resolution
        turnManager.IncrementTurn();

        isProcessingTurn = false;
        
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
            //Round start: Read the Room repeats Sweep; Empath Apotheosis refreshes
            SweepResolver.Run(GetAllUnits(), encounterStart: false);
            AwarenessManager.ApplyApotheosis(GetAllUnits());

            StartPlayerTurn();
            return;
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

        CombatLog.Add("=== PLAYER TURN ===");

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

        CombatLog.Add("=== SURPRISE ROUND ===");
        CombatLog.Add("Hidden units act before initiative is rolled.");
        AudioEventBus.Fire(AudioEventIds.Stealth.SurpriseRoundBegin);
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
                CombatLog.Add("=== SURPRISE ROUND ENDS ===");
                StartPlayerTurn();
                return;
            }

            turnManager.SetState(TurnManager.CombatState.EnemyTurn);
            StartCoroutine(RunTurn());
            return;
        }

        Unit unit = playerUnits[turnManager.GetCurrentIndex()];
        currentUnit = unit;

        //skip dead, dying and stabilized units, they cannot act
        if (!unit.CanAct())
        {
            turnManager.AdvanceUnit();
            PromptNextUnit();
            return;
        }

        //Debug Log: List of available actions
        string actions = "";

        foreach (ActionDefinition action in unit.GetAllActions())
        {
            bool available = unit.IsActionAvailable(action);

            actions += action.actionID + (available ? " " : "(CD) ");
        }

        OnActionBarRefreshRequested?.Invoke(unit);

        GridHighlighter.Instance.ClearAll();

        //Highlight valid moves for current unit
        if (currentUnit != null && !currentUnit.IsDead())
        {
            GridHighlighter.Instance.HighlightValidMoves(currentUnit, this);
        }

        CombatLog.Add(unit.unitName + ": Choose Action - " + actions);

        var unitActions = actionPlanner.GetActions(unit);

        if (unitActions.Count > 0)
        {
            CombatLog.Add("Action selected. Choose another or right-click to cancel");
        }
    }

    void ProcessEndPhase()
    {
        //Apply end-of-turn effect logic, then reduce ability cooldowns
        foreach (Unit unit in playerUnits)
        {
            unit.ProcessEffects();
            unit.ReduceCooldowns();
        }

        foreach (Unit unit in enemyUnits)
        {
            unit.ProcessEffects();
            unit.ReduceCooldowns();
            unit.ResetInvulnerableTarget(); //training targets only; no-op for everything else
        }

        DetectionStateManager.TickAlertCountDowns();
        FieldZoneManager.ProcessEndPhase(GetAllUnits());

        // Apply unit aura effects to nearby allies
        foreach (Unit unit in GetAllUnits())
        {
            foreach (var source in unit.effectSources)
            {
                foreach (var effect in source.GetEffects(unit))
                {
                    if (effect is TacticalUplinkEffect uplink)
                    {
                        uplink.ApplyToNearbyAllies(unit, GetAllUnits());
                    }
                }
            }
        }

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

    public void StartCombat(EncounterData encounter, List<Unit> playerUnitsInput, List<Unit> enemyUnitsInput, bool surpriseRound = false)
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
        turnManager.ResetTurnCount();
        missionState = new MissionState();

        CombatLog.Add("Intializing Combat...");
        
        //determine squads
        playerUnits = new List<Unit>(playerUnitsInput);
        enemyUnits = new List<Unit>(enemyUnitsInput);

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
                unit.oaState.maxPerRound = Mathf.Max(1, agilityMod);
            }
        }

        ThreatZoneManager.Reset();

        foreach (Unit unit in GetAllUnits())
        {
            ThreatZoneManager.RegisterUnit(unit);
        }

        DetectionStateManager.Initialize(GetAllUnits(), surpriseRound ? playerUnits : null);

        AwarenessManager.Initialize(GetAllUnits());
        AwarenessManager.ApplyApotheosis(GetAllUnits());
        AuraOfCalm.Initialize(GetAllUnits());
        SweepResolver.Run(GetAllUnits(), encounterStart: true);

        if (surpriseRound)
            StartSurpriseRound();
        else
            StartPlayerTurn();
        AudioEventBus.Fire(AudioEventIds.GameFlow.CombatStart);
    }

    public void PlayerChooseAction(ActionDefinition action)
    {
        //Get current unit
        Unit unit = CurrentUnit;

        if (unit == null || unit.IsDead()) return;

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

            GridHighlighter.Instance.HighlightValidTargets(unit, action.actionID, this);

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

    public void AddPlannedAction(CombatAction action)
    {
        if (!ActionValidator.IsValid(action, GetValidationResolver()))
        {
            CombatLog.Add("Invalid Action");

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
            CombatLog.Add($"Not enough action resources for {action.definition.actionID}");

            GridHighlighter.Instance.ClearAll();

            if (currentUnit != null && !currentUnit.IsDead())
            {
                GridHighlighter.Instance.HighlightValidMoves(currentUnit, this);
            }

            return;
        }

        // Check for Command Override — interrupt to offer bonus action
        foreach (Unit playerUnit in playerUnits)
        {
            if (playerUnit.pendingCommandOverride)
            {
                playerUnit.pendingCommandOverride = false;
                currentUnit = playerUnit;
                OnCommandOverrideTriggered?.Invoke(playerUnit);
                OnActionBarRefreshRequested?.Invoke(playerUnit);
                CombatLog.Add($"{playerUnit.unitName}: Command Override — choose a bonus action.");
                return;
            }
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
        var actions = actionPlanner.GetActions(currentUnit);

        //First handle targeting state
        if (pendingAction != null)
        {
            pendingAction = null;

            CombatLog.Add(currentUnit.unitName + " canceled targeting.");

            GridHighlighter.Instance.ClearAll();
            GridHighlighter.Instance.HighlightValidMoves(currentUnit, this);

            OnTargetSelectionRequested?.Invoke(null);
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

        pendingAction = null;

        GridHighlighter.Instance.HighlightValidMoves(currentUnit, this);

        OnTargetSelectionRequested?.Invoke(null);
    }

    void Update()
    {
        if (!IsPlayerTurn) return;
        if (currentUnit == null) return;

        if (Input.GetMouseButtonDown(1))
        {
            CancelCurrentUnitAction();
        }
    }

    public bool IsTilePlannedOccupied(Vector2Int pos)
    {
        foreach (var kvp in actionPlanner.GetAllPlanned())
        {
            var actions = kvp.Value;

            foreach (var act in actions)
            {
                if (act.definition.IsMove()  && act.movePosition.HasValue)
                {
                    if (act.movePosition.Value == pos) return true;
                }
            }
        }

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

        if (unit == null || unit.IsDead()) return;

        ActionDefinition pending = pendingAction;

        //If we are targeting an ability
        if (pending != null)
        {
            var def = pending;

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
        if (currentUnit == null || currentUnit.IsDead()) return;

        var unitActions = actionPlanner.GetActions(currentUnit);

        CombatLog.Add($"{currentUnit.unitName} confirmed turn.");

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

    void PlaceReinforcements(EscalationHandler.EscalationResult result)
    {
        if (!result.HasReinforcements) return;

        for (int i = 0; i < result.units.Count; i++)
        {
            Unit unit = result.units[i];
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

        // Build a deadline 3 turns from now, giving players time to reach the zone
        int deadline = turnManager.turnCount + 3;

        string zoneId = GridManager.EmergencyExtractionZoneId;

        extractionManager = new ExtractionMissionManager(
            new List<string> { zoneId },
            deadline);

        extractionManager.isEmergencyExtraction = true;

        // Inject the extraction objective so the UI reflects it
        ObjectiveDefinition extractionObjective = new ObjectiveDefinition(
            "emergency_extraction",
            $"Reach the extraction zone within {deadline} turns",
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
}