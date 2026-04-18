using UnityEngine;
using System.Collections.Generic;
using System.Threading;

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
                    ActionDefinition.ActionCategory.Swift
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
}
