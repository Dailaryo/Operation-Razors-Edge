using System.Collections.Concurrent;
using UnityEngine;

public class NextAttackBonusEffect : StatusEffect
{
    int bonus;

    public NextAttackBonusEffect(int amount) : base("Next Attack Boost", 1)
    {
        bonus = amount;
        Priority = 50;
    }

    bool consumed = false;

    public override void OnBeforeAction(ActionContext ctx)
    {
        if (consumed) return;

        if (ctx.attacker == Target)
        { 
            var def = ctx.definition;

            if (!def.dealsDamage) return;

            //Consume effect immediately
            consumed = true;
            duration = 0;
            
            ctx.modifiedDamage += bonus;

            CombatLog.Add($"{Target.unitName} gains +{bonus} damage from Tactical Boost");
        }
    }
}
