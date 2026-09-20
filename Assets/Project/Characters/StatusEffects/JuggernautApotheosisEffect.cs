using System.Collections.Generic;
using UnityEngine;

public class JuggernautApotheosisEffect : StatusEffect
{
    public JuggernautApotheosisEffect() : base("JuggernautApotheosis", 0)
    {
        isPersistent = true;
    }

    public override void OnApply(Unit unit)
    {
        base.OnApply(unit);
        // Apply permanent ability surge stat bonuses directly
        // (no fatigue, no duration limit)
        unit.AddEffect(new JuggernautApotheosisStatEffect());
    }
}

public class JuggernautApotheosisStatEffect : StatusEffect
{
    public JuggernautApotheosisStatEffect() : base("JuggernautApotheosisStats", 0)
    {
        isPersistent = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Power, 10, 1f,
                "juggernaut_apotheosis", StatModifier.BonusType.Untyped)
        };
    }
}
