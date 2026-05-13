using System.Collections.Generic;
using UnityEngine;

public class ApplyStatusEffectToUnitsInRangeEffect : IActionEffect
{
    private System.Func<Unit, StatusEffect> effectFactory;
    private int range;
    private bool targetsAllies;

    public ApplyStatusEffectToUnitsInRangeEffect(
        System.Func<Unit, StatusEffect> effectFactory,
        int range,
        bool targetsAllies)
    {
        this.effectFactory = effectFactory;
        this.range = range;
        this.targetsAllies = targetsAllies;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null) return;

        foreach (Unit unit in resolver.GetAllUnits())
        {
            if (resolver.IsUnitDead(unit)) continue;

            bool isAlly = unit.isEnemy == ctx.attacker.isEnemy;
            if (targetsAllies != isAlly) continue;

            int dist = DistanceUtility.MovementCostDistance(
                ctx.attacker.position,
                resolver.GetPosition(unit));

            if (dist > range) continue;

            if (!resolver.IsSimulation)
                unit.AddEffect(effectFactory(ctx.attacker));
        }
    }
}