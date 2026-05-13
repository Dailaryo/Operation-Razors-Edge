using UnityEngine;
using System.Collections.Generic;

public class OverwatchSpecialistEffect : StatusEffect
{
    public OverwatchSpecialistEffect() : base("OverwatchSpecialist", -1)
    {
        isPersistent = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
        => new List<StatModifier>();

    public override void OnTurnEnd(Unit unit) { }
}