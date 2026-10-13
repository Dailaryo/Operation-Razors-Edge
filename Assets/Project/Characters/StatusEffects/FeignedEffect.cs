using UnityEngine;
using System.Collections.Generic;

public class FeignedEffect : StatusEffect
{
    private int agilityPenalty;

    public FeignedEffect(Unit target): base("Feigned", 1)
    {
        //Negate their ability bonus to evasion
        agilityPenalty = Mathf.Min(0,-target.GetModifier(StatType.Agility));
        isHarmful = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Defense, agilityPenalty, 1f,
            "feigned", StatModifier.BonusType.Untyped)
        };
    }
}
