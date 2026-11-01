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

    // Training drills (planet_training) — Milestone 28 debug content, all repeatable combat
    public const string TrainingTargetRange = "zone_event_training_target_range";   // TR-01
    public const string TrainingSaveGauntlet = "zone_event_training_save_gauntlet";  // TR-02
    public const string TrainingMindLab = "zone_event_training_mind_lab";       // TR-03
    public const string TrainingSparringRing = "zone_event_training_sparring_ring";  // TR-04
    public const string TrainingFiringLine = "zone_event_training_firing_line";    // TR-05
    public const string TrainingTriage = "zone_event_training_triage";         // TR-06
    public const string TrainingCrowdControl = "zone_event_training_crowd_control";  // TR-07
    public const string TrainingEscalation = "zone_event_training_escalation";     // TR-08
    public const string TrainingLargeTarget = "zone_event_training_large_target";   // TR-09
}