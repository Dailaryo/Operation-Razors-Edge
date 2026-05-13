using UnityEngine;

public static class FlagIds
{
    // Faction disposition flags
    // Values: 0 = neutral, 1 = allied, -1 = hostile
    public const string FactionCoalitionDisposition = "faction_coalition_disposition";
    public const string FactionSyndicateDisposition = "faction_syndicate_disposition";
    public const string FactionImperiumDisposition = "faction_imperium_disposition";

    // Mission outcome flags (for reference — set dynamically as missionId + suffix)
    // Format: "{missionId}_complete", "{missionId}_failed", "{missionId}_locked"
}
