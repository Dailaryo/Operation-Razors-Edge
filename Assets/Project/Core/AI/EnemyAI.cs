using System.Collections.Generic;
using UnityEngine;
using static TMPro.Examples.ObjectSpin;
using static Unit;

public class EnemyAI
{
    public (ActionDefinition action, Unit target) ChooseAction(Unit unit, List<Unit> enemies)
    {
        //Get available actions
        List<ActionDefinition> available = new List<ActionDefinition>();

        foreach (ActionDefinition action in unit.GetAllActions())
        {
            if (unit.IsActionAvailable(action))
                available.Add(action);
        }

        //Choose action
        ActionDefinition chosenAction = ChooseBestAction(unit, available);

        //Choose target
        Unit target = ChooseTarget(unit, enemies, chosenAction);

        //SAFETY: if no valid target, do nothing fallback
        if (target == null) return (chosenAction, null);

        //Range check
        var def = chosenAction;

        int distance = DistanceUtility.Manhattan(unit.position, target.position);

        if (distance > def.maxRange)
        {
			//Move instead of attacking
			return (ActionDatabase.Definitions[ActionIds.Move], target); //target still used to guide movement;
        }

        return (chosenAction, target);
    }

	ActionDefinition ChooseBestAction(Unit unit, List<ActionDefinition> actions)
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

	ActionDefinition ChooseAggressiveAction(List<ActionDefinition> actions)
	{
		var melee = actions.Find(a => a.actionID == ActionIds.MeleeAttack);
		if (melee != null) return melee;

		var ranged = actions.Find(a => a.actionID == ActionIds.RangedAttack);
		if (ranged != null) return ranged;

		return actions[0];
	}

	ActionDefinition ChooseDefensiveAction(List<ActionDefinition> actions)
	{
		var defensive = actions.Find(a => a.actionID == ActionIds.FightingDefensively);
		if (defensive != null) return defensive;

		var defense = actions.Find(a => a.actionID == ActionIds.TotalDefense);
		if (defense != null) return defense;

		return actions[0];
	}

	//--- Target Selection helpers for AI behavior ---
	Unit ChooseTarget(Unit unit, List<Unit> enemies, ActionDefinition action)
    {
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

                if (best == null || unit.attackRating > best.attackRating)
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