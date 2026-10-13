using UnityEngine;

public class DreadnoughtFearlessEffect : StatusEffect
{
    public DreadnoughtFearlessEffect() : base("DreadnoughtFearless", 0)
    {
        isPersistent = true;
    }

    public override bool BlocksDescriptors(EffectDescriptor incoming, int sourceLevel, Unit bearer)
    {
        return (incoming & EffectDescriptor.Fear) != 0;
    }
}
