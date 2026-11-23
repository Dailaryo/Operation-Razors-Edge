using UnityEngine;

//Dice damage with no attack roll (powers). Routes through ApplyFinalResult so simulation,
//massive damage and death handling match weapon damage.
public class PowerDamageEffect : IActionEffect
{
    private int diceCount;
    private int diceSides;
    private int flatBonus;

    public PowerDamageEffect(int diceCount, int diceSides, int flatBonus = 0)
    {
        this.diceCount = diceCount;
        this.diceSides = diceSides;
        this.flatBonus = flatBonus;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.finalTarget == null) return;

        int total = PowerDice.Roll(ctx, diceCount, diceSides, flatBonus: flatBonus);

        var power = ctx.definition as PsionicPowerDefinition;
        if (power != null && (power.descriptors & EffectDescriptor.MindAffecting) != 0)
            total = PowerDice.ReduceMindAffecting(ctx.finalTarget, total, !resolver.IsSimulation);

        ctx.baseDamage = total;
        ctx.modifiedDamage = total;
        resolver.ApplyFinalResult(ctx);
    }
}
