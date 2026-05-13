using UnityEngine;
using static ActionDefinition;

public static class ConditionEvaluator
{
    public static bool Evaluate(ConditionType condition, ActionContext ctx)
    {
        switch (condition)
        {
            //Attacker Conditions
            case ConditionType.AttackerIsTarget:
                //True when the effect owner is the one performing the action.
                return ctx.attacker != null;

            case ConditionType.AttackerIsMelee:
                return ctx.definition != null && ctx.definition.attackType == ActionDefinition.AttackType.Melee;

            case ConditionType.AttackIsRanged:
                return ctx.definition != null && ctx.definition.attackType == ActionDefinition.AttackType.Ranged;

            //Hit Conditions
            case ConditionType.ActionDealsDamage:
                return ctx.definition != null && ctx.definition.dealsDamage;

            case ConditionType.OnHit:
                return ctx.shouldApplyDamage;

            case ConditionType.OnCrit:
                return ctx.isCritical;

            //Targer conditions
            case ConditionType.TargetBelowHalfHP:
                if (ctx.finalTarget == null) return false;
                return ctx.finalTarget.currentHP < (ctx.finalTarget.maxHP / 2);

            case ConditionType.TargetIsFlanked:
                //FUTURE: requires flanking detection system
                Debug.LogWarning("ConditionType.TargetIsFlanked evaluated before flanking system exists");
                return false;

            //Positional
            case ConditionType.AttackerIsFlanking:
                //FUTURE: requires flanking detection system
                Debug.LogWarning("ConditionType.AttackerIsFlanking evaluated before flanking system exists");
                return false;

            default:
                Debug.LogWarning($"ConditionEvaluator: unhandled condition '{condition}'");
                return false;
        }
    }

    //Evaluates all conditions in a set.  Returns true only if ALL pass.
    public static bool EvaluateAll(ConditionType[] conditions, ActionContext ctx)
    {
        foreach (var condition in conditions)
        {
            if (!Evaluate(condition, ctx)) return false;
        }
        return true;
    }
}
