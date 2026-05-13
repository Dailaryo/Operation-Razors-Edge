using UnityEngine;
using System.Collections.Generic;

public static class EndingDatabase
{
    // Ordered list: EndingResolver evaluates top to bottom, first match wins.
    // The fallback ending (no required flags) must always be last.
    public static readonly List<EndingData> Definitions = new List<EndingData>
    {
        // Best ending: both required encounters complete
        new EndingData(
            EndingIds.VictoryFull,
            "The Empire Fractures",
            "The assassination was clean and the perimeter held. Command is calling it a success. " +
            "What comes next is anyone's guess.",
            new List<string> { "scout_encounter_complete", "assassination_complete" }
        ),

        // Partial ending: only assassination complete
        new EndingData(
            EndingIds.VictoryPartial,
            "A Costly Win",
            "The target is dead, but the scout teams took heavy losses. " +
            "The campaign continues with fewer options than before.",
            new List<string> { "assassination_complete" }
        ),

        // Fallback: no required flags — always matches last
        new EndingData(
            EndingIds.Default,
            "Unfinished Business",
            "The operation is over, but the objectives remain incomplete. " +
            "The war goes on.",
            new List<string>()
        )
    };
}
