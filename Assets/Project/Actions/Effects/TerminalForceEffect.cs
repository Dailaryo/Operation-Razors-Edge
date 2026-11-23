using System.Collections.Generic;
using UnityEngine;

public class TerminalForceEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (ctx.finalTarget == null || ctx.attacker == null) return;

        if (resolver.IsSimulation) return;

        ctx.attacker.SetCooldown(ctx.definition, 3);

        int bonusDamage = 0;

        for (int i = 0; i < 3; i++)
        {
            if (resolver.IsUnitDead(ctx.finalTarget)) break;

            int roll = Random.Range(1, 21);
            int accuracy = ctx.attacker.GetAccuracy(false);
            int attackTotal = roll + accuracy + ctx.attacker.GetBAB(); //d20 to-hit, same as ResolveHit and Volley Fire
            int defense = ctx.finalTarget.GetDefense();

            if (attackTotal >= defense)
            {
                int damage = Random.Range(1, 9) + bonusDamage; // d8 base
                ctx.finalTarget.TakeDamage(damage);
                CombatLog.Add($"{ctx.attacker.unitName}: Terminal Force hit {i + 1} — {damage} damage.");
                bonusDamage += Random.Range(1, 7); // +1d6 per hit after first
            }
            else
            {
                CombatLog.Add($"{ctx.attacker.unitName}: Terminal Force hit {i + 1} — missed.");
            }
        }
    }
}