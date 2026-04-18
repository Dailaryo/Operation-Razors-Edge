using UnityEngine;

public class DefenseBuffEffect : StatusEffect
{
    float defenseMultiplier;

    public DefenseBuffEffect(float multiplier, int duration) : base("Defense Buff", duration)
    {
        defenseMultiplier = multiplier;
        Priority = 10;
    }

    public float GetDefenseMultiplier()
    {
        return defenseMultiplier;
    }
}
