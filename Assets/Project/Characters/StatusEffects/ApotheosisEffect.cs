using UnityEngine;
using System.Collections.Generic;

public class ApotheosisEffect : StatusEffect
{
    private string sourceName;

    public ApotheosisEffect(string sourceName) : base("Apotheosis", -1)
    {
        this.sourceName = sourceName;
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
            unit.resource.max = Mathf.Max(0, unit.resource.max - 2);
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
        => new List<StatModifier>();

    public override void OnTurnEnd(Unit unit) { }
}