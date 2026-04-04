using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CombatManager : MonoBehaviour
{
    List<Unit> playerUnits;
    List<Unit> enemyUnits;
    int currentUnitIndex = 0;
    List<CombatAction> plannedActions = new List<CombatAction>();
    ActionType pendingAction;
    public Unit CurrentUnit => currentUnit;  
    public bool IsPlayerTurn => currentState == CombatState.PlayerTurn;
    public enum CombatState
    {
        PlayerTurn,
        EnemyTurn,
        Resolving,
        Ended
    }

    public List<Unit> GetAllUnits()
    {
        List<Unit> all = new List<Unit>();
        all.AddRange(playerUnits);
        all.AddRange(enemyUnits);
        return all;
    }

    public ActionType GetPendingAction()
    {
        return pendingAction;
    }

    private CombatState currentState;
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

        actions.AddRange(plannedActions);

        CombatLog.Add("=== NEW TURN ===");

        foreach (Unit unit in enemyUnits)
        {
            if (!unit.IsDead())
            {
                currentUnit = unit;

                yield return new WaitForSeconds(0.4f);

                EnemyAI enemyAI = new EnemyAI();
                var decision = enemyAI.ChooseAction(unit, playerUnits);

                CombatLog.Add(unit.unitName + " chooses " + decision.action);

                CombatAction action = new CombatAction(
                    unit,
                    decision.target,
                    decision.action,
                    enemyUnits,
                    playerUnits,
                    this
                );

                if (decision.action == ActionType.Move && decision.target != null)
                {
                    EnemyAI ai = new EnemyAI();
                    Vector2Int movePos = ai.GetMoveTowards(unit, decision.target);
                    action.SetMovePosition(movePos);
                }

                actions.Add(action);
            }
        }

        CombatResolver resolver = new CombatResolver(this);

        currentState = CombatState.Resolving;

        yield return resolver.ResolveTurn(actions);
        
        //End of turn Processing (effects, cooldowns handled later)
        ProcessEndPhase();

        isProcessingTurn = false;
        
        CheckCombatEnd();
    }

    void CheckCombatEnd()
    {
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

            Object.FindAnyObjectByType<GameFlowManager>().EndCombat(false);
            return;
        }

        if (AreAllUnitsDead(enemyUnits))
        {
            CombatLog.Add("All enemies defeated");

            Object.FindAnyObjectByType<GameFlowManager>().EndCombat(true);
            return;
        }

        StartPlayerTurn();
    }

    void StartPlayerTurn()
    {
        SetState(CombatState.PlayerTurn);
        
        currentUnitIndex = 0;
        plannedActions.Clear();

        CombatLog.Add("=== PLAYER TURN ===");

        PromptNextUnit();
    }

    void PromptNextUnit()
    {
        if (isProcessingTurn) return;
        
        //if all units have chosen actions
        if (currentUnitIndex >= playerUnits.Count)
        {
            SetState(CombatState.EnemyTurn);
            StartCoroutine(RunTurn());
            return;
        }

        Unit unit = playerUnits[currentUnitIndex];
        currentUnit = unit;

        //skip dead units
        if (unit.IsDead())
        {
            currentUnitIndex++;
            PromptNextUnit();
            return;
        }

        string actions = "";

        foreach (ActionType action in unit.availableActions)
        {
            if (!unit.IsActionAvailable(action))
            {
                actions += action.ToString() + "(CD) ";
            }
            else
            {
                actions += action.ToString() + " ";
            }
        }

        GridHighlighter.Instance.ClearAll();

        //Highlight valid moves for current unit
        if (currentUnit != null && !currentUnit.IsDead())
        {
            GridHighlighter.Instance.HighlightValidMoves(currentUnit, this);
        }

        CombatLog.Add(unit.unitName + ": Choose Action - " + actions);
    }

    void SelectAction(ActionType actionType)
    {
        Unit unit = playerUnits[currentUnitIndex];

        if (!unit.availableActions.Contains(actionType))
        {
            CombatLog.Add(unit.unitName + " cannot use that ability!");
            return;
        }

        if (!unit.IsActionAvailable(actionType))
        {
            CombatLog.Add(unit.unitName + "'s " + actionType + " is on cooldown!");
            return;
        }
        
        //Actions that require manual target selection
        if (actionType == ActionType.Attack || actionType == ActionType.FocusShot)
        {
            pendingAction = actionType;

            CombatLog.Add(unit.unitName + " selecting target...");
            FindAnyObjectByType<CombatUIController>().ShowTargets(enemyUnits);
            return;
        }

        //Non-targeted actions execute immediately
        plannedActions.Add(new CombatAction(unit, null, actionType, playerUnits, enemyUnits, this));

        currentUnitIndex++;

        PromptNextUnit();
    }

    void SetState(CombatState newState)
    {
        bool IsValidTransition(CombatState from, CombatState to)
        {
            switch (from)
            {
                case CombatState.PlayerTurn:
                    return to == CombatState.EnemyTurn;

                case CombatState.EnemyTurn:
                    return to == CombatState.Resolving;

                case CombatState.Resolving:
                    return to == CombatState.PlayerTurn || to == CombatState.Ended;

                case CombatState.Ended:
                    return false;

            }

            return false;
        }
        
        //Prevent redundant transitions
        if (currentState == newState) return;

        if (!IsValidTransition(currentState, newState))
        {
            Debug.LogError($"INVALID STATE TRANSITION: {currentState} -> {newState}");
            return;
        }

        currentState = newState;
        CombatLog.Add($"STATE -> {newState}");
    }

    public void SelectTarget(int index)
    {
        if (index >= enemyUnits.Count) return;

        Unit target = enemyUnits[index];

        if (target.IsDead())
        {
            CombatLog.Add("Invalid target!");
            return;
        }

        Unit unit = playerUnits[currentUnitIndex];

        plannedActions.Add(new CombatAction(unit, target, pendingAction, playerUnits, enemyUnits, this));

        currentUnitIndex++;
        PromptNextUnit();
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
    }

    public void StartCombat(MissionData mission, int bonusAttack)
    {
        CombatLog.Add("Intializing Combat...");
        
        //determine squads
        playerUnits = new List<Unit>();
        enemyUnits = new List<Unit>();

        playerUnits.Add(new Unit("Commander", false, 100, 15 + bonusAttack, 5));
        playerUnits.Add(new Unit("Sniper", false, 80, 20 + bonusAttack, 7));
        playerUnits.Add(new Unit("Tank", false, 150, 10 + bonusAttack, 3));

        for (int i = 0; i < mission.enemyCount; i++)
        {
            enemyUnits.Add(new Unit("Drone", true, 60, 12, 4));
        }

        GridManager grid = FindAnyObjectByType<GridManager>();

        //Player side (left)
        grid.TryPlaceUnit(playerUnits[0], new Vector2Int(0, 2));
        grid.TryPlaceUnit(playerUnits[1], new Vector2Int(0, 3));
        grid.TryPlaceUnit(playerUnits[2], new Vector2Int(0, 1));

        //Enemy side (right)
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            grid.TryPlaceUnit(enemyUnits[i], new Vector2Int(7, i + 1));
        }

        StartPlayerTurn();
    }

    public void PlayerChooseAction(ActionType action)
    {
        if (!IsPlayerTurn) return;

        SelectAction(action);
    }

    public void SetCurrentUnit(Unit unit)
    {
        currentUnit = unit;
    }

    public void AddPlannedAction(CombatAction action)
    {
        plannedActions.Add(action);

        currentUnitIndex++;
        PromptNextUnit();
    }
}