using UnityEngine;

public class StatModifier
{
    public StatType stat;

    public int flatBonus;

    public float multiplier = 1f;

    public string source;

    public StatModifier(StatType stat, int flat, float mult, string source)
    {
        this.stat = stat;
        flatBonus = flat;
        multiplier = mult;
        this.source = source;
    }
}
