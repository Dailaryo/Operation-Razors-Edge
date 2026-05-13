using UnityEngine;

public class CoordinatedFireEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.finalTarget == null) return;

        bool diagnosed = ctx.finalTarget.activeEffects
            .Exists(e => e is DiagnosedEffect);

        if (!diagnosed)
        {
            CombatLog.Add($"{ctx.attacker.unitName}: Coordinated Fire requires a Diagnosed target.");
            return;
        }

        if (!resolver.IsSimulation)
        {
            ctx.attacker.AddEffect(new CoordinatedFireMarkerEffect(ctx.finalTarget));
            CombatLog.Add($"{ctx.attacker.unitName}: Coordinated Fire — next ally to attack {ctx.finalTarget.unitName} gains bonus.");
        }
    }
}