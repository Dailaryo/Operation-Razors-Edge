using UnityEngine;
using System.Collections.Generic;

//For a class action that works "as" a power (Empath Group Suggestion = Suggestion): rolls that power's immunity,
//power resistance and save against ctx.finalTarget. Inner effects run only on a failed save (or no save).
//The power's DC, save type and descriptors apply; the action itself still costs no PP
public class SaveAsPowerEffect : IActionEffect
{
    private string powerId;
    private List<IActionEffect> onAffected;

    public SaveAsPowerEffect(string powerId, List<IActionEffect> onAffected = null)
    {
        this.powerId = powerId;
        this.onAffected = onAffected ?? new List<IActionEffect>();
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.finalTarget == null || resolver.IsUnitDead(ctx.finalTarget)) return;

        var power = PowerDatabase.Get(powerId);
        if (power == null) return;

        //PsionicSaveResolver reads the power from ctx.definition: swap it in for the roll only
        var original = ctx.definition;
        ctx.definition = power;
        var outcome = PsionicSaveResolver.Resolve(ctx, ctx.finalTarget, resolver);
        ctx.definition = original;

        if (outcome == PsionicSaveOutcome.Failed || outcome == PsionicSaveOutcome.NoSave)
            foreach ( var effect in onAffected ) effect.Execute(ctx, resolver);
    }
}
