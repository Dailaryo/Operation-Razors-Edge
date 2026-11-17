using UnityEngine;

public static class ActionDefinitionExtensions
{
    public static bool IsMove(this ActionDefinition def)
    {
        return def.category == ActionDefinition.ActionCategory.Move;
    }

    //Targets a grid tile rather than a unit (Deploy Field, Break Window).
    //Move-category tile actions (Shadowstep) go through the move path instead
    public static bool IsGroundTargeted(this ActionDefinition def)
    {
        return def.targetingMode == ActionDefinition.TargetingMode.Tile
            && !def.requiresTarget
            && !def.IsMove();
    }
}
