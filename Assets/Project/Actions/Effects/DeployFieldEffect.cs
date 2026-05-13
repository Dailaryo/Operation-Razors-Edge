using System.Collections.Generic;
using UnityEngine;

public class DeployFieldEffect : IActionEffect
{
    private List<System.Func<StatusEffect>> effectFactories;

    public DeployFieldEffect(List<System.Func<StatusEffect>> effectFactories)
    {
        this.effectFactories = effectFactories;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (!ctx.action.movePosition.HasValue)
        {
            CombatLog.Add($"{ctx.attacker.unitName}: DeployField requires a tile target.");
            return;
        }

        if (!resolver.IsSimulation)
        {
            FieldZoneManager.PlaceZone(
                ctx.action.movePosition.Value,
                radius: 2,
                duration: 2,
                effectFactories);
        }

        CombatLog.Add($"{ctx.attacker.unitName} deploys a field zone at {ctx.action.movePosition.Value}.");
    }
}