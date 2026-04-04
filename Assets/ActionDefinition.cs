using UnityEngine;

public class ActionDefinition
{
    public ActionType actionType;

    public TargetType targetType;

    public bool requiresTarget;

    public int minRange;
    public int maxRange;

    public ActionDefinition(ActionType actionType, TargetType targetType, bool requiresTarget, int minRange, int maxRange)
    {
        this.actionType = actionType;
        this.targetType = targetType;
        this.requiresTarget = requiresTarget;
        this.minRange = minRange;
        this.maxRange = maxRange;
    }
}
