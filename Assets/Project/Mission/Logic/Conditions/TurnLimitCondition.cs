using UnityEngine;

public class TurnLimitCondition : IObjectiveCondition
{
    public int turnLimit;

    public TurnLimitCondition(int turnLimit)
    {
        this.turnLimit = turnLimit;
    }

    public bool Evaluate(CombatContext context)
    {
        return context.turnCount <= turnLimit;
    }

    public bool IsFailed(CombatContext context)
    {
        return context.turnCount > turnLimit;
    }
}
