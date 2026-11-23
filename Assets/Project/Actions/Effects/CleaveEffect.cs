using UnityEngine;
using System.Collections.Generic;

//Pathfinder Cleave: on a hit, one extra melee attack against a different enemy adjacent to the first target and within
//reach. Great Cleave: keep going while the attacks hit, each new target adjacent to the previous one, no enemy twice.
//Using Cleave costs -2 Defense through the attacker's next round
public class CleaveEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation || ctx.attacker == null) return;

        ctx.attacker.AddEffect(new StatBonusEffect(StatType.Defense, -2, "Cleave", StatModifier.BonusType.Untyped) { duration = 1 });
        CombatLog.Add($"{ctx.attacker.unitName} cleaves: -2 Defense until the end of their next round.");

        //Runs after the hit's damage is applied (CombatResolver.ExecuteAction); a miss never gets here
        ctx.onHitResolved += hitCtx => Chain(hitCtx, resolver);
    }

    static void Chain(ActionContext first, CombatResolver resolver)
    {
        Unit attacker = first.attacker;
        bool great = attacker.sourceCharacterData != null &&
                     attacker.sourceCharacterData.equippedFeatIds.Contains(FeatIds.GreatCleave);

        var struck = new HashSet<Unit> { first.finalTarget };
        Unit previous = first.finalTarget;
        var followUpDef = ActionDatabase.Get(ActionIds.MeleeAttack);

        while (attacker.CanAct())
        {
            Unit next = FindNext(attacker, previous, struck, resolver);
            if (next == null)
            {
                CombatLog.Add($"{attacker.unitName}'s Cleave: no other enemy adjacent to {previous.unitName} within reach.");
                return;
            }

            struck.Add(next);
            CombatLog.Add($"{attacker.unitName} cleaves into {next.unitName}!");

            bool hit = resolver.ExecuteFollowUpAttack(attacker, next, followUpDef,
                DevastatingDice(attacker), "Devastating Cleave");
            if (!great || !hit) return;

            previous = next;
        }
    }

    //Devastating Cleave (Helix Berserker): Cleave and Rampage follow-up attacks roll one extra damage die
    public static int DevastatingDice(Unit attacker) =>
        attacker.HasPassive(PassiveIds.HelixDevastatingCleave) ? 1 : 0;

    //Living enemy adjacent to the previous target and to the attacker (reach 1), not struck yet; lowest HP first
    static Unit FindNext(Unit attacker, Unit previous, HashSet<Unit> struck, CombatResolver resolver)
    {
        Vector2Int attackerPos = resolver.GetPosition(attacker);
        Vector2Int previousPos = resolver.GetPosition(previous);
        Unit best = null;

        foreach (var unit in resolver.GetAllUnits())
        {
            if (unit.isEnemy == attacker.isEnemy || struck.Contains(unit) || resolver.IsUnitDead(unit)) continue;

            Vector2Int pos = resolver.GetPosition(unit);
            if (!DistanceUtility.IsAdjacent(previousPos, pos) || !DistanceUtility.IsAdjacent(attackerPos, pos)) continue;

            if (best == null || resolver.GetCurrentHP(unit) < resolver.GetCurrentHP(best)) best = unit;
        }

        return best;
    }
}
