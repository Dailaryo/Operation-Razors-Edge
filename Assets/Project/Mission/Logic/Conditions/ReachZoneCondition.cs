using UnityEngine;
using System.Collections.Generic;

public class ReachZoneCondition : IObjectiveCondition
{
    public string zoneId;
    public int requiredUnitCount;

    public ReachZoneCondition(string zoneId, int requiredUnitCount = 1)
    {
        this.zoneId = zoneId;
        this.requiredUnitCount = requiredUnitCount;
    }

    public virtual bool Evaluate(CombatContext context)
    {
        List<Unit> unitsInZone = context.gridManager.GetUnitsInZone(zoneId);

        int count = 0;
        foreach (Unit unit in unitsInZone)
        {
            if (!unit.isEnemy && !unit.IsDead()) count++;
        }

        return count >= requiredUnitCount;
    }

    public virtual bool IsFailed(CombatContext context)
    {
        return false;
    }
}
