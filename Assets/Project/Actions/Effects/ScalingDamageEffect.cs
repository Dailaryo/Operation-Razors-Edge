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

        Debug.Log($"{ctx.attacker.unitName} gains {bonus} damage from {stat} scaling");
    }
}
