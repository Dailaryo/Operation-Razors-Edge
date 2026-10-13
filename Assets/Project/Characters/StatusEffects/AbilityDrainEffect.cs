using UnityEngine;
using System.Collections.Generic;

//Ability damage/drain: a penalty to one attribute that lasts until the Hub.
//Instances on the same attribute merge (same type + name) and their amounts stack.
public class AbilityDrainEffect : StatusEffect
{
    public StatType attribute;
    public int amount;

    public AbilityDrainEffect(StatType attribute, int amount)
        : base("AbilityDrain_" + attribute, 0)
    {
        this.attribute = attribute;
        this.amount = amount;
        lastsUntilHub = true;
        isHarmful = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(attribute, -amount, 1f, effectName, StatModifier.BonusType.Untyped)
        };
    }

    public override void OnRefresh(StatusEffect incoming)
    {
        if (incoming is AbilityDrainEffect other)
            amount += other.amount;
    }
}
