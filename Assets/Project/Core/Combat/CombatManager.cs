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

    private EscalationHandler escalationHandler;

    public List<Unit> GetAllUnits()
    {
        List<Unit> all = new List<Unit>();
        all.AddRange(playerUnits);
        all.AddRange(enemyUnits);
        return all;
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
            if (!unit.IsDead() && !unit.IsDying() && !unit.IsStabilized())
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
        if (unit.IsDead() || unit.IsDying() || unit.IsStabilized())
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
        }

        CombatLog.Add($"--- End Phase Complete ---");
        CombatLog.Add($"Turn {turnManager.turnCount} complete");
    }

    public void StartCombat(EncounterData encounter, List<Unit> playerUnitsInput, List<Unit> enemyUnitsInput, int bonusAttack, bool surpriseRound = false)
    {
        if (missionUI != null)
        {
            missionUI.Initialize(encounter, turnManager, this);
        }
        
        currentEncounter = encounter;
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

        //TEMP: Apply bonus attack
        foreach (var unit in playerUnits)
        {
            unit.attackRating += bonusAttack;
        }

        GridManager grid = gridManager;

        //Player side (left)
        Vector2Int[] playerPositions = new Vector2Int[]
        {
            new Vector2Int(0, 2),
            new Vector2Int(0, 3),
            new Vector2Int(0, 1)
        };

        for (int i = 0; i < playerUnits.Count && i < playerPositions.Length; i++)
        {
            grid.TryPlaceUnit(playerUnits[i], playerPositions[i]);
        }

        if (playerUnits.Count == 0)
        {
            Debug.LogError("No player units available for combat!");
        }

        //Enemy side (right)
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            grid.TryPlaceUnit(enemyUnits[i], new Vector2Int(7, i + 1));
        }

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
                if (unit.cooldowns[action] == Unit.-1)
                    unit.cooldowns.Remove(action);
            }

            // Reset per-combat effect flags
            foreach (var effect in unit.activeEffects)
            {
                if (effect is KineticAbsorptionEffect ka) ka.ResetForCombat();
                if (effect is ElectronicShroudEffect es) es.ResetForCombat();
                if (effect is ResilienceEffect res) res.ResetForCombat();
            }

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