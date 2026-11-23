using UnityEngine;

//Psychic Crush I–V (PF): Focus save first; on a failed Focus, a Resolve save at the same DC.
//Failed both: the target drops to -1 HP and is dying.
//Saved: damage by tier (never below -1 HP) and sickened 1 round.
//  Focus saved: I–II none, III 1/ML, IV 9d6 + ML, V 11d6 + ML.
//  Resolve saved: (2 x tier + 1)d6 + ML.
//Resolve bonus: I +4 above half HP; II +4 at full / +2 above half; III +2 at full; IV–V none.
//IV–V: automatically fail the Resolve save below half HP. V: -2 on the Focus save below half HP.
public class PsychicCrushEffect : IActionEffect
{
    private int tier;

    public PsychicCrushEffect(int tier)
    {
        this.tier = tier;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        Unit target = ctx.finalTarget;
        if (target == null || resolver.IsUnitDead(target)) return;

        var power = (PsionicPowerDefinition)ctx.definition;
        bool log = !resolver.IsSimulation;

        int hp = resolver.GetCurrentHP(target);
        int maxHP = target.GetMaxHP();
        bool fullHP = hp >= maxHP;
        bool aboveHalf = hp * 2 > maxHP;
        bool belowHalf = hp * 2 < maxHP;

        int focusModifier = (tier >= 5 && belowHalf) ? -2 : 0;

        switch (PsionicSaveResolver.Resolve(ctx, target, resolver, focusModifier))
        {
            case PsionicSaveOutcome.Saved:
                ApplySavedResult(ctx, resolver, target, focusSaved: true);
                return;

            case PsionicSaveOutcome.Failed:
            case PsionicSaveOutcome.NoSave:
                break;

            default:
                return; //Resisted or Immune
        }

        //Failed Focus: Resolve save at the same DC
        bool resolveSaved;
        if (tier >= 4 && belowHalf)
        {
            resolveSaved = false;
            if (log) CombatLog.Add($"{target.unitName} is below half health and cannot resist the crush.");
        }
        else
        {
            int dc = power.GetSaveDC(ctx.attacker);
            int bonus = GetResolveBonus(fullHP, aboveHalf);
            var total = target.RollSave(Unit.SaveType.Resolve, out int natural,
                power.descriptors | EffectDescriptor.Psionic, log) + bonus;
            resolveSaved = natural == 20 || (natural != 1 && total >= dc);

            if (log)
                CombatLog.Add($"{target.unitName} Resolve save vs {power.DisplayName}: " +
                              $"{total} (nat {natural}, +{bonus}) vs DC {dc} — {(resolveSaved ? "Saved" : "Failed")}");
        }

        if (resolveSaved)
        {
            ApplySavedResult(ctx, resolver, target, focusSaved: false);
            return;
        }

        if (log) CombatLog.Add($"{target.unitName}'s mind is crushed!");
        resolver.DropToDying(target);
    }

    private int GetResolveBonus(bool fullHP, bool aboveHalf)
    {
        switch (tier)
        {
            case 1: return aboveHalf ? 4 : 0;
            case 2: return fullHP ? 4 : (aboveHalf ? 2 : 0);
            case 3: return fullHP ? 2 : 0;
            default: return 0;
        }
    }

    private void ApplySavedResult(ActionContext ctx, CombatResolver resolver, Unit target, bool focusSaved)
    {
        int ml = PsionicPowerDefinition.GetManifesterLevel(ctx.attacker);
        bool log = !resolver.IsSimulation;
        string label = ctx.definition.DisplayName;

        int damage;
        if (focusSaved && tier <= 2) damage = 0;
        else if (focusSaved && tier == 3)
        {
            damage = ml;
            if (log) CombatLog.Add($"{ctx.attacker.unitName}'s {label} deals ML {ml} damage (Focus saved).");
        }
        else damage = PowerDice.Roll(ctx, 2 * tier + 1, 6, flatBonus: ml, flatLabel: "ML");

        //Thought Shield IV+: a successful save against Psychic Crush deals no damage
        if (damage > 0 && ThoughtShieldEffect.BlocksPsychicCrushDamage(target))
        {
            if (log) CombatLog.Add($"{target.unitName}'s Thought Shield blocks {label}'s damage.");
            damage = 0;
        }

        if (damage > 0)
        {
            damage = PowerDice.ReduceMindAffecting(target, damage, log);

            //Never below -1 HP
            int cap = Mathf.Max(0, resolver.GetCurrentHP(target) + 1);
            if (damage > cap)
            {
                if (log) CombatLog.Add($"{label} can't drop {target.unitName} below -1 HP: {damage} damage capped at {cap}.");
                damage = cap;
            }

            if (damage > 0)
            {
                ctx.baseDamage = damage;
                ctx.modifiedDamage = damage;
                resolver.ApplyFinalResult(ctx);
            }
        }

        if (!resolver.IsSimulation && !resolver.IsUnitDead(target))
            target.AddEffect(new SickenedEffect(1));
    }
}
