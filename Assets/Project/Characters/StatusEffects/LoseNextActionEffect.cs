using UnityEngine;
using System.Collections.Generic;

public class LoseNextActionEffect : StatusEffect
{
    public override bool BlocksActions => true;

    public LoseNextActionEffect() : base("LoseNextAction", 1) { isHarmful = true; }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>();
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}