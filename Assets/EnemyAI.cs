using UnityEngine;
using System.Collections.Generic;
using static Unit;

public class EnemyAI
{
    public (ActionType action, Unit target) ChooseAction(Unit unit, List<Unit> enemies)
    {
        //Get available actions
        List<ActionType> available = new List<ActionType>();

        foreach (ActionType action in unit.availableActions)
        {
            if (unit.IsActionAvailable(action))
                available.Add(action);
        }

        //Choose action
        ActionType chosenAction = ChooseBestAction(unit, available);

        //Choose target
        Unit target = ChooseTarget(unit, enemies, chosenAction);

        //SAFETY: if no valid target, do nothing fallback
        if (target == null) return (chosenAction, null);

        //Range check
        var def = ActionDatabase.Definitions[chosenAction];

        int distance = DistanceUtility.Manhattan(unit.position, target.position);

        if (distance > def.maxRange)
        {
            //Move instead of attacking
            return (ActionType.Move, target); //target still used to guide movement;
        }

        return (chosenAction, target);
    }

    ActionType ChooseBestAction(Unit unit, List<ActionType> actions)
    {
        //Random override first
        if (Random.value < 0.2f)
        {
            return actions[Random.Range(0, actions.Count)];
        }

        //Behavior Logic
        switch (unit.behaviorType)
        {
            case AIBehaviorType.Aggressive:
                return ChooseAggressiveAction(actions);

            case AIBehaviorType.Defender:
                return ChooseDefensiveAction(actions);

            default: return actions[0];
        }
    }

    ActionType ChooseAggressiveAction(List<ActionType> actions)
    {
        if (actions.Contains(ActionType.FocusShot))
            return ActionType.FocusShot;

        if (actions.Contains(ActionType.Attack))
            return ActionType.Attack;

        return actions[0];
    }

    ActionType ChooseDefensiveAction(List<ActionType> actions)
    {
        if (actions.Contains(ActionType.Taunt))
            return ActionType.Taunt;

        if (actions.Contains(ActionType.Defend))
            return ActionType.Defend;

        return actions[0];
    }

    //--- Target Selection helpers for AI behavior ---
    Unit ChooseTarget(Unit unit, List<Unit> enemies, ActionType action)
    {
        if (action == ActionType.Taunt)
            return unit; //self-target

        //Aggressive units target weakest
        if (unit.behaviorType == AIBehaviorType.Aggressive)
            return GetLowestHPUnit(enemies);

        //Defender units target strongest threat
        if (unit.behaviorType == AIBehaviorType.Defender)
            return GetHighestAttackUnit(enemies);

        return GetRandomUnit(enemies);

        Unit GetHighestAttackUnit(List<Unit> units)
        {
            Unit best = null;

            foreach (Unit unit in units)
            {
                if (unit.IsDead()) continue;

                if (best == null || unit.attack > best.attack)
                    best = unit;
            }

            return best;
        }
    }

    Unit GetRandomUnit(List<Unit> units)
    {
        List<Unit> alive = units.FindAll(u => !u.IsDead());

        if (alive.Count == 0) return null;

        return alive[UnityEngine.Random.Range(0, alive.Count)];
    }

    Unit GetLowestHPUnit(List<Unit> units)
    {
        Unit lowest = null;

        foreach (Unit unit in units)
        {
            if (!unit.IsDead())
            {
                if (lowest == null || unit.currentHP < lowest.currentHP)
                {
                    lowest = unit;
                }
            }
        }

        return lowest;
    }

    public Vector2Int GetMoveTowards(Unit unit, Unit target)
    {
        Vector2Int current = unit.position;
        Vector2Int targetPos = target.position;

        List<Vector2Int> options = new List<Vector2Int>
        {
            current + Vector2Int.up,
            current + Vector2Int.down,
            current + Vector2Int.left,
            current + Vector2Int.right
        };

        Vector2Int best = current;
        int bestDistance = int.MaxValue;

        foreach (var pos in options)
        {
            if (!GridManager.Instance.IsWithinBounds(pos)) continue;
            if (GridManager.Instance.IsTileOccupied(pos)) continue;

            int dist = DistanceUtility.Manhattan(pos, targetPos);

            if (dist < bestDistance)
            {
                bestDistance = dist;
                best = pos;
            }
        }

        return best;
    }
}