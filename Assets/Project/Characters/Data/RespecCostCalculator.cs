using UnityEngine;

public static class RespecCostCalculator
{
    public static int Calculate(CharacterData data)
    {
        if (data.respecCount == 0) return 0;

        return 50 * data.level * (data.respecCount * data.respecCount);
    }
}
