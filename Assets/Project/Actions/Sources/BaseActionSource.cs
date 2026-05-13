using UnityEngine;
using System.Collections.Generic;

public class BaseActionSource : IActionSource
{
    private List<string> actionIds;

    public BaseActionSource(List<string> actionIds)
    {
        this.actionIds = actionIds;
    }

    public List<ActionDefinition> GetActions(Unit unit)
    {
        List<ActionDefinition> result = new List<ActionDefinition>();

        foreach (var id in actionIds)
        {
            var def = ActionDatabase.Get(id);
            if (def != null)
            {
                result.Add(def);
            }
        }

        return result;
    }
}
