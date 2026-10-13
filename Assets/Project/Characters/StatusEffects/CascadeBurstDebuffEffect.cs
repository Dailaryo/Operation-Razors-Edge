using UnityEngine;
using System.Collections.Generic;

public class CascadeBurstDebuffEffect : StatusEffect
{
    public CascadeBurstDebuffEffect() : base("CascadeBurstDebuff", 1) { isHarmful = true; }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Accuracy,   -1, 1f, "CascadeBurst", StatModifier.BonusType.Untyped),
            new StatModifier(StatType.Evasion,    -1, 1f, "CascadeBurst", StatModifier.BonusType.Untyped),
            new StatModifier(StatType.Fortitude,  -1, 1f, "CascadeBurst", StatModifier.BonusType.Untyped),
            new StatModifier(StatType.Willpower,  -1, 1f, "CascadeBurst", StatModifier.BonusType.Untyped),
        };
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}