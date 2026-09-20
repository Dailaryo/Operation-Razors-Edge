using UnityEngine;

public class DreadnoughtFearlessEffect : StatusEffect
{
    public DreadnoughtFearlessEffect() : base("DreadnoughtFearless", 0)
    {
        isPersistent = true;
    }

    public bool BlocksFearEffect(StatusEffect incomingEffect)
    {
        return incomingEffect is ShakenEffect;
        // Extend as Frightened, Panicked are added
    }
}
