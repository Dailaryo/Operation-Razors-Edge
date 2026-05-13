using UnityEngine;
using System.Collections.Generic;

public class TacticalUplinkBuffEffect : StatusEffect
{
    public int bonus;

    public TacticalUplinkBuffEffect(int bonus) : base("TacticalUplinkBuff", 1)
    {
        this.bonus = bonus;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Accuracy, bonus, 1f, "TacticalUplink",
                StatModifier.BonusType.Equipment)
        };
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}