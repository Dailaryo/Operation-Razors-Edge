using UnityEngine;

public class BonusDamageEffect : StatusEffect
{
    int bonus;

    public BonusDamageEffect(int amount) : base("Bonus Damage", 0)
    {
        bonus = amount;
        Priority = 5;

        Conditions = new[]
        {
            ConditionType.AttackerIsTarget,
            ConditionType.ActionDealsDamage
        };
    }

    public override void OnBeforeAction(ActionContext ctx)
    {
        if (!ConditionsMet(ctx)) return;

        //Additional runtime check: effect owner must be the attacker
        if (ctx.attacker != Target) return;

        ctx.modifiedDamage += bonus;
        CombatLog.Add($"{Target.unitName} gains +{bonus} bonus damage");
    }
}
