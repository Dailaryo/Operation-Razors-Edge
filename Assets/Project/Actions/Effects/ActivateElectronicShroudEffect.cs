using UnityEngine;

public class ActivateElectronicShroudEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation) return;

        // Check if already active this combat
        var existing = ctx.attacker.activeEffects
            .Find(e => e is ElectronicShroudEffect) as ElectronicShroudEffect;

        if (existing != null && existing.active)
        {
            CombatLog.Add($"{ctx.attacker.unitName}: Electronic Shroud already active.");
            return;
        }

        ctx.attacker.SetCooldown(ctx.definition, Unit.CombatCooldown);

        var shroud = new ElectronicShroudEffect();
        shroud.Activate();
        ctx.attacker.AddEffect(shroud);

        CombatLog.Add($"{ctx.attacker.unitName}: Electronic Shroud active — untargetable by ranged attacks for 1 round.");
    }
}