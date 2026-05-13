using UnityEngine;
using System.Collections.Generic;

public class FeatActionSource : IActionSource
{
    private string featId;
    private List<string> actionIds;

    public FeatActionSource(string featId, List<string> actionIds)
    {
        this.featId = featId;
        this.actionIds = actionIds;
    }

    public List<ActionDefinition> GetActions(Unit unit)
    {
        List<ActionDefinition> result = new List<ActionDefinition>();

        foreach (string id in actionIds)
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
