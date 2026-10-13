using UnityEngine;

//Applies a status effect built from the full action context (manifester, ML, DC) to the final target
public class ApplyContextStatusEffect : IActionEffect
{
    private System.Func<ActionContext, StatusEffect> factory;

    public ApplyContextStatusEffect(System.Func<ActionContext, StatusEffect> factory)
    {
        this.factory = factory;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.finalTarget == null || resolver.IsSimulation) return;
        if (resolver.IsUnitDead(ctx.finalTarget)) return;

        var applied = ctx.finalTarget.AddEffect(factory(ctx));
        if (applied != null) ctx.appliedEffects.Add(applied);
    }
}
