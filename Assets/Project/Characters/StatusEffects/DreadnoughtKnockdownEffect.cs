using System.Collections.Generic;
using UnityEngine;

public class DreadnoughtKnockdownEffect : StatusEffect
{
    public DreadnoughtKnockdownEffect() : base("DreadnoughtKnockdown", 0)
    {
        isPersistent = true;
    }

    public override void OnAfterAction(ActionContext ctx)
    {
        if (!ConditionsMet(ctx)) return;

        // Only fires on melee attacks that dealt damage
        if (ctx.definition.attackType != ActionDefinition.AttackType.Melee) return;
        if (!ctx.shouldApplyDamage) return;

        //Class passives live in effectSources, so Target is never set: check the attacker's passives instead
        if (ctx.attacker.sourceCharacterData == null ||
            !ctx.attacker.sourceCharacterData.unlockedPassiveIds.Contains(PassiveIds.DreadnoughtKnockdown)) return;

        if (ctx.finalTarget == null || ctx.finalTarget.IsDead()) return;
        if (ctx.modifiedDamage <= 0) return;

        int dc = Mathf.RoundToInt(ctx.modifiedDamage);
        int saveRoll = ctx.finalTarget.RollSave(Unit.SaveType.Resolve);

        CombatLog.Add($"{ctx.finalTarget.unitName} Knockdown save: {saveRoll} vs DC {dc}");

        if (saveRoll < dc)
        {
            // Apply prone — scoped when ProneEffect is authored (M22 opportunity attacks)
            // For now: log the result as a placeholder
            CombatLog.Add($"{ctx.finalTarget.unitName} is knocked prone!");
        }
    }
}
