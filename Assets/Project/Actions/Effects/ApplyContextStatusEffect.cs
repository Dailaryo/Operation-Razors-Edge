using UnityEngine;

//Applies a status effect built from the full action context (manifester, ML, DC) to the final target
public class ApplyContextStatusEffect : IActionEffect
{
    private System.Func<ActionContext, CombatResolver, StatusEffect> factory;
    private bool logDuration;

    //logDuration: log the applied duration (rolled or level-based durations the player should see)
    public ApplyContextStatusEffect(System.Func<ActionContext, StatusEffect> factory, bool logDuration = false)
        : this((ctx, resolver) => factory(ctx), logDuration) { }

    //For effects that need the resolver when built (e.g. Inflict Pain keeps its CombatManager for later damage)
    public ApplyContextStatusEffect(System.Func<ActionContext, CombatResolver, StatusEffect> factory, bool logDuration = false)
    {
        this.factory = factory;
        this.logDuration = logDuration;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.finalTarget == null || resolver.IsSimulation) return;
        if (resolver.IsUnitDead(ctx.finalTarget)) return;

        StatusEffect effect = factory(ctx, resolver);
        int baseDuration = effect.duration; //before Unit.AddEffect extends it

        var applied = ctx.finalTarget.AddEffect(effect);
        if (applied == null) return;

        ctx.appliedEffects.Add(applied);

        if (logDuration && baseDuration > 0)
        {
            string rounds = effect.duration > baseDuration
                ? $"{effect.duration} rounds ({baseDuration} +1: no actions left this round)"
                : $"{baseDuration} rounds";

            CombatLog.Add($"{ctx.finalTarget.unitName}: {ActionDefinition.FormatActionName(applied.effectName)} " +
                          $"for {rounds}{(ctx.isMaximized ? " (maximized)" : "")}.");
        }
    }
}
