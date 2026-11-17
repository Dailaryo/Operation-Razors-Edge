using UnityEngine;

public class TSStabilizeEffect : IActionEffect
{
    private const int DC = 10;

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        //Rolls and results happen once, in the execution phase; planning replays (move highlights, validation) skip them
        if (resolver.IsSimulation) return;

        if (ctx.attacker == null || ctx.finalTarget == null) return;

        if (!ctx.finalTarget.IsDying() && !ctx.finalTarget.IsDisabled())
        {
            CombatLog.Add($"{ctx.finalTarget.unitName} does not need stabilization.");
            return;
        }

        if (!DistanceUtility.IsAdjacent(ctx.attacker.position, ctx.finalTarget.position))
        {
            CombatLog.Add($"{ctx.attacker.unitName} must be adjacent to stabilize {ctx.finalTarget.unitName}.");
            return;
        }

        var context = new SkillCheckContext
        {
            checkId       = SkillCheckIds.TreatInjuryStabilize,
            attacker      = ctx.attacker,
            defender      = ctx.finalTarget,
            callerContext = SkillCheckContext.CallerContext.Medical,
            dcOverride    = DC,
            showPreview   = false,
            onResult      = result =>
            {
                if (!result.success)
                {
                    CombatLog.Add($"{ctx.attacker.unitName} fails to stabilize " +
                                  $"{ctx.finalTarget.unitName} (TreatInjury DC {DC}).");
                    return;
                }

                CombatLog.Add($"{ctx.attacker.unitName} stabilizes {ctx.finalTarget.unitName}.");

                if (!resolver.IsSimulation)
                {
                    ctx.finalTarget.RemoveEffect<DyingState>();
                    ctx.finalTarget.RemoveEffect<DisabledState>();
                    if (!ctx.finalTarget.activeEffects.Exists(e => e is StabilizedState))
                        ctx.finalTarget.AddEffect(new StabilizedState());
                    ctx.finalTarget.OnHealthChanged?.Invoke(ctx.finalTarget);
                }
            }
        };

        SkillCheckExecutor.Execute(context);
    }
}
