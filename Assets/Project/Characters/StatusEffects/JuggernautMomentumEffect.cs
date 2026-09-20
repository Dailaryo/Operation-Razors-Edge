using System.Collections.Generic;
using UnityEngine;

public class JuggernautMomentumEffect : StatusEffect
{
    private Unit lastTarget;
    private int stacks;

    public JuggernautMomentumEffect() : base("JuggernautMomentum", 0)
    {
        isPersistent = true;
    }

    public int GetBonusDamage(Unit target)
    {
        if (target != lastTarget)
        {
            lastTarget = target;
            stacks = 0;
        }
        return stacks;
    }

    public void RecordHit(Unit target)
    {
        if (target != lastTarget)
        {
            lastTarget = target;
            stacks = 1;
        }
        else
        {
            stacks++;
        }
    }

    public override void OnTurnEnd(Unit unit) { }
}
