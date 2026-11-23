using UnityEngine;

public class SystemOverrideEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        //Rolls and results happen once, in the execution phase; planning replays (move highlights, validation) skip them
        if (resolver.IsSimulation) return;

        if (ctx.finalTarget == null) return;

        // Compute defender's Focus save as the opposed DC
        var defenderFocusSave = ctx.finalTarget.RollSave(Unit.SaveType.Focus);

        var context = new SkillCheckContext
        {
            checkId = SkillCheckIds.SystemOverrideOpposed,
            attacker = ctx.attacker,
            defender = ctx.finalTarget,
            defenderTotalOverride = defenderFocusSave.total,
            defenderNaturalRoll = defenderFocusSave.natural,
            callerContext = SkillCheckContext.CallerContext.Combat,
            showPreview = false,
            onResult = result =>
            {
                if (result.success)
                {
                    CombatLog.Add($"{ctx.attacker.unitName} overrides {ctx.finalTarget.unitName}'s systems.");
                    if (!resolver.IsSimulation)
                    {
                        ctx.finalTarget.AddEffect(new LoseNextActionEffect());

                        // Feedback Loop bonus damage
                        if (ctx.attacker.activeEffects.Exists(e => e is FeedbackLoopEffect))
                        {
                            int feedbackDamage = ctx.attacker.GetModifier(StatType.Cognition);
                            ctx.finalTarget.TakeDamage(feedbackDamage);
                            CombatLog.Add($"{ctx.attacker.unitName}: Feedback Loop deals {feedbackDamage} damage.");
                        }
                    }
                }
                else
                {
                    CombatLog.Add($"{ctx.attacker.unitName} fails to override {ctx.finalTarget.unitName}'s systems.");
                }
            }
        };

        SkillCheckExecutor.Execute(context);
    }
}