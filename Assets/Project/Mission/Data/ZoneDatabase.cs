using System.Collections.Generic;
using UnityEngine;

public static class ZoneDatabase
{
    public static Dictionary<string, ZoneDefinition> Definitions =
        new Dictionary<string, ZoneDefinition>()
        {
            { ZoneIds.OuterRimPerimeter, new ZoneDefinition(
                ZoneIds.OuterRimPerimeter,
                "Outer Perimeter",
                ZoneDefinition.ZoneType.Hostile
            )
            {
                encounterIds = new List<string> { ZoneEventIds.PerimeterPatrol, ZoneEventIds.PerimeterHoldout }

            }},

            { ZoneIds.OuterRimSettlement, new ZoneDefinition(
                ZoneIds.OuterRimSettlement,
                "Coalition Settlement",
                ZoneDefinition.ZoneType.Social
            )
            {
                encounterIds = new List<string> { ZoneEventIds.SettlementMerchant, ZoneEventIds.SettlementNegotiation }

            }},

            { ZoneIds.OuterRimCommandPost, new ZoneDefinition(
                ZoneIds.OuterRimCommandPost,
                "Abandoned Command Post",
                ZoneDefinition.ZoneType.Neutral
            )
            {
                encounterIds = new List<string> { ZoneEventIds.CommandPostAssault, ZoneEventIds.CommandPostIntel }
            }},
            
            // --- Training Grounds (Milestone 28 debug content) ---
            { ZoneIds.TrainingBasics, new ZoneDefinition(
                ZoneIds.TrainingBasics,
                "Basics Range",
                ZoneDefinition.ZoneType.Hostile
            )
            {
                encounterIds = new List<string> { ZoneEventIds.TrainingTargetRange, ZoneEventIds.TrainingMindLab }
            }},

            { ZoneIds.TrainingSaves, new ZoneDefinition(
                ZoneIds.TrainingSaves,
                "Saves Lab",
                ZoneDefinition.ZoneType.Hostile
            )
            {
                encounterIds = new List<string> { ZoneEventIds.TrainingSaveGauntlet }
            }},

            { ZoneIds.TrainingCombat, new ZoneDefinition(
                ZoneIds.TrainingCombat,
                "Combat Yard",
                ZoneDefinition.ZoneType.Hostile
            )
            {
                encounterIds = new List<string> { ZoneEventIds.TrainingSparringRing, ZoneEventIds.TrainingFiringLine, ZoneEventIds.TrainingTriage }
            }},

            { ZoneIds.TrainingLargeScale, new ZoneDefinition(
                ZoneIds.TrainingLargeScale,
                "Large-scale Range",
                ZoneDefinition.ZoneType.Hostile
            )
            {
                encounterIds = new List<string>
                {
                    ZoneEventIds.TrainingCrowdControl,
                    ZoneEventIds.TrainingEscalation,
                    ZoneEventIds.TrainingLargeTarget
                }
            }},
        };

    public static ZoneDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out ZoneDefinition zone)) return zone;

        Debug.LogWarning($"[ZoneDatabase] Zone not found: {id}");
        return null;
    }
}