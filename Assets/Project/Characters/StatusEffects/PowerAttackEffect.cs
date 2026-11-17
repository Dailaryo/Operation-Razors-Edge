using UnityEngine;
using System.Collections.Generic;

//Power Attack (Pathfinder): -penalty to melee attack rolls and +2x penalty to melee damage until the end of the round.
//penalty = 1 + BAB / 4 (-1/+2, rising at BAB 4, 8, 12, 16, 20). DEFERRED: two-handed x1.5 damage waits for weapons
public class PowerAttackEffect : StatusEffect
{
    private int penalty;

    public PowerAttackEffect(int penalty) : base("Power Attack", 1)
    {
        this.penalty = penalty;
        Priority = 5;
    }

    public static int GetPenalty(Unit unit) => 1 + Mathf.Max(0, unit.GetBAB()) / 4;

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.MeleeAccuracy, -penalty, 1f, "Power Attack", StatModifier.BonusType.Untyped)
        };
    }

    //Runs after InitializeDamage (ApplyPreActionEffects), on this unit's melee attacks only
    public override void OnBeforeAction(ActionContext ctx)
    {
        if (ctx.attacker != Target) return;
        if (ctx.definition == null || ctx.definition.attackType != ActionDefinition.AttackType.Melee) return;

        ctx.modifiedDamage += penalty * 2;
        CombatLog.Add($"{Target.unitName} gains +{penalty * 2} damage from Power Attack");
    }
}
