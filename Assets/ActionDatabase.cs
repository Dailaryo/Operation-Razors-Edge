using UnityEngine;
using System.Collections.Generic;

public static class ActionDatabase
{
    public static Dictionary<ActionType, ActionDefinition> Definitions =
        new Dictionary<ActionType, ActionDefinition>()
        {
            {ActionType.Attack, new ActionDefinition(ActionType.Attack, TargetType.Enemy, true, 1, 1) },
            {ActionType.FocusShot, new ActionDefinition(ActionType.FocusShot, TargetType.Enemy, true, 2, 5) },
            {ActionType.Defend, new ActionDefinition(ActionType.Defend, TargetType.None, false, 0, 0) },
            {ActionType.TacticalBoost, new ActionDefinition(ActionType.TacticalBoost, TargetType.Ally, false, 0, 0) },
            {ActionType.Taunt, new ActionDefinition(ActionType.Taunt, TargetType.Enemy, false, 0, 0) }
        };
}
