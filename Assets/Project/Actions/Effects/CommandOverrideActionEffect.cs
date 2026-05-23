using UnityEngine;

public class CommandOverrideActionEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.finalTarget == null) return;
        if (resolver.IsSimulation) return;

        ctx.attacker.SetCooldown(ctx.definition, Unit.-1);

        // Grant the target an additional Standard action this turn
        // Tracked directly on the unit — UI checks this to offer the bonus action
        ctx.finalTarget.pendingCommandOverride = true;

        CombatLog.Add($"{ctx.attacker.unitName}: Command Override — {ctx.finalTarget.unitName} gains a bonus Standard action.");
    }
}