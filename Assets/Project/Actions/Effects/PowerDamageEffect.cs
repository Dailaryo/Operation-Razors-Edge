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

        int total = flatBonus + PowerDice.Roll(ctx, diceCount, diceSides);

        var power = ctx.definition as PsionicPowerDefinition;
        if (power != null && (power.descriptors & EffectDescriptor.MindAffecting) != 0)
            total = ctx.finalTarget.ApplyMindAffectingDamageReduction(total);

        ctx.baseDamage = total;
        ctx.modifiedDamage = total;
        resolver.ApplyFinalResult(ctx);
    }
}
