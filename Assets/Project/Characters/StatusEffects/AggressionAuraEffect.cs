using UnityEngine;

public class AggressionAuraEffect : StatusEffect
{
    public AggressionAuraEffect() : base("AggressionAura", 0)
    {
        isPersistent = true;
    }

    public bool BlocksFearEffect(StatusEffect incomingEffect, int unitLevel)
    {
        if (!IsFearEffect(incomingEffect)) return false;
        if (incomingEffect.sourceLevel <= 0) return false;
        return incomingEffect.sourceLevel < unitLevel;
    }

    private bool IsFearEffect(StatusEffect effect)
    {
        return effect is ShakenEffect;
        // Extend this check as new fear effects are added (Frightened, Panicked)
    }
}
