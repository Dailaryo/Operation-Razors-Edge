using UnityEngine;
using System.Collections.Generic;

//Infiltrator Phantom: 20% miss chance on all attacks made against this unit
public class BlurredPresenceEffect : StatusEffect
{
    public const int MissChance = 20;

    public BlurredPresenceEffect()
        : base("BlurredPresence", -1)
    {
        isPersistent = true;
    }

    public override int GetIncomingMissChance(Unit owner) => MissChance;

    //Persistent: never ticks down
    public override void OnTurnEnd(Unit unit) { }
}
