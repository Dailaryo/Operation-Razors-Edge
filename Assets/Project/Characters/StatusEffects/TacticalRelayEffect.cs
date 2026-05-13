using UnityEngine;
using System.Collections.Generic;

public class TacticalRelayEffect : StatusEffect
{
    public TacticalRelayEffect() : base("TacticalRelay", -1)
    {
        isPersistent = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
        => new List<StatModifier>();

    public override void OnTurnEnd(Unit unit) { }
}