using UnityEngine;
using System.Collections.Generic;

public class HighCommandEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation) return;

        ctx.attacker.SetCooldown(ctx.definition, Unit.CombatCooldown);
        ctx.attacker.AddEffect(new HighCommandActiveEffect());

        // Trigger NeuralSync for free
        new NeuralSyncActionEffect().Execute(ctx, resolver);

        CombatLog.Add($"{ctx.attacker.unitName}: HIGH COMMAND — all Vanguard systems at maximum.");
    }
}