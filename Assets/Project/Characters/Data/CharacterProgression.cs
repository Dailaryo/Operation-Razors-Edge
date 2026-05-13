using System.Collections.Generic;
using UnityEngine;

public static class CharacterProgression
{
    public static HashSet<int> FeatCadence = new HashSet<int> { 1, 4, 7, 10, 13, 16, 19};

    public static HashSet<int> ASICadence = new HashSet<int> { 4, 8, 12, 16, 20 };

    public static bool IsFeatLevel(int level)
    {
        return FeatCadence.Contains(level);
    }

    public static bool IsASILevel(int level)
    {
        return ASICadence.Contains(level);
    }

    public static int GetXPToNextLevel(int currentLevel)
    {
        return currentLevel * 1000;
    }

    public static int GetTotalXPForLevel(int level)
    {
        return 1000 * level * (level - 1) / 2;
    }
}
