using System.Collections.Generic;
using UnityEngine;

public static class ZoneEventDatabase
{
    public static Dictionary<string, ZoneEventDefinition> Definitions =
        new Dictionary<string, ZoneEventDefinition>()
        {
            // --- Outer Rim Perimeter (Hostile) ---
            { ZoneEventIds.PerimeterPatrol, new ZoneEventDefinition(
                ZoneEventIds.PerimeterPatrol,
                "Imperial Patrol",
                ZoneEventDefinition.ZoneEventType.Combat)
            {
                combatEncounterId = EncounterIds.ScoutEncounter,
                narrativeFlagOnComplete = "perimeter_patrol_complete",
                repeatable = false
            }},

            { ZoneEventIds.PerimeterHoldout, new ZoneEventDefinition(
                ZoneEventIds.PerimeterHoldout,
                "Imperial Holdout",
                ZoneEventDefinition.ZoneEventType.Combat)
            {
                combatEncounterId = EncounterIds.HoldPosition,
                narrativeFlagOnComplete = "perimeter_holdout_complete",
                repeatable = false
            }},

            // --- Outer Rim Command Post (Neutral) ---
            { ZoneEventIds.CommandPostAssault, new ZoneEventDefinition(
                ZoneEventIds.CommandPostAssault,
                "Assault the Command Post",
                ZoneEventDefinition.ZoneEventType.Combat)
            {
                combatEncounterId = EncounterIds.Assassination,
                narrativeFlagOnComplete = "command_post_assault_complete",
                repeatable = false
            }},

            { ZoneEventIds.CommandPostIntel, new ZoneEventDefinition(
                ZoneEventIds.CommandPostIntel,
                "Recover Intel",
                ZoneEventDefinition.ZoneEventType.Narrative)
            {
                dialogueTreeId = null,  // Wired in Milestone 17
                narrativeFlagOnComplete = "command_post_intel_complete",
                unlockCondition = "command_post_assault_complete",
                repeatable = false
            }},

            // --- Outer Rim Settlement (Social) ---
            { ZoneEventIds.SettlementMerchant, new ZoneEventDefinition(
                ZoneEventIds.SettlementMerchant,
                "Coalition Trader",
                ZoneEventDefinition.ZoneEventType.Merchant)
            {
                merchantId = null, //No planet-side merchant authored yet; assign when a PlanetSide MerchantDefinition is created
                repeatable = true
            }},

            { ZoneEventIds.SettlementNegotiation, new ZoneEventDefinition(
                ZoneEventIds.SettlementNegotiation,
                "Negotiate with the Settlement",
                ZoneEventDefinition.ZoneEventType.SkillCheck)
            {
                skillCheckId = null,  // Assign once a suitable SkillCheckDefinition is authored
                narrativeFlagOnComplete = "settlement_negotiation_complete",
                repeatable = false
            }},
            
            // --- Training Grounds (Milestone 28) — repeatable, no flags, spawnLevel 0 = squad level ---
            { ZoneEventIds.TrainingTargetRange, new ZoneEventDefinition(
                ZoneEventIds.TrainingTargetRange,
                "TR-01 Target Range",
                ZoneEventDefinition.ZoneEventType.Combat)
            {
                combatEncounterId = EncounterIds.TrainingTargetRange,
                repeatable = true,
                enemySpawnEntries = new List<ZoneEventDefinition.EnemySpawnEntry>
                {
                    new ZoneEventDefinition.EnemySpawnEntry(EnemyIds.TrainingTargetStatic, 0, 1)
                }
            }},

            { ZoneEventIds.TrainingSaveGauntlet, new ZoneEventDefinition(
                ZoneEventIds.TrainingSaveGauntlet,
                "TR-02 Save Gauntlet",
                ZoneEventDefinition.ZoneEventType.Combat)
            {
                combatEncounterId = EncounterIds.TrainingSaveGauntlet,
                repeatable = true,
                enemySpawnEntries = new List<ZoneEventDefinition.EnemySpawnEntry>
                {
                    new ZoneEventDefinition.EnemySpawnEntry(EnemyIds.TrainingHardenedResolve, 0, 1),
                    new ZoneEventDefinition.EnemySpawnEntry(EnemyIds.TrainingHardenedResponse, 0, 1),
                    new ZoneEventDefinition.EnemySpawnEntry(EnemyIds.TrainingHardenedFocus, 0, 1)
                }
            }},

            { ZoneEventIds.TrainingMindLab, new ZoneEventDefinition(
                ZoneEventIds.TrainingMindLab,
                "TR-03 Mind Lab",
                ZoneEventDefinition.ZoneEventType.Combat)
            {
                combatEncounterId = EncounterIds.TrainingMindLab,
                repeatable = true,
                enemySpawnEntries = new List<ZoneEventDefinition.EnemySpawnEntry>
                {
                    new ZoneEventDefinition.EnemySpawnEntry(EnemyIds.TrainingTargetStatic, 0, 1),
                    new ZoneEventDefinition.EnemySpawnEntry(EnemyIds.TrainingTargetMindless, 0, 1)
                }
            }},

            { ZoneEventIds.TrainingSparringRing, new ZoneEventDefinition(
                ZoneEventIds.TrainingSparringRing,
                "TR-04 Sparring Ring",
                ZoneEventDefinition.ZoneEventType.Combat)
            {
                combatEncounterId = EncounterIds.TrainingSparringRing,
                repeatable = true,
                enemySpawnEntries = new List<ZoneEventDefinition.EnemySpawnEntry>
                {
                    new ZoneEventDefinition.EnemySpawnEntry(EnemyIds.TrainingStriker, 0, 2)
                }
            }},

            { ZoneEventIds.TrainingFiringLine, new ZoneEventDefinition(
                ZoneEventIds.TrainingFiringLine,
                "TR-05 Firing Line",
                ZoneEventDefinition.ZoneEventType.Combat)
            {
                combatEncounterId = EncounterIds.TrainingFiringLine,
                repeatable = true,
                requiresStealth = true, //stealth approach: a successful squad check grants the surprise round
                enemySpawnEntries = new List<ZoneEventDefinition.EnemySpawnEntry>
                {
                    new ZoneEventDefinition.EnemySpawnEntry(EnemyIds.TrainingMarksman, 0, 2)
                }
            }},

            { ZoneEventIds.TrainingTriage, new ZoneEventDefinition(
                ZoneEventIds.TrainingTriage,
                "TR-06 Triage Drill",
                ZoneEventDefinition.ZoneEventType.Combat)
            {
                combatEncounterId = EncounterIds.TrainingTriage,
                repeatable = true,
                enemySpawnEntries = new List<ZoneEventDefinition.EnemySpawnEntry>
                {
                    new ZoneEventDefinition.EnemySpawnEntry(EnemyIds.TrainingBruiser, 0, 1)
                }
            }},

            { ZoneEventIds.TrainingCrowdControl, new ZoneEventDefinition(
                ZoneEventIds.TrainingCrowdControl,
                "TR-07 Crowd Control",
                ZoneEventDefinition.ZoneEventType.Combat)
            {
                combatEncounterId = EncounterIds.TrainingCrowdControl,
                repeatable = true,
                enemySpawnEntries = new List<ZoneEventDefinition.EnemySpawnEntry>
                {
                    new ZoneEventDefinition.EnemySpawnEntry(EnemyIds.TrainingStrikerWeak, 0, 4)
                }
            }},

            { ZoneEventIds.TrainingEscalation, new ZoneEventDefinition(
                ZoneEventIds.TrainingEscalation,
                "TR-08 Escalation",
                ZoneEventDefinition.ZoneEventType.Combat)
            {
                combatEncounterId = EncounterIds.TrainingEscalation,
                repeatable = true,
                enemySpawnEntries = new List<ZoneEventDefinition.EnemySpawnEntry>
                {
                    new ZoneEventDefinition.EnemySpawnEntry(EnemyIds.TrainingStriker, 0, 2)
                }
            }},

            { ZoneEventIds.TrainingLargeTarget, new ZoneEventDefinition(
                ZoneEventIds.TrainingLargeTarget,
                "TR-09 Large Target",
                ZoneEventDefinition.ZoneEventType.Combat)
            {
                combatEncounterId = EncounterIds.TrainingLargeTarget,
                repeatable = true,
                enemySpawnEntries = new List<ZoneEventDefinition.EnemySpawnEntry>
                {
                    new ZoneEventDefinition.EnemySpawnEntry(EnemyIds.TrainingLargeTarget, 0, 1)
                }
            }},
        };

    public static ZoneEventDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out ZoneEventDefinition ZoneEvent)) return ZoneEvent;

        Debug.LogWarning($"[ZoneEventDatabase] ZoneEvent not found: {id}");
        return null;
    }
}