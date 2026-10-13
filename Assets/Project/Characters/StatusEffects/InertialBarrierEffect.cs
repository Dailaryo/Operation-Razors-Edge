using UnityEngine;

//Inertial Barrier: DR 10/— until it has prevented its capacity (10 x ML, max 150), then discharged.
//10 minutes/level: lasts until Hub return (TEMP)
public class InertialBarrierEffect : DamageReductionEffect
{
    public int remaining;

    public InertialBarrierEffect(int capacity) : base(10)
    {
        effectName = "InertialBarrier";
        remaining = capacity;
        lastsUntilHub = true;
    }
}
