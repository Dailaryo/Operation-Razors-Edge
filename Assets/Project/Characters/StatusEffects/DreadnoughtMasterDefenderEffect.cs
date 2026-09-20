using System.Collections.Generic;
using UnityEngine;

public class DreadnoughtMasterDefenderEffect : StatusEffect
{
    public DreadnoughtMasterDefenderEffect() : base("DreadnoughtMasterDefender", 0)
    {
        isPersistent = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        if (unit.sourceCharacterData == null) return new List<StatModifier>();

        // Requires medium, heavy, or powered armor
        if (!HasQualifyingArmor(unit)) return new List<StatModifier>();

        // Requires fighting defensively or total defense
        // (unless Bastion level 13 passive is present — checked separately)
        bool inStance = unit.activeEffects.Exists(e =>
            e is DefenseBuffEffect);

        bool bastionOverride = unit.sourceCharacterData.unlockedPassiveIds
            .Contains(PassiveIds.BastionMasterDefenderAlways);

        if (!inStance && !bastionOverride) return new List<StatModifier>();

        int classLevel = unit.sourceCharacterData.level;
        int bonus = classLevel >= 10 ? 6
                  : classLevel >= 7  ? 4
                  : classLevel >= 4  ? 2
                  : 0;

        if (bonus == 0) return new List<StatModifier>();

        return new List<StatModifier>
        {
            new StatModifier(StatType.Evasion, bonus, 1f,
                "master_defender", StatModifier.BonusType.Competence)
        };
    }

    private bool HasQualifyingArmor(Unit unit)
    {
        if (!unit.sourceCharacterData.HasItemEquipped(EquipmentSlot.Armor))
            return false;

        var armorInstance = unit.sourceCharacterData.GetEquippedItem(EquipmentSlot.Armor);
        var armor = ItemDatabase.Get(armorInstance.definitionId);
        if (armor == null) return false;

        return armor.armorWeightClass == ItemDefinition.ArmorWeightClass.Medium
            || armor.armorWeightClass == ItemDefinition.ArmorWeightClass.Heavy
            || armor.armorWeightClass == ItemDefinition.ArmorWeightClass.Powered;
    }
}
