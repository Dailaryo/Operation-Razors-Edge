using UnityEngine;

public static class ActionDefinitionExtensions
{
    public static bool IsMove(this ActionDefinition def)
    {
        return def.category == ActionDefinition.ActionCategory.Move;
    }
}
