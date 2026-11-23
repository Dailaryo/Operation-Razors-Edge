using UnityEngine;

public class FPStimInjectionEffect : IActionEffect
{
    // 3/combat limit tracked via cooldown on the ActionDefinition.
    // Stat choice (which stat gets +2) requires a UI selection mechanism
    // not yet present for mid-action choices — stub with a fixed INS bonus
    // for now; expand when UI selection for action parameters is implemented.

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation) return;

        if (ctx.attacker == null || ctx.finalTarget == null) return;

        if (!DistanceUtility.IsAdjacent(ctx.attacker.position, ctx.finalTarget.position))
        {
            CombatLog.Add($"{ctx.attacker.unitName} must be adjacent for Stim Injection.");
            return;
        }

        CombatLog.Add($"{ctx.attacker.unitName} injects a combat stimulant into " +
                      $"{ctx.finalTarget.unitName} (+2 to a chosen stat for 3 rounds).");

        if (!resolver.IsSimulation)
        {
            // Placeholder: applies +2 Power for 3 rounds
            // Stat selection UI deferred — replace Power with player-chosen stat when available
            ctx.finalTarget.AddEffect(
                new StatBonusEffect(StatType.Power, 2, "StimInjection", StatModifier.BonusType.Morale)
                { duration = 3 });
        }
    }
}
