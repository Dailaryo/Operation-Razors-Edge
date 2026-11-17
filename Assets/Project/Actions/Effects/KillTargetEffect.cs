using UnityEngine;

//Death effects: the target dies outright (Recall Death on a failed save)
public class KillTargetEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.finalTarget == null || resolver.IsUnitDead(ctx.finalTarget)) return;

        if (!resolver.IsSimulation)
            CombatLog.Add($"{ctx.finalTarget.unitName} succumbs to {ctx.definition.DisplayName}.");

        resolver.KillUnit(ctx.finalTarget);
    }
}
