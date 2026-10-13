using UnityEngine;

public enum PsionicSaveOutcome { NoSave, Failed, Saved, Resisted, Immune }

public static class PsionicSaveResolver
{
    public static PsionicSaveOutcome Resolve(ActionContext ctx, Unit target, CombatResolver resolver,
        int saveModifier = 0, bool allowSave = true)
    {
        var power = ctx.definition as PsionicPowerDefinition;
        if (power == null || target == null)
        {
            Debug.LogWarning("[PsionicSaveResolver] Called without a psionic power or target");
            return PsionicSaveOutcome.Failed;
        }

        bool log = !resolver.IsSimulation;

        //Allies (and self) accept the power: no PR, no save
        if (target.isEnemy == ctx.attacker.isEnemy) return PsionicSaveOutcome.NoSave;

        var descriptors = power.descriptors | EffectDescriptor.Psionic;
        int sourceLevel = ctx.attacker.sourceCharacterData?.level ?? 0;

        //Immediate-action defenses resolve before immunity, power resistance and the save
        if ((descriptors & EffectDescriptor.MindAffecting) != 0)
            resolver.TryFireReaction(target, ReactionTrigger.TargetedByMindAffecting, ctx.attacker);

        if (target.IsImmuneTo(descriptors, sourceLevel, consume: !resolver.IsSimulation))
        {
            if (log) CombatLog.Add($"{target.unitName} is immune to {power.actionID}.");
            return PsionicSaveOutcome.Immune;
        }

        if (power.allowsPowerResistance &&
            !PsionicResistanceCalculator.CheckPenetration(ctx.attacker, target, log, descriptors))
            return PsionicSaveOutcome.Resisted;

        if (!allowSave || !power.saveType.HasValue) return PsionicSaveOutcome.NoSave;

        int dc = power.GetSaveDC(ctx.attacker);
        int total = target.RollSave(power.saveType.Value, out int natural, descriptors) + saveModifier;
        bool saved = natural == 20 || (natural != 1 && total >= dc);

        if (log)
            CombatLog.Add($"{target.unitName} {power.saveType.Value} save vs {power.actionID}: " +
                          $"{total} (nat {natural}) vs DC {dc} — {(saved ? "Saved" : "Failed")}");

        if (!saved && !resolver.IsSimulation)
            target.NotifySaveFailed(power.saveType.Value, descriptors);

        if (saved && target.NegatesOnSuccessfulSave(descriptors))
        {
            if (log) CombatLog.Add($"{target.unitName} shrugs off {power.actionID} entirely.");
            return PsionicSaveOutcome.Immune;
        }

        return saved ? PsionicSaveOutcome.Saved : PsionicSaveOutcome.Failed;
    }
}