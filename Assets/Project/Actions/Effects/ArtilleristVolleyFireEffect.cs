using System.Collections.Generic;
using UnityEngine;

public class ArtilleristVolleyFireEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null || ctx.finalTarget == null) return;

        Vector2Int attackerPos = resolver.GetPosition(ctx.attacker);
        Vector2Int targetPos   = resolver.GetPosition(ctx.finalTarget);

        Vector2 coneDir = ((Vector2)(targetPos - attackerPos)).normalized;

        foreach (Unit unit in resolver.GetAllUnits())
        {
            if (resolver.IsUnitDead(unit)) continue;
            if (unit.isEnemy == ctx.attacker.isEnemy) continue;
            if (unit == ctx.attacker) continue;

            Vector2Int unitPos = resolver.GetPosition(unit);
            int dist = DistanceUtility.MovementCostDistance(attackerPos, unitPos);
            if (dist > 3) continue;

            // Check if unit is within 90-degree cone of target direction
            Vector2 toUnit = ((Vector2)(unitPos - attackerPos)).normalized;
            float dot = Vector2.Dot(coneDir, toUnit);
            if (dot < 0f) continue; // Behind the attacker

            if (resolver.IsSimulation) continue;

            // Each unit in the cone takes damage — resolved as a separate hit
            int roll = Random.Range(1, 21);
            int accuracy = ctx.attacker.GetAccuracy(false);
            int attackTotal = roll + accuracy + ctx.attacker.GetBAB();
            int defense = unit.GetDefense();

            if (roll == 20 || (roll != 1 && attackTotal >= defense)) //natural 1 always misses, natural 20 always hits
            {
                int damage = Random.Range(1, 9); // 1d8 base volley damage
                unit.TakeDamage(damage);
                CombatLog.Add($"{ctx.attacker.unitName} Volley Fire hits " +
                              $"{unit.unitName} for {damage}.");
            }
            else
            {
                CombatLog.Add($"{ctx.attacker.unitName} Volley Fire misses {unit.unitName}.");
            }
        }
    }
}
