using UnityEngine;
using System.Collections.Generic;

public class WeaponMountEffect : StatusEffect
{
    public WeaponMountEffect() : base("WeaponMount", 3) { }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Accuracy, 1, 1f, "WeaponMount",
                StatModifier.BonusType.Equipment)
        };
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}