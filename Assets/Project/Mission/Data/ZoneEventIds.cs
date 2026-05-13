using UnityEngine;

public static class ZoneEventIds
{
    // Outer Rim Perimeter (Hostile zone) — combat encounters
    public const string PerimeterPatrol = "encounter_perimeter_patrol";
    public const string PerimeterHoldout = "encounter_perimeter_holdout";

    // Outer Rim Command Post (Neutral zone) — combat + narrative
    public const string CommandPostAssault = "encounter_command_post_assault";
    public const string CommandPostIntel = "encounter_command_post_intel";

    // Outer Rim Settlement (Social zone) — merchant + skill check
    public const string SettlementMerchant = "encounter_settlement_merchant";
    public const string SettlementNegotiation = "encounter_settlement_negotiation";
}