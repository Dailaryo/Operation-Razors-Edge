using UnityEngine;

public class MinorMedicalMiracleEffect : IActionEffect
{
    private const int DC           = 30;
    private const int FortDC       = 15;
    private const int RoundWindow  = 3;

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        //Rolls and results happen once, in the execution phase; planning replays (move highlights, validation) skip them
        if (resolver.IsSimulation) return;

        if (ctx.attacker == null || ctx.finalTarget == null) return;

        // Target must be dead
        if (!ctx.finalTarget.IsDead())
        {
            CombatLog.Add($"{ctx.finalTarget.unitName} is not dead — Minor Medical Miracle has no target.");
            return;
        }

        // Adjacency check
        if (!DistanceUtility.IsAdjacent(ctx.attacker.position, ctx.finalTarget.position))
        {
            CombatLog.Add($"{ctx.attacker.unitName} must be adjacent to attempt Minor Medical Miracle.");
            return;
        }

        // Window check — target must have died within the last 3 rounds
        var targetData = ctx.finalTarget.sourceCharacterData;
        if (targetData == null || targetData.deathRound < 0)
        {
            CombatLog.Add($"{ctx.finalTarget.unitName} has no death record — cannot attempt miracle.");
            return;
        }

        int currentRound = ctx.action?.manager != null
            ? ctx.action.manager.GetCurrentRound()
            : -1;

        if (currentRound < 0 || (currentRound - targetData.deathRound) > RoundWindow)
        {
            CombatLog.Add($"Too much time has passed to save {ctx.finalTarget.unitName} " +
                          $"(died round {targetData.deathRound}, now round {currentRound}).");
            return;
        }

        // TreatInjury check DC 30 — no Take 10/20
        var context = new SkillCheckContext
        {
            checkId       = SkillCheckIds.TreatInjuryStabilize,
            attacker      = ctx.attacker,
            defender      = ctx.finalTarget,
            callerContext = SkillCheckContext.CallerContext.Medical,
            dcOverride    = DC,
            showPreview   = false,
            onResult      = result =>
            {
                if (!result.success)
                {
                    CombatLog.Add($"{ctx.attacker.unitName} fails the Minor Medical Miracle check " +
                                  $"({result.total} vs DC {DC}).");
                    return;
                }

                CombatLog.Add($"{ctx.attacker.unitName} succeeds the Minor Medical Miracle check " +
                              $"({result.total} vs DC {DC})! {ctx.finalTarget.unitName} must make a Fortitude save...");

                // Target makes Fortitude save DC 15
                int fortRoll  = Random.Range(1, 21);
                int fortTotal = fortRoll + ctx.finalTarget.GetDerivedStat(StatType.Fortitude);

                CombatLog.Add($"{ctx.finalTarget.unitName} Fortitude save: {fortRoll} + " +
                              $"{ctx.finalTarget.GetDerivedStat(StatType.Fortitude)} = {fortTotal} vs DC {FortDC}.");

                if (fortTotal < FortDC)
                {
                    CombatLog.Add($"{ctx.finalTarget.unitName} fails the Fortitude save — remains dead.");
                    return;
                }

                // Success — stabilize at 0 HP
                if (!resolver.IsSimulation)
                {
                    ctx.finalTarget.currentHP = 0;
                    ctx.finalTarget.sourceCharacterData.deathRound = -1;
                    ctx.finalTarget.RemoveEffect<DyingState>();
                    ctx.finalTarget.AddEffect(new StabilizedState());
                    ctx.finalTarget.OnHealthChanged?.Invoke(ctx.finalTarget);

                    CombatLog.Add($"{ctx.finalTarget.unitName} is miraculously stabilized at 0 HP!");
                }
            }
        };

        SkillCheckExecutor.Execute(context);
    }
}
