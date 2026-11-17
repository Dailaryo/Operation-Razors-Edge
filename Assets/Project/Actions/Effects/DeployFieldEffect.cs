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
            //Simulations (highlights, validation, AI scoring) replay this many times per click; log only the real action
            if (!resolver.IsSimulation)
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

        if (!resolver.IsSimulation)
            CombatLog.Add($"{ctx.attacker.unitName} deploys a field zone at {ctx.action.movePosition.Value}.");
    }
}