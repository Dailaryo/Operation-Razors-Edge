using UnityEngine;
using System.Collections.Generic;

public class SensorNetEffect : StatusEffect
{
    public SensorNetEffect() : base("SensorNet", 1) { }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(
                StatType.SkillPerception,
                1,
                1f,
                "SensorNet",
                StatModifier.BonusType.Untyped)
        };
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}