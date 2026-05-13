using UnityEngine;
using System.Collections.Generic;

public static class NpcDatabase
{
    public static Dictionary<string, NpcDefinition> Definitions =
        new Dictionary<string, NpcDefinition>();

    public static NpcDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out NpcDefinition npc)) return npc;

        Debug.LogWarning($"[NpcDatabase] NPC not found: {id}");
        return null;
    }
}
