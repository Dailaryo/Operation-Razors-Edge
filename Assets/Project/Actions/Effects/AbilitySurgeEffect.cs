using System.Collections.Generic;
using UnityEngine;

public class AbilitySurgeEffect : StatusEffect
{
    private int powBonus;
    private int defensePenalty;
    private int fatigueCountdown;

    public AbilitySurgeEffect(int classLevel, int powBonus = 8) : base("AbilitySurge", classLevel)
    {
        this.powBonus = powBonus;
        defensePenalty = -2;
        fatigueCountdown = classLevel;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Power,  powBonus,       1f, "ability_surge", StatModifier.BonusType.Untyped),
            new StatModifier(StatType.Evasion, defensePenalty, 1f, "ability_surge", StatModifier.BonusType.Untyped)
        };
    }

    public override void OnExpire(Unit unit)
    {
        // Apply fatigue for the same duration the surge lasted
        unit.AddEffect(new AbilitySurgeFatigueEffect(fatigueCountdown));
    }
}
