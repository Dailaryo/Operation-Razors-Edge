using UnityEngine;

//Extra damage dice on an attack (Brutal Strike +1d6). Added with the attack's other damage before the hit roll,
//like ScalingDamageEffect, so a miss deals none of it. Shown as its own part in the damage breakdown
public class BonusDamageDiceEffect : IActionEffect
{
    private int diceCount;
    private int diceSides;
    private string label;

    public BonusDamageDiceEffect(int diceCount, int diceSides, string label)
    {
        this.diceCount = diceCount;
        this.diceSides = diceSides;
        this.label = label;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        int roll = 0;
        for (int i = 0; i < diceCount; i++)
            roll += ctx.isMaximized ? diceSides : Random.Range(1, diceSides + 1);

        ctx.modifiedDamage += roll;
        ctx.AddDamagePart($"{label} {diceCount}d{diceSides}{(ctx.isMaximized ? " maximized" : "")}", roll);
    }
}
