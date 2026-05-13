using UnityEngine;

public interface IObjectiveCondition
{
    bool Evaluate(CombatContext context);
    bool IsFailed(CombatContext context);
}
