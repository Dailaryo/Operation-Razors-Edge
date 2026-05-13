using UnityEngine;
using System.Collections.Generic;

public class TargetLockEffect : StatusEffect
{
    public TargetLockEffect() : base("TargetLock", -1)
    {
        isPersistent = true;
        Priority = 5;
    }

    public override void OnBeforeAction(ActionContext ctx)
    {
        if (ctx.attacker != Target) return;
        if (!ctx.definition.dealsDamage) return;
        if (ctx.finalTarget == null) return;

        bool targetDiagnosed = ctx.finalTarget.activeEffects
            .Exists(e => e is DiagnosedEffect);

        if (!targetDiagnosed) return;

        int bonus = UnityEngine.Random.Range(1, 7) + UnityEngine.Random.Range(1, 7);
        ctx.modifiedDamage += bonus;
        CombatLog.Add($"{ctx.attacker.unitName}: Target Lock +{bonus} damage!");
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
        => new List<StatModifier>();

    public override void OnTurnEnd(Unit unit) { }
}