using UnityEngine;
using System.Collections.Generic;

public class SnapshotComparisonData
{
    public string characterName;
    public List<SnapshotComparisonEntry> entries = new List<SnapshotComparisonEntry>();

    public class SnapshotComparisonEntry
    {
        public string fieldName;
        public string snapshotValue;
        public string currentValue;
        public bool hasChanged;
    }

    public static SnapshotComparisonData Compare(CharacterSnapshot snapshot, CharacterData current)
    {
        var result = new SnapshotComparisonData();
        result.characterName = current.name;

        result.Add("Level", snapshot.level.ToString(), current.level.ToString());
        result.Add("XP", snapshot.xp.ToString(), current.xp.ToString());
        result.Add("MaxHP", snapshot.baseMaxHP.ToString(), current.baseMaxHP.ToString());
        result.Add("HP Damage", snapshot.hpDamage.ToString(), current.hpDamage.ToString());

        //Attributes (ASI totals)
        StatType[] attrs = new StatType[]
        {
            StatType.Power, StatType.Agility, StatType.Endurance,
            StatType.Cognition, StatType.Insight, StatType.Presence
        };

        foreach (var attr in attrs)
        {
            snapshot.abilityScoreIncreases.TryGetValue(attr, out int snapASI);
            current.abilityScoreIncreases.TryGetValue(attr, out int currASI);
            result.Add($"ASI_{attr}", snapASI.ToString(), currASI.ToString());
        }

        //Skills
        foreach (SkillType skill in System.Enum.GetValues(typeof(SkillType)))
        {
            snapshot.skillRanks.TryGetValue(skill, out int snapRanks);
            current.skillRanks.TryGetValue (skill, out int currRanks);
            result.Add($"Skill_{skill}", snapRanks.ToString(), currRanks.ToString());
        }

        //Feats: compare counts and list
        result.Add("FeatCount",
            snapshot.equippedFeatIds.Count.ToString(),
            current.equippedFeatIds.Count.ToString());

        //New feats gained
        foreach (var feat in current.equippedFeatIds)
        {
            if (!snapshot.equippedFeatIds.Contains(feat))
                result.Add($"NewFeat_{feat}", "(none)", feat);
        }

        return result;
    }

    private void Add(string fieldName, string snapValue, string currValue)
    {
        entries.Add(new SnapshotComparisonEntry
        {
            fieldName = fieldName,
            snapshotValue = snapValue,
            currentValue = currValue,
            hasChanged = snapValue != currValue
        });
    }
}
