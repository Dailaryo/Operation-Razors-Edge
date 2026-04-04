using UnityEngine;

public class TacticalBoostEffect : StatusEffect
{
    int bonusAmount;

    public TacticalBoostEffect(int amount, int duration) : base("Tactical Boost", duration)
    {
        bonusAmount = amount;
        Priority = 50;
    }

    public override void OnBeforeAction(ActionContext ctx)
    {
        //Only affect the unit that has this buff
        if (ctx.attacker == Target)
        {
            ctx.modifiedDamage += bonusAmount;

            CombatLog.Add($"{Target.unitName} gains +{bonusAmount} damage from Tactical Boost");
        }
    }
}
