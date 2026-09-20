using UnityEngine;

public class CMReviveEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null || ctx.finalTarget == null) return;

        if (!ctx.finalTarget.IsDying() && !ctx.finalTarget.IsDisabled()
            && !ctx.finalTarget.IsStabilized())
        {
            CombatLog.Add($"{ctx.finalTarget.unitName} does not need reviving.");
            return;
        }

        if (!DistanceUtility.IsAdjacent(ctx.attacker.position, ctx.finalTarget.position))
        {
            CombatLog.Add($"{ctx.attacker.unitName} must be adjacent to revive {ctx.finalTarget.unitName}.");
            return;
        }

        CombatLog.Add($"{ctx.attacker.unitName} revives {ctx.finalTarget.unitName} to 1 HP.");

        if (!resolver.IsSimulation)
        {
            ctx.finalTarget.currentHP = 1;
            ctx.finalTarget.RemoveEffect<DyingState>();
            ctx.finalTarget.RemoveEffect<DisabledState>();
            ctx.finalTarget.RemoveEffect<StabilizedState>();
            ctx.finalTarget.OnHealthChanged?.Invoke(ctx.finalTarget);
        }
    }
}
