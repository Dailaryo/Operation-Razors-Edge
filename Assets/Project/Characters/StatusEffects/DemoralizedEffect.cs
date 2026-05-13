using UnityEngine;
using System.Collections.Generic;

public class DemoralizedEffect : StatusEffect
{
    public DemoralizedEffect(): base("Demoralized", 2) { } //lasts 2 turns

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Accuracy, -2, 1f,
            "demoralized", StatModifier.BonusType.Morale),
            new StatModifier(StatType.Evasion, -2, 1f,
            "demoralized", StatModifier.BonusType.Morale)
        };
    }
}
