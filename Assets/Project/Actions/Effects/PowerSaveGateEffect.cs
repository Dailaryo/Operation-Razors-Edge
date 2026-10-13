using UnityEngine;
using System.Collections.Generic;

public class PowerSaveGateEffect : IActionEffect
{
    private List<IActionEffect> onAffected;
    private List<IActionEffect> onSaved;

    public PowerSaveGateEffect(List<IActionEffect> onAffected, List<IActionEffect> onSaved = null)
    {
        this.onAffected = onAffected ?? new List<IActionEffect>();
        this.onSaved = onSaved ?? new List<IActionEffect>();
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.finalTarget == null) return;

        switch (PsionicSaveResolver.Resolve(ctx, ctx.finalTarget, resolver))
        {
            case PsionicSaveOutcome.Failed:
            case PsionicSaveOutcome.NoSave:
                foreach (var effect in onAffected) effect.Execute(ctx, resolver);
                break;

            case PsionicSaveOutcome.Saved:
                foreach (var effect in onSaved) effect.Execute(ctx, resolver);
                break;
        }
    }
}
