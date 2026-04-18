using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class EnemyPlanner
{
    public List<CombatAction> Plan(Unit unit, List<Unit> enemies, CombatManager manager)
    {
        List<ActionPlan> plans = GeneratePlans(unit, enemies, manager);

        if (plans.Count == 0) return new List<CombatAction>();

        //Pick best plan
        plans.Sort((a, b) => b.score.CompareTo(a.score));

        return plans[0].actions;
    }

    List<ActionPlan> GeneratePlans(Unit unit, List<Unit> enemies, CombatManager manager)
    {
        List<ActionPlan> plans = new List<ActionPlan>();

        //Generate single-action plans
        var actions = GenerateValidActions(unit, enemies, manager, null);

        var moveFirst = GenerateMoveOptions(unit, new CombatResolver(manager, true), manager);

        foreach (var move in moveFirst)
        {
            var movePlan = new ActionPlan();
            movePlan.actions.Add(move);

            var followups = GenerateValidActions(unit, enemies, manager, movePlan);

            foreach (var next in followups)
            {
                if (!next.definition.dealsDamage) continue;

                var combo = new ActionPlan();
                combo.actions.Add(move);
                combo.actions.Add(next);

                if (CanExecutePlan(combo, unit))
                {
                    combo.score = ScorePlan(combo, unit, manager);
                    plans.Add(combo);
                }
            }
        }

        foreach (var action in actions)
        {
            var plan = new ActionPlan();
            plan.actions.Add(action);

            if (CanExecutePlan(plan, unit))
            {
                plan.score = ScorePlan(plan, unit, manager);
                plans.Add(plan);
            }

            //Generate two-action sequences
            var followups = GenerateValidActions(unit, enemies, manager, plan);

            foreach (var next in followups)
            {
                var combo = new ActionPlan();
                combo.actions.Add(action);
                combo.actions.Add(next);

                if (action.definition.IsMove() && next.definition.IsMove()) continue;

                if (CanExecutePlan(combo, unit))
                {
                    combo.score = ScorePlan(combo, unit, manager);
                    plans.Add(combo);
                }
            }
        }

        return plans;
    }

    class ActionPlan
    {
        public List<CombatAction> actions = new List<CombatAction>();
        public float score;
    }

    List<CombatAction> GenerateValidActions(Unit unit, List <Unit> enemies, CombatManager manager, ActionPlan existingPlan)
    {
        List<CombatAction> results = new List<CombatAction>();

        CombatResolver resolver = new CombatResolver(manager, true);

        if (existingPlan != null)
        {
            foreach (var prior in existingPlan.actions)
            {
                ActionContext ctx = new ActionContext
                {
                    attacker = prior.source,
                    originalTarget = prior.target,
                    finalTarget = prior.target,
                    baseDamage = prior.source.attackRating,
                    modifiedDamage = prior.source.attackRating,
                    definition = prior.definition,
                    action = prior,
                    temporaryEffects = prior.temporaryEffects
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
        }

        foreach (var action in unit.GetAllActions())
        {
            if (!unit.IsActionAvailable(action)) continue;

            var def = action;

            //Targeted actions
            if (def.requiresTarget)
            {
                foreach (Unit target in enemies)
                {
                    if (target.IsDead()) continue;

                    CombatAction actionInstance = new CombatAction(unit, target, def, manager.GetAllUnits(), manager.GetAllUnits(), manager);

                    bool valid = ActionValidator.IsValid(actionInstance, resolver);

                    Debug.Log($"[AI] {unit.unitName} checking {def.actionID} on {target?.unitName} -> valid: {valid}");

                    if (valid)
                    {
                        results.Add(actionInstance);
                    }

                    if (ActionValidator.IsValid(actionInstance, resolver)) results.Add(actionInstance);
                }
            }
            else
            {
                CombatAction actionInstance = new CombatAction(unit, null, def, manager.GetAllUnits(), manager.GetAllUnits(), manager);

                if (ActionValidator.IsValid(actionInstance, resolver)) results.Add(actionInstance);
            }
        }

        //Add movemenet options
        foreach (var move in GenerateMoveOptions(unit, resolver, manager))
        {
            results.Add(move);
        }

        return results;
    }

    List<CombatAction> GenerateMoveOptions(Unit unit, CombatResolver resolver, CombatManager manager)
    {
        List<CombatAction> moves = new List<CombatAction>();

        List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Vector2Int currentPos = resolver.GetPosition(unit);
            Vector2Int pos = currentPos + dir;

			var moveDef = ActionDatabase.Definitions[ActionIds.Move];

			CombatAction move = new CombatAction(unit, null, moveDef, null, null, manager);
            move.SetMovePosition(pos);

            if (ActionValidator.IsValid(move, resolver))
            {
                moves.Add(move);
            }
        }

        return moves;
    }

    float ScorePlan(ActionPlan plan, Unit unit, CombatManager manager)
    {
        CombatResolver simResolver = new CombatResolver(manager, true);

        // Simulate full plan
        foreach (var action in plan.actions)
        {
            ActionContext ctx = new ActionContext
            {
                attacker = action.source,
                originalTarget = action.target,
                finalTarget = action.target,
                baseDamage = action.source.attackRating,
                modifiedDamage = action.source.attackRating,
                definition = action.definition,
                action = action,
                temporaryEffects = action.temporaryEffects
            };

            var def = action.definition;
            BaseAction baseAction = new GenericAction(def);
            if (baseAction == null) continue;

            baseAction.Execute(ctx, simResolver);

            if (ctx.isMovement)
            {
                simResolver.ResolveMove(ctx);
            }
            else if (ctx.shouldApplyDamage)
            {
                simResolver.ApplyFinalResult(ctx);
            }
        }

        float score = 0;

        Unit closestEnemy = FindClosestEnemy(unit, simResolver, manager);

        if (closestEnemy != null)
        {
            Vector2Int currentPos = simResolver.GetPosition(unit);
            int dist = DistanceUtility.Manhattan(simResolver.GetPosition(unit), simResolver.GetPosition(closestEnemy));

            //Strong pressure to close distance
            score -= dist * 2f;
        }

        foreach (var action in plan.actions)
        {
            var def = action.definition;

            //Penalize wasted actions
            score -= 1;

            //Damage actions
            if (def.dealsDamage && action.target != null)
            {
                int estimatedDamage = unit.attackRating;

                score += estimatedDamage * 2f;

                //Kill bonus
                if (action.target.currentHP <= estimatedDamage) score += 20;
            }

            //Movement value (closer = better)
            if (action.definition.IsMove() && action.movePosition.HasValue)
            {
                Unit closest = FindClosestEnemy(unit, simResolver, manager);

                Vector2Int unitPos = simResolver.GetPosition(unit);
                Vector2Int enemyPos = simResolver.GetPosition(closest);

                int dist = DistanceUtility.Manhattan(unitPos, enemyPos);
            }
            if (def.dealsDamage && action.target != null)
            {
                int dist = DistanceUtility.Manhattan(
                    simResolver.GetPosition(unit),
                    simResolver.GetPosition(action.target)
                );

                if (dist <= def.maxRange)
                {
                    score += 10; //Strongly encourage attacking
                }
            }


            //Defensive value
            if (action.definition.actionID == ActionIds.TotalDefense)
            {
                Unit closest = FindClosestEnemy(unit, simResolver, manager);

                if (closest != null)
                {
                    int dist = DistanceUtility.Manhattan(simResolver.GetPosition(unit), simResolver.GetPosition(closest));

                    if (dist <= 2) score += 4; //Good when threatened
                    else score -= 3; //Bad when idle
                }
            }
        }

        return score;
    }

    Unit FindClosestEnemy(Unit unit, CombatResolver resolver, CombatManager manager)
    {
        Unit best = null;
        int bestDist = int.MaxValue;

        foreach (var u in manager.GetAllUnits())
        {
            if (u.isEnemy == unit.isEnemy || resolver.IsUnitDead(u)) continue;

            int d = DistanceUtility.Manhattan(resolver.GetPosition(unit), resolver.GetPosition(u));

            if (d < bestDist)
            {
                bestDist = d;
                best = u;
            }
        }

        return best;
    }

    bool CanExecutePlan(ActionPlan plan, Unit unit)
    {
        var limits = ActionEconomyResolver.GetDefaultLimits();

        List<CombatAction> simulated = new List<CombatAction>();

        foreach (var action in plan.actions)
        {
            bool success = ActionEconomyResolver.TryAssignCategories(unit, simulated, action, limits);

            if (!success) return false;

            simulated.Add(action);
        }

        return true;

    }
}
