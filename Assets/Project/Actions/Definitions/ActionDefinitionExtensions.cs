using UnityEngine;

public static class ActionDefinitionExtensions
{
    public static bool IsMove(this ActionDefinition def)
    {
        //Movement = the action carries a MoveEffect. Category alone isn't enough: TS Stabilize costs a move action
        //but targets an ally. The move action cost itself still comes from category (ActionEconomyResolver)
        return def.effects != null && def.effects.Exists(e => e is MoveEffect);
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
