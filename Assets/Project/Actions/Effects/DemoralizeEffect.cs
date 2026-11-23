using UnityEngine;

public class DemoralizeEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        //Rolls and results happen once, in the execution phase; planning replays (move highlights, validation) skip them
        if (resolver.IsSimulation) return;

        if (ctx.attacker == null || ctx.finalTarget == null) return;

        var fear = EffectDescriptor.Fear | EffectDescriptor.MindAffecting;
        int sourceLevel = ctx.attacker.sourceCharacterData?.level ?? 0;

        resolver.TryFireReaction(ctx.finalTarget, ReactionTrigger.TargetedByMindAffecting, ctx.attacker);

        if (ctx.finalTarget.IsImmuneTo(fear, sourceLevel, consume: !resolver.IsSimulation))
        {
            if (!resolver.IsSimulation)
                CombatLog.Add($"{ctx.finalTarget.unitName} is immune to Demoralize.");
            return;
        }

        var defenderTotal = ctx.finalTarget.RollSave(Unit.SaveType.Focus, fear);

        var context = new SkillCheckContext
        {
            checkId = SkillCheckIds.DemoralizeOpposed,
            attacker = ctx.attacker,
            defender = ctx.finalTarget,
            callerContext = SkillCheckContext.CallerContext.Combat,
            showPreview = false,
            defenderTotalOverride = defenderTotal.total,
            defenderNaturalRoll = defenderTotal.natural,
            onResult = result =>
            {
                if (!result.success) return;
                if (!resolver.IsSimulation)
                {
                    var effect = new ShakenEffect();
                    effect.sourceLevel = sourceLevel;
                    ctx.finalTarget.AddEffect(effect);
                }
            }
        };

        SkillCheckExecutor.Execute(context);
    }
}
