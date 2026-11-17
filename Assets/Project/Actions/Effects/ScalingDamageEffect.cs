using UnityEngine;

public class ScalingDamageEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.definition == null) return;

        var stat = ctx.definition.scalingStat;
        float multiplier = ctx.definition.scalingMultiplier;

        int statValue = ctx.attacker.GetDerivedStat(stat);

        int bonus = Mathf.RoundToInt(statValue * multiplier);

        ctx.modifiedDamage += bonus;

        //Simulations (move highlights, validation, AI scoring) replay this effect many times per click; log only the real attack
        if (!resolver.IsSimulation)
        {
            string detail = stat == StatType.MeleePower
                ? $" (POW {ctx.attacker.GetModifier(StatType.Power):+0;-0;+0}, bonuses {ctx.attacker.GetStat(StatType.MeleePower):+0;-0;+0})"
                : "";
            Debug.Log($"{ctx.attacker.unitName} gains {bonus} damage from {stat} scaling{detail}");
        }
    }
}
