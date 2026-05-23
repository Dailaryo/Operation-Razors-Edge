using UnityEngine;

public class OverdriveProtocolActionEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (!resolver.IsSimulation)
        {
            // Grant an additional Standard action this turn
            // Implemented as unlocking a second RangedAttack via a temporary effect
            ctx.attacker.AddEffect(new OverdriveEffect());
            ctx.attacker.SetCooldown(ctx.definition, Unit.-1);
            CombatLog.Add($"{ctx.attacker.unitName} enters Overdrive.");
        }
    }
}