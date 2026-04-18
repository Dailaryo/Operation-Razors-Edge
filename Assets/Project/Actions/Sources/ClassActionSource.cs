using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;

public class ClassActionSource : IActionSource
{
    private List<ActionDefinition> actions;

    public ClassActionSource(List<ActionDefinition> actions)
    {
        this.actions = actions;
    }

    public List<ActionDefinition> GetActions(Unit unit)
    {
        return actions;
    }
}
