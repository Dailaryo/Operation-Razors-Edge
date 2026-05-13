using UnityEngine;
using System.Collections.Generic;

public class ItemActionSource : IActionSource
{
    private string itemId;
    private List<string> actionIds;

    public ItemActionSource(string itemId, List<string> actionIds)
    {
        this.itemId = itemId;
        this.actionIds = actionIds;
    }

    public List<ActionDefinition> GetActions(Unit unit)
    {
        List<ActionDefinition> result = new List<ActionDefinition>();

        foreach (string id in actionIds)
        {
            var def = ActionDatabase.Get(id);
            if (def != null) result.Add(def);
        }

        return result;
    }
}
