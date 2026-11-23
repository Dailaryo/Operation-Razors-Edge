using UnityEngine;
using System.Collections.Generic;

public class StatBonusEffect : StatusEffect
{
    private StatModifier modifier;

    public StatBonusEffect(StatType stat, int flatbonus, string source,
        StatModifier.BonusType bonusType = StatModifier.BonusType.Untyped)
        : base($"{source}_{stat}_bonus", -1)  //-1: polled sources (feats, class, species). Runtime buffs set duration or isPersistent
    {
        modifier = new StatModifier(stat, flatbonus, 1f, source, bonusType);
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier> { modifier };
    }
}
