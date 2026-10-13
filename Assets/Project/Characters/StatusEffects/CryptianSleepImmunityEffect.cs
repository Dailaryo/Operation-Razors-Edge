using UnityEngine;

public class CryptianSleepImmunityEffect : StatusEffect
{
    public CryptianSleepImmunityEffect() : base("CryptianSleepImmunity", 0)
    {
        isPersistent = true;
    }

    public override bool BlocksDescriptors(EffectDescriptor incoming, int sourceLevel, Unit bearer)
    {
        return (incoming & EffectDescriptor.Sleep) != 0;
    }
}
