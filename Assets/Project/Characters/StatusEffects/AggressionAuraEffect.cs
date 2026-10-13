using UnityEngine;

public class AggressionAuraEffect : StatusEffect
{
    public AggressionAuraEffect() : base("AggressionAura", 0)
    {
        isPersistent = true;
    }

    public override bool BlocksDescriptors(EffectDescriptor incoming, int sourceLevel, Unit bearer)
    {
        if ((incoming & EffectDescriptor.Fear) == 0) return false;
        if (sourceLevel <= 0) return false;

        int bearerLevel = bearer.sourceCharacterData?.level ?? 0;
        return sourceLevel < bearerLevel;
    }
}
