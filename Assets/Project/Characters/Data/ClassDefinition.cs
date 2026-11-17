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

    //Class bonus feats: base list (every class feat level) plus additions unlocked by a taken path option
    public List<string> bonusFeatIds = new List<string>();
    public Dictionary<string, List<string>> pathBonusFeatIds = new Dictionary<string, List<string>>();

    //True if featId may be taken with a class-cadence feat point by this character.
    //Enforced bt the full level-up flow (not yet build); pregen routes are author-checked
    public bool IsBonusFeatAllowed(CharacterData data, string featId)
    {
        if (bonusFeatIds.Contains(featId)) return true;

        foreach (var kvp in pathBonusFeatIds)
            if (data.takenChoiceOptionIds.Contains(kvp.Key) && kvp.Value.Contains(featId))
                return true;

        return false;
    }

    //Class Skills
    public List<SkillType> classSkills;

    //Weapon proficiencies
    public List<WeaponTag> weaponProficiencies;

    //Feats every class grants (current and future classes): equipped at build, no feat point
    public static readonly List<string> UniversalClassFeatIds = new List<string>
    {
        FeatIds.SimpleWeaponsProficiency,
    };

    //Level Table
    public Dictionary<int, List<ClassLevelEntry>> levelTable;
    public ResourceDefinition resource;

    //Psionic dependencies
    public bool isPsionicClass = false;
    public StatType psionicKeyAbility = StatType.Insight;
    public Dictionary<int, int> powerPointProgression;
    public ManifestationType manifestationType;
    //Powers Discovered: class level -> powers known per power level (index 0 = 0-level ... 9 = 9th)
    public Dictionary<int, int[]> powersDiscovered;
    //Powers this class is able to learn
    public List<string> powerListIds = new List<string>();

    public class ClassLevelEntry
    {
        public List<string> actionUnlockIds;
        public List<string> passiveIds;
        public string choiceNodeId;
        public string prerequisiteOptionId; // null = always applies
        public List<int> bonusPowerSlots; //each entry = one extra power known of that power level
        public List<string> grantedItemIds; //class feature items created at this level (Power Crystal)

        public ClassLevelEntry(
            List<string> actionUnlockIds = null,
            List<string> passiveIds = null,
            string choicNodeId = null,
            string prerequisiteOptionId = null,
            List<int> bonusPowerSlots = null,
            List<string> grantedItemIds = null)
        {
            this.actionUnlockIds = actionUnlockIds ?? new List<string>();
            this.passiveIds = passiveIds ?? new List<string>();
            this.choiceNodeId = choicNodeId;
            this.prerequisiteOptionId = prerequisiteOptionId;
            this.bonusPowerSlots = bonusPowerSlots ?? new List<int>();
            this.grantedItemIds = grantedItemIds ?? new List<string>();
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
        return CalculateBAB(babProgression, level);
    }

    //Shared BAB curve: also used by EnemyFactory progression profiles (no ClassDefinition instance)
    public static int CalculateBAB(BABProgression progression, int level)
    {
        switch (progression)
        {
            case BABProgression.Full: return level;
            case BABProgression.ThreeQuarters: return Mathf.FloorToInt(level * 0.75f);
            case BABProgression.Half: return Mathf.FloorToInt(level * 0.5f);
            default: return 0;
        }
    }

    //Seeds the character's class skill list from this class.
    //Psionic classes also gain Autohypnosis and Psicraft as bonus class skills.
    //Must run BEFORE BackgroundDefinition.Apply so background overlap is detected correctly.
    public void ApplyClassSkills(CharacterData data)
    {
        foreach (var skill in classSkills)
            if (!data.classSkills.Contains(skill))
                data.classSkills.Add(skill);

        if (!isPsionicClass) return;

        if (!data.classSkills.Contains(SkillType.Autohypnosis))
            data.classSkills.Add(SkillType.Autohypnosis);
        if (!data.classSkills.Contains(SkillType.Psicraft))
            data.classSkills.Add(SkillType.Psicraft);
    }

    //Equips the feats every class grants (UniversalClassFeatIds). Runs at build/respec alongside ApplyClassSkills
    public void ApplyClassFeats(CharacterData data)
    {
        foreach (var featId in UniversalClassFeatIds)
            if (!data.equippedFeatIds.Contains(featId))
                data.equippedFeatIds.Add(featId);
    }

    //LevelUp Application
    //Called by CharacterData.LevelUp() - mutates CharacterData only
    public void ApplyLevelUp(CharacterData data, int newLevel)
    {
        //HP growth: class hit points + Endurance modifier, minimum 1 per level (d20)
        data.baseMaxHP += GetHPPerLevel(data);

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
                //Path-gated entries (actions, passives AND choice nodes) apply only on the taken path
                if (entry.prerequisiteOptionId != null &&
                    !data.takenChoiceOptionIds.Contains(entry.prerequisiteOptionId))
                    continue;

                foreach (var id in entry.actionUnlockIds)
                    if (!data.unlockedActions.Contains(id))
                        data.unlockedActions.Add(id);

                foreach (var id in entry.passiveIds)
                    if (!data.unlockedPassiveIds.Contains(id))
                        data.unlockedPassiveIds.Add(id);

                foreach (var itemId in entry.grantedItemIds)
                    ClassItemGrants.Grant(data, classId, itemId);

                if (!data.isEnemy && entry.choiceNodeId != null)
                {
                    //One pending entry per pick: the node stays pending until every pick is made
                    var node = ChoiceNodeDatabase.Get(entry.choiceNodeId);
                    int picks = node != null ? node.GetPickCount(data) : 1;
                    for (int i = 0; i < picks; i++)
                        data.pendingChoiceNodeIds.Add(entry.choiceNodeId);
                }
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
        return CalculateSaveBonus(progression, level);
    }

    //Shared save curve: also used by EnemyFactory progression profiles (no ClassDefinition instance)
    public static int CalculateSaveBonus(SaveProgression progression, int level)
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

    //HP gained per level: class hitpoints + Endurance modifier minimum 1
    public int GetHPPerLevel(CharacterData data)
    {
        int endMod = CharacterProgression.GetAbilityModifier(data.GetAttributeScore(StatType.Endurance));
        return Mathf.Max(1, hpPerLevel + endMod);
    }
}
