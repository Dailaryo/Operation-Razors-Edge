using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class SaveManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    //Serializable DTOs
    [System.Serializable]
    public class SaveData
    {
        public int saveVersion;
        public int currentActIndex;
        public int creditBalance;
        public List<PlanetSaveData> planets = new List<PlanetSaveData>();
        public List<FlagSaveData> flags = new List<FlagSaveData>();
        public List<CharacterSaveData> squad = new List<CharacterSaveData>();
        public List<string> journal = new List<string>();
        public List<ItemInstanceSaveData> shipInventory = new List<ItemInstanceSaveData>();
        public List<ItemInstanceSaveData> squadInventory = new List<ItemInstanceSaveData>();
        public List<SnapshotSaveData> deploymentSnapshots = new List<SnapshotSaveData>();
    }

    [System.Serializable]
    public class PlanetSaveData
    {
        public string planetId;
        public int accessibility; //stored as int, cast to enum on load
        
        //Exploration state
        public List<string> unlockedZones = new List<string>();
        public List<string> completedEncounters = new List<string>();
        public List<RandomEncounterCountEntry> randomEncounterCounts = new List<RandomEncounterCountEntry>();
    }

    [System.Serializable]
    public class RandomEncounterCountEntry
    {
        public string zoneId;
        public int count;
    }

    [System.Serializable]
    public class FlagSaveData
    {
        public string key;
        public int value;
    }

    [System.Serializable]
    public class SkillRankEntry
    {
        public string skillType; //stored as string, parsed to enum on load
        public int ranks;
    }

    [System.Serializable]
    public class EquippedItemEntry
    {
        public string slot; //stored as string, parsed to enum on load
        public ItemInstanceSaveData instance;
    }

    [System.Serializable]
    public class CharacterSaveData
    {
        //Any field added here must also be added to CharacterSnapshot, SnapshotSaveData and CharacterData.ResetToLevelOne()
        public string characterId;
        public int hpDamage;
        public int baseMaxHP;
        public int level;
        public int xp;
        public int xpToNextLevel;
        public int featPointsAvailable;
        public int skillRanksAvailable;
        public int powerPointsSpent;
        public int zeroLevelUsesSpent;
        public int manifesterLevel;
        public List<string> equippedFeatIds = new List<string>();
        public List<string> baseActionIds = new List<string>();
        public List<string> knownPowerIds = new List<string>();
        public List<SkillRankEntry> skillRanks = new List<SkillRankEntry>();
        public List<EquippedItemEntry> equippedItems = new List<EquippedItemEntry>();
        public List<ASIEntry> abilityScoreIncreases = new List<ASIEntry>();
        public bool isIncapacitated;
        public bool incapacitatedWithoutRollback;
        public bool defensiveManifesting;
        public int psionicResistance;
        public int respecCount;
        public int cyberneticCapacityBonus;
        public int mutationPointsSpent;
        public bool levelUpPending;
        public List<string> unlockedActions = new List<string>();
        public List<string> unlockedFeatIds = new List<string>();
        public List<string> takenChoiceOptionIds = new List<string>();
        public List<string> pendingChoiceNodeIds = new List<string>();
        public List<string> unlockedPassiveIds = new List<string>();
        public List<string> preparedPowerIds = new List<string>();
        public List<string> classSkills = new List<string>(); //SkillType names
        public List<StatModifierEntry> backgroundSkillBonuses = new List<StatModifierEntry>();
    }

    [System.Serializable]
    public class ItemInstanceSaveData
    {
        public string instanceId;
        public string definitionId;
        public List<string> installedModIds = new List<string>();
        public string customDisplayName;
        public int charges;
        public string ownerCharacterId;
        public string grantedByClassId;

        public static ItemInstanceSaveData From(ItemInstance inst)
        {
            return new ItemInstanceSaveData
            {
                instanceId = inst.instanceId,
                definitionId = inst.definitionId,
                installedModIds = new List<string>(inst.installedModIds),
                customDisplayName = inst.customDisplayName,
                charges = inst.charges,
                ownerCharacterId = inst.ownerCharacterId,
                grantedByClassId = inst.grantedByClassId
            };
        }

        public ItemInstance ToInstance()
        {
            return new ItemInstance
            {
                instanceId = instanceId,
                definitionId = definitionId,
                installedModIds = new List<string>(installedModIds),
                customDisplayName = customDisplayName,
                charges = charges,
                ownerCharacterId = ownerCharacterId,
                grantedByClassId = grantedByClassId
            };
        }
    }

    [System.Serializable]
    public class ASIEntry
    {
        public string statType; //stored as string, parsed to enum on load
        public int value;
    }

    [System.Serializable]
    public class StatModifierEntry
    {
        public string stat;      //stored as string, parsed to enum on load
        public int flatBonus;
        public float multiplier;
        public string source;
        public string bonusType; //stored as string, parsed to enum on load
    }

    [System.Serializable]
    public class SnapshotSaveData
    {
        // Mirrors CharacterSnapshot. Any field added here must also be added to
        // CharacterSnapshot, CharacterSaveData and CharacterData.ResetToLevelOne().
        public string characterId;

        public int hpDamage;
        public int baseMaxHP;
        public int level;
        public int xp;
        public int xpToNextLevel;
        public int featPointsAvailable;
        public int skillRanksAvailable;
        public int manifesterLevel;
        public int psionicResistance;
        public int cyberneticCapacityBonus;
        public int mutationPointsSpent;
        public List<string> equippedFeatIds = new List<string>();
        public List<string> pendingChoiceNodeIds = new List<string>();
        public List<string> unlockedFeatIds = new List<string>();
        public List<string> takenChoiceOptionIds = new List<string>();
        public List<string> unlockedActions = new List<string>();
        public List<string> unlockedPassiveIds = new List<string>();
        public List<string> knownPowerIds = new List<string>();
        public List<string> preparedPowerIds = new List<string>();
        public List<string> classSkills = new List<string>(); //SkillType names

        public List<StatModifierEntry> backgroundSkillBonuses = new List<StatModifierEntry>();
        public List<SkillRankEntry> skillRanks = new List<SkillRankEntry>();
        public List<ASIEntry> abilityScoreIncreases = new List<ASIEntry>();
        public List<EquippedItemEntry> cyberneticItems = new List<EquippedItemEntry>();
    }

    //Save
    public static void Save(CampaignData campaign, List<CharacterData> squad)
    {
        if (GameFlowManager.Instance != null &&
            GameFlowManager.Instance.currentState == GameFlowManager.GameState.Combat)
        {
            Debug.LogWarning("[SaveManager] Save blocked during active combat");
            return;
        }

        SaveData data = new SaveData();
        data.saveVersion = SaveVersion.Current;

        if (SquadManager.Instance != null)
            SquadManager.Instance.CommitRuntimeToCharacters();

        //Campaign state
        data.currentActIndex = campaign.currentActIndex;

        foreach (var planet in campaign.planets)
        {
            data.planets.Add(new PlanetSaveData
            {
                planetId = planet.planetId,
                accessibility = (int)planet.accessibility,
                unlockedZones = new List<string>(planet.unlockedZones),
                completedEncounters = new List<string>(planet.completedEncounters),
                randomEncounterCounts = planet.randomEncounterCounts
                .Select(kvp => new SaveManager.RandomEncounterCountEntry
                { zoneId = kvp.Key, count = kvp.Value })
                .ToList()
            });
        }

        //Narrative flags: collect all keys from PlayerPrefs by
        //iterating the known flag keys from NarrativeManager
        foreach (var flag in NarrativeFlagManager.GetAllKeys())
        {
            data.flags.Add(new FlagSaveData
            {
                key = flag,
                value = NarrativeFlagManager.GetFlag(flag)
            });
        }

        //Journal
        data.journal = new List<string>(NarrativeJournal.GetRawEntries());

        //Squad
        foreach (var character in squad)
        {
            var charSave = new CharacterSaveData
            {
                characterId = character.characterId,
                hpDamage = character.hpDamage,
                baseMaxHP = character.baseMaxHP,
                level = character.level,
                xp = character.xp,
                xpToNextLevel = character.xpToNextLevel,
                featPointsAvailable = character.featPointsAvailable,
                skillRanksAvailable = character.skillRanksAvailable,
                powerPointsSpent = character.powerPointsSpent,
                zeroLevelUsesSpent = character.zeroLevelUsesSpent,
                manifesterLevel = character.manifesterLevel,
                knownPowerIds = new List<string>(character.knownPowerIds),
                equippedFeatIds = new List<string>(character.equippedFeatIds),
                baseActionIds = new List<string>(character.baseActionIds),
                isIncapacitated = character.isIncapacitated,
                incapacitatedWithoutRollback = character.incapacitatedWithoutRollback,
                defensiveManifesting = character.defensiveManifesting,
                psionicResistance = character.psionicResistance,
                respecCount = character.respecCount,
                cyberneticCapacityBonus = character.cyberneticCapacityBonus,
                mutationPointsSpent = character.mutationPointsSpent,
                levelUpPending = character.levelUpPending,
                unlockedActions = new List<string>(character.unlockedActions),
                unlockedFeatIds = new List<string>(character.unlockedFeatIds),
                takenChoiceOptionIds = new List<string>(character.takenChoiceOptionIds),
                pendingChoiceNodeIds = new List<string>(character.pendingChoiceNodeIds),
                unlockedPassiveIds = new List<string>(character.unlockedPassiveIds),
                preparedPowerIds = new List<string>(character.preparedPowerIds)
            };

            foreach (var kvp in character.skillRanks)
            {
                charSave.skillRanks.Add(new SkillRankEntry
                {
                    skillType = kvp.Key.ToString(),
                    ranks = kvp.Value
                });
            }

            foreach (var kvp in character.equippedItemIds)
            {
                charSave.equippedItems.Add(new EquippedItemEntry
                {
                    slot = kvp.Key.ToString(),
                    instance = ItemInstanceSaveData.From(kvp.Value)
                });
            }

            foreach (var kvp in character.abilityScoreIncreases)
            {
                charSave.abilityScoreIncreases.Add(new ASIEntry
                {
                    statType = kvp.Key.ToString(),
                    value = kvp.Value
                });
            }

            foreach (var skill in character.classSkills)
                charSave.classSkills.Add(skill.ToString());

            foreach (var mod in character.backgroundSkillBonuses)
            {
                charSave.backgroundSkillBonuses.Add(new StatModifierEntry
                {
                    stat = mod.stat.ToString(),
                    flatBonus = mod.flatBonus,
                    multiplier = mod.multiplier,
                    source = mod.source,
                    bonusType = mod.bonusType.ToString()
                });
            }

            data.squad.Add(charSave);
        }

        //Deployment snapshots: the Hub restores incapacitated characters from these,
        //so they must survive a save made away from the Hub
        if (GameFlowManager.Instance != null &&
            GameFlowManager.Instance.currentState != GameFlowManager.GameState.Hub)
        {
            foreach (var kvp in SquadManager.Instance.GetSnapshots())
                data.deploymentSnapshots.Add(ToSnapshotSaveData(kvp.Key, kvp.Value));
        }

        //Inventory
        data.creditBalance = SquadManager.Instance.creditBalance.credits;
        data.shipInventory = new List<ItemInstanceSaveData>();
        foreach (var inst in ShipInventory.GetAll())
            data.shipInventory.Add(ItemInstanceSaveData.From(inst));

        data.squadInventory = new List<ItemInstanceSaveData>();
        foreach (var inst in SquadManager.Instance.squadInventory.GetAll())
            data.squadInventory.Add(ItemInstanceSaveData.From(inst));

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"[SaveManager] Saved to {SavePath}");
    }

    //Load

    public static bool Load(CampaignData campaign, List<CharacterData> squad)
    {
        if (GameFlowManager.Instance != null &&
            GameFlowManager.Instance.currentState == GameFlowManager.GameState.Combat)
        {
            Debug.LogWarning("[SaveManager] Load blocked during active combat");
            return false;
        }

        if (!File.Exists(SavePath))
        {
            Debug.Log("[SaveManager] No save file found");
            return false;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null)
        {
            Debug.LogWarning("[SaveManager] Failed to parse save file");
            return false;
        }

        // Future-version guard: saves written by a newer build cannot be loaded
        if (data.saveVersion > SaveVersion.Current)
        {
            Debug.LogWarning($"[SaveManager] Save version {data.saveVersion} is newer than current ({SaveVersion.Current}). Cannot load.");
            return false;
        }

        // Migration: bring save up to current version
        while (data.saveVersion < SaveVersion.Current)
        {
            switch (data.saveVersion)
            {
                case 0: MigrateV0ToV1(data); break;
            }
            data.saveVersion++;
        }

        //Campaign state
        campaign.currentActIndex = data.currentActIndex;

        foreach (var planetSave in data.planets)
        {
            PlanetData planet = campaign.GetPlanet(planetSave.planetId);
            if (planet == null) continue;
            planet.accessibility = (PlanetData.PlanetAccessibility)planetSave.accessibility;
            planet.unlockedZones = new List<string>(planetSave.unlockedZones ?? new List<string>());
            planet.completedEncounters = new List<string>(planetSave.completedEncounters ?? new List<string>());
            planet.randomEncounterCounts = new Dictionary<string, int>();
            if (planetSave.randomEncounterCounts != null)
            {
                foreach (var entry in planetSave.randomEncounterCounts)
                    planet.randomEncounterCounts[entry.zoneId] = entry.count;
            }
        }

        //Flags
        foreach (var flag in data.flags)
        {
            NarrativeFlagManager.SetFlag(flag.key, flag.value);
        }

        //Journal
        if (data.journal != null) NarrativeJournal.LoadEntries(data.journal);

        //Squad
        foreach (var charSave in data.squad)
        {
            CharacterData character = squad.Find(c => c.characterId == charSave.characterId);
            if (character == null) continue;

            character.hpDamage = charSave.hpDamage;
            character.baseMaxHP = charSave.baseMaxHP;
            character.level = charSave.level;
            character.xp = charSave.xp;
            character.xpToNextLevel = charSave.xpToNextLevel;
            character.featPointsAvailable = charSave.featPointsAvailable;
            character.skillRanksAvailable = charSave.skillRanksAvailable;
            character.powerPointsSpent = charSave.powerPointsSpent;
            character.zeroLevelUsesSpent = charSave.zeroLevelUsesSpent;
            character.equippedFeatIds = new List<string>(charSave.equippedFeatIds);
            character.baseActionIds = new List<string>(charSave.baseActionIds);
            character.isIncapacitated = charSave.isIncapacitated;
            character.incapacitatedWithoutRollback = charSave.incapacitatedWithoutRollback;
            character.defensiveManifesting = charSave.defensiveManifesting;

            character.skillRanks.Clear();
            foreach (var entry in charSave.skillRanks)
            {
                if (System.Enum.TryParse(entry.skillType, out SkillType skillType))
                    character.skillRanks[skillType] = entry.ranks;
            }

            character.equippedItemIds.Clear();
            foreach (var entry in charSave.equippedItems)
            {
                if (System.Enum.TryParse(entry.slot, out EquipmentSlot slot) && entry.instance != null)
                {
                    character.equippedItemIds[slot] = entry.instance.ToInstance();
                }
            }

            character.abilityScoreIncreases.Clear();
            foreach (var entry in charSave.abilityScoreIncreases)
            {
                if (System.Enum.TryParse(entry.statType, out StatType statType))
                    character.abilityScoreIncreases[statType] = entry.value;
            }

            character.manifesterLevel = charSave.manifesterLevel;
            character.psionicResistance = charSave.psionicResistance;
            character.respecCount = charSave.respecCount;
            character.cyberneticCapacityBonus = charSave.cyberneticCapacityBonus;
            character.mutationPointsSpent = charSave.mutationPointsSpent;
            character.levelUpPending = charSave.levelUpPending;

            character.knownPowerIds = new List<string>(charSave.knownPowerIds);
            character.unlockedActions = new List<string>(charSave.unlockedActions);
            character.unlockedFeatIds = new List<string>(charSave.unlockedFeatIds);
            character.takenChoiceOptionIds = new List<string>(charSave.takenChoiceOptionIds);
            character.pendingChoiceNodeIds = new List<string>(charSave.pendingChoiceNodeIds);
            character.unlockedPassiveIds = new List<string>(charSave.unlockedPassiveIds);
            character.preparedPowerIds = new List<string>(charSave.preparedPowerIds);

            character.classSkills.Clear();
            foreach (var name in charSave.classSkills)
            {
                if (System.Enum.TryParse(name, out SkillType skill))
                    character.classSkills.Add(skill);
            }

            character.backgroundSkillBonuses.Clear();
            foreach (var entry in charSave.backgroundSkillBonuses)
            {
                if (System.Enum.TryParse(entry.stat, out StatType stat) &&
                    System.Enum.TryParse(entry.bonusType, out StatModifier.BonusType bonusType))
                {
                    character.backgroundSkillBonuses.Add(
                        new StatModifier(stat, entry.flatBonus, entry.multiplier, entry.source, bonusType));
                }
            }
        }

        //Inventory
        ShipInventory.Clear();
        foreach (var saved in data.shipInventory)
            ShipInventory.AddItem(saved.ToInstance());

        SquadManager.Instance.squadInventory = new Inventory(0f);
        foreach (var saved in data.squadInventory)
            SquadManager.Instance.squadInventory.AddItem(saved.ToInstance());
        SquadManager.Instance.creditBalance = new CreditBalance();
        SquadManager.Instance.creditBalance.Add(data.creditBalance);

        SquadManager.Instance.CreateRuntimeSquad();

        //Overwrite the snapshots CreateRuntimeSquad just recaptured with the saved deployment snapshots
        foreach (var snapSave in data.deploymentSnapshots)
            SquadManager.Instance.SetSnapshot(snapSave.characterId, FromSnapshotSaveData(snapSave));

        Debug.Log("[SaveManager] Loaded successfully");
        return true;
    }

    //Delete
    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("[SaveManager] Save file deleted");
        }
    }

    public static bool SaveExists()
    {
        return File.Exists(SavePath);
    }

    private static SnapshotSaveData ToSnapshotSaveData(string characterId, CharacterSnapshot snap)
    {
        var save = new SnapshotSaveData
        {
            characterId = characterId,
            hpDamage = snap.hpDamage,
            baseMaxHP = snap.baseMaxHP,
            level = snap.level,
            xp = snap.xp,
            xpToNextLevel = snap.xpToNextLevel,
            featPointsAvailable = snap.featPointsAvailable,
            skillRanksAvailable = snap.skillRanksAvailable,
            manifesterLevel = snap.manifesterLevel,
            psionicResistance = snap.psionicResistance,
            cyberneticCapacityBonus = snap.cyberneticCapacityBonus,
            mutationPointsSpent = snap.mutationPointsSpent,
            equippedFeatIds = new List<string>(snap.equippedFeatIds),
            pendingChoiceNodeIds = new List<string>(snap.pendingChoiceNodeIds),
            unlockedFeatIds = new List<string>(snap.unlockedFeatIds),
            takenChoiceOptionIds = new List<string>(snap.takenChoiceOptionIds),
            unlockedActions = new List<string>(snap.unlockedActions),
            unlockedPassiveIds = new List<string>(snap.unlockedPassiveIds),
            knownPowerIds = new List<string>(snap.knownPowerIds),
            preparedPowerIds = new List<string>(snap.preparedPowerIds)
        };

        foreach (var skill in snap.classSkills)
            save.classSkills.Add(skill.ToString());

        foreach (var mod in snap.backgroundSkillBonuses)
        {
            save.backgroundSkillBonuses.Add(new StatModifierEntry
            {
                stat = mod.stat.ToString(),
                flatBonus = mod.flatBonus,
                multiplier = mod.multiplier,
                source = mod.source,
                bonusType = mod.bonusType.ToString()
            });
        }

        foreach (var kvp in snap.skillRanks)
            save.skillRanks.Add(new SkillRankEntry { skillType = kvp.Key.ToString(), ranks = kvp.Value });

        foreach (var kvp in snap.abilityScoreIncreases)
            save.abilityScoreIncreases.Add(new ASIEntry { statType = kvp.Key.ToString(), value = kvp.Value });

        foreach (var kvp in snap.cyberneticItemIds)
        {
            save.cyberneticItems.Add(new EquippedItemEntry
            {
                slot = kvp.Key.ToString(),
                instance = new ItemInstanceSaveData
                {
                    instanceId = kvp.Value.instanceId,
                    definitionId = kvp.Value.definitionId,
                    installedModIds = new List<string>(kvp.Value.installedModIds),
                    customDisplayName = kvp.Value.customDisplayName
                }
            });
        }

        return save;
    }

    private static CharacterSnapshot FromSnapshotSaveData(SnapshotSaveData save)
    {
        var snap = new CharacterSnapshot
        {
            hpDamage = save.hpDamage,
            baseMaxHP = save.baseMaxHP,
            level = save.level,
            xp = save.xp,
            xpToNextLevel = save.xpToNextLevel,
            featPointsAvailable = save.featPointsAvailable,
            skillRanksAvailable = save.skillRanksAvailable,
            manifesterLevel = save.manifesterLevel,
            psionicResistance = save.psionicResistance,
            cyberneticCapacityBonus = save.cyberneticCapacityBonus,
            mutationPointsSpent = save.mutationPointsSpent,
            equippedFeatIds = new List<string>(save.equippedFeatIds),
            pendingChoiceNodeIds = new List<string>(save.pendingChoiceNodeIds),
            unlockedFeatIds = new List<string>(save.unlockedFeatIds),
            takenChoiceOptionIds = new List<string>(save.takenChoiceOptionIds),
            unlockedActions = new List<string>(save.unlockedActions),
            unlockedPassiveIds = new List<string>(save.unlockedPassiveIds),
            knownPowerIds = new List<string>(save.knownPowerIds),
            preparedPowerIds = new List<string>(save.preparedPowerIds),
            classSkills = new List<SkillType>(),
            backgroundSkillBonuses = new List<StatModifier>(),
            skillRanks = new Dictionary<SkillType, int>(),
            abilityScoreIncreases = new Dictionary<StatType, int>(),
            cyberneticItemIds = new Dictionary<EquipmentSlot, ItemInstance>()
        };

        foreach (var name in save.classSkills)
        {
            if (System.Enum.TryParse(name, out SkillType skill))
                snap.classSkills.Add(skill);
        }

        foreach (var entry in save.backgroundSkillBonuses)
        {
            if (System.Enum.TryParse(entry.stat, out StatType stat) &&
                System.Enum.TryParse(entry.bonusType, out StatModifier.BonusType bonusType))
            {
                snap.backgroundSkillBonuses.Add(
                    new StatModifier(stat, entry.flatBonus, entry.multiplier, entry.source, bonusType));
            }
        }

        foreach (var entry in save.skillRanks)
        {
            if (System.Enum.TryParse(entry.skillType, out SkillType skillType))
                snap.skillRanks[skillType] = entry.ranks;
        }

        foreach (var entry in save.abilityScoreIncreases)
        {
            if (System.Enum.TryParse(entry.statType, out StatType statType))
                snap.abilityScoreIncreases[statType] = entry.value;
        }

        foreach (var entry in save.cyberneticItems)
        {
            if (System.Enum.TryParse(entry.slot, out EquipmentSlot slot) && entry.instance != null)
            {
                snap.cyberneticItemIds[slot] = new ItemInstance
                {
                    instanceId = entry.instance.instanceId,
                    definitionId = entry.instance.definitionId,
                    installedModIds = new List<string>(entry.instance.installedModIds),
                    customDisplayName = entry.instance.customDisplayName
                };
            }
        }

        return snap;
    }

    private static void MigrateV0ToV1(SaveData data)
    {
        // abilityScoreIncreases added in V1.
        // JsonUtility deserializes the missing field as an empty list,
        // which correctly maps to an empty dictionary on load.
        // No structural action required.
        Debug.Log("[SaveManager] Migrated save from V0 to V1");
    }
}
