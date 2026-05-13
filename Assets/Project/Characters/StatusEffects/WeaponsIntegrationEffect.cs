using UnityEngine;
using System.Collections.Generic;

public class WeaponsIntegrationEffect : StatusEffect
{
    public WeaponsIntegrationEffect() : base("WeaponsIntegration", -1)
    {
        isPersistent = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Accuracy,
                unit.GetModifier(StatType.Cognition),
                1f, "WeaponsIntegration",
                StatModifier.BonusType.Untyped)
        };
    }

    public override void OnTurnEnd(Unit unit) { }
}