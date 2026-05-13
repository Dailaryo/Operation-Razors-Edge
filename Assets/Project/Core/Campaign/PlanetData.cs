using System.Collections.Generic;
using UnityEngine;

public class PlanetData
{
    public string planetId;
    public string planetName;

    public int difficultyTier;

    public string civilizationId;
    public TechTierProfile tierOverrides;

    public List<string> encounterIds;

    // Zone content authored at design time — populated in PlanetDatabase
    public List<string> zoneIds;

    // Runtime exploration state — managed by ExplorationController
    public List<string> unlockedZones;
    public List<string> completedEncounters;
    public Dictionary<string, int> randomEncounterCounts;
    public string currentZone;  // null when not in PlanetSide

    public enum PlanetAccessibility
    {
        Active,
        Left,
        Inaccessible, //Was accessible, now closed
        Locked //Not yet accessible
    }

    public PlanetAccessibility accessibility;

    public PlanetData(string id, string name, int tier, string civId = null)
    {
        planetId = id;
        planetName = name;
        difficultyTier = tier;
        civilizationId = civId;
        tierOverrides = null;
        encounterIds = new List<string>();
        zoneIds = new List<string>();
        unlockedZones = new List<string>();
        completedEncounters = new List<string>();
        randomEncounterCounts = new Dictionary<string, int>();
        currentZone = null;
        accessibility = PlanetAccessibility.Active;
    }

    public int GetTierForCategory(TechCategory category, CivilizationData civ)
    {
        if (tierOverrides != null && tierOverrides.HasExplicitTier(category))
            return tierOverrides.GetTierForCategory(category);
        return civ.baselineTiers.GetTierForCategory(category);
    }
}
