using UnityEngine;

//Unshakable Zeal (PF), partial: when the bearer would be affected by a fear or emotion effect, the power is
//dismissed to negate it (one use). Lasts until the Hub (1 hour/level).
//Deferred: +4 morale bonus on the next check of the same type after a failed attack roll, save,
//skill, concentration or ability check against the same opponent or effect.
public class UnshakableZealEffect : StatusEffect
{
    public UnshakableZealEffect() : base("UnshakableZeal", 0)
    {
        lastsUntilHub = true;
    }

    public override bool BlocksDescriptors(EffectDescriptor incoming, int sourceLevel, Unit bearer)
    {
        return !ended && (incoming & (EffectDescriptor.Fear | EffectDescriptor.Emotion)) != 0;
    }

    public override void OnBlockedDescriptors(Unit bearer, EffectDescriptor incoming)
    {
        ended = true;
        CombatLog.Add($"{bearer.unitName} dismisses Unshakable Zeal to shrug off the effect.");
    }
}
