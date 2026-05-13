using UnityEngine;

public class SensorLinkShareEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation) return;

        int perceptionResult = SkillResolver.GetSkillTotal(ctx.attacker, SkillType.Perception);

        foreach (Unit unit in resolver.GetAllUnits())
        {
            if (unit == ctx.attacker) continue;
            if (unit.isEnemy != ctx.attacker.isEnemy) continue;
            if (unit.IsDead() || unit.IsDying() || unit.IsStabilized()) continue;

            int dist = DistanceUtility.MovementCostDistance(
                ctx.attacker.position, resolver.GetPosition(unit));

            if (dist > 1) continue;

            unit.AddEffect(new SensorLinkBuffEffect(perceptionResult));
        }

        CombatLog.Add($"{ctx.attacker.unitName}: Sensor Link — shares Perception {perceptionResult} with adjacent allies.");
    }
}