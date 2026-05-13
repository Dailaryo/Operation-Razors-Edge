using UnityEngine;

public class SurvivalCondition : IObjectiveCondition
{
    public int requiredTurns;

    public SurvivalCondition(int requiredTurns)
    {
        this.requiredTurns = requiredTurns;
    }

    public bool Evaluate(CombatContext context)
    {
        return context.turnCount >= requiredTurns;
    }

    public bool IsFailed(CombatContext context)
    {
        return false;
    }
}
