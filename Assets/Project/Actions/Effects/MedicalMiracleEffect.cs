using UnityEngine;

public class MedicalMiracleEffect : IActionEffect
{
    private const int DC          = 40;
    private const int FortDC      = 20;
    private const int RoundWindow = 18; // 3 minutes = 18 combat rounds

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null || ctx.finalTarget == null) return;

        if (!ctx.finalTarget.IsDead())
        {
            CombatLog.Add($"{ctx.finalTarget.unitName} is not dead — Medical Miracle has no target.");
            return;
        }

        if (!DistanceUtility.IsAdjacent(ctx.attacker.position, ctx.finalTarget.position))
        {
            CombatLog.Add($"{ctx.attacker.unitName} must be adjacent to attempt Medical Miracle.");
            return;
        }

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
                    CombatLog.Add($"{ctx.attacker.unitName} fails the Medical Miracle check " +
                                  $"({result.total} vs DC {DC}).");
                    return;
                }

                CombatLog.Add($"{ctx.attacker.unitName} succeeds the Medical Miracle check " +
                              $"({result.total} vs DC {DC})! {ctx.finalTarget.unitName} must make a Fortitude save...");

                int fortRoll  = Random.Range(1, 21);
                int fortTotal = fortRoll + ctx.finalTarget.GetDerivedStat(StatType.Fortitude);

                CombatLog.Add($"{ctx.finalTarget.unitName} Fortitude save: {fortRoll} + " +
                              $"{ctx.finalTarget.GetDerivedStat(StatType.Fortitude)} = {fortTotal} vs DC {FortDC}.");

                if (fortTotal < FortDC)
                {
                    CombatLog.Add($"{ctx.finalTarget.unitName} fails the Fortitude save — remains dead.");
                    return;
                }

                // Success — restore 1d6 HP
                if (!resolver.IsSimulation)
                {
                    int restored = Random.Range(1, 7);

                    ctx.finalTarget.currentHP = Mathf.Min(restored, ctx.finalTarget.maxHP);
                    ctx.finalTarget.sourceCharacterData.deathRound = -1;
                    ctx.finalTarget.RemoveEffect<DyingState>();
                    ctx.finalTarget.RemoveEffect<StabilizedState>();
                    ctx.finalTarget.OnHealthChanged?.Invoke(ctx.finalTarget);

                    CombatLog.Add($"{ctx.finalTarget.unitName} is brought back from death with {restored} HP!");
                }
            }
        };

        SkillCheckExecutor.Execute(context);
    }
}
