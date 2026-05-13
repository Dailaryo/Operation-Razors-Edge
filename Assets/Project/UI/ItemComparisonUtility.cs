using System.Collections.Generic;

public static class ItemComparisonUtility
{
    public class ComparisonResult
    {
        // Per-stat delta: positive means candidate is better
        public Dictionary<StatType, int> statDeltas = new Dictionary<StatType, int>();
    }

    public static ComparisonResult Compare(ItemDefinition equipped, ItemDefinition candidate)
    {
        var result = new ComparisonResult();

        if (candidate == null) return result;

        // Collect candidate stats
        var candidateStats = new Dictionary<StatType, int>();
        if (candidate.statModifiers != null)
        {
            foreach (var mod in candidate.statModifiers)
            {
                candidateStats.TryGetValue(mod.stat, out int existing);
                candidateStats[mod.stat] = existing + mod.flatBonus;
            }
        }

        // Collect equipped stats
        var equippedStats = new Dictionary<StatType, int>();
        if (equipped != null && equipped.statModifiers != null)
        {
            foreach (var mod in equipped.statModifiers)
            {
                equippedStats.TryGetValue(mod.stat, out int existing);
                equippedStats[mod.stat] = existing + mod.flatBonus;
            }
        }

        // Compute deltas for all stats mentioned by either item
        var allStats = new HashSet<StatType>(candidateStats.Keys);
        allStats.UnionWith(equippedStats.Keys);

        foreach (var stat in allStats)
        {
            candidateStats.TryGetValue(stat, out int candidateVal);
            equippedStats.TryGetValue(stat, out int equippedVal);
            result.statDeltas[stat] = candidateVal - equippedVal;
        }

        return result;
    }
}