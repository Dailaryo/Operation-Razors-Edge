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
        //Planning simulations replay this effect on the real unit: restore, cooldown and log only when it resolves
        if (resolver.IsSimulation) return;

        ctx.attacker.resource?.Restore(amount);
        ctx.attacker.SetCooldown(ctx.definition, Unit.CombatCooldown);

        CombatLog.Add($"{ctx.attacker.unitName} restores {amount} {ctx.attacker.resource?.definition.displayName}.");
    }
}
