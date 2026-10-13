using UnityEngine;
using System.Collections.Generic;

//Sickened: -2 on attack rolls and saves
//(weapon damage, skill and ability check penalties wait on stats that cover them)
public class SickenedEffect : StatusEffect
{
    public SickenedEffect(int rounds) : base("Sickened", rounds) { isHarmful = true; }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Accuracy, -2, 1f, "Sickened", StatModifier.BonusType.Untyped),
            new StatModifier(StatType.Fortitude, -2, 1f, "Sickened", StatModifier.BonusType.Untyped),
            new StatModifier(StatType.Evasion,   -2, 1f, "Sickened", StatModifier.BonusType.Untyped),
            new StatModifier(StatType.Willpower, -2, 1f, "Sickened", StatModifier.BonusType.Untyped)
        };
    }
}
