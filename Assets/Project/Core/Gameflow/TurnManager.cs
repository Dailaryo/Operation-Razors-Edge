using UnityEngine;

public class TurnManager
{
    public enum CombatState
    {
        PlayerTurn,
        EnemyTurn,
        Resolving,
        Ended
    }

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

    public void ResetTurnCount()
    {
        turnCount = 0;
    }

    public void IncrementTurn()
    {
        turnCount++;
    }
}
