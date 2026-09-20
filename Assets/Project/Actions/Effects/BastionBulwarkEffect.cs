using UnityEngine;

public class BastionBulwarkEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null || ctx.finalTarget == null) return;
        if (resolver.IsSimulation) return;

        // Remove any existing Bulwark marker from prior targets
        foreach (var unit in resolver.GetAllUnits())
            unit.RemoveEffect<BulwarkMarkerEffect>();

        // Mark the new protected ally
        ctx.finalTarget.AddEffect(new BulwarkMarkerEffect(ctx.attacker));

        CombatLog.Add($"{ctx.attacker.unitName} takes up Bulwark stance protecting " +
                      $"{ctx.finalTarget.unitName}.");
    }
}
