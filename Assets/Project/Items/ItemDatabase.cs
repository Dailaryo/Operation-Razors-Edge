using UnityEngine;
using System.Collections.Generic;

public static class ItemDatabase
{
    public static Dictionary<string, ItemDefinition> Definitions =
        new Dictionary<string, ItemDefinition>()
        {
            // Items registered here once ItemIds and content are ready
        };
    public static ItemDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out ItemDefinition item)) return item;

        Debug.LogWarning($"[ItemDatabase] Item not found: {id}");
        return null;
    }

}
