using UnityEngine;
using System.Collections.Generic;

public class OverchargeEffect : StatusEffect
{
    private int bonus;

    public OverchargeEffect(int bonus) : base("Overcharge", 1)
    {
        this.bonus = bonus;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Accuracy, bonus, 1f, "Overcharge", StatModifier.BonusType.Untyped)
        };
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}
