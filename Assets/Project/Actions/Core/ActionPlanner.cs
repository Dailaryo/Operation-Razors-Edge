using UnityEngine;
using System.Collections.Generic;

public class ActionPlanner
{
    private Dictionary<Unit, List<CombatAction>> plannedActions =
        new Dictionary<Unit, List<CombatAction>>();

    public void Clear()
    {
        plannedActions.Clear();
    }

    public List<CombatAction>GetActions(Unit unit)
    {
        if (!plannedActions.ContainsKey(unit))
            plannedActions[unit] = new List<CombatAction>();

        return plannedActions[unit];
    }

    public bool TryAddAction(CombatAction action)
    {
        Unit unit = action.source;

        if (!plannedActions.ContainsKey(unit))
            plannedActions[unit] = new List<CombatAction>();

        var actions = plannedActions[unit];

        bool success = ActionEconomyResolver.TryAssignCategories(
            unit, actions, action, ActionEconomyResolver.GetLimitsForUnit(unit));

        if (!success) return false;

        action.sequenceIndex = actions.Count;
        actions.Add(action);

        return true;
    }

    public Dictionary<Unit, List<CombatAction>> GetAllPlanned()
    {
        return plannedActions;
    }

    public List<CombatAction> GetAllActionsFlat()
    {
        List<CombatAction> result = new List<CombatAction>();

        foreach (var kvp in plannedActions)
        {
            result.AddRange(kvp.Value);
        }

        return result;
    }
}
