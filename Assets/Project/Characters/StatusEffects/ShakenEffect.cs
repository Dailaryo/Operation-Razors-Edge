using UnityEngine;
using System.Collections.Generic;

public class ShakenEffect : StatusEffect
{
    public ShakenEffect() : base("Shaken", 2) //lasts 2 turns
    {
        descriptors = EffectDescriptor.Fear | EffectDescriptor.MindAffecting;
        isHarmful = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Accuracy, -2, 1f,
            "shaken", StatModifier.BonusType.Morale),
            new StatModifier(StatType.Evasion, -2, 1f,
            "shaken", StatModifier.BonusType.Morale)
        };
    }
}
