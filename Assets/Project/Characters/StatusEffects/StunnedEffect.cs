using UnityEngine;
using System.Collections.Generic;

//Stunned (d20 Modern): no actions, loses AGI bonus to Defense, -2 Defense
public class StunnedEffect : StatusEffect
{
    public StunnedEffect(int rounds) : base("Stunned", rounds) { isHarmful = true; }

    public override bool BlocksActions => true;

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        int agilityBonus = Mathf.Max(0, unit.GetModifier(StatType.Agility));
        return new List<StatModifier>
        {
            new StatModifier(StatType.Defense, -(agilityBonus + 2), 1f, "Stunned", StatModifier.BonusType.Untyped)
        };
    }
}
