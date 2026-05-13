using UnityEngine;

public class SquadStrengthCondition : IObjectiveCondition
{
    public int minimumUnits;

    public SquadStrengthCondition(int minimumUnits)
    {
        this.minimumUnits = minimumUnits;
    }

    public bool Evaluate(CombatContext context)
    {
        int count = 0;
        foreach (Unit unit in context.playerUnits)
        {
            if ((!unit.IsDead())) count++;
        }

        return count >= minimumUnits;
    }

    public bool IsFailed(CombatContext context)
    {
        return !Evaluate(context);
    }
}
