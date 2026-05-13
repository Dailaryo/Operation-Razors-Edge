using UnityEngine;
using System.Collections.Generic;

public class TacticalRelayBuffEffect : StatusEffect
{
    public TacticalRelayBuffEffect() : base("TacticalRelayBuff", 1)
    {
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
        if (ctx.attacker != Target) return;

        ctx.modifiedDamage += 1;
        CombatLog.Add($"{Target.unitName}: Tactical Relay +1 damage.");
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
        => new List<StatModifier>();

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}