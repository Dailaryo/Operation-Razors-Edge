using UnityEngine;

public static class ActionValidator
{
    public static bool IsValid(CombatAction action)
    {
        if (action.actionType == ActionType.Move)
        {
            if (action.movePosition == null) return false;

            Vector2Int pos = action.movePosition.Value;

            if (!GridManager.Instance.IsWithinBounds(pos)) return false;

            if (GridManager.Instance.IsTileOccupied(pos)) return false;

            int distance = DistanceUtility.Manhattan(
                action.source.position,
                action.movePosition.Value
            );

            int maxMove = 3; //temp

            return distance <= maxMove;
        }
        
        var def = ActionDatabase.Definitions[action.actionType];

        //Target required but missing
        if (def.requiresTarget && action.target == null) return false;

        //Target should not exist
        if (!def.requiresTarget && action.target != null) return false;

        //Target is dead
        if (action.target.IsDead()) return false;

        //Taret type validation
        switch (def.targetType)
        {
            case TargetType.Enemy: return action.target.isEnemy != action.source.isEnemy;
            case TargetType.Ally: return action.target.isEnemy == action.source.isEnemy;
            case TargetType.Self: return action.target == action.source;
        }

        //No Target needed
        if (!def.requiresTarget) return true;

        //Range validation
        if (def.requiresTarget)
        {
            int distance = DistanceUtility.Manhattan(
                action.source.position,
                action.target.position
            );

            if (distance < def.minRange || distance > def.maxRange) return false;

        }

        if (action.actionType == ActionType.Move)
        {
            int distance = DistanceUtility.Manhattan(
                action.source.position,
                action.targetPosition
            );

            int maxMove = 3; // temp

            if (distance > maxMove) return false;
        }

        return true;
    }
}
