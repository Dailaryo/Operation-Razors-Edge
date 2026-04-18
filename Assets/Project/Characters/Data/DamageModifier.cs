using UnityEngine;

public class DamageModifier
{
    public string source;

    public int flatBonus;
    public float multiplier = 1f;

    public bool stackable = false;

    public DamageModifier(string source, int flat = 0, float mult = 1f, bool stackable = false)
    {
        this.source = source;
        flatBonus = flat;
        multiplier = mult;
        this.stackable = stackable;
    }
}
