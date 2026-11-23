using UnityEngine;

public class CoordinatedFireEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation) return;

        if (ctx.finalTarget == null) return;

        bool diagnosed = ctx.finalTarget.activeEffects
            .Exists(e => e is DiagnosedEffect);

        if (!diagnosed)
        {
            CombatLog.Add($"{ctx.attacker.unitName}: Coordinated Fire requires a Diagnosed target.");
            return;
        }

        //The mark sits on the target, so the attacking ally's resolver finds it (CollectEffects reads attacker and target)
        ctx.finalTarget.AddEffect(new CoordinatedFireMarkerEffect(ctx.attacker));
        CombatLog.Add($"{ctx.attacker.unitName}: Coordinated Fire on {ctx.finalTarget.unitName} — the next ally attack " +
                      $"gets +{CoordinatedFireMarkerEffect.AccuracyBonus} to hit and +1d6 damage.");
    }
}