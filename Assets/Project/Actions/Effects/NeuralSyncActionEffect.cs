using UnityEngine;

public class NeuralSyncActionEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation) return;

        bool hasDistributed = ctx.attacker.activeEffects
            .Exists(e => e is DistributedProcessingEffect);

        bool hasTacticalRelay = ctx.attacker.activeEffects
            .Exists(e => e is TacticalRelayEffect);

        foreach (Unit unit in resolver.GetAllUnits())
        {
            if (resolver.IsUnitDead(unit)) continue;
            if (unit.isEnemy != ctx.attacker.isEnemy) continue;

            int dist = DistanceUtility.MovementCostDistance(
                ctx.attacker.position, resolver.GetPosition(unit));

            if (dist > 6) continue;

            unit.AddEffect(new FullSpectrumEffect(hasDistributed ? 1 : 0));

            if (hasTacticalRelay)
                unit.AddEffect(new TacticalRelayBuffEffect());
        }

        CombatLog.Add($"{ctx.attacker.unitName} activates Neural Sync.");
    }
}