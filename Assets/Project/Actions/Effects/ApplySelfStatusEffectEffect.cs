using UnityEngine;

public class ApplySelfStatusEffectEffect : IActionEffect
{
    private System.Func<Unit, StatusEffect> effectFactory;

    public ApplySelfStatusEffectEffect(System.Func<Unit, StatusEffect> factory)
    {
        effectFactory = factory;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.attacker == null || resolver.IsSimulation) return;

        var applied = ctx.attacker.AddEffect(effectFactory(ctx.attacker));
        if (applied != null) ctx.appliedEffects.Add(applied);
    }
}