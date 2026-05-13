using UnityEngine;
using System.Collections.Generic;

public static class ItemModDatabase
{
    public static Dictionary<string, ItemModDefinition> Definitions =
        new Dictionary<string, ItemModDefinition>()
        {
            // Mods registered here once ItemModIds and content are ready
        };

    public static ItemModDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out ItemModDefinition mod)) return mod;

        Debug.LogWarning($"[ItemModDatabase] Mod not found: {id}");
        return null;
    }
}