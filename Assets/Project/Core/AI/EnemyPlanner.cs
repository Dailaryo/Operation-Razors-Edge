using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyPlanner
{
    public List<CombatAction> Plan(Unit unit, List<Unit> enemies, CombatManager manager)
    {
        //Scripted pattern first (D4). A step that can't be used falls back to the utility planner for this round.
        bool allowMovement = true;

        if (unit.attackPattern != null && unit.attackPattern.Count > 0)
        {
            if (TryPlanPattern(unit, enemies, manager, out List<CombatAction> patterned, out allowMovement))
                return patterned;
        }

        List<ActionPlan> plans = GeneratePlans(unit, enemies, manager, allowMovement);

        if (plans.Count == 0) return new List<CombatAction>();

        //Pick best plan
        plans.Sort((a, b) => b.score.CompareTo(a.score));

        return plans[0].actions;
    }

    List<ActionPlan> GeneratePlans(Unit unit, List<Unit> enemies, CombatManager manager, bool allowMovement = true)
    {
        List<ActionPlan> plans = new List<ActionPlan>();

        //Generate single-action plans
        var actions = GenerateValidActions(unit, enemies, manager, null, allowMovement);

        var moveFirst = allowMovement
            ? GenerateMoveOptions(unit, new CombatResolver(manager, true), manager)
            : new List<CombatAction>();

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
            var followups = GenerateValidActions(unit, enemies, manager, plan, allowMovement);

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

    List<CombatAction> GenerateValidActions(Unit unit, List<Unit> enemies, CombatManager manager, ActionPlan existingPlan, bool allowMovement = true)
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
                    baseDamage = Mathf.RoundToInt(ExpectedBaseDamage(prior.source)),
                    modifiedDamage = Mathf.RoundToInt(ExpectedBaseDamage(prior.source)),
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
            if (!allowMovement && action.IsMove()) continue;

            //DEFERRED: AI consumable use needs an item choice (ctx.consumableItemId); skip until enemies carry consumables
            if (action.actionID == ActionIds.UseConsumable) continue;

            var def = action;

            //Targeted actions
            if (def.requiresTarget)
            {
                // Mindless enemies cannot target units they have no detection of.
                // Intelligent enemies can target any unit including those at last known position
                // (ActionValidator's CanTarget gate handles the final validity check).
                bool isIntelligent = unit.sourceCharacterData == null ||
                    !unit.sourceCharacterData.IsMindless;

                List<Unit> visibleTargets = isIntelligent
                    ? enemies
                    : enemies.FindAll(e => DetectionStateManager.CanTarget(unit, e));

                //Move on from downed opponents (Dying/Stabilized) while any opponent can still act
                if (visibleTargets.Exists(e => e.CanAct()))
                    visibleTargets = visibleTargets.FindAll(e => e.CanAct());

                foreach (Unit target in visibleTargets)
                {
                    if (target.IsDead()) continue;

                    CombatAction actionInstance = new CombatAction(unit, target, def, manager);

                    bool valid = ActionValidator.IsValid(actionInstance, resolver);

                    Debug.Log($"[AI] {unit.unitName} checking {def.actionID} on {target?.unitName} -> valid: {valid}");

                    if (valid)
                    {
                        results.Add(actionInstance);
                    }
                }
            }
            else
            {
                CombatAction actionInstance = new CombatAction(unit, null, def, manager);

                if (ActionValidator.IsValid(actionInstance, resolver)) results.Add(actionInstance);
            }
        }

        //Add movement options (not for a patterned unit falling back from a fixed step)
        if (allowMovement)
        {
            foreach (var move in GenerateMoveOptions(unit, resolver, manager))
            {
                results.Add(move);
            }
        }

        return results;
    }

    List<CombatAction> GenerateMoveOptions(Unit unit, CombatResolver resolver, CombatManager manager)
    {
        List<CombatAction> moves = new List<CombatAction>();

        var moveDef = ActionDatabase.Definitions[ActionIds.Move];

        HashSet<Vector2Int> reachable = MovementResolver.GetReachableTiles(unit, resolver);

        foreach (Vector2Int pos in reachable)
        {
            //Destination already claimed this round (squad plan or an earlier enemy's plan)
            if (manager.IsTilePlannedOccupied(pos)) continue;

            CombatAction move = new CombatAction(unit, null, moveDef, manager);
            move.SetMovePosition(pos);
            moves.Add(move);
        }

        return moves;
    }

    float ScorePlan(ActionPlan plan, Unit unit, CombatManager manager)
    {
        CombatResolver simResolver = new CombatResolver(manager, true);

        Dictionary<CombatAction, ActionContext> contextMap = new Dictionary<CombatAction, ActionContext>();

        // Simulate full plan
        foreach (var action in plan.actions)
        {
            ActionContext ctx = new ActionContext
            {
                attacker = action.source,
                originalTarget = action.target,
                finalTarget = action.target,
                baseDamage = Mathf.RoundToInt(ExpectedBaseDamage(action.source)),
                modifiedDamage = Mathf.RoundToInt(ExpectedBaseDamage(action.source)),
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

            contextMap[action] = ctx;
        }

        float score = 0;

        Unit closestEnemy = FindClosestEnemy(unit, simResolver, manager);

        if (closestEnemy != null)
        {
            Vector2Int currentPos = simResolver.GetPosition(unit);
            int dist = DistanceUtility.Chebyshev(simResolver.GetPosition(unit), simResolver.GetPosition(closestEnemy));

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
                contextMap.TryGetValue(action, out ActionContext simCtx);

                int estimatedDamage = simCtx.simulatedDamage > 0
                    ? simCtx.simulatedDamage
                    : Mathf.RoundToInt(ExpectedBaseDamage(unit));

                //d20 expectation: weight by the chance to hit this target
                score += estimatedDamage * EstimateHitChance(unit, action.target, def.attackType) * 2f;

                //Kill bonus
                if (action.target.currentHP <= estimatedDamage) score += 20;
            }

            //Movement value (closer = better)
            if (action.definition.IsMove() && action.movePosition.HasValue)
            {
                //For Intelligent enemies only
                bool isIntelligent = unit.sourceCharacterData == null ||
                    !unit.sourceCharacterData.IsMindless;

                Unit closest = FindClosestEnemy(unit, simResolver, manager);

                //If no directly targetable enemy is found, intellignet enemies move toward
                //the last known position of any target they have lost sight of.
                Vector2Int? movementTarget = null;

                if (closest != null)
                {
                    movementTarget = simResolver.GetPosition(closest);
                }
                else if (isIntelligent)
                {
                    foreach (Unit potentialTarget in manager.GetAllUnits())
                    {
                        if (potentialTarget.isEnemy == unit.isEnemy) continue;

                        Vector2Int? lastKnown = DetectionStateManager.GetLastKnownPosition(unit, potentialTarget);

                        if (lastKnown.HasValue)
                        {
                            movementTarget = lastKnown;
                            break;
                        }
                    }
                }

                if (movementTarget == null) continue;

                Vector2Int enemyPos = movementTarget.Value;
                Vector2Int unitPos = simResolver.GetPosition(unit);

                int dist = DistanceUtility.Chebyshev(unitPos, enemyPos);

                if (dist <= action.definition.GetMaxRange(unit))
                {
                    score += 8f;
                }
                else
                {
                    score += (10f - dist);
                }

                if (isIntelligent)
                {
                    List<Vector2Int> path = MovementResolver.GetPath(unit, action.movePosition.Value, simResolver);

                    foreach (Vector2Int tile in path)
                    {
                        List<Unit> threateners = ThreatZoneManager.GetThreateningUnits(tile, isEnemy: false);

                        foreach (Unit threatener in threateners)
                        {
                            if (threatener.oaState == null) continue;
                            if (!threatener.oaState.CanTakeOA()) continue;

                            float hitChance = EstimateHitChance(threatener, unit, ActionDefinition.AttackType.Melee);

                            float expectedDamage = ExpectedBaseDamage(threatener) * hitChance;

                            score -= expectedDamage;
                        }
                    }
                }
            }
            if (def.dealsDamage && action.target != null)
            {
                int dist = DistanceUtility.Chebyshev(
                    simResolver.GetPosition(unit),
                    simResolver.GetPosition(action.target)
                );

                if (dist <= def.GetMaxRange(unit))
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
                    int dist = DistanceUtility.Chebyshev(simResolver.GetPosition(unit), simResolver.GetPosition(closest));

                    if (dist <= 2) score += 4; //Good when threatened
                    else score -= 3; //Bad when idle
                }
            }
        }

        return score;
    }

    //Closest opponent still in the fight; a downed one (Dying/Stabilized) only if no other remains
    Unit FindClosestEnemy(Unit unit, CombatResolver resolver, CombatManager manager)
    {
        Unit best = null, bestDowned = null;
        int bestDist = int.MaxValue, bestDownedDist = int.MaxValue;

        foreach (var u in manager.GetAllUnits())
        {
            if (u.isEnemy == unit.isEnemy || resolver.IsUnitDead(u)) continue;

            int d = DistanceUtility.Chebyshev(resolver.GetPosition(unit), resolver.GetPosition(u));

            if (u.CanAct())
            {
                if (d < bestDist) { bestDist = d; best = u; }
            }
            else if (d < bestDownedDist)
            {
                bestDownedDist = d;
                bestDowned = u;
            }
        }

        return best ?? bestDowned;
    }

    bool CanExecutePlan(ActionPlan plan, Unit unit)
    {
        var limits = ActionEconomyResolver.GetLimitsForUnit(unit);

        List<CombatAction> simulated = new List<CombatAction>();

        foreach (var action in plan.actions)
        {
            bool success = ActionEconomyResolver.TryAssignCategories(unit, simulated, action, limits);

            if (!success) return false;

            simulated.Add(action);
        }

        return true;

    }

    //Scripted pattern (D4/D5)

    //Plans this round from the unit's attack pattern. Returns false (reason logged) when the step can't be used;
    //Plan() then falls back to the utility planner, with movement only if the step itself allowed movement.
    bool TryPlanPattern(Unit unit, List<Unit> enemies, CombatManager manager,
        out List<CombatAction> actions, out bool allowFallbackMovement)
    {
        actions = new List<CombatAction>();

        int stepIndex = unit.patternStepIndex % unit.attackPattern.Count;
        PatternStep step = unit.attackPattern[stepIndex];

        //Advance one round whether or not the step works, so the cycle never stalls on one step
        unit.patternStepIndex = (stepIndex + 1) % unit.attackPattern.Count;

        allowFallbackMovement = step.approach || step.actionId == ActionIds.Move;

        //Enemy step: deliberately do nothing this round
        if (string.IsNullOrEmpty(step.actionId))
        {
            Debug.Log($"[Pattern] {unit.unitName} step {stepIndex}: idle.");
            return true;
        }

        //Move isn't in a unit's action list (movement is grid-driven), so a Move step uses the shared definition
        ActionDefinition def = step.actionId == ActionIds.Move
            ? ActionDatabase.Definitions[ActionIds.Move]
            : unit.GetAllActions().Find(a => a.actionID == step.actionId);
        if (def == null)
            return PatternFallback(unit, stepIndex, $"'{step.actionId}' is not one of this unit's actions");

        if (!unit.IsActionAvailable(def))
            return PatternFallback(unit, stepIndex, $"'{step.actionId}' is on cooldown or out of uses");

        CombatResolver resolver = new CombatResolver(manager, true);

        //Candidates: opponents still in the fight first (enemies move on from downed squad members)
        List<Unit> candidates = enemies.FindAll(e => e != null && !e.IsDead() && e.CanAct());
        if (candidates.Count == 0) candidates = enemies;

        //A step that never moves (approach false) picks its target only among those it can use the action on
        //from where it stands, so "LowestHP" means the lowest-HP target in range and line of sight
        if (!step.approach && def.requiresTarget && !def.IsMove())
            candidates = candidates.FindAll(e => ActionValidator.IsValid(new CombatAction(unit, e, def, manager), resolver));

        Unit target = ResolvePatternTarget(unit, step, candidates);

        //Move step: step toward the target as far as this round's movement allows
        if (def.IsMove())
        {
            if (target == null)
                return PatternFallback(unit, stepIndex, $"Move step has no living target for rule {step.targetRule}");

            CombatAction move = BestApproachMove(unit, target, resolver, manager);
            if (move == null)
                return PatternFallback(unit, stepIndex, $"no reachable tile closer to {target.unitName}");

            actions.Add(move);
            Debug.Log($"[Pattern] {unit.unitName} step {stepIndex}: Move toward {target.unitName}.");
            return true;
        }

        if (def.requiresTarget && target == null)
        {
            //Player-facing: a unit that never moves (Marksman) says why it isn't attacking
            if (!step.approach)
                CombatLog.Add($"{unit.unitName} holds position: no target in range or line of sight.");

            return PatternFallback(unit, stepIndex, $"no valid target for rule {step.targetRule}");
        }

        CombatAction action = new CombatAction(unit, def.requiresTarget ? target : null, def, manager);

        //Usable from where the unit stands
        if (ActionValidator.IsValid(action, resolver))
        {
            actions.Add(action);
            Debug.Log($"[Pattern] {unit.unitName} step {stepIndex}: {def.actionID} -> {(target != null ? target.unitName : "no target")}.");
            return true;
        }

        //Approach: only steps flagged approach move first to bring the target into range
        if (step.approach && target != null)
        {
            CombatAction move = BestApproachMove(unit, target, resolver, manager);

            if (move != null)
            {
                var plan = new ActionPlan();
                plan.actions.Add(move);
                plan.actions.Add(action);

                CombatResolver afterMove = new CombatResolver(manager, true);
                SimulateMove(move, afterMove);

                if (CanExecutePlan(plan, unit) && ActionValidator.IsValid(action, afterMove))
                {
                    actions.AddRange(plan.actions);
                    Debug.Log($"[Pattern] {unit.unitName} step {stepIndex}: approach, then {def.actionID} -> {target.unitName}.");
                    return true;
                }

                //Can't reach this round: close the distance and try again next round
                actions.Add(move);
                Debug.Log($"[Pattern] {unit.unitName} step {stepIndex}: approaching {target.unitName} (out of reach for {def.actionID}).");
                return true;
            }
        }

        return PatternFallback(unit, stepIndex,
            $"'{def.actionID}' is not valid against {(target != null ? target.unitName : "no target")} (range, line of sight or action economy)");
    }

    bool PatternFallback(Unit unit, int stepIndex, string reason)
    {
        Debug.LogWarning($"[Pattern] {unit.unitName} step {stepIndex}: {reason}. Using the utility planner this round.");
        return false;
    }

    //Living opponent chosen by the step's rule (Self = the unit itself, None = no target)
    Unit ResolvePatternTarget(Unit unit, PatternStep step, List<Unit> enemies)
    {
        if (step.targetRule == PatternStep.TargetRule.Self) return unit;
        if (step.targetRule == PatternStep.TargetRule.None) return null;

        Unit best = null;

        foreach (Unit candidate in enemies)
        {
            if (candidate == null || candidate.IsDead()) continue;

            switch (step.targetRule)
            {
                case PatternStep.TargetRule.Nearest:
                    if (best == null ||
                        DistanceUtility.Chebyshev(unit.position, candidate.position) <
                        DistanceUtility.Chebyshev(unit.position, best.position))
                        best = candidate;
                    break;

                case PatternStep.TargetRule.LowestHP:
                    if (best == null || candidate.currentHP < best.currentHP) best = candidate;
                    break;

                case PatternStep.TargetRule.HighestHP:
                    if (best == null || candidate.currentHP > best.currentHP) best = candidate;
                    break;

                case PatternStep.TargetRule.SpecificCharacterId:
                    if (candidate.sourceCharacterData != null &&
                        candidate.sourceCharacterData.characterId == step.specificCharacterId)
                        return candidate;
                    break;
            }
        }

        return best;
    }

    //Reachable tile that ends closest to the target; null if no tile is closer than standing still
    CombatAction BestApproachMove(Unit unit, Unit target, CombatResolver resolver, CombatManager manager)
    {
        Vector2Int targetPos = resolver.GetPosition(target);
        int bestDist = DistanceUtility.Chebyshev(resolver.GetPosition(unit), targetPos);
        CombatAction best = null;

        foreach (CombatAction move in GenerateMoveOptions(unit, resolver, manager))
        {
            int d = DistanceUtility.Chebyshev(move.movePosition.Value, targetPos);

            if (d < bestDist)
            {
                bestDist = d;
                best = move;
            }
        }

        return best;
    }

    //Applies a planned move to a simulation resolver so later validation sees the new position
    void SimulateMove(CombatAction move, CombatResolver resolver)
    {
        ActionContext ctx = new ActionContext
        {
            attacker = move.source,
            definition = move.definition,
            action = move,
            temporaryEffects = move.temporaryEffects
        };

        new GenericAction(move.definition).Execute(ctx, resolver);

        if (ctx.isMovement)
            resolver.ResolveMove(ctx);
    }

    //d20 estimates (replace the retired attackRating)

    //Average base damage of the main-hand weapon (dice + bonus), mirroring CombatResolver.RollBaseDamage; unarmed 1d4 = 2.5
    float ExpectedBaseDamage(Unit unit)
    {
        return unit.GetExpectedBaseDamage();
    }

    //Chance a d20 attack hits: (21 + ability mod + BAB - Defense) / 20, clamped 5%-95%.
    //Damage with no attack roll (saves, automatic) counts as a certain hit.
    float EstimateHitChance(Unit attacker, Unit target, ActionDefinition.AttackType attackType)
    {
        if (attackType == ActionDefinition.AttackType.None) return 1f;

        int attackBonus = attacker.GetAccuracy(attackType == ActionDefinition.AttackType.Melee) + attacker.GetBAB();

        return Mathf.Clamp((21f + attackBonus - target.GetDefense()) / 20f, 0.05f, 0.95f);
    }
}
