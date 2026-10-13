using UnityEngine;

public class CMBattlefieldPatchEffect : IActionEffect
{
    // Free action 1/round, touch range, 1d4 + INS mod HP, no medkit required.
    // "1/round" limit is enforced by ActionEconomyResolver via Free action category.

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null || ctx.finalTarget == null) return;

        if (ctx.finalTarget.IsDead())
        {
            CombatLog.Add($"{ctx.finalTarget.unitName} cannot be patched — already dead.");
            return;
        }

        if (!DistanceUtility.IsAdjacent(ctx.attacker.position, ctx.finalTarget.position))
        {
            CombatLog.Add($"{ctx.attacker.unitName} must be adjacent for Battlefield Patch.");
            return;
        }

        int roll  = Random.Range(1, 5); // 1d4
        int bonus = ctx.attacker.GetModifier(StatType.Insight);
        int total = Mathf.Max(1, roll + bonus);

        CombatLog.Add($"{ctx.attacker.unitName} applies a battlefield patch to " +
                      $"{ctx.finalTarget.unitName} for {total} HP.");

        if (!resolver.IsSimulation)
        {
            ctx.finalTarget.currentHP = Mathf.Min(
                ctx.finalTarget.currentHP + total,
                ctx.finalTarget.GetMaxHP());

            ctx.finalTarget.OnHealthChanged?.Invoke(ctx.finalTarget);

            if (ctx.finalTarget.IsDying() || ctx.finalTarget.IsDisabled())
            {
                if (ctx.finalTarget.currentHP > 0)
                {
                    ctx.finalTarget.RemoveEffect<DyingState>();
                    ctx.finalTarget.RemoveEffect<DisabledState>();
                    ctx.finalTarget.RemoveEffect<StabilizedState>();
                    CombatLog.Add($"{ctx.finalTarget.unitName} is back on their feet.");
                }
                else
                {
                    ctx.finalTarget.RemoveEffect<DyingState>();
                    ctx.finalTarget.RemoveEffect<DisabledState>();
                    if (!ctx.finalTarget.activeEffects.Exists(e => e is StabilizedState))
                        ctx.finalTarget.AddEffect(new StabilizedState());
                    CombatLog.Add($"{ctx.finalTarget.unitName} is stabilized.");
                }
            }
        }
    }
}
