using UnityEngine;

public class HealEffect : IActionEffect
{
    private int diceCount;
    private int diceSides;
    private StatType scalingStat;

    public HealEffect(int diceCount, int diceSides, StatType scalingStat)
    {
        this.diceCount = diceCount;
        this.diceSides = diceSides;
        this.scalingStat = scalingStat;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.finalTarget == null) return;

        if (ctx.finalTarget.IsDead())
        {
            CombatLog.Add($"{ctx.finalTarget.unitName} cannot be healed — already dead.");
            return;
        }

        int roll = 0;
        for (int i = 0; i < diceCount; i++)
            roll += Random.Range(1, diceSides + 1);

        int bonus = ctx.attacker.GetModifier(scalingStat);
        int total = Mathf.Max(1, roll + bonus);

        if (!resolver.IsSimulation)
        {
            ctx.finalTarget.currentHP = Mathf.Min(
                ctx.finalTarget.currentHP + total,
                ctx.finalTarget.GetMaxHP());

            ctx.finalTarget.OnHealthChanged?.Invoke(ctx.finalTarget);
            CombatLog.Add($"{ctx.attacker.unitName} patches {ctx.finalTarget.unitName} for {total} HP.");

            // Stabilize dying units even if HP stays at or below 0
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