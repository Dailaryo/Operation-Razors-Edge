using UnityEngine;
using System.Collections.Generic;

public class FeatEffectSource : IEffectSource
{
    private string featId;
    private List<StatusEffect> effects;

    public FeatEffectSource(string featId, List<StatusEffect> effects)
    {
        this.featId = featId;
        this.effects = effects;
    }

    public List<StatusEffect> GetEffects(Unit unit)
    {
        return effects;
    }
}
