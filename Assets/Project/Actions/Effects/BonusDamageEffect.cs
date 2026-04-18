using UnityEngine;

public class BonusDamageEffect : StatusEffect
{
    int bonus;

    public BonusDamageEffect(int amount) : base("Bonus Damage", 0)
    {
        bonus = amount;
        Priority = 5;
    }

    public override void OnBeforeAction(ActionContext ctx)
    {
        //Only affect the attacker using this action
        if (ctx.attacker == Target)
        {
            ctx.modifiedDamage += bonus;

            CombatLog.Add($"{Target.unitName} gains +{bonus} bonus damage");
        }
    }
}
