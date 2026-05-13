using System.Collections.Generic;
using UnityEngine;

public static class PlanetDatabase
{
    public static Dictionary<string, PlanetData> Definitions =
        new Dictionary<string, PlanetData>()
        {
            { PlanetIds.PlanetStart, new PlanetData(
                PlanetIds.PlanetStart,
                "Outer Rim",
                1
            )
            {
                encounterIds = new List<string>
                {
                    EncounterIds.ScoutEncounter,
                    EncounterIds.HoldPosition,
                    EncounterIds.Assassination
                },
                zoneIds = new List<string>
                {
                    ZoneIds.OuterRimPerimeter,
                    ZoneIds.OuterRimSettlement,
                    ZoneIds.OuterRimCommandPost
                }
            }},
        };

    public static PlanetData Get(string id)
    {
        if (Definitions.TryGetValue(id, out PlanetData planet)) return planet;

        Debug.LogWarning($"[PlanetDatabase] Planet not found: {id}");
        return null;
    }
}