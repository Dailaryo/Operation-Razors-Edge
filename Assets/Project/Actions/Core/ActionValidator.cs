using UnityEngine;

public static class ActionValidator
{
    public static bool IsValid(CombatAction action, CombatResolver resolver)
    {
        resolver = BuildSimulationState(action, resolver);
        
        if (action == null) return false;

        if (action.source == null) return false;

        if (action.definition.IsMove())
        {
            if (action.movePosition == null) return false;

            Vector2Int pos = action.movePosition.Value;

            if (!GridManager.Instance.IsWithinBounds(pos)) return false;

            bool occupied = resolver.IsTileOccupied(pos, action.source);

            //Block if another unit plans to move there
            if (action.manager.IsTilePlannedOccupied(pos))
            {
                occupied = true;
            }

            if (occupied) return false;

            ////Debug.Log($"[VALIDATOR] Checking move {action.source.position} → {pos}");

            ////Debug.Log("WithinBounds: " + GridManager.Instance.IsWithinBounds(pos));
            ////Debug.Log("Occupied: " + GridManager.Instance.IsTileOccupied(pos));

            Vector2Int origin = resolver.GetPosition(action.source);

            int distance = DistanceUtility.Manhattan(origin, action.movePosition.Value);

            ////Debug.Log("Distance: " + distance);

            int maxMove = 3; //temp

            return distance <= maxMove;
        }

        //For actions that require a target
        var def = action.definition;

        // No target needed
        if (!def.requiresTarget)
        {
            return true;
        }

        // Must have a target
        if (action.target == null) return false;
        if (resolver.IsUnitDead(action.target)) return false;

        // Target type validation
        switch (def.targetType)
        {
            case TargetType.Enemy:
                if (action.target.isEnemy == action.source.isEnemy) return false;
                break;

            case TargetType.Ally:
                if (action.target.isEnemy != action.source.isEnemy) return false;
                break;

            case TargetType.Self:
                if (action.target != action.source) return false;
                break;
        }

        // Range validation (ONLY ONCE)
        Vector2Int targetOrigin = resolver.GetPosition(action.source);

        int targetDistance = DistanceUtility.Manhattan(
            targetOrigin,
            resolver.GetPosition(action.target)
        );

        if (targetDistance < def.minRange || targetDistance > def.maxRange)
            return false;

        return true;
    }

    static CombatResolver BuildSimulationState(CombatAction action, CombatResolver resolver)
    {
        //Only simulate if we aren't already
        if (!resolver.IsSimulation)
        {
            resolver = new CombatResolver(action.manager, true);
        }

        var plannedActions = action.manager.GetPlannedActions(action.source);

        foreach (var prior in plannedActions)
        {
            if (prior == action) break;

            ActionContext ctx = new ActionContext
            {
                attacker = prior.source,
                originalTarget = prior.target,
                finalTarget = prior.target,

                baseDamage = prior.source.attackRating,
                modifiedDamage = prior.source.attackRating,

                action = prior,
                movePosition = prior.movePosition,

                definition = prior.definition
            };

            var def = prior.definition;
            BaseAction baseAction = new GenericAction(def);

            if (baseAction == null) continue;

            baseAction.Execute(ctx, resolver);

            if (ctx.isMovement)
            {
                resolver.ResolveMove(ctx);
            }
            else if (ctx.shouldApplyDamage)
            {
                resolver.ApplyFinalResult(ctx);
            }
        }

        return resolver;
    }
}
