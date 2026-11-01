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
            
            // Training planet (Milestone 28): debug-only drills. Not listed in any Act, so
            // CampaignManager.BuildCampaign never includes it and act progress is unaffected.
            { PlanetIds.Training, new PlanetData(
                PlanetIds.Training,
                "Training Grounds",
                1
            )
            {
                encounterIds = new List<string>
                {
                    EncounterIds.TrainingTargetRange,
                    EncounterIds.TrainingSaveGauntlet,
                    EncounterIds.TrainingMindLab,
                    EncounterIds.TrainingSparringRing,
                    EncounterIds.TrainingFiringLine,
                    EncounterIds.TrainingTriage,
                    EncounterIds.TrainingCrowdControl,
                    EncounterIds.TrainingEscalation,
                    EncounterIds.TrainingLargeTarget
                },
                zoneIds = new List<string>
                {
                    ZoneIds.TrainingBasics,
                    ZoneIds.TrainingSaves,
                    ZoneIds.TrainingCombat,
                    ZoneIds.TrainingLargeScale
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