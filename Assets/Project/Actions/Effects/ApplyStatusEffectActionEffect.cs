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

        effect.OnApply(target);

        if(!resolver.IsSimulation)
        {
            target.AddEffect(effect);
        }
    }
}
