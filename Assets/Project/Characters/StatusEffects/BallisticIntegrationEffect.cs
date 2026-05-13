using UnityEngine;
using System.Collections.Generic;

public class BallisticIntegrationEffect : StatusEffect
{
    public BallisticIntegrationEffect() : base("BallisticIntegration", -1)
    {
        isPersistent = true;
        Priority = 5;
    }

    public override void OnBeforeAction(ActionContext ctx)
    {
        if (ctx.attacker != Target) return;
        if (ctx.definition.attackType != ActionDefinition.AttackType.Ranged) return;
        if (!ctx.definition.dealsDamage) return;

        int bonus = Mathf.Max(0, ctx.attacker.GetModifier(StatType.Cognition) / 2);
        ctx.modifiedDamage += bonus;

        if (bonus > 0)
            CombatLog.Add($"{ctx.attacker.unitName}: Ballistic Integration +{bonus} damage.");
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
        => new List<StatModifier>();

    public override void OnTurnEnd(Unit unit) { }
}