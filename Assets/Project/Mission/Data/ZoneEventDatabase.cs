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
        };

    public static ZoneEventDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out ZoneEventDefinition ZoneEvent)) return ZoneEvent;

        Debug.LogWarning($"[ZoneEventDatabase] ZoneEvent not found: {id}");
        return null;
    }
}