using UnityEngine;

public class NeuralIntrusionEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        //Rolls and results happen once, in the execution phase; planning replays (move highlights, validation) skip them
        if (resolver.IsSimulation) return;

        if (ctx.finalTarget == null) return;

        int defenderFocusSave = ctx.finalTarget.RollSave(Unit.SaveType.Focus);

        var context = new SkillCheckContext
        {
            checkId = SkillCheckIds.SystemOverrideOpposed,
            attacker = ctx.attacker,
            defender = ctx.finalTarget,
            defenderTotalOverride = defenderFocusSave,
            callerContext = SkillCheckContext.CallerContext.Combat,
            showPreview = false,
            onResult = result =>
            {
                if (result.success)
                {
                    if (!resolver.IsSimulation)
                        ctx.finalTarget.AddEffect(new LoseNextActionEffect());
                    CombatLog.Add($"{ctx.attacker.unitName}: Neural Intrusion — {ctx.finalTarget.unitName} loses next action.");
                }
                else
                {
                    CombatLog.Add($"{ctx.attacker.unitName}: Neural Intrusion failed against {ctx.finalTarget.unitName}.");
                }
            }
        };

        SkillCheckExecutor.Execute(context);
    }
}