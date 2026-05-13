using UnityEngine;
using System.Collections.Generic;

public class DiagnoseWeaknessEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.finalTarget == null) return;

        if (!resolver.IsSimulation)
        {
            ctx.finalTarget.AddEffect(new DiagnosedEffect());

            // Signal Scramble 1 — add Accuracy penalty
            bool hasScramble1 = ctx.attacker.sourceCharacterData != null &&
                ctx.attacker.sourceCharacterData.unlockedPassiveIds
                    .Contains(PassiveIds.SignalScramble1);

            bool hasScramble2 = ctx.attacker.sourceCharacterData != null &&
                ctx.attacker.sourceCharacterData.unlockedPassiveIds
                    .Contains(PassiveIds.SignalScramble2);

            if (hasScramble2)
                ctx.finalTarget.AddEffect(new SignalScrambleEffect(2));
            else if (hasScramble1)
                ctx.finalTarget.AddEffect(new SignalScrambleEffect(1));

            // Relay Network — affect all enemies in 3-tile radius
            bool hasRelay = ctx.attacker.sourceCharacterData != null &&
                ctx.attacker.sourceCharacterData.unlockedPassiveIds
                    .Contains(PassiveIds.RelayNetwork);

            if (hasRelay)
            {
                foreach (Unit unit in resolver.GetAllUnits())
                {
                    if (resolver.IsUnitDead(unit)) continue;
                    if (unit.isEnemy == ctx.attacker.isEnemy) continue;
                    if (unit == ctx.finalTarget) continue;

                    int dist = DistanceUtility.MovementCostDistance(
                        resolver.GetPosition(ctx.finalTarget),
                        resolver.GetPosition(unit));

                    if (dist <= 3)
                    {
                        unit.AddEffect(new DiagnosedEffect());
                        CombatLog.Add($"{unit.unitName} caught in Relay Network diagnosis.");
                    }
                }
            }

            CombatLog.Add($"{ctx.attacker.unitName} diagnoses {ctx.finalTarget.unitName}.");
        }
    }
}