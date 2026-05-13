using UnityEngine;
using System.Collections.Generic;

public class CharacterSnapshot
{
    //Captured fields
    public int currentHP;
    public int baseMaxHP;
    public int level;
    public int xp;
    public int xpToNextLevel;
    public int featPointsAvailable;
    public int skillRanksAvailable;

    public List<string> equippedFeatIds;
    public List<string> pendingChoiceNodeIds;

    public Dictionary<SkillType, int> skillRanks;
    public Dictionary<EquipmentSlot, ItemInstance> cyberneticItemIds;
    public Dictionary<StatType, int> abilityScoreIncreases;

    //Factory
    public static CharacterSnapshot Capture(CharacterData data)
    {
        var snap = new CharacterSnapshot();
        snap.currentHP = data.currentHP;
        snap.baseMaxHP = data.baseMaxHP;
        snap.level = data.level;
        snap.xp = data.xp;
        snap.xpToNextLevel = data.xpToNextLevel;
        snap.featPointsAvailable = data.featPointsAvailable;
        snap.skillRanksAvailable = data.skillRanksAvailable;

        snap.equippedFeatIds = new List<string>(data.equippedFeatIds);
        snap.pendingChoiceNodeIds = new List<string>(data.pendingChoiceNodeIds);

        snap.skillRanks = new Dictionary<SkillType, int>();
        foreach (var kvp in data.skillRanks)
            snap.skillRanks[kvp.Key] = kvp.Value;

        snap.abilityScoreIncreases = new Dictionary<StatType, int>();
        foreach (var kvp in data.abilityScoreIncreases)
            snap.abilityScoreIncreases[kvp.Key] = kvp.Value;

        snap.cyberneticItemIds = new Dictionary<EquipmentSlot, ItemInstance>();
        foreach (var kvp in data.equippedItemIds)
        {
            if (CharacterData.IsCyberneticSlot(kvp.Key))
                snap.cyberneticItemIds[kvp.Key] = new ItemInstance
                {
                    instanceId = kvp.Value.instanceId,
                    definitionId = kvp.Value.definitionId,
                    installedModIds = new List<string>(kvp.Value.installedModIds),
                    customDisplayName = kvp.Value.customDisplayName,
                };
        }

        return snap;
    }

    //Restore
    public void Restore(CharacterData target)
    {
        target.currentHP = currentHP;
        target.baseMaxHP = baseMaxHP;
        target.level = level;
        target.xp = xp;
        target.xpToNextLevel = xpToNextLevel;
        target.featPointsAvailable = featPointsAvailable;
        target.skillRanksAvailable = skillRanksAvailable;
        target.levelUpPending = false;

        target.equippedFeatIds = new List<string>(equippedFeatIds);
        target.pendingChoiceNodeIds = new List<string>(pendingChoiceNodeIds);

        target.skillRanks = new Dictionary<SkillType, int>();
        foreach (var kvp in skillRanks)
            target.skillRanks[kvp.Key] = kvp.Value;

        target.abilityScoreIncreases = new Dictionary<StatType, int>();
        foreach (var kvp in abilityScoreIncreases)
            target.abilityScoreIncreases[kvp.Key] = kvp.Value;

        // Collect cybernetic keys to remove first
        var cyberSlotsToClear = new List<EquipmentSlot>();
        foreach (var kvp in target.equippedItemIds)
        {
            if (CharacterData.IsCyberneticSlot(kvp.Key))
                cyberSlotsToClear.Add(kvp.Key);
        }
        foreach (var slot in cyberSlotsToClear)
            target.equippedItemIds.Remove(slot);

        // Then write snapshot cybernetics back
        foreach (var kvp in cyberneticItemIds)
            target.equippedItemIds[kvp.Key] = new ItemInstance
            {
                instanceId = kvp.Value.instanceId,
                definitionId = kvp.Value.definitionId,
                installedModIds = new List<string>(kvp.Value.installedModIds),
                customDisplayName = kvp.Value.customDisplayName,
            };
    }
}
