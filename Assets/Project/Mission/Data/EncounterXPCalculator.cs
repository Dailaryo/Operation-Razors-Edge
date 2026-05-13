using UnityEngine;
using System.Collections.Generic;

public static class EncounterXPCalculator
{
    // Scaling thresholds from the design doc.
    // levelDelta = characterLevel - encounterLevel
    // Positive delta means the character is above the encounter level.
    private const int FullXPThreshold = 2; //delta <= this: full XP
    private const int HighXPThreshold = 3; //delta <= this: 75% XP
    private const int MidXPThreshold = 4; //delta <= this: 50% XP
    private const int LowXPThreshold = 5; //delta <= this: 25% XP
    private const int MinimalXPThreshold = 6; //delta <= this: 5% XP
    //delta > 6: no XP

    private const float HighXPRate = 0.75f;
    private const float MidXPRate = 0.5f;
    private const float LowXPRate = 0.25f;
    private const float MinimalXPRate = 0.05f;

    // Calculates scaled XP for a single character against a single encounter level.
    // encounterLevel is the effective level of the encounter — for combat encounters
    // this is the highest enemy level in the group; for skill challenges and other
    // non-combat encounters this is set by the EncounterDefinition directly.
    public static int Calculate(int baseXp, int encounterLevel, int characterLevel)
    {
        int delta = characterLevel - encounterLevel;

        if (delta <= FullXPThreshold) return baseXp;
        if (delta <= HighXPThreshold) return Mathf.RoundToInt(baseXp * HighXPRate);
        if (delta <= MidXPThreshold) return Mathf.RoundToInt(baseXp * MidXPRate);
        if (delta <= LowXPThreshold) return Mathf.RoundToInt(baseXp * LowXPRate);
        if (delta <= MinimalXPThreshold) return Mathf.RoundToInt(baseXp * MinimalXPRate);

        return 0;
    }

    // Builds a per-character XP map for the whole squad against one encounter level.
    // Returns a Dictionary keyed by characterId for use in GrantXPToSquadScaled().
    public static Dictionary<string, int> CalculateForSquad(
        int baseXP,
        int encounterLevel,
        List<CharacterData> squad)
    {
        var result = new Dictionary<string, int>();

        foreach (CharacterData character in squad)
        {
            if (character.isIncapacitated) continue;

            int scaled = Calculate(baseXP, encounterLevel, character.level);
            result[character.characterId] = scaled;
        }

        return result;
    }
}
