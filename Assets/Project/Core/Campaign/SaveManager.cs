using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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
        public string characterId;
        public int currentHP;
        public int baseMaxHP;
        public int level;
        public int xp;
        public int xpToNextLevel;
        public int featPointsAvailable;
        public int skillRanksAvailable;
        public List<string> equippedFeatIds = new List<string>();
        public List<string> baseActionIds = new List<string>();
        public List<SkillRankEntry> skillRanks = new List<SkillRankEntry>();
        public List<EquippedItemEntry> equippedItems = new List<EquippedItemEntry>();
        public List<ASIEntry> abilityScoreIncreases = new List<ASIEntry>();
        public bool isIncapacitated;
    }

    [System.Serializable]
    public class ItemInstanceSaveData
    {
        public string instanceId;
        public string definitionId;
        public List<string> installedModIds = new List<string>();
        public string customDisplayName;
    }

    [System.Serializable]
    public class ASIEntry
    {
        public string statType; //stored as string, parsed to enum on load
        public int value;
    }

    //Save
    public static void Save(CampaignData campaign, List<CharacterData> squad)
    {
        SaveData data = new SaveData();
        data.saveVersion = SaveVersion.Current;

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
                currentHP = character.currentHP,
                baseMaxHP = character.baseMaxHP,
                level = character.level,
                xp = character.xp,
                xpToNextLevel = character.xpToNextLevel,
                featPointsAvailable = character.featPointsAvailable,
                skillRanksAvailable = character.skillRanksAvailable,
                equippedFeatIds = new List<string>(character.equippedFeatIds),
                baseActionIds = new List<string>(character.baseActionIds),
                isIncapacitated = character.isIncapacitated
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
                    instance = new ItemInstanceSaveData
                    {
                        instanceId = kvp.Value.instanceId,
                        definitionId = kvp.Value.definitionId,
                        installedModIds = new List<string>(kvp.Value.installedModIds),
                        customDisplayName = kvp.Value.customDisplayName
                    }
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

            data.squad.Add(charSave);
        }

        //Inventory
        data.creditBalance = SquadManager.Instance.creditBalance.credits;
        data.shipInventory = new List<ItemInstanceSaveData>();
        foreach (var inst in ShipInventory.GetAll())
            data.shipInventory.Add(new ItemInstanceSaveData
            {
                instanceId = inst.instanceId,
                definitionId = inst.definitionId,
                installedModIds = new List<string>(inst.installedModIds),
                customDisplayName = inst.customDisplayName
            });

        data.squadInventory = new List<ItemInstanceSaveData>();
        foreach (var inst in SquadManager.Instance.squadInventory.GetAll())
            data.squadInventory.Add(new ItemInstanceSaveData
            {
                instanceId = inst.instanceId,
                definitionId = inst.definitionId,
                installedModIds = new List<string>(inst.installedModIds),
                customDisplayName = inst.customDisplayName
            });

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"[SaveManager] Saved to {SavePath}");
    }

    //Load

    public static bool Load(CampaignData campaign, List<CharacterData> squad)
    {
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

            character.currentHP = charSave.currentHP;
            character.baseMaxHP = charSave.baseMaxHP;
            character.level = charSave.level;
            character.xp = charSave.xp;
            character.xpToNextLevel = charSave.xpToNextLevel;
            character.featPointsAvailable = charSave.featPointsAvailable;
            character.skillRanksAvailable = charSave.skillRanksAvailable;
            character.equippedFeatIds = new List<string>(charSave.equippedFeatIds);
            character.baseActionIds = new List<string>(charSave.baseActionIds);
            character.isIncapacitated = charSave.isIncapacitated;

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
                    character.equippedItemIds[slot] = new ItemInstance
                    {
                        instanceId = entry.instance.instanceId,
                        definitionId = entry.instance.definitionId,
                        installedModIds = new List<string>(entry.instance.installedModIds),
                        customDisplayName = entry.instance.customDisplayName
                    };
                }
            }

            character.abilityScoreIncreases.Clear();
            foreach (var entry in charSave.abilityScoreIncreases)
            {
                if (System.Enum.TryParse(entry.statType, out StatType statType))
                    character.abilityScoreIncreases[statType] = entry.value;
            }
        }

        //Inventory
        ShipInventory.Clear();
        foreach (var saved in data.shipInventory)
            ShipInventory.AddItem(new ItemInstance
            {
                instanceId = saved.instanceId,
                definitionId = saved.definitionId,
                installedModIds = new List<string>(saved.installedModIds),
                customDisplayName = saved.customDisplayName
            });

        SquadManager.Instance.squadInventory = new Inventory(0f);
        foreach (var saved in data.squadInventory)
            SquadManager.Instance.squadInventory.AddItem(new ItemInstance
            {
                instanceId = saved.instanceId,
                definitionId = saved.definitionId,
                installedModIds = new List<string>(saved.installedModIds),
                customDisplayName = saved.customDisplayName
            });
        SquadManager.Instance.creditBalance = new CreditBalance();
        SquadManager.Instance.creditBalance.Add(data.creditBalance);

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

    private static void MigrateV0ToV1(SaveData data)
    {
        // abilityScoreIncreases added in V1.
        // JsonUtility deserializes the missing field as an empty list,
        // which correctly maps to an empty dictionary on load.
        // No structural action required.
        Debug.Log("[SaveManager] Migrated save from V0 to V1");
    }
}
