using UnityEngine;
using System.Collections.Generic;

//Any number of stat modifiers for a duration in rounds. Duration 0 with concentration = lasts while maintained.
public class TimedStatModifierEffect : StatusEffect
{
    private List<StatModifier> modifiers;

    public TimedStatModifierEffect(string name, int duration, params StatModifier[] modifiers)
        : base(name, duration)
    {
        this.modifiers = new List<StatModifier>(modifiers);
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>(modifiers);
    }
}
