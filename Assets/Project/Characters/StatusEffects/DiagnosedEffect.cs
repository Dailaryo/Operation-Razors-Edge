using UnityEngine;
using System.Collections.Generic;

public class DiagnosedEffect : StatusEffect
{
    public DiagnosedEffect() : base("Diagnosed", 2)
    {
        isHarmful = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(
                StatType.Defense,
                -1,
                1f,
                "Diagnosed",
                StatModifier.BonusType.Untyped)
        };
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}
