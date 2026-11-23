using UnityEngine;
using System.Collections.Generic;

//Power Attack / Deadly Aim (Pathfinder): -penalty to melee (Power Attack) or ranged (Deadly Aim) attack rolls and
//+2x penalty to that attack type's damage until the end of the round.
//penalty = 1 + BAB / 4 (-1/+2, rising at BAB 4, 8, 12, 16, 20).
//DEFERRED: Power Attack's two-handed x1.5 damage waits for weapons; Deadly Aim's touch-attack exclusion waits for touch attacks
public class PowerAttackEffect : StatusEffect
{
    private int penalty;
    private bool ranged;

    public PowerAttackEffect(int penalty, bool ranged = false) : base(ranged ? "Deadly Aim" : "Power Attack", 1)
    {
        this.penalty = penalty;
        this.ranged = ranged;
        Priority = 5;
    }

    public static int GetPenalty(Unit unit) => 1 + Mathf.Max(0, unit.GetBAB()) / 4;

    ActionDefinition.AttackType AffectedAttackType =>
        ranged ? ActionDefinition.AttackType.Ranged : ActionDefinition.AttackType.Melee;

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(ranged ? StatType.RangedAccuracy : StatType.MeleeAccuracy, -penalty, 1f, effectName,
                StatModifier.BonusType.Untyped)
        };
    }

    //Runs after InitializeDamage (ApplyPreActionEffects), on this unit's attacks of the matching type only.
    //Only hit point damage reaches here, so effects that deal no HP damage never get the bonus
    public override void OnBeforeAction(ActionContext ctx)
    {
        if (ctx.attacker != Target) return;
        if (ctx.definition == null || ctx.definition.attackType != AffectedAttackType) return;

        ctx.modifiedDamage += penalty * 2;
        CombatLog.Add($"{Target.unitName} gains +{penalty * 2} damage from {effectName}");
    }
}