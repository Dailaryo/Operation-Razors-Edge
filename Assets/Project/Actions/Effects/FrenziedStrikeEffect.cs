using UnityEngine;
using System.Collections.Generic;

//Frenzied Strike (Helix Berserker L16): full-round action. One melee attack against every enemy adjacent to the
//attacker (lowest HP first; each is a follow-up Melee Attack, so no provocation), and -2 Defense until the end of the
//attacker's next round. Each attack resolves on its own: Killing Blow, Savage Momentum and Rampage apply per attack
public class FrenziedStrikeEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation || ctx.attacker == null) return;

        Unit attacker = ctx.attacker;

        attacker.AddEffect(new StatBonusEffect(StatType.Defense, -2, "FrenziedStrike") { duration = 1 });
        CombatLog.Add($"{attacker.unitName} attacks in a frenzy: -2 Defense until the end of their next round.");

        Vector2Int attackerPos = resolver.GetPosition(attacker);
        var targets = new List<Unit>();
        foreach (var unit in resolver.GetAllUnits())
        {
            if (unit.isEnemy == attacker.isEnemy || resolver.IsUnitDead(unit) || !unit.CanAct()) continue;
            if (DistanceUtility.IsAdjacent(attackerPos, resolver.GetPosition(unit))) targets.Add(unit);
        }

        if (targets.Count == 0)
        {
            CombatLog.Add($"{attacker.unitName}'s Frenzied Strike: no enemies within reach.");
            return;
        }

        targets.Sort((a, b) => resolver.GetCurrentHP(a).CompareTo(resolver.GetCurrentHP(b)));
        var meleeAttack = ActionDatabase.Get(ActionIds.MeleeAttack);

        foreach (var target in targets)
        {
            if (!attacker.CanAct()) break;                                   //dropped by a reaction mid-sequence
            if (resolver.IsUnitDead(target) || !target.CanAct()) continue;   //already dropped (e.g. by Rampage)

            resolver.ExecuteFollowUpAttack(attacker, target, meleeAttack);
        }
    }
}
