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

        // Stance: fighting defensively or total defense (names set by the ActionDatabase entries)
        bool fightingDefensively = unit.activeEffects.Exists(e => e.effectName == "Fighting Defensively");
        bool totalDefense = unit.activeEffects.Exists(e => e.effectName == "Total Defense");

        // Bastion level 13 (Master Defender Always On): no stance required
        bool bastionOverride = unit.sourceCharacterData.unlockedPassiveIds
            .Contains(PassiveIds.BastionMasterDefenderAlways);

        if (!fightingDefensively && !totalDefense && !bastionOverride) return new List<StatModifier>();

        int classLevel = unit.sourceCharacterData.level;
        if (classLevel < 4) return new List<StatModifier>();

        // Total defense: +2 / +4 / +6 at class levels 4 / 7 / 10. Fighting defensively: flat +2.
        // Bastion Always On with neither stance active uses the total defense value.
        int scaling = classLevel >= 10 ? 6
                    : classLevel >= 7 ? 4
                    : 2;

        int bonus = fightingDefensively && !totalDefense ? 2 : scaling;

        return new List<StatModifier>
        {
            new StatModifier(StatType.Defense, bonus, 1f,
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
