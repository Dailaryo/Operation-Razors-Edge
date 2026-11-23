using System.Collections.Generic;
using UnityEngine;

public class GridCollapseEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null || resolver.IsSimulation) return;

        ctx.attacker.SetCooldown(ctx.definition, Unit.CombatCooldown);

        var enemies = new List<Unit>();

        foreach (Unit unit in resolver.GetAllUnits())
        {
            if (resolver.IsUnitDead(unit)) continue;
            if (unit.isEnemy == ctx.attacker.isEnemy) continue;

            int dist = DistanceUtility.MovementCostDistance(
                ctx.attacker.position, resolver.GetPosition(unit));

            if (dist <= 8) enemies.Add(unit);
        }

        // Sort by distance — target closest 3
        enemies.Sort((a, b) =>
            DistanceUtility.MovementCostDistance(ctx.attacker.position, resolver.GetPosition(a))
            .CompareTo(
            DistanceUtility.MovementCostDistance(ctx.attacker.position, resolver.GetPosition(b))));

        int targets = Mathf.Min(3, enemies.Count);

        int dc = 10 + ctx.attacker.GetModifier(StatType.Cognition);

        for (int i = 0; i < targets; i++)
        {
            Unit target = enemies[i];
            var saveTotal = target.RollSave(Unit.SaveType.Focus);
            int margin = dc - saveTotal.total;

            if (saveTotal < dc)
            {
                if (!resolver.IsSimulation)
                {
                    target.AddEffect(new LoseNextActionEffect());
                    CombatLog.Add($"{target.unitName}: Grid Collapse — loses next action.");

                    if (margin >= 5)
                    {
                        target.AddEffect(new DiagnosedEffect());
                        CombatLog.Add($"{target.unitName}: Grid Collapse — also Diagnosed.");
                    }
                }
            }
            else
            {
                CombatLog.Add($"{target.unitName} resists Grid Collapse.");
            }
        }
    }
}