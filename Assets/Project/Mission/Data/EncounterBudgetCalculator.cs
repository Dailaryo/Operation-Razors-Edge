using UnityEngine;
using System.Collections.Generic;

public static class EncounterBudgetCalculator
{
    // Tuning parameter — adjust during playtesting without code changes.
    // Applied as a multiplier to the raw (dangerLevel + averagePartyLevel) sum.
    public const float DifficultyMultiplier = 1.0f;

    // Returns the point budget for a random encounter in the given zone,
    // scaled against the current squad's average level.
    // Returns 0 if squad has no living members.
    public static int Calculate(ZoneDefinition zone, List<CharacterData> squad)
    {
        int averageLevel = GetAveragePartyLevel(squad);
        if (averageLevel == 0) return 0;

        float raw = (zone.dangerLevel + averageLevel) * DifficultyMultiplier;
        return Mathf.Max(1, Mathf.RoundToInt(raw));
    }

    static int GetAveragePartyLevel(List<CharacterData> squad)
    {
        if (squad == null || squad.Count == 0) return 0;

        int total = 0;
        int livingCount = 0;

        foreach (CharacterData character in squad)
        {
            if (character.isIncapacitated) continue;

            total += character.level;
            livingCount++;
        }

        if (livingCount ==  0) return 0;

        return Mathf.RoundToInt((float)total / livingCount);
    }
}
