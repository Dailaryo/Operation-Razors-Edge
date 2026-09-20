using UnityEngine;
using System.Collections.Generic;

public class CharacterData
{
    public string characterId;
    public string name;
    public string classId;
    public string backgroundId;
    public string speciesId;
    public string lootTableId;

    // Background choice selections — set by pregen builders / character creation UI
    public string backgroundFeatChoice = "";
    public List<SkillType> backgroundSkillChoices = new List<SkillType>();

    public int baseMaxHP;
    public int baseAttack;
    public int baseInitiative;
    public int moveSpeed = 6;
    public int currentHP;
    public int level = 1;
    public int xp = 0;
    public int xpToNextLevel = 1000;
    public int featPointsAvailable = 0;
    public int skillRanksAvailable = 0;
    public int manifesterLevel = 0;
    public int psionicResistance = 0;
    public int respecCount = 0;
    public int cyberneticCapacityBonus = 0;
    public int deathRound = -1; // Set when unit dies; -1 means alive. Used by miracle actions.

    public int power;
    public int agility;
    public int endurance;
    public int cognition;
    public int insight;
    public int presence;

    public bool levelUpPending = false;
    public bool isIncapacitated = false;
    public bool isEnemy = false;
    public bool isFlatFooted = false;
    public bool IsMindless => cognition < 3;

    public List<string> unlockedActions = new List<string>();
    public List<string> baseActionIds = new List<string>();
    public List<string> equippedFeatIds = new List<string>();
    public List<string> unlockedFeatIds = new List<string>();
    public List<string> takenChoiceOptionIds = new List<string>();
    public List<string> pendingChoiceNodeIds = new List<string>(); public List<string> knownPowerIds = new List<string>();
    public List<string> preparedPowerIds = new List<string>();
    public List<string> unlockedPassiveIds = new List<string>();

    // Pregen intended progression — used by LevelUp() to auto-select choices
    public List<string> intendedChoiceOptions = new List<string>();
    public List<string> intendedFeatProgression = new List<string>();

    public List<SkillType> classSkills = new List<SkillType>();

    public List<StatModifier> backgroundSkillBonuses = new List<StatModifier>();

    public Dictionary<SkillType, int> skillRanks = new Dictionary<SkillType, int>();
    public Dictionary<EquipmentSlot, ItemInstance> equippedItemIds = new Dictionary<EquipmentSlot, ItemInstance>();
    public Dictionary<StatType, int> abilityScoreIncreases = new Dictionary<StatType, int>();

    public ReachProfile reachProfile = ReachProfile.Default;

    public Unit.SizeCategory size = Unit.SizeCategory.Medium;

    public event System.Action OnCharacterChanged;

    public CharacterData(string id, string name, int hp, int attack, int initiative)
    {
        this.characterId = id;
        this.name = name;
        this.baseMaxHP = hp;
        this.baseAttack = attack;
        this.baseInitiative = initiative;
        this.currentHP = hp; //Start full
        baseActionIds = new List<string>(ActionIds.Universal);
    }

    public void GainXP(int amount)
    {
        xp += amount;

        while (xp >= xpToNextLevel)
        {
            xp -= xpToNextLevel;
            LevelUp();
            xpToNextLevel = CharacterProgression.GetXPToNextLevel(level);
        }
    }

    public void LevelUp()
    {
        level++;

        if (CharacterProgression.IsFeatLevel(level))
        {
            featPointsAvailable++;
            CombatLog.Add($"{name} earned a feat point (character level)! ({featPointsAvailable}available)");
        }

        if (CharacterProgression.IsASILevel(level))
        {
            CombatLog.Add($"{name} reached an Ability Score increase level ({level})!");
        }

        var classDef = ClassDatabase.Get(classId);
        if (classDef != null)
        {
            classDef.ApplyLevelUp(this, level);
            AudioEventBus.Fire(AudioEventIds.GameFlow.LevelUp);
            skillRanksAvailable += GetSkillRanksPerLevel(classDef);
        }
        else
        {
            //Fallback if no class assigned - keeps hardcoded behavior during transition
            baseMaxHP += 10;
            CombatLog.Add($"{name} leveled up to {level} [no class]");
        }

        var speciesDef = SpeciesDatabase.Get(speciesId);
        if (speciesDef != null)
            speciesDef.ApplyLevelUp(this);

        //Ability unlock hook
        //Future: ClassDefinition.GetUnlocksAtLevel(level) populates this
        OnLevelUp?.Invoke(this, level);

        if (!isEnemy)
            levelUpPending = true;

        CombatLog.Add($"{name} leveled up to {level}");
    }

    public bool HasFeat(string featId)
    {
        return equippedFeatIds.Contains(featId);
    }

    public System.Action<CharacterData, int> OnLevelUp;

    public int GetSkillRanksPerLevel(ClassDefinition classDef)
    {
        int cogMod = (cognition - 10) / 2;
        return Mathf.Max(1, classDef.skillRanksPerLevel + cogMod);
    }

    public int GetCyberneticCapacity()
    {
        int enduranceMod = (endurance - 10) / 2;
        return Mathf.Max(1, 2 + enduranceMod + cyberneticCapacityBonus);
    }

    public bool HasItemEquipped(EquipmentSlot slot)
    {
        return equippedItemIds.ContainsKey(slot);
    }

    public ItemInstance GetEquippedItem(EquipmentSlot slot)
    {
        equippedItemIds.TryGetValue(slot, out ItemInstance instance);
        return instance;
    }

    public int GetEquippedCyberneticCount()
    {
        int count = 0;

        foreach (EquipmentSlot slot in System.Enum.GetValues(typeof(EquipmentSlot)))
        {
            if (IsCyberneticSlot(slot) && equippedItemIds.ContainsKey(slot))
                count++;
        }

        return count;
    }

    public float GetMaxCarryWeight()
    {
        // Approximates Table 4-11 (d20 Modern Core, p.121) medium load capacity
        // Anchored at STR 10 = 66 lb, doubling every 5 STR points
        // Max error vs. table: 13 lb at extreme low STR, <3 lb for STR 5-20
        int str = Mathf.Max(1, power);
        return Mathf.Round(66f * Mathf.Pow(2f, (str - 10f) / 5f));
    }

    public static bool IsCyberneticSlot(EquipmentSlot slot)
    {
        return slot == EquipmentSlot.Cybernetic1
            || slot == EquipmentSlot.Cybernetic2
            || slot == EquipmentSlot.Cybernetic3
            || slot == EquipmentSlot.Cybernetic4
            || slot == EquipmentSlot.Cybernetic5;
    }

    public bool TryEquipItem(EquipmentSlot slot, ItemInstance instance, out string reason)
    {
        var item = ItemDatabase.Get(instance.definitionId);
        if (item == null)
        {
            reason = $"{instance.definitionId} not found in ItemDatabase";
            return false;
        }

        if (IsCyberneticSlot(slot))
        {
            if (item.itemType != ItemType.Cybernetic)
            {
                reason = $"'{item.displayName}' is not a cybernetic item and cannot be equipped in a cybernetic slot";
                return false;
            }

            if (GameFlowManager.Instance == null ||
                GameFlowManager.Instance.currentState != GameFlowManager.GameState.Hub)
            {
                reason = "Cybernetics can only be modified at the Hub";
                return false;
            }
        }

        equippedItemIds[slot] = instance;
        reason = null;
        return true;
    }

    public bool TryUnequipItem(EquipmentSlot slot, out string reason)
    {
        if (!equippedItemIds.ContainsKey(slot))
        {
            reason = $"No item equipped in slot {slot}";
            return false;
        }

        if (IsCyberneticSlot(slot))
        {
            if (GameFlowManager.Instance == null ||
                GameFlowManager.Instance.currentState != GameFlowManager.GameState.Hub)
            {
                reason = "Cybernetics can only be modified at the Hub";
                return false;
            }
        }

        equippedItemIds.Remove(slot);
        reason = null;
        return true;
    }

    public int GetAttributeScore(StatType attribute)
    {
        int base_ = 0;
        switch (attribute)
        {
            case StatType.Power: base_ = power; break;
            case StatType.Agility: base_ = agility; break;
            case StatType.Endurance: base_ = endurance; break;
            case StatType.Cognition: base_ = cognition; break;
            case StatType.Insight: base_ = insight; break;
            case StatType.Presence: base_ = presence; break;
        }
        abilityScoreIncreases.TryGetValue(attribute, out int asi);
        return base_ + asi;
    }

    public void ResetToLevelOne()
    {
        level = 0;
        baseMaxHP = 0;
        currentHP = 0;

        featPointsAvailable = 0;
        skillRanksAvailable = 0;
        manifesterLevel = 0;
        psionicResistance = 0;
        cyberneticCapacityBonus = 0;

        xpToNextLevel = CharacterProgression.GetXPToNextLevel(1);

        equippedFeatIds.Clear();
        unlockedFeatIds.Clear();
        skillRanks.Clear();
        takenChoiceOptionIds.Clear();
        pendingChoiceNodeIds.Clear();
        abilityScoreIncreases.Clear();
        unlockedActions.Clear();
        unlockedPassiveIds.Clear();
        knownPowerIds.Clear();
        preparedPowerIds.Clear();
        classSkills.Clear();
        backgroundSkillBonuses.Clear();

        intendedChoiceOptions.Clear();
        intendedFeatProgression.Clear();

        levelUpPending = false;
    }

    public void NotifyChanged()
    {
        OnCharacterChanged?.Invoke();
    }
}
