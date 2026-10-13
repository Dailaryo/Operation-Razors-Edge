using UnityEngine;

//Brain Lock: mentally paralyzed. No actions or movement; still defends normally (not stunned)
public class BrainLockedEffect : StatusEffect
{
    public BrainLockedEffect(int rounds) : base("BrainLocked", rounds)
    {
        descriptors = EffectDescriptor.MindAffecting;
        isHarmful = true;
    }

    public override bool BlocksActions => true;
}
