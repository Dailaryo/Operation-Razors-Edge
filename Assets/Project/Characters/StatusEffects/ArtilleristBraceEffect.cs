using System.Collections.Generic;
using UnityEngine;

public class ArtilleristBraceEffect : StatusEffect
{
    private bool movedThisTurn = false;

    public ArtilleristBraceEffect() : base("ArtilleristBrace", 0)
    {
        isPersistent = true;
    }

    public override void OnBeforeAction(ActionContext ctx)
    {
        if (ctx.attacker != Target) return;
        if (ctx.isMovement)
            movedThisTurn = true;
    }

    public override void OnTurnEnd(Unit unit)
    {
        movedThisTurn = false; // Reset at end of turn
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        if (movedThisTurn) return new List<StatModifier>();

        return new List<StatModifier>
        {
            new StatModifier(StatType.BaseAttackBonus, 1, 1f,
                "artillerist_brace", StatModifier.BonusType.Competence),
            new StatModifier(StatType.MeleePower, 2, 1f,
                "artillerist_brace", StatModifier.BonusType.Competence)
        };
    }
}
