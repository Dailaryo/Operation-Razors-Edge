using UnityEngine;
using System.Collections.Generic;

public static class ActionEconomyResolver
{
    public static bool TryAssignCategories(
        Unit unit,
        List<CombatAction> existingActions,
        CombatAction newAction,
        Dictionary<ActionDefinition.ActionCategory, int> limits
    )
    {
        var def = newAction.definition;

        Dictionary<ActionDefinition.ActionCategory, int> usage =
            new Dictionary<ActionDefinition.ActionCategory, int>();

        foreach (var cat in limits.Keys)
        {
            usage[cat] = Count(existingActions, cat);
        }

        newAction.resolvedCategories.Clear();

        foreach (var required in def.consumesCategories)
        {
            var options = GetValidCategories(required);

            bool assigned = false;

            foreach (var option in options)
            {
                if (usage[option] < limits[option])
                {
                    usage[option]++;
                    newAction.resolvedCategories.Add(option);
                    assigned = true;
                    break;
                }
            }

            if (!assigned) return false;
        }

        return true;
    }

    static int Count(List<CombatAction> actions, ActionDefinition.ActionCategory cat)
    {
        int count = 0;

        foreach (var act in actions)
        {
            foreach (var c in act.resolvedCategories)
            {
                if (c == cat) count++;
            }
        }

        return count;
    }

    public static List<ActionDefinition.ActionCategory> GetValidCategories(ActionDefinition.ActionCategory original)
    {
        switch (original)
        {
            case ActionDefinition.ActionCategory.Standard:
                return new List<ActionDefinition.ActionCategory> { ActionDefinition.ActionCategory.Standard };

            case ActionDefinition.ActionCategory.Move:
                return new List<ActionDefinition.ActionCategory>
                {
                    ActionDefinition.ActionCategory.Move,
                    ActionDefinition.ActionCategory.Standard
                };

            case ActionDefinition.ActionCategory.Swift:
                return new List<ActionDefinition.ActionCategory>
                {
                    ActionDefinition.ActionCategory.Swift,
                    ActionDefinition.ActionCategory.Move,
                    ActionDefinition.ActionCategory.Standard
                };

            case ActionDefinition.ActionCategory.Free:
                return new List<ActionDefinition.ActionCategory> { ActionDefinition.ActionCategory.Free };

            default:
                return new List<ActionDefinition.ActionCategory> { original };
        }
    }

    public static Dictionary<ActionDefinition.ActionCategory, int> GetDefaultLimits()
    {
        return new Dictionary<ActionDefinition.ActionCategory, int>()
        {
            { ActionDefinition.ActionCategory.Move, 1 },
            { ActionDefinition.ActionCategory.Standard, 1 },
            { ActionDefinition.ActionCategory.Swift, 1 },
            { ActionDefinition.ActionCategory.Free, int.MaxValue }
        };
    }
    public static Dictionary<ActionDefinition.ActionCategory, int> GetLimitsForUnit(Unit unit)
    {
        var limits = GetDefaultLimits();

        if (unit.activeEffects.Exists(e => e is OverdriveEffect))
            limits[ActionDefinition.ActionCategory.Standard] = 2;

        //Command Override: one bonus Standard this round (CommandOverrideBonusEffect, granted during the last round)
        if (unit.activeEffects.Exists(e => e is CommandOverrideBonusEffect))
            limits[ActionDefinition.ActionCategory.Standard] += 1;

        //Disabled (0 HP), or acting in the surprise round: a single move OR standard action per round. With no Move slot,
        //a move takes the one Standard slot (GetValidCategories), so they exclude each other; full-round actions can't fit.
        //Swift/free stay available (Pathfinder). Applied after Overdrive so it always caps.
        //DEFERRED (Charge): a unit limited to a standard action may still charge, but only up to its speed (Pathfinder)
        if (unit.IsDisabled() || unit.limitedToSingleAction)
        {
            limits[ActionDefinition.ActionCategory.Move] = 0;
            limits[ActionDefinition.ActionCategory.Standard] = 1;
        }

        return limits;
    }
}
