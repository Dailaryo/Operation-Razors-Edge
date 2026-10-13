using UnityEngine;

public class PhytonianSleepImmunityEffect : StatusEffect
{
    public PhytonianSleepImmunityEffect() : base("PhytonianSleepImmunity", 0)
    {
        isPersistent = true;
    }

    public override bool BlocksDescriptors(EffectDescriptor incoming, int sourceLevel, Unit bearer)
    {
        return (incoming & EffectDescriptor.Sleep) != 0;
    }
}