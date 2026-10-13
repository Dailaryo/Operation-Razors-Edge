using UnityEngine;
using System.Collections.Generic;

public class SignalScrambleEffect : StatusEffect
{
    private int penalty;

    public SignalScrambleEffect(int penalty) : base("SignalScramble", 2)
    {
        this.penalty = penalty;
        isHarmful = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Accuracy, -penalty, 1f, "SignalScramble",
                StatModifier.BonusType.Untyped)
        };
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}