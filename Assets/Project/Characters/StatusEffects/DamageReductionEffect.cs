using UnityEngine;
using System.Collections.Generic;

public class DamageReductionEffect : StatusEffect
{
    public int amount;

    public DamageReductionEffect(int amount)
        : base("DamageReduction", -1)
    {
        this.amount = amount;
        isPersistent = true;
    }

    public override void OnTurnEnd(Unit unit) { }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>();
    }
}
