using UnityEngine;
using System.Collections.Generic;

public class TargetEliminationCondition : IObjectiveCondition
{
    public string targetUnitId;

    public TargetEliminationCondition(string targetUnitId)
    {
        this.targetUnitId = targetUnitId;
    }

    public bool Evaluate(CombatContext context)
    {
        Unit target = FindTarget(context);

        if (target == null)
        {
            Debug.LogWarning($"[TargetEliminationCondition] Unit not found: {targetUnitId}");
            return false;
        }

        return target.IsDead();
    }

    public bool IsFailed(CombatContext context)
    {
        return false;
    }

    Unit FindTarget(CombatContext context)
    {
        foreach (Unit unit in context.playerUnits)
        {
            if (unit.characterId == targetUnitId) return unit;
        }

        foreach (Unit unit in context.enemyUnits)
        {
            if (unit.characterId == targetUnitId) return unit;
        }

        return null;
    }
}
