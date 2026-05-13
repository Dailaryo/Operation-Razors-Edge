using UnityEngine;
using System.Collections.Generic;

public static class FactionDatabase
{
    public static readonly Dictionary<string, FactionData> Definitions = BuildDefinitions();

    private static Dictionary<string, FactionData> BuildDefinitions()
    {
        var coalition = new FactionData(
            FactionIds.Coalition,
            "The Outer Rim Coalition",
            "A loose alliance of frontier settlements resisting Imperial expansion."
        );
        coalition.missionHooks.Add("scout_encounter");
        coalition.missionHooks.Add("hold_position");

        var imperium = new FactionData(
            FactionIds.Imperium,
            "The Human Imperium",
            "The collapsing central authority whose internal power struggle drives the campaign."
        );
        imperium.missionHooks.Add("assassination");

        return new Dictionary<string, FactionData>
        {
            { FactionIds.Coalition, coalition },
            { FactionIds.Imperium,  imperium  }
        };
    }

    public static FactionData Get(string factionId)
    {
        Definitions.TryGetValue(factionId, out FactionData result);
        return result;
    }
}