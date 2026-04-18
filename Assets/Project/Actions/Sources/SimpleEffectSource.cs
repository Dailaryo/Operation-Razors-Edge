using UnityEngine;
using System.Collections.Generic;

public class SimpleEffectSource : IEffectSource
{
    private List<StatusEffect> effects;

    public SimpleEffectSource(List<StatusEffect> effects)
    {
        this.effects = effects;
    }

    public List<StatusEffect> GetEffects(Unit unit)
    {
        return effects;
    }
}
