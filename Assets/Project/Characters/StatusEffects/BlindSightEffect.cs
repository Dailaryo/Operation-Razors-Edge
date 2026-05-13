using UnityEngine;
using System.Collections.Generic;

public class BlindSightEffect : StatusEffect
{
    public int range;

    public BlindSightEffect(int range)
        : base("BlindSight", -1)
    {
        this.range = range;
        isPersistent = true;
    }

    //No stat modifiers, this is a capability flag, not a numeric bonus
    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>();
    }

    //Persistent: never ticks down
    public override void OnTurnEnd(Unit unit) { }
}
