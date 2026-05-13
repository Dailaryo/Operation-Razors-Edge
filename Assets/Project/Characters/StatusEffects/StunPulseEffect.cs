using UnityEngine;
using System.Collections.Generic;

public class StunPulseEffect : StatusEffect
{
    public StunPulseEffect() : base("StunPulse", 1) { }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(
                StatType.Accuracy,
                -1,
                1f,
                "StunPulse",
                StatModifier.BonusType.Untyped)
        };
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}