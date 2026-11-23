using UnityEngine;

public class ArtilleristIndirectFireEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null || ctx.finalTarget == null) return;

        ctx.ignoresLoS = true;

        if (!resolver.IsSimulation)
            CombatLog.Add($"{ctx.attacker.unitName} fires Indirect Fire on " +
                          $"{ctx.finalTarget.unitName} — cover ignored.");
    }
}
