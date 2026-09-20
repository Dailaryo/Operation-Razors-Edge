using System.Collections.Generic;
using UnityEngine;

public class AbilitySurgeFatigueEffect : StatusEffect
{
    public AbilitySurgeFatigueEffect(int duration) : base("AbilitySurgeFatigue", duration) { }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.BaseAttackBonus, -2, 1f, "ability_surge_fatigue", StatModifier.BonusType.Untyped),
            new StatModifier(StatType.Fortitude,       -2, 1f, "ability_surge_fatigue", StatModifier.BonusType.Untyped),
            new StatModifier(StatType.Evasion,         -2, 1f, "ability_surge_fatigue", StatModifier.BonusType.Untyped),
            new StatModifier(StatType.Willpower,       -2, 1f, "ability_surge_fatigue", StatModifier.BonusType.Untyped),
        };
    }
}
