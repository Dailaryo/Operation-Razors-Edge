using System.Collections.Generic;
using UnityEngine;

public class ArmamentBurstEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null) return;

        var allUnits = resolver.GetAllUnits();
        int hitCount = 0;

        foreach (Unit unit in allUnits)
        {
            if (resolver.IsUnitDead(unit)) continue;
            if (unit.isEnemy == ctx.attacker.isEnemy) continue;

            int dist = DistanceUtility.MovementCostDistance(
                ctx.attacker.position, resolver.GetPosition(unit));

            if (dist > 3) continue;

            int damage = Mathf.Max(1, Random.Range(1, 9) - 2); // d8 weapon, -2

            if (!resolver.IsSimulation)
            {
                unit.TakeDamage(damage);
                CombatLog.Add($"{ctx.attacker.unitName}: Armament Burst hits {unit.unitName} for {damage}.");
                hitCount++;
            }
        }

        if (hitCount == 0)
            CombatLog.Add($"{ctx.attacker.unitName}: Armament Burst — no targets in range.");
    }
}