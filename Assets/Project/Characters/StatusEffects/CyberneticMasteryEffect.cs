using UnityEngine;
using System.Collections.Generic;

public class CyberneticMasteryEffect : StatusEffect
{
    public CyberneticMasteryEffect() : base("CyberneticMastery", -1)
    {
        isPersistent = true;
    }

    public override void OnApply(Unit unit)
    {
        base.OnApply(unit);
        if (unit.resource != null)
            unit.resource.max += 2;
    }

    public override void OnExpire(Unit unit)
    {
        if (unit.resource != null)
            unit.resource.max = System.Math.Max(0, unit.resource.max - 2);
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
        => new List<StatModifier>();

    public override void OnTurnEnd(Unit unit) { }
}