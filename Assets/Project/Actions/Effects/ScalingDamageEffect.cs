using UnityEngine;

public class ScalingDamageEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.definition == null) return;

        var stat = ctx.definition.scalingStat;
        float multiplier = ctx.definition.scalingMultiplier;

        int bonus = Mathf.RoundToInt(ctx.attacker.GetDerivedStat(stat) * multiplier);
        ctx.modifiedDamage += bonus;

        //MeleePower at x1 (d20 Strength to damage): one damage-breakdown part per source, so the log line itemises
        //the POW modifier and every MeleePower bonus. Any other scaling stays one lumped part
        if (stat == StatType.MeleePower && Mathf.Approximately(multiplier, 1f))
        {
            foreach (var (label, amount) in ctx.attacker.GetMeleePowerParts())
                ctx.AddDamagePart(label, amount);
        }
        else
        {
            ctx.AddDamagePart($"{stat} scaling", bonus);
        }
    }
}