using UnityEngine;
using System.Collections.Generic;

public class SquadRuntime
{
    public CharacterData source;

    public int currentHP;

    public List<StatusEffect> persistenttEffects = new List<StatusEffect>();

    public SquadRuntime(CharacterData source)
    {
        this.source = source;
        this.currentHP = source.currentHP;
    }

    public void ApplyDamage(int amount)
    {
        currentHP -= amount;

        if (currentHP < 0) currentHP = 0;
    }

    public void HealToFull()
    {
        currentHP = source.baseMaxHP;
        persistenttEffects.Clear();
    }
}
