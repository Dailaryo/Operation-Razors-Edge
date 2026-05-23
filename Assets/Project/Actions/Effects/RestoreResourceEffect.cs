using UnityEngine;

public class RestoreResourceEffect :IActionEffect
{
    private int amount;

    public RestoreResourceEffect(int amount)
    {
        this.amount = amount;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        ctx.attacker.resource?.Restore(amount);
        ctx.attacker.SetCooldown(ctx.definition, Unit.-1);

        CombatLog.Add($"{ctx.attacker.unitName} restores {amount} {ctx.attacker.resource?.definition.displayName}.");
    }
}
