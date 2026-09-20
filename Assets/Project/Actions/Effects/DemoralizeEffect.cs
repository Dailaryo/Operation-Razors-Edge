using UnityEngine;

public class DemoralizeEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null || ctx.finalTarget == null) return;

        int defenderTotal = ctx.finalTarget.RollSave(Unit.SaveType.Focus);

        var context = new SkillCheckContext
        {
            checkId = SkillCheckIds.DemoralizeOpposed,
            attacker = ctx.attacker,
            defender = ctx.finalTarget,
            callerContext = SkillCheckContext.CallerContext.Combat,
            showPreview = false,
            defenderTotalOverride = defenderTotal,
            onResult = result =>
            {
                if (!result.success) return;
                if (!resolver.IsSimulation)
                {
                    var effect = new ShakenEffect();
                    effect.sourceLevel = ctx.attacker?.sourceCharacterData?.level ?? 0;
                    ctx.finalTarget.AddEffect(effect);
                }
            }
        };

        SkillCheckExecutor.Execute(context);
    }
}
