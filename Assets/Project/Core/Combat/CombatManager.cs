using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MissionData;

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

    public Unit CurrentUnit => currentUnit;  

    public bool IsPlayerTurn => turnManager.IsPlayerTurn();

    public System.Action<List<Unit>> OnCombatEnded;

    private MissionData currentMission;

    private bool lastCombatVictory;

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

    private Unit currentUnit;

    bool isProcessingTurn = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            if (!unit.IsDead())
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
        CombatLog.Add($"[Mission] Type: {currentMission.objectiveType}");

        if (currentMission.objectiveType == MissionObjectiveType.Survival)
        {
            CombatLog.Add($"[Mission] Turn: {turnManager.turnCount}/{currentMission.turnLimit}");
        }

        if (currentMission.objectiveType == MissionObjectiveType.TargetElimination)
        {
            if (currentMission.targetUnit != null)
            {
                CombatLog.Add($"[Mission] Target Alive: {!currentMission.targetUnit.IsDead()}");
            }
        }

        bool AreAllUnitsDead(List<Unit> units)
        {
            foreach (Unit unit in units)
            {
                if (!unit.IsDead())
                {
                    return false;
                }
            }
            return true;
        }

        if (AreAllUnitsDead(playerUnits))
        {
            CombatLog.Add("All player units defeated!");

            CleanUpUnits();

            OnCombatEnded?.Invoke(playerUnits);
            lastCombatVictory = false;
            gameFlowManager.EndCombat(false);
            return;
        }

        bool missionSuccess = MissionObjectiveEvaluator.IsMissionSuccessful(
            currentMission,
            playerUnits,
            enemyUnits,
            turnManager
        );

        if (missionSuccess)
        {
            CombatLog.Add("Mission Objective Complete");

            CleanUpUnits();

            OnCombatEnded?.Invoke(playerUnits);
            lastCombatVictory = true;
            gameFlowManager.EndCombat(true);
            return;
        }

        StartPlayerTurn();
    }

    void StartPlayerTurn()
    {
        turnManager.SetState(TurnManager.CombatState.PlayerTurn);

        turnManager.ResetTurn();
        actionPlanner.Clear();

        CombatLog.Add("=== PLAYER TURN ===");

        PromptNextUnit();
    }

    void PromptNextUnit()
    {
        if (isProcessingTurn) return;
        
        //if all units have chosen actions
        if (turnManager.GetCurrentIndex() >= playerUnits.Count)
        {
            //Restore grid visuals before enemy turn
            GridHighlighter.Instance.RefreshUnitHighlights(this);

            turnManager.SetState(TurnManager.CombatState.EnemyTurn);
            StartCoroutine(RunTurn());
            return;
        }

        Unit unit = playerUnits[turnManager.GetCurrentIndex()];
        currentUnit = unit;

        //skip dead units
        if (unit.IsDead())
        {
            turnManager.AdvanceUnit();
            PromptNextUnit();
            return;
        }

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

        CombatLog.Add($"--- End Phase Complete ---");
        CombatLog.Add($"Turn {turnManager.turnCount} complete");
    }

    public void StartCombat(MissionData mission, List<Unit> playerUnitsInput, List<Unit> enemyUnitsInput, int bonusAttack)
    {
        if (missionUI != null)
        {
            missionUI.Initialize(mission, turnManager);
        }
        
        currentMission = mission;
        turnManager.ResetTurnCount();

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

        Unit targetUnit = null;

        mission.targetUnit = targetUnit;

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

        StartPlayerTurn();
    }

    public void PlayerChooseAction(ActionDefinition action)
    {
        //Get current unit
        Unit unit = CurrentUnit;

        if (unit == null || unit.IsDead()) return;

        //Targeting actions
        var def = action;

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
            GetAllUnits(),
            GetAllUnits(),
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

        var unitActions = actionPlanner.GetActions(unit);

        pendingAction = default;

        //Check if unit is done
        bool hasMove = false;
        bool hasStandard = false;

        foreach (var act in unitActions)
        {
            foreach (var cat in act.resolvedCategories)
            {
                if (cat == ActionDefinition.ActionCategory.Move) hasMove = true;

                if (cat == ActionDefinition.ActionCategory.Standard) hasStandard = true;
            }
        }

        if (hasMove && hasStandard)
        {
            turnManager.AdvanceUnit();
        }

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

    public enum IntentVisibilityLevel
    {
        None,
        Partial,
        Full,
    }

    public IntentVisibilityLevel GetIntentVisibility(Unit viewer, Unit target)
    {
        //TEMP: default behavior
        if (target.isEnemy)
            return IntentVisibilityLevel.None;

        return IntentVisibilityLevel.Full;
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
            GetAllUnits(),
            GetAllUnits(),
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
            GetAllUnits(),
            GetAllUnits(),
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
}