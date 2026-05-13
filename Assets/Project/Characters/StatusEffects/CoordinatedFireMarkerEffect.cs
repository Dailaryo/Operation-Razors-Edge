using UnityEngine;
using System.Collections.Generic;

public class CoordinatedFireMarkerEffect : StatusEffect
{
    private Unit markedTarget;

    public CoordinatedFireMarkerEffect(Unit target) : base("CoordinatedFireMarker", 1)
    {
        markedTarget = target;
        Priority = 5;
    }

    public override void OnBeforeAction(ActionContext ctx)
    {
        if (ctx.attacker == Target) return; // Not the Technosavant
        if (ctx.attacker.isEnemy != Target.isEnemy) return; // Must be an ally
        if (ctx.finalTarget != markedTarget) return; // Must be targeting the marked unit
        if (!ctx.definition.dealsDamage) return;

        ctx.modifiedDamage += UnityEngine.Random.Range(1, 7); // +1d6

        // Accuracy bonus: add directly to the attack roll modifier
        ctx.attackRollModifier += 2;

        CombatLog.Add($"{ctx.attacker.unitName}: Coordinated Fire — +2 Accuracy, +1d6 damage.");

        duration = 0; // Consume after triggering
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        // +2 Accuracy for all allies targeting the marked target
        return new List<StatModifier>();
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}