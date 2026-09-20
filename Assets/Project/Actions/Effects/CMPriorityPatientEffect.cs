using UnityEngine;

public class CMPriorityPatientEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null || ctx.finalTarget == null) return;

        CombatLog.Add($"{ctx.attacker.unitName} designates {ctx.finalTarget.unitName} as Priority Patient.");

        if (!resolver.IsSimulation)
        {
            // Remove any existing priority patient designation first
            ctx.attacker.RemoveEffect<CMPriorityPatientStatus>();
            // Apply new designation
            ctx.attacker.AddEffect(new CMPriorityPatientStatus(ctx.finalTarget));
        }
    }
}
