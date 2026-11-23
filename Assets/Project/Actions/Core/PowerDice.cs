using UnityEngine;
using System.Collections.Generic;

//Variable dice for power effects. Maximize Power: every die rolls its maximum.
//Every roll is logged in real runs: "Anyha rolls Mind Thrust 3d6 (4, 2, 5): 11", and with a flat bonus
//"Anyha rolls Psychic Crush III 7d6 (...): 24 + ML 12 = 36". Planning, validation and AI rolls stay silent
public static class PowerDice
{
    //label: what is being rolled; defaults to the action's display name
    public static int Roll(ActionContext ctx, int count, int sides, string label = null,
                           int flatBonus = 0, string flatLabel = "bonus")
    {
        bool log = ctx != null && ctx.logRolls;
        string roller = ctx?.attacker != null ? ctx.attacker.unitName : "Unknown";
        label ??= ctx?.definition != null ? ctx.definition.DisplayName : "dice";

        return Roll(ctx != null && ctx.isMaximized, count, sides, roller, label, log, flatBonus, flatLabel);
    }

    //For rolls with no action context (e.g. Inflict Pain's end-phase repeats)
    public static int Roll(bool maximized, int count, int sides, string roller, string label, bool log,
                           int flatBonus = 0, string flatLabel = "bonus")
    {
        int roll = 0;
        var dice = new List<int>();

        if (maximized) roll = count * sides;
        else
        {
            for (int i = 0; i < count; i++)
            {
                int die = Random.Range(1, sides + 1);
                dice.Add(die);
                roll += die;
            }
        }

        int total = roll + flatBonus;

        if (log)
        {
            string detail = maximized ? "(maximized)" : $"({string.Join(", ", dice)})";
            string result = flatBonus != 0 ? $"{roll} + {flatLabel} {flatBonus} = {total}" : $"{total}";
            CombatLog.Add($"{roller} rolls {label} {count}d{sides} {detail}: {result}");
        }

        return total;
    }

    //Applies the target's mind-affecting damage reduction (Intellect Fortress II+) and logs it when it changed anything
    public static int ReduceMindAffecting(Unit target, int damage, bool log)
    {
        int reduced = target.ApplyMindAffectingDamageReduction(damage);

        if (log && reduced != damage)
        {
            var source = target.activeEffects.Find(e => e.MindAffectingDamageMultiplier < 1f);
            string name = source != null ? ActionDefinition.FormatActionName(source.effectName) : "Mind-affecting resistance";
            CombatLog.Add($"{target.unitName}'s {name} reduces mind-affecting damage: {damage} → {reduced}");
        }

        return reduced;
    }
}