using UnityEngine;
using System.Collections.Generic;

public class ItemDefinition
{
    //Identity
    public string itemId;
    public string displayName;
    public string description;

    //Classification
    public ItemType itemType;
    public EquipmentSlot slot;
    public int techTier;
    public TechCategory techCategory;

    //Metadata
    public float weight;
    public int baseValue;
    public int modSlotCount;

    //Stat contributions
    public List<StatModifier> statModifiers;

    //Behavior
    public IActionSource actionSource;
    public IEffectSource effectSource;

    //Weapon data: only meaningful when itemType == ItemType.Weapon
    public WeaponTag weaponTag;
    public int damageDiceCount; //number of dice (2)d6
    public int damageDiceSides; //sides per die 2d(6)
    public int damageBonus;  //flat bonus added after rolling

    public string TierLabel => TechTierProfile.GetDiegeticLabel(techCategory, techTier);

    //Constructor
    public ItemDefinition(
        string itemId,
        string displayName,
        string description,
        ItemType itemType,
        EquipmentSlot slot,
        int techTier = 6,
        TechCategory techCategory = TechCategory.Weapons,
        float weight = 0f,
        int baseValue = 0,
        int modSlotCount = 0,
        List<StatModifier> statModifiers = null,
        IActionSource actionSource = null,
        IEffectSource effectSource = null,
        WeaponTag weaponTag = WeaponTag.Unarmed,
        int damageDiceCount = 1,
        int damageDiceSides = 4,
        int damageBonus =0
    )
    {
        this.itemId = itemId;
        this.displayName = displayName;
        this.description = description;
        this.itemType = itemType;
        this.slot = slot;
        this.techTier = techTier;
        this.techCategory = techCategory;
        this.weight = weight;
        this.baseValue = baseValue;
        this.modSlotCount = modSlotCount;
        this.statModifiers = statModifiers ?? new List<StatModifier>();
        this.actionSource = actionSource;
        this.effectSource = effectSource;
        this.weaponTag = weaponTag;
        this.damageDiceCount = damageDiceCount;
        this.damageDiceSides = damageDiceSides;
        this.damageBonus = damageBonus;
    }
}