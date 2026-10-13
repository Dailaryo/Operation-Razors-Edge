using UnityEngine;
using System.Collections.Generic;

public class SensorLinkFeatEffect : StatusEffect
{
    public SensorLinkFeatEffect() : base("SensorLink", -1)
    {
        isPersistent = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.SkillPerception, 1, 1f, "SensorLink",
                StatModifier.BonusType.Untyped)
        };
    }

    public override void OnTurnEnd(Unit unit) { }
}