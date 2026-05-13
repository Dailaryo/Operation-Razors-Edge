using UnityEngine;

public class FeintEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null || ctx.finalTarget == null) return;

        var context = new SkillCheckContext
        {
            checkId = SkillCheckIds.FeintOpposed,
            attacker = ctx.attacker,
            defender = ctx.finalTarget,
            callerContext = SkillCheckContext.CallerContext.Combat,
            showPreview = false,
            onResult = result =>
            {
                if (!result.success) return;
                if (!resolver.IsSimulation)
                    ctx.finalTarget.AddEffect(new FeignedEffect(ctx.finalTarget));
            }
        };

        SkillCheckExecutor.Execute(context);
    }
}
