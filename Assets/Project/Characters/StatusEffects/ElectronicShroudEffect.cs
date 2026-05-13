using UnityEngine;
using System.Collections.Generic;

public class ElectronicShroudEffect : StatusEffect
{
    public bool active = false;

    public ElectronicShroudEffect() : base("ElectronicShroud", -1)
    {
        isPersistent = true;
    }

    public void Activate()
    {
        active = true;
        duration = 1;
        isPersistent = false;
    }

    public override void OnExpire(Unit unit)
    {
        active = false;
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }

    public void ResetForCombat() => active = false;

    public override List<StatModifier> GetStatModifiers(Unit unit)
        => new List<StatModifier>();
}