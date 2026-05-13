using UnityEngine;
using System.Collections.Generic;

public class OverdriveEffect : StatusEffect
{
    public OverdriveEffect() : base("Overdrive", 1) { }

    public override List<StatModifier> GetStatModifiers(Unit unit)
        => new List<StatModifier>();

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}