using UnityEngine;

public static class ActionValidator
{
    public static bool IsValid(CombatAction action, CombatResolver resolver)
    {
        if (action == null) return false;

        if (action.source == null) return false;

        // Units under an action-blocking condition cannot plan actions
        if (action.source.IsActionBlocked())
            return false;

        // Catapsi: a free 0-level manifestation inside an opposing field uses the whole round
        if (CatapsiEffect.ViolatesFullRound(action, resolver))
            return false;

        // Electronic Shroud blocks ranged attacks
        if (action.definition.attackType == ActionDefinition.AttackType.Ranged &&
            action.target != null)
        {
            var shroud = action.target.activeEffects
                .Find(e => e is ElectronicShroudEffect) as ElectronicShroudEffect;
            if (shroud != null && shroud.active)
                return false;
        }

        int effectiveCost = action.source.GetEffectiveResourceCost(action.definition);
        if (effectiveCost > 0)
        {
            if (!action.source.HasResource(effectiveCost)) return false;
        }

        // Power point validation for psionic powers
        if (action.definition is PsionicPowerDefinition psionicDef)
        {
            if (action.source.powerPoints == null) return false;
            if (action.source.IsManifestingBlocked()) return false;

            //Mindwipe: a power lost to negative levels can't be manifested
            var negative = action.source.activeEffects.Find(e => e is NegativeLevelsEffect) as NegativeLevelsEffect;
            if (negative != null && negative.lostPowerId == psionicDef.actionID) return false;

            //Sum PP and free 0-level uses already commited by this unit's earlier planned actions
            int committedPP = 0;
            int committedFreeUses = 0;

            foreach (var prior in action.manager.GetPlannedActions(action.source))
            {
                if (prior == action) break;
                if (!(prior.definition is PsionicPowerDefinition priorPower)) continue;

                bool priorFree = AugmentationProcessor.UsesFreeZeroLevel(action.source, priorPower, committedFreeUses);
                if (priorFree) committedFreeUses++;

                committedPP += AugmentationProcessor.GetTotalCost(priorPower, prior.augmentations, priorFree, prior.maximize);
            }

            //Maximize Power requires the class feature
            if (action.maximize &&
                (action.source.sourceCharacterData == null ||
                 !action.source.sourceCharacterData.unlockedPassiveIds.Contains(PassiveIds.TelepathMaximizePower)))
                return false;

            var cost = AugmentationProcessor.Validate(
                action.source, psionicDef, action.augmentations, committedPP, committedFreeUses, action.maximize);

            if (!cost.isValid) return false;

            if (!MaterialComponentResolver.CanPay(action, psionicDef, out _)) return false;
        }

        resolver = BuildSimulationState(action, resolver);

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

            return MovementResolver.IsReachable(action.source, pos, resolver);
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

        // Detection gate: cannot directly target a unit you cannot detect.
        // Area-of-effect tile-targeted actions bypass this check (requiresTarget is false for those).
        if (!DetectionStateManager.CanTarget(action.source, action.target))
            return false;

        // Range validation (ONLY ONCE)
        Vector2Int targetOrigin = resolver.GetPosition(action.source);

        int targetDistance = DistanceUtility.MovementCostDistance(
            targetOrigin,
            resolver.GetPosition(action.target)
        );

        int effectiveMaxRange = def.GetMaxRange(action.source);

        // Signal Scramble II extends SystemOverride range by 2 tiles
        if (def.actionID == ActionIds.SystemOverride &&
            action.source.sourceCharacterData != null &&
            action.source.sourceCharacterData.unlockedPassiveIds.Contains(PassiveIds.SignalScramble2))
        {
            effectiveMaxRange += 2;
        }

        if (targetDistance < def.minRange || targetDistance > effectiveMaxRange)
            return false;

        //LoS check
        if (def.requiresLoS)
        {
            Vector2Int seerPos = resolver.GetPosition(action.source);
            Vector2Int targetPos = resolver.GetPosition(action.target);

            bool bypassLoS = action.source.HasBlindSight &&
                DistanceUtility.MovementCostDistance(seerPos, targetPos) <= action.source.BlindSightRange;

            if (!bypassLoS && !LoSResolver.HasLoS(seerPos, targetPos, GridManager.Instance))
                return false;
        }

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

                baseDamage = Mathf.RoundToInt(prior.source.GetExpectedBaseDamage()),
                modifiedDamage = Mathf.RoundToInt(prior.source.GetExpectedBaseDamage()),

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
