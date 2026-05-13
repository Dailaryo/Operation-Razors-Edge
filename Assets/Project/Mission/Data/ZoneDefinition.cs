using UnityEngine;
using System.Collections.Generic;

public class ZoneDefinition
{
    public string zoneId;
    public string displayName;
    public ZoneType zoneType;
    public int dangerLevel;
    public string unlockCondition; // NarrativeFlag id required to access; null = always accessible
    public List<string> encounterIds; // Ordered list of EncounterIds available in this zone

    public enum ZoneType
    {
        Social, // NPCs, merchants, skill checks; combat only when explicitly authored
        Neutral, // Mixed content; encounters can be any type
        Hostile // Primarily combat; random encounters enabled when authored
    }

    public ZoneDefinition(string id, string name, ZoneType type, string unlockCondition = null, int danger = 1)
    {
        zoneId = id;
        displayName = name;
        zoneType = type;
        this.unlockCondition = unlockCondition;
        dangerLevel = danger;
        encounterIds = new List<string>();
    }
}
