using UnityEngine;
using System.Collections.Generic;

public class DistributedProcessingEffect : StatusEffect
{
    public DistributedProcessingEffect() : base("DistributedProcessing", -1)
    {
        isPersistent = true;
        instancedPassive = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
        => new List<StatModifier>();

    public override void OnTurnEnd(Unit unit) { }
}