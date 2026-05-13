using UnityEngine;
using System.Collections.Generic;

public class TechTierProfile
{
    public int defaultTier;
    private Dictionary<TechCategory, int> categoryTiers;

    public TechTierProfile(int defaultTier, Dictionary<TechCategory, int> categoryTiers = null)
    {
        this.defaultTier = defaultTier;
        this.categoryTiers = categoryTiers ?? new Dictionary<TechCategory, int>();
    }

    public int GetTierForCategory(TechCategory category)
    {
        if (categoryTiers.TryGetValue(category, out int tier)) return tier;
        return defaultTier;
    }

    public bool HasExplicitTier(TechCategory category)
    {
        return categoryTiers.ContainsKey(category);
    }

    public static string GetDiegeticLabel(TechCategory category, int pl)
    {
        string tierName = pl switch
        {
            6 => "Fusion Age",
            7 => "Gravity Age",
            8 => "Energy Age",
            _ => "Unknown"
        };
        return $"{tierName} {category}";
    }
}
