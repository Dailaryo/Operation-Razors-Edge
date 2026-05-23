using UnityEngine;
using System.Collections.Generic;

public class ClassDefinition
{
    //Identity
    public string classId;
    public string displayName;
    public StatType statBias;

    //Combat Progression
    public enum BABProgression { Full, ThreeQuarters, Half}
    public BABProgression babProgression;

    //HP growth
    public int hpPerLevel;

    //Skill growth
    public int skillRanksPerLevel;

    //Saving Throw Progression
    public enum SaveProgression { Good, Poor}
    public SaveProgression resolveSave;
    public SaveProgression responseSave;
    public SaveProgression focusSave;

    //Feat Cadence
    //List of levels that award a feat point
    public List<int> featCadence;

    //Class Skills
    public List<SkillType> classSkills;

    //Weapon proficiencies
    public List<WeaponTag> weaponProficiencies;

    //Level Table
    public Dictionary<int, List<ClassLevelEntry>> levelTable;
    public ResourceDefinition resource;

    //Psionic dependencies
    public bool isPsionicClass = false;
    public StatType psionicKeyAbility = StatType.Insight;
    public Dictionary<int, int> powerPointProgression;
    public ManifestationType manifestationType;

    public class ClassLevelEntry
    {
        public List<string> actionUnlockIds;
        public List<string> passiveIds;
        public string choiceNodeId;
        public string prerequisiteOptionId; // null = always applies

        public ClassLevelEntry(
            List<string> actionUnlockIds = null,
            List<string> passiveIds = null,
            string choicNodeId = null,
            string prerequisiteOptionId = null)
        {
            this.actionUnlockIds = actionUnlockIds ?? new List<string>();
            this.passiveIds = passiveIds ?? new List<string>();
            this.choiceNodeId = choicNodeId;
            this.prerequisiteOptionId = prerequisiteOptionId;
        }
    }

    //Constructor
    public ClassDefinition(
        string classId,
        string diplayName,
        StatType statBias,
        BABProgression babProgression,
        int hpPerLevel,
        int skillRanksPerLevel,
        SaveProgression resolveSave,
        SaveProgression responseSave,
        SaveProgression focusSave,
        List<int> featCadence,
        List<SkillType> classSkills = null,
        List<WeaponTag> weaponProficiencies = null,
        Dictionary<int, List<ClassLevelEntry>> levelTable = null)
    {
        this.classId = classId;
        this.displayName = diplayName;
        this.statBias = statBias;
        this.babProgression = babProgression;
        this.hpPerLevel = hpPerLevel;
        this.skillRanksPerLevel = skillRanksPerLevel;
        this.resolveSave = resolveSave;
        this.responseSave = responseSave;
        this.focusSave  = focusSave;
        this.featCadence = featCadence ?? new List<int>();
        this.classSkills = classSkills ?? new List<SkillType>();
        this.weaponProficiencies = weaponProficiencies ?? new List<WeaponTag>();
        this.levelTable = levelTable ?? new Dictionary<int, List<ClassLevelEntry>>();
    }

    //BAB Calculation
    public int GetBAB(int level)
    {
        switch (babProgression)
        {
            case BABProgression.Full: return level;
            case BABProgression.ThreeQuarters: return Mathf.FloorToInt(level * 0.75f);
            case BABProgression.Half: return Mathf.FloorToInt(level * 0.5f);
            default: return 0;
        }
    }

    //LevelUp Application
    //Called by CharacterData.LevelUp() - mutates CharacterData only
    public void ApplyLevelUp(CharacterData data, int newLevel)
    {
        //HP growth
        data.baseMaxHP += hpPerLevel;

        //Feat point cadence
        if (featCadence.Contains(newLevel))
        {
            data.featPointsAvailable++;
            if (!data.isEnemy)
                CombatLog.Add($"{data.name} earned a feat point! ({data.featPointsAvailable} available");
            AudioEventBus.Fire(AudioEventIds.GameFlow.FeatEarned);
        }

        //Action unlocks - record IDs on CharacterData for ClassActionSource to read
        if (levelTable.TryGetValue(newLevel, out var entries))
        {
            foreach (var entry in entries)
            {
                if (entry.prerequisiteOptionId == null ||
                    data.takenChoiceOptionIds.Contains(entry.prerequisiteOptionId))
                {
                    foreach (var id in entry.actionUnlockIds)
                        if (!data.unlockedActions.Contains(id))
                            data.unlockedActions.Add(id);

                    foreach (var id in entry.passiveIds)
                        if (!data.unlockedPassiveIds.Contains(id))
                            data.unlockedPassiveIds.Add(id);
                }

                if (!data.isEnemy && entry.choiceNodeId != null)
                    if (!data.pendingChoiceNodeIds.Contains(entry.choiceNodeId))
                        data.pendingChoiceNodeIds.Add(entry.choiceNodeId);
            }
        }

        // Psionic progression
        if (isPsionicClass)
        {
            data.manifesterLevel++;
            CombatLog.Add($"{data.name}: Manifester level increased to {data.manifesterLevel}");
        }

        CombatLog.Add($"{data.name} leveled up to {newLevel} [{displayName}]");
    }

    public int GetSaveBonus(SaveProgression progression, int level)
    {
        switch (progression)
        {
            case SaveProgression.Good: return 2 + Mathf.FloorToInt(level / 2f);
            case SaveProgression.Poor: return Mathf.FloorToInt(level / 3f);
            default: return 0;
        }
    }

    public class ResourceDefinition
    {
        public string resourceId;
        public string displayName;

        public enum RegenMode
        {
            None, //No passive regen
            PerTurn, //regens flat amount each turn
            PerAction, //regens on specific action types
            FullOnCombatStart, //starts full, no regen during combat
            Permanent //MP is spent permanently at level-up; never refreshes
        }

        public RegenMode regenMode;
        public int baseMax;
        public int regenAmount; //meaningful for PerTurn only, 0 otherwise

        public ResourceDefinition(
        string resourceId,
        string displayName,
        RegenMode regenMode,
        int baseMax,
        int regenAmount = 0)
        {
            this.resourceId = resourceId;
            this.displayName = displayName;
            this.regenMode = regenMode;
            this.baseMax = baseMax;
            this.regenAmount = regenAmount;
        }

        // Returns the maximum resource pool for a given character level.
        // Override in subclasses or via virtual dispatch if needed.
        // Default: returns baseMax for all non-scaling resources.
        public virtual int GetMaxAtLevel(int level)
        {
            return baseMax;
        }
    }

    public bool IsProficientWith(WeaponTag tag)
    {
        return weaponProficiencies.Contains(tag);
    }
}
