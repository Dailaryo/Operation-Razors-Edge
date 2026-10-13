using UnityEngine;

public class ApplyStatusEffectActionEffect : IActionEffect
{
    private System.Func<Unit, StatusEffect> effectFactory;

    public ApplyStatusEffectActionEffect(System.Func<Unit, StatusEffect> factory)
    {
        effectFactory = factory;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.action.target == null) return;

        var target = ctx.action.target;

        var effect = effectFactory(target);

        if (!resolver.IsSimulation)
        {
            var applied = target.AddEffect(effect);
            if (applied != null) ctx.appliedEffects.Add(applied);
        }
    }
}
