using System.Collections.Generic;
using UnityEngine;

public class CascadeBurstEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation) return;

        if (ctx.attacker == null) return;

        int dc = 10 + ctx.attacker.GetModifier(StatType.Cognition);

        foreach (Unit unit in resolver.GetAllUnits())
        {
            if (resolver.IsUnitDead(unit)) continue;
            if (unit.isEnemy == ctx.attacker.isEnemy) continue;

            int dist = DistanceUtility.MovementCostDistance(
                ctx.attacker.position, resolver.GetPosition(unit));

            if (dist > 4) continue;

            var saveTotal = unit.RollSave(Unit.SaveType.Focus);

            if (saveTotal < dc)
            {
                if (!resolver.IsSimulation)
                {
                    unit.AddEffect(new CascadeBurstDebuffEffect());
                    CombatLog.Add($"{unit.unitName} is disrupted by Cascade Burst.");
                }
            }
            else
            {
                CombatLog.Add($"{unit.unitName} resists Cascade Burst.");
            }
        }
    }
}