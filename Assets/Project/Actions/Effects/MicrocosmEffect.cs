using UnityEngine;

//Microcosm (PF psychic 9), per target. MultiTargetEffect supplies the combined 30-HD budget.
//10 HD or fewer: no save.
//Failed save or no save: enemies are defeated; squad members are treated as dead (normal rollback).
//11–15 HD, saved: out of the encounter. Enemies are defeated; squad members are unavailable until the
//  Hub, with no rollback.
//16+ HD, saved: negated.
public class MicrocosmEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        Unit target = ctx.finalTarget;
        if (target == null || resolver.IsUnitDead(target)) return;

        int hd = target.sourceCharacterData != null ? target.sourceCharacterData.level : 0;
        bool log = !resolver.IsSimulation;

        switch (PsionicSaveResolver.Resolve(ctx, target, resolver, allowSave: hd > 10))
        {
            case PsionicSaveOutcome.Failed:
            case PsionicSaveOutcome.NoSave:
                if (log) CombatLog.Add($"{target.unitName}'s mind is sealed inside a microcosm.");
                resolver.KillUnit(target);
                return;

            case PsionicSaveOutcome.Saved:
                if (hd >= 16) return;
                if (log) CombatLog.Add($"{target.unitName} resists, but is lost to the fight.");
                resolver.RemoveFromEncounter(target);
                return;
        }
    }
}
