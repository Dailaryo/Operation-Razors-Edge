using UnityEngine;

public class ShieldProtocolActionEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.finalTarget == null) return;

        if (!resolver.IsSimulation)
        {
            ctx.finalTarget.AddEffect(new ShieldProtocolEffect());
            CombatLog.Add($"{ctx.attacker.unitName}: Shield Protocol on {ctx.finalTarget.unitName}.");
        }
    }
}