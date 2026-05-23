using System.Collections.Generic;
using UnityEngine;

public static class PregenDatabase
{
    public static Dictionary<string, CharacterData> Definitions =
        new Dictionary<string, CharacterData>()
        {
            { PregenIds.AlexandraIvanova, AlexandraIvanova.Build() },
            { PregenIds.RogDarrel, RogDarrel.Build() },
        };

    public static CharacterData Get(string id)
    {
        if (Definitions.TryGetValue(id, out CharacterData data)) return data;

        Debug.LogWarning($"[PregenDatabase] Pregen not found: {id}");
        return null;
    }
}