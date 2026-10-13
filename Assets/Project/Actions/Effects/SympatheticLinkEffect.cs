using System.Collections.Generic;
using UnityEngine;

//Empath Sympathetic Link: move one harmful condition from an ally within 60 ft. onto the target enemy.
//Focus save (DC 10 + 1/2 level + Presence) negates entirely: nothing moves, the use is still spent.
//The condition moves as it is: remaining duration, stacked amounts, and any concentration holding it.
//Until the selection UI exists, the default pick is the nearest ally, then the longest-lasting condition.
public class SympatheticLinkEffect : IActionEffect
{
    public const int Range = 12;  //60 ft.

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        var caster = ctx.attacker;
        var enemy = ctx.finalTarget;
        if (caster == null || enemy == null || resolver.IsSimulation) return;
        if (resolver.IsUnitDead(enemy)) return;

        Unit ally = ctx.action?.transferSource;
        StatusEffect condition = ctx.action?.transferEffect;

        //The planned pick may be gone by resolution (expired, ally moved or died)
        if (ally == null || condition == null || !CanTransfer(caster, ally, condition))
            (ally, condition) = FindDefault(caster, resolver.GetAllUnits());

        if (condition == null)
        {
            CombatLog.Add($"{caster.unitName} finds no harmful condition to draw out.");
            return;
        }

        int level = caster.sourceCharacterData?.level ?? 0;
        int dc = 10 + level / 2 + caster.GetModifier(StatType.Presence);
        var descriptors = EffectDescriptor.MindAffecting | condition.descriptors;

        resolver.TryFireReaction(enemy, ReactionTrigger.TargetedByMindAffecting, caster);

        if (enemy.IsImmuneTo(descriptors, level, consume: true))
        {
            CombatLog.Add($"{enemy.unitName} is unaffected by {caster.unitName}'s Sympathetic Link.");
            return;
        }

        int total = enemy.RollSave(Unit.SaveType.Focus, out int natural, descriptors);
        bool saved = natural == 20 || (natural != 1 && total >= dc);

        if (saved)
        {
            CombatLog.Add($"{enemy.unitName} resists {caster.unitName}'s Sympathetic Link ({total} vs DC {dc}).");
            return;
        }

        enemy.NotifySaveFailed(Unit.SaveType.Focus, descriptors);

        ally.DetachEffect(condition);
        if (enemy.AddEffect(condition) == null)
        {
            ally.AddEffect(condition); //blocked after all: it stays where it was
            return;
        }

        CombatLog.Add($"{caster.unitName} draws {condition.effectName} out of {ally.unitName} and into {enemy.unitName}.");
    }

    public static bool CanTransfer(Unit caster, Unit ally, StatusEffect condition)
    {
        if (ally.isEnemy != caster.isEnemy || ally.IsDead()) return false;
        if (DistanceUtility.MovementCostDistance(caster.position, ally.position) > Range) return false;

        return condition.isHarmful && !condition.ended && ally.activeEffects.Contains(condition);
    }

    //Nearest ally with a transferable condition; among that ally's conditions, the longest-lasting
    public static (Unit ally, StatusEffect condition) FindDefault(Unit caster, List<Unit> allUnits)
    {
        Unit bestAlly = null;
        StatusEffect best = null;
        int bestDistance = int.MaxValue;
        int bestRemaining = -1;

        foreach (var ally in allUnits)
        {
            foreach (var effect in ally.activeEffects)
            {
                if (!CanTransfer(caster, ally, effect)) continue;

                int distance = DistanceUtility.MovementCostDistance(caster.position, ally.position);
                int remaining = Remaining(effect);

                if (distance < bestDistance || (distance == bestDistance && remaining > bestRemaining))
                {
                    bestAlly = ally;
                    best = effect;
                    bestDistance = distance;
                    bestRemaining = remaining;
                }
            }
        }

        return (bestAlly, best);
    }

    private static int Remaining(StatusEffect effect)
    {
        if (effect.lastsUntilHub || effect.isPersistent || effect.maintainerCount > 0) return int.MaxValue;
        return effect.duration;
    }
}