using UnityEngine;

public class TreatInjuryEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null || ctx.finalTarget == null) return;

        //Only valid on dying targets
        if (!ctx.finalTarget.IsDying())
        {
            CombatLog.Add($"{ctx.finalTarget.unitName} is not dying and does not need stabilization.");
            return;
        }

        //Adjacency is enforced by ActionDefinition range (minRange: 1, maxRange: 1)
        //but we double-check here for safety
        if (!DistanceUtility.IsAdjacent(ctx.attacker.position, ctx.finalTarget.position))
        {
            CombatLog.Add($"{ctx.attacker.unitName} must be adjacent to treat {ctx.finalTarget.unitName}.");
            return;
        }

        var context = new SkillCheckContext
        {
            checkId = SkillCheckIds.TreatInjuryStabilize,
            attacker = ctx.attacker,
            defender = ctx.finalTarget,
            callerContext = SkillCheckContext.CallerContext.Medical,
            showPreview = false,
            onResult = result =>
            {
                if (result.success)
                {
                    CombatLog.Add($"{ctx.attacker.unitName} stabilizes {ctx.finalTarget.unitName}.");
                    if (!resolver.IsSimulation)
                    {
                        ctx.finalTarget.RemoveEffect<DyingState>();
                        ctx.finalTarget.AddEffect(new StabilizedState());
                    }
                }
                else
                {
                    CombatLog.Add($"{ctx.attacker.unitName} fails to stabilize {ctx.finalTarget.unitName} (Treat Injury DC 15).");
                }
            }
        };

        SkillCheckExecutor.Execute(context);
    }
}
