using UnityEngine;

public class TurnManager
{
    public enum CombatState
    {
        SurpriseRound,
        InitiativeRoll,
        PlayerTurn,
        EnemyTurn,
        Resolving,
        Ended
    }

    //Turn being played: 0 = surprise round, 1 = first full round (everyone acts). Advanced by
    //CombatManager.CheckCombatEnd once a turn has resolved and combat continues.
    public int turnCount { get; private set; }

    public CombatState CurrentState { get; private set; }

    private int currentUnitIndex = 0;

    public int GetCurrentIndex()
    {
        return currentUnitIndex;
    }

    public void ResetTurn()
    {
        currentUnitIndex = 0;
    }

    public void AdvanceUnit()
    {
        currentUnitIndex++;
    }

    public void SetState(CombatState newState)
    {
        CurrentState = newState;
    }

    public bool IsPlayerTurn()
    {
        return CurrentState == CombatState.PlayerTurn;
    }

    public void ResetTurnCount(bool surprisedRound = false)
    {
        turnCount = surprisedRound ? 0 : 1;
    }

    public void IncrementTurn()
    {
        turnCount++;
    }
}
