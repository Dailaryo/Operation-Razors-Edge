using UnityEngine;
using System.Collections.Generic;

public class CharacterSheetData
{
    //Identity
    public string characterName;
    public string speciesId;
    public string classId;
    public string backgroundId;
    public int level;
    public int xp;
    public int xpToNextLevel;

    //Attributes: each entry (baseScore, totalScore, modifier)
    public Dictionary<StatType, (int baseScore, int totalScore, int modifier)> attributes
        = new Dictionary<StatType, (int, int, int)>();

    //Combat stats
    public int maxHP;
    public int currentHP;
    public int bab;
    public int defense;
    public int protection;
    public int initiative;
    public int resolveSave;
    public int responseSave;
    public int focusSave;

    //Skills: each entry (ranks, attributeModifier, miscBonus, total, isClassSkill)
    public Dictionary<SkillType, (int ranks, int attrMod, int misc, int total, bool isClassSkill)> skills
        = new Dictionary<SkillType, (int, int, int, int, bool)>();

    //Resources
    public string resourceName;
    public int resourceCurrent;
    public int resourceMax;

    //Feats
    public List<string> featIds = new List<string>();

    //Carry weight
    public float currentCarryWeight;
    public float maxCarryWeight;

    //Cybernetics
    public int cyberneticsEquipped;
    public int cyberneticCapacity;

    //Equipped items (slot -> display name)
    public Dictionary<EquipmentSlot, string> equippedItemNames
        = new Dictionary<EquipmentSlot, string>();

    // -------------------------------------------------------------------------

    public static CharacterSheetData FromUnit(Unit unit)
    {
        var sheet = new CharacterSheetData();
        sheet.Populate(unit, unit.sourceCharacterData);
        return sheet;
    }

    public static CharacterSheetData FromCharacterData(CharacterData data)
    {
        Unit tempUnit = CharacterFactory.CreatePlayerUnit(data);
        var sheet = new CharacterSheetData();
        sheet.Populate(tempUnit, data);
        return sheet;
    }

    private void Populate(Unit unit, CharacterData data)
    {
        // Identity
        characterName = data.name;
        speciesId = data.speciesId;
        classId = data.classId;
        backgroundId = data.backgroundId;
        level = data.level;
        xp = data.xp;
        xpToNextLevel = data.xpToNextLevel;

        // Attributes
        StatType[] attrTypes = new StatType[]
        {
            StatType.Power, StatType.Agility, StatType.Endurance,
            StatType.Cognition, StatType.Insight, StatType.Presence
        };

        foreach (var attr in attrTypes)
        {
            int baseScore = GetBaseAttributeScore(data, attr);
            int totalScore = unit.GetAttribute(attr);
            int mod = unit.GetModifier(attr);
            attributes[attr] = (baseScore, totalScore, mod);
        }

        // Combat stats
        maxHP = unit.GetMaxHP();
        currentHP = unit.currentHP;
        bab = unit.GetBAB();
        defense = unit.GetDefense();
        protection = unit.GetProtection();
        initiative = unit.GetDerivedStat(StatType.Initiative);
        resolveSave = unit.GetSaveValue(Unit.SaveType.Resolve);
        responseSave = unit.GetSaveValue(Unit.SaveType.Response);
        focusSave = unit.GetSaveValue(Unit.SaveType.Focus);

        // Skills
        if (SkillDatabase.Definitions != null)
        {
            foreach (var kvp in SkillDatabase.Definitions)
            {
                SkillDefinition skillDef = kvp.Value;

                if (!System.Enum.TryParse(skillDef.skillId, out SkillType skillType))
                {
                    Debug.LogWarning($"[CharacterSheetData] Could not parse SkillType from skillId '{skillDef.skillId}'");
                    continue;
                }

                // Psionic skills are hidden from characters without a psionic class
                if (!SkillResolver.IsSkillAvailable(data, skillType)) continue;

                data.skillRanks.TryGetValue(skillType, out int ranks);
                int attrMod = unit.GetModifier(skillDef.governingAttribute);
                int misc = unit.GetSkillModifierBonus(SkillResolver.GetSkillStatType(skillType));
                int total = SkillResolver.GetSkillTotal(unit, skillType);
                bool isClassSkill = SkillResolver.IsClassSkill(unit, skillType);

                skills[skillType] = (ranks, attrMod, misc, total, isClassSkill);
            }
        }

        // Resource
        if (unit.resource != null)
        {
            resourceName = unit.resource.definition.displayName;
            resourceCurrent = unit.resource.current;
            resourceMax = unit.resource.max;
        }

        // Feats
        featIds = new List<string>(data.equippedFeatIds);

        // Carry weight
        maxCarryWeight = data.GetMaxCarryWeight();
        currentCarryWeight = 0f;
        foreach (var kvp in data.equippedItemIds)
        {
            var item = ItemDatabase.Get(kvp.Value.definitionId);
            if (item != null) currentCarryWeight += item.weight;
        }

        // Cybernetics
        cyberneticsEquipped = data.GetEquippedCyberneticCount();
        cyberneticCapacity = data.GetCyberneticCapacity();

        // Equipped item display names
        foreach (var kvp in data.equippedItemIds)
        {
            var item = ItemDatabase.Get(kvp.Value.definitionId);
            if (item != null)
            {
                string displayName = kvp.Value.customDisplayName ?? item.displayName;
                equippedItemNames[kvp.Key] = displayName;
            }
        }
    }

    private static int GetBaseAttributeScore(CharacterData data, StatType attr)
    {
        switch (attr)
        {
            case StatType.Power: return data.power;
            case StatType.Agility: return data.agility;
            case StatType.Endurance: return data.endurance;
            case StatType.Cognition: return data.cognition;
            case StatType.Insight: return data.insight;
            case StatType.Presence: return data.presence;
            default: return 0;
        }
    }
}
