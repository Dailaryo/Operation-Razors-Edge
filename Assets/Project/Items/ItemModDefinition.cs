using UnityEngine;
using System.Collections.Generic;

public class ItemModDefinition
{
    // Identity
    public string modId;
    public string displayName;
    public string description;
    public int techTier;
    public int baseValue;

    // Restrictions — which item types this mod can be installed on
    // Empty list means no restriction
    public List<ItemType> compatibleItemTypes;

    // Stat contributions
    public List<StatModifier> statModifiers;

    // Behavior — either or both may be null
    public IEffectSource effectSource;

    // Constructor
    public ItemModDefinition(
        string modId,
        string displayName,
        string description,
        int techTier = 6,
        int baseValue = 0,
        List<ItemType> compatibleItemTypes = null,
        List<StatModifier> statModifiers = null,
        IEffectSource effectSource = null
    )
    {
        this.modId = modId;
        this.displayName = displayName;
        this.description = description;
        this.techTier = techTier;
        this.baseValue = baseValue;
        this.compatibleItemTypes = compatibleItemTypes ?? new List<ItemType>();
        this.statModifiers = statModifiers ?? new List<StatModifier>();
        this.effectSource = effectSource;
    }

    public bool IsCompatibleWith(ItemDefinition item)
    {
        if (compatibleItemTypes.Count == 0) return true;
        return compatibleItemTypes.Contains(item.itemType);
    }
}