using UnityEngine;

public class ProtectUnitCondition : IObjectiveCondition
{
    public string protectedUnitId;

    public ProtectUnitCondition(string protectedUnitId)
    {
        this.protectedUnitId = protectedUnitId;
    }

    public bool Evaluate(CombatContext context)
    {
        Unit target = FindUnit(context);

        if (target == null)
        {
            Debug.LogWarning($"[ProtectUnitCondition] Unit not found: {protectedUnitId}");
            return false;
        }

        return (!target.IsDead());
    }

    public bool IsFailed(CombatContext context)
    {
        Unit target = FindUnit(context);

        if (target == null) return true;

        return target.IsDead();
    }

    Unit FindUnit(CombatContext context)
    {
        foreach (Unit unit in context.playerUnits)
        {
            if (unit.characterId == protectedUnitId) return unit;
        }

        foreach (Unit unit in context.enemyUnits)
        {
            if (unit.characterId == protectedUnitId) return unit;
        }

        return null;
    }
}
