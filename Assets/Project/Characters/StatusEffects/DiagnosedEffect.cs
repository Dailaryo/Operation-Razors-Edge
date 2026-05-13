using UnityEngine;
using System.Collections.Generic;

public class DiagnosedEffect : StatusEffect
{
    public DiagnosedEffect() : base("Diagnosed", 2)
    {
        // Duration 2: applied at end of attacker's turn, expires after target's
        // next turn end — net effect is active through attacker's following turn
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(
                StatType.Evasion,
                -1,
                1f,
                "Diagnosed",
                StatModifier.BonusType.Untyped)
        };
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}
