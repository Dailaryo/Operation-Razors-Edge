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
        if (ctx.attacker == null) return;

        var effect = effectFactory(ctx.attacker);
        ctx.attacker.AddEffect(effect);
    }
}