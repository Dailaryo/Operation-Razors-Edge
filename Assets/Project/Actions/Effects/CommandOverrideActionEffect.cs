using UnityEngine;

public class CommandOverrideActionEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.finalTarget == null) return;
        if (resolver.IsSimulation) return;

        ctx.attacker.SetCooldown(ctx.definition, Unit.CombatCooldown);

        //The bonus Standard comes in the target's next planning phase: this round's plans are already locked in
        ctx.finalTarget.AddEffect(new CommandOverrideBonusEffect());

        CombatLog.Add($"{ctx.attacker.unitName}: Command Override — {ctx.finalTarget.unitName} gains a bonus " +
                      $"Standard action next round.");
    }
}