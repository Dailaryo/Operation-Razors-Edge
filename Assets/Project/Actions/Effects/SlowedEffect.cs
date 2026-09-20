using System.Collections.Generic;
using UnityEngine;

public class SlowedEffect : StatusEffect
{
    public SlowedEffect() : base("Slowed", 2) { }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Evasion, -2, 1f,
                "slowed", StatModifier.BonusType.Morale)
        };
    }
}
