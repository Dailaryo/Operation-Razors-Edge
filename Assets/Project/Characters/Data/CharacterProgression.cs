using System.Collections.Generic;
using UnityEngine;

public static class CharacterProgression
{
    public static HashSet<int> FeatCadence = new HashSet<int> { 1, 4, 7, 10, 13, 16, 19};

    public static HashSet<int> ASICadence = new HashSet<int> { 4, 8, 12, 16, 20 };

    public static bool IsFeatLevel(int level)
    {
        return FeatCadence.Contains(level);
    }

    public static bool IsASILevel(int level)
    {
        return ASICadence.Contains(level);
    }

    public static int GetXPToNextLevel(int currentLevel)
    {
        return currentLevel * 1000;
    }

    public static int GetTotalXPForLevel(int level)
    {
        return 1000 * level * (level - 1) / 2;
    }

    //d20 ability modifier: floor((score - 10) / 2).
    //Plain integer division truncates toward zero — wrong for odd scores below 10 (7 must be -2).
    public static int GetAbilityModifier(int score)
    {
        return Mathf.FloorToInt((score - 10) / 2f);
    }

    public static bool IsCoreAttribute(StatType stat)
    {
        return stat == StatType.Power || stat == StatType.Agility || stat == StatType.Endurance
            || stat == StatType.Cognition || stat == StatType.Insight || stat == StatType.Presence;
    }

    //Applies a +1 ability score increase. Cognition and Endurance gains are retroactive:
    //if the modifier rises, the character gains the extra skill ranks / HP for every level already gained.
    public static void ApplyAbilityScoreIncreases(CharacterData data, StatType attribute)
    {
        var classDef = ClassDatabase.Get(data.classId);

        int ranksBefore = classDef != null ? data.GetSkillRanksPerLevel(classDef) : 0;
        int hpBefore = classDef != null ? classDef.GetHPPerLevel(data) : 0;

        data.abilityScoreIncreases.TryGetValue(attribute, out int current);
        data.abilityScoreIncreases[attribute] = current + 1;

        if (classDef == null) return;

        int rankGain = (data.GetSkillRanksPerLevel(classDef) - ranksBefore) * data.level;
        if (rankGain > 0)
        {
            data.skillRanksAvailable += rankGain;
            CombatLog.Add($"{data.name}: {attribute} increase grants {rankGain} retroactive skill rank(s)");
        }

        int hpGain = (classDef.GetHPPerLevel(data) - hpBefore) * data.level;
        if (hpGain > 0)
        {
            data.baseMaxHP += hpGain;
            CombatLog.Add($"{data.name}: {attribute} increase grants {hpGain} retroactive HP");
        }
    }
}
