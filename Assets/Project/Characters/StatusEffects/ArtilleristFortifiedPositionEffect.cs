using System.Collections.Generic;
using UnityEngine;

public class ArtilleristFortifiedPositionEffect : StatusEffect
{
    private bool movedThisTurn = false;
    private bool fortifiedActive = false;

    public ArtilleristFortifiedPositionEffect() : base("ArtilleristFortifiedPosition", 0)
    {
        isPersistent = true;
    }

    public override void OnBeforeAction(ActionContext ctx)
    {
        if (ctx.attacker != Target) return;
        if (ctx.isMovement)
            movedThisTurn = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        if (!fortifiedActive) return new List<StatModifier>();

        return new List<StatModifier>
        {
            new StatModifier(StatType.Defense,  4, 1f,
                "fortified_position", StatModifier.BonusType.Competence)
        };
    }

    public override void OnTurnEnd(Unit unit)
    {
        bool wasActive = fortifiedActive;
        fortifiedActive = !movedThisTurn;
        movedThisTurn = false;

        if (fortifiedActive && !wasActive)
            unit.AddEffect(new DamageReductionEffect(2));
        else if (!fortifiedActive && wasActive)
            unit.RemoveEffect<DamageReductionEffect>();
    }
}
