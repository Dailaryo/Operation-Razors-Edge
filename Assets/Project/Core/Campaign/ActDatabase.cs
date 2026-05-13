using System.Collections.Generic;
using UnityEngine;

public static class ActDatabase
{
    public static Dictionary<string, ActData> Definitions =
        new Dictionary<string, ActData>()
        {
            { ActIds.Act1, new ActData(
                0,
                "Act 1"
            )
            {
                planetIds = new List<string>
                {
                    PlanetIds.PlanetStart
                }
            }},
        };

    public static ActData Get(string id)
    {
        if (Definitions.TryGetValue(id, out ActData act)) return act;

        Debug.LogWarning($"[ActDatabase] Act not found: {id}");
        return null;
    }
}