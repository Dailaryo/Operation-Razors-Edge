using UnityEngine;
using System.Collections.Generic;

//Mind Blank (PF): +8 resistance bonus on all saves against mind-affecting effects, until the Hub (24 hours).
//Deferred: immunity to information-gathering powers (Detect Thoughts, Mindprobe, Metafaculty, scrying).
public class MindBlankEffect : StatusEffect
{
    public MindBlankEffect() : base("MindBlank", 0)
    {
        lastsUntilHub = true;
    }

    public override List<StatModifier> GetConditionalStatModifiers(Unit unit, EffectDescriptor incoming)
    {
        if ((incoming & EffectDescriptor.MindAffecting) == 0) return new List<StatModifier>();

        return new List<StatModifier>
        {
            new StatModifier(StatType.Willpower, 8, 1f, "MindBlank", StatModifier.BonusType.Resistance),
            new StatModifier(StatType.Fortitude, 8, 1f, "MindBlank", StatModifier.BonusType.Resistance),
            new StatModifier(StatType.Evasion,   8, 1f, "MindBlank", StatModifier.BonusType.Resistance)
        };
    }
}
