using System.Collections.Generic;
using UnityEngine;

public class CampaignData
{
    public List<ActData> acts;
    public List<PlanetData> planets;

    public int currentActIndex;

    public Dictionary<string, EncounterData> encounters = new Dictionary<string, EncounterData>();

    public CampaignData()
    {
        acts = new List<ActData>();
        planets = new List<PlanetData>();
        currentActIndex = 0;
    }

    public PlanetData GetPlanet(string planetId)
    {
        foreach (var planet in planets)
        {
            if (planet.planetId == planetId) return planet;
        }

        Debug.LogWarning($"[CampaignData] Planet not found: {planetId}");
        return null;
    }

    public ActData GetCurrentAct()
    {
        if (currentActIndex < 0 || currentActIndex >= acts.Count)
        {
            Debug.LogWarning($"[CampaignData] currentActIndex {currentActIndex} out of range");
            return null;
        }

        return acts[currentActIndex];
    }

    public CivilizationData GetCivilizationForPlanet(string planetId)
    {
        var planet = GetPlanet(planetId);
        if (planet == null) return null;
        if (planet.civilizationId == null) return null;
        return CivilizationDatabase.Get(planet.civilizationId);
    }

    public EncounterData GetEncounter(string encounterId)
    {
        if (encounters.TryGetValue(encounterId, out EncounterData encounter)) return encounter;
        Debug.LogWarning($"[CampaignData] Encounter not found: {encounterId}");
        return null;
    }
}
