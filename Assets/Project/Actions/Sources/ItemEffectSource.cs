using UnityEngine;
using System.Collections.Generic;

public class ItemEffectSource : IEffectSource
{
    private string itemId;
    private List<StatusEffect> effects;

    public ItemEffectSource(string itemId, List<StatusEffect> effects)
    {
        this.itemId = itemId;
        this.effects = effects;
    }

    public List<StatusEffect> GetEffects(Unit unit)
    {
        return effects;
    }
}
