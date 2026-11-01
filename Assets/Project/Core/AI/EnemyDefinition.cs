using UnityEngine;
using System.Collections.Generic;

public class EnemyDefinition
{
    //Identity
    public string enemyId;
    public string displayName;
    public string description;
    public bool isClassed;

    //Scaling anchor
    public int baseLevel = 1;
    public int xpValue;

    //Classless stat block (used directly for classless pipeline
    //used as species substitute for classed pipleine when isStatBlockSpeciesSubstitute is true
    public int baseMaxHP;
    public int baseInitiative;
    public int power = 10;
    public int agility = 10;
    public int endurance = 10;
    public int cognition = 8;
    public int insight = 8;
    public int presence = 8;

    //Classed pipeline fields
    public string speciesId;
    public string classId;
    public bool isStatBlockSpeciesSubstitute;

    //Actions
    public List<string> additionalActionIds = new List<string>();
    public Dictionary<int, List<string>> levelActionUnlocks = new Dictionary<int, List<string>>();

    //Feats
    public List<string> featIds = new List<string>();

    //Progression profile(D2-A): level-appropriate BAB and saves evaluated at spawnLevel,
    //using the same curves as ClassDefinition. null = no progression (attribute modifier only, as before).
    public ClassDefinition.BABProgression? babProgression = null;
    public ClassDefinition.SaveProgression? resolveProgression = null;   //StatType.Fortitude
    public ClassDefinition.SaveProgression? responseProgression = null;  //StatType.Evasion
    public ClassDefinition.SaveProgression? focusProgression = null;     //StatType.Willpower

    //Flat offsets (D1 / D2-C): applied as one SimpleEffectSource of StatBonusEffects, on top of the profile.
    //Use for deliberate skew, e.g. +4 Willpower for a "hard target".
    public List<StatModifier> statBonuses = new List<StatModifier>();


    //Defensive settings (R2): flat damage reduction (highest DR source applies) and innate power resistance
    public int damageReduction = 0;
    public int psionicResistance = 0;

    //Equipment and loot
    public Dictionary<EquipmentSlot, string> equippedItemIds = new Dictionary<EquipmentSlot, string>();
    public string lootTableId;

    //Unit properties
    public int moveSpeed = 6;
    public Unit.SizeCategory size = Unit.SizeCategory.Medium;

    //Training flags
    public bool ignoreDifficultyScaling = false;  //D3: skip EnemyScaler pass 2 and the classed post-build multiplier
    public bool isInvulnerableTarget = false;      //D6-B: damage logs normally, HP resets in the end phase, never dies


    //Scripted behaviour (D4/D5): one step per round, wrapping. Empty = utility planner as today.
    //A patterned unit only moves when a step's actionId is ActionIds.Move.
    public List<PatternStep> attackPattern = new List<PatternStep>();

    //Awareness checks: set by hand for stat-block enemies. Enemies with a species use the species' category
    public KnowledgeCategory knowledgeCategory = KnowledgeCategory.Human;

    public EnemyDefinition(string enemy, string name, bool classed = false, int level = 1)
    {
        enemyId = enemy;
        displayName = name;
        isClassed = classed;
        baseLevel = level;
    }
}
