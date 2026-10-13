using System.Collections.Generic;
using UnityEngine;

public static class PregenDatabase
{
    // Pre-written level-up routes, keyed by pregen ID then character level.
    // MUST stay declared above Definitions: building a pregen levels it up,
    // which reads these plans during static initialization.
    public static Dictionary<string, Dictionary<int, PregenLevelPlan>> Plans =
        new Dictionary<string, Dictionary<int, PregenLevelPlan>>()
        {
            { PregenIds.AlexandraIvanova, AlexandraIvanova.BuildRoute() },
            { PregenIds.RogDarrel,        RogDarrel.BuildRoute() },
            { PregenIds.MeharelNueva,     MeharelNueva.BuildRoute() },
            { PregenIds.GrokBaird,        GrokBaird.BuildRoute() },
            { PregenIds.HarveyJones,      HarveyJones.BuildRoute() },
            { PregenIds.AnyhaMcRae,       AnyhaMcRae.BuildRoute() },
        };

    public static Dictionary<int, PregenLevelPlan> GetPlan(string id)
    {
        if (id != null && Plans.TryGetValue(id, out var plan)) return plan;
        return null; //Custom characters have no plan
    }

    public static Dictionary<string, CharacterData> Definitions =
        new Dictionary<string, CharacterData>()
        {
            { PregenIds.AlexandraIvanova, AlexandraIvanova.Build() },
            { PregenIds.RogDarrel, RogDarrel.Build() },
            { PregenIds.MeharelNueva, MeharelNueva.Build() },
            { PregenIds.GrokBaird, GrokBaird.Build() },
            { PregenIds.HarveyJones, HarveyJones.Build() },
            { PregenIds.AnyhaMcRae, AnyhaMcRae.Build() },
        };

    public static CharacterData Get(string id)
    {
        if (Definitions.TryGetValue(id, out CharacterData data)) return data;

        Debug.LogWarning($"[PregenDatabase] Pregen not found: {id}");
        return null;
    }
}