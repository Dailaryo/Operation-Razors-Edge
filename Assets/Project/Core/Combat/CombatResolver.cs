using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatResolver
{
    CombatManager manager;

    private Dictionary<Unit, SimulatedUnit> simUnits;

    private bool isSimulation = false;

    public bool IsSimulation => isSimulation;

    public Vector2Int GetPosition(Unit unit)
    {
        if (isSimulation)
        {
            return simUnits[unit].position;
        }

        return unit.position;
    }

    public bool IsUnitDead(Unit unit)
    {
        if (isSimulation)
        {
            return simUnits[unit].isDead;
        }

        return unit.IsDead();
    }

    public bool IsTileOccupied(Vector2Int pos, Unit ignoreUnit = null)
    {
        if (isSimulation)
        {
            foreach (var kvp in simUnits)
            {
                var unit = kvp.Key;
                var sim = kvp.Value;

                if (unit == ignoreUnit) continue;
                if (sim.isDead) continue;

                if (sim.position == pos) return true;
            }

            return false;
        }

        Unit unitAt = GridManager.Instance.GetUnitAt(pos);

        if (unitAt == null) return false;
        if (unitAt == ignoreUnit) return false;
        if (unitAt.IsDead()) return false;

        return true;
    }

    public CombatResolver(CombatManager manager, bool simulationMode)
    {
        this.manager = manager;
        this.isSimulation = simulationMode;

        if (isSimulation)
        {
            simUnits = new Dictionary<Unit, SimulatedUnit>();

            foreach (var unit in manager.GetAllUnits())
            {
                simUnits[unit] = new SimulatedUnit(unit);
            }
        }
    }

    int GetActionPriority(CombatAction action)
    {
        int baseInitiative = action.source.initiativeRoll;

        int actionPriority = GetActionSpeed(action.definition.actionID);

        int total = baseInitiative + actionPriority;

        Debug.Log($"{action.source.unitName} initiative: {baseInitiative} + {actionPriority} = {total}");

        return total;
    }

    int GetActionSpeed(string type)
    {
        switch (type)
        {
            case ActionIds.TotalDefense: return 1;

            default: return 0;
        }
    }

    public IEnumerator ResolveTurn(List<CombatAction> actions)
    {
        SortActions(actions);

        yield return new WaitForSeconds(0.5f);

        yield return ExecuteActions(actions);
    }

    void SortActions(List<CombatAction> actions)
    {
        Dictionary<CombatAction, int> tieBreakers = new Dictionary<CombatAction, int>();

        //Assign random tie-breaker values once per turn
        foreach (var action in actions)
        {
            tieBreakers[action] = Random.Range(0, 10000);
        }

        actions.Sort((a, b) =>
        {
            int aPriority = GetActionPriority(a);
            int bPriority = GetActionPriority(b);

            if (aPriority != bPriority)
                return bPriority.CompareTo(aPriority);

            //Enforce per-unit sequence order
            if (a.source == b.source)
                return a.sequenceIndex.CompareTo(b.sequenceIndex);

            //Stable tie-breaker
            return tieBreakers[b].CompareTo(tieBreakers[a]);
        });
    }

    IEnumerator ExecuteActions(List<CombatAction> actions)
    {
        foreach (CombatAction action in actions)
        {
            // Run passive detection sweep once per unit at the start of their turn,
            // so detection state reflects current positions before the action validates.
            if (!isSimulation && action.sequenceIndex == 0)
            {
                List<Unit> allUnits = manager.GetAllUnits();
                List<Unit> playerUnits = allUnits.FindAll(u => !u.isEnemy);
                List<Unit> enemyUnits = allUnits.FindAll(u => u.isEnemy);

                if (action.source.isEnemy)
                    DetectionStateManager.RunPassiveSweep(enemyUnits, playerUnits);
                else
                    DetectionStateManager.RunPassiveSweep(playerUnits, enemyUnits);

                DetectionStateManager.ResetExposed(action.source);
            }

            if (!action.source.IsDead() && !action.source.IsDying() && !action.source.IsStabilized())
            {
                //Debug.Log("Executing action from: " + action.source.unitName);
                    
                manager.SetCurrentUnit(action.source);

                //Regen resource on first actionof this unit's turn
                if (action.sequenceIndex == 0)
                {
                    action.source.RegenResource();
                }
    
                ExecuteAction(action);

                //Refresh grid visuals after state changes
                GridHighlighter.Instance.RefreshUnitHighlights(manager);
                 
                yield return new WaitForSeconds(0.75f);
            }
        }
    }

    public void ExecuteAction(CombatAction action)
    {
        if (action.source.IsDead()) return;

        ActionContext ctx = BuildContext(action);

        var def = ctx.definition;
        BaseAction baseAction = new GenericAction(def);

        if (baseAction == null)
        {
            Debug.LogError($"Missing action: {ctx}");
            return;
        }

        //Execute action (behavior only)
        baseAction.Execute(ctx, this);
        if (ctx.definition.resourceCost > 0)
            AudioEventBus.Fire(AudioEventIds.Combat.AbilityActivate);

        // Check if this action provokes OAs (ranged attacks, psionic actions)
        // Skip in simulation mode — OA logic does not run during AI planning
        if (!isSimulation)
        {
            CheckActionProvocation(ctx);

            // If the actor was killed by the OA, abort without executing the action
            if (ctx.attacker.IsDead()) return;
        }

        //Spend resource cost
        int effectiveCost = ctx.attacker.GetEffectiveResourceCost(ctx.definition);
        if (effectiveCost > 0)
        {
            ctx.attacker.SpendResource(effectiveCost);
        }

        //Post -process (centralized effects)
        ApplyPostActionEffects(ctx);

        if (ctx.isMovement)
        {
            ResolveMove(ctx);
        }
        else if (ctx.shouldApplyDamage)
        {
            if (!ResolveHit(ctx))
            {
                CombatLog.Add($"{ctx.attacker.unitName} misses {ctx.finalTarget.unitName}");
                return;
            }

            InitializeDamage(ctx);

            ApplyPreActionEffects(ctx);

            ApplyFinalResult(ctx);

            DetectionStateManager.EvaluateBreak(ctx.attacker, ctx.definition);
        }
        else if (ctx.isWindowBreak)
        {
            ResolveWindowBreak(ctx);
        }

        action.OnResolved();
    }

    public List<Unit> GetAllUnits()
    {
        return manager.GetAllUnits();
    }

    void HandleUnitDeath(Unit unit)
    {
        if (!unit.isEnemy)
        {
            //Player unit: handoff to SquadManager for incapacitation and snapshot logic
            SquadManager.Instance.SetIncapacitated(unit);
        }
        else
        {
            //Enemy unit: collect equipped item drops and loot table drops into a queue
            //The loot UI prompt is deferred; for now drops are staged in CombatManager
            List<ItemInstance> lootQueue = new List<ItemInstance>();

            //Equipped items always drop
            if (unit.sourceCharacterData != null)
            {
                foreach (var kvp in unit.sourceCharacterData.equippedItemIds)
                {
                    lootQueue.Add(kvp.Value);
                }
            }

            //Loot table drops, if the enemy has one
            if (unit.sourceCharacterData != null &&
                !string.IsNullOrEmpty(unit.sourceCharacterData.lootTableId))
            {
                List<ItemInstance> tableDrops = LootResolver.ResolveLoot(unit.sourceCharacterData.lootTableId);
                lootQueue.AddRange(tableDrops);
            }

            //Stage drops for post-combat loot screen
            LootQueue.Enqueue(lootQueue);

            if (lootQueue.Count > 0)
                CombatLog.Add($"[Loot] {unit.unitName} dropped {lootQueue.Count} item(s)");
        }

        ThreatZoneManager.UnregisterUnit(unit);
        
        //Remove from grid
        GridManager.Instance.RemoveUnit(unit);

        CombatLog.Add($"{unit.unitName} has been defeated");
        AudioEventBus.Fire(AudioEventIds.Combat.UnitDeath);
    }

    void HandleUnitDying(Unit unit)
    {
        //OnlyApply DyingState once, a second hit on an already dying unit just deals HP loss
        if (!unit.IsDying()) return;

        if (!unit.activeEffects.Exists(e => e is DyingState))
        {
            unit.RemoveEffect<DisabledState>();
            unit.AddEffect(new DyingState());
        }
    }

    void HandleUnitDisabled(Unit unit)
    {
        if (!unit.activeEffects.Exists(e => e is DisabledState))
        {
            unit.AddEffect(new DisabledState());
        }
    }

    public void ResolveMove(ActionContext ctx)
    {
        //Debug.Log($"Action: {ctx.actionId}, isMovement: {ctx.isMovement}");

        if (!ctx.movePosition.HasValue) return;

        Vector2Int newPos = ctx.movePosition.Value;

        if (!GridManager.Instance.IsWithinBounds(newPos))
        {
            CombatLog.Add("Invalid move (out of bounds)");
            return;
        }

        if (IsTileOccupied(newPos, ctx.action.source))
        {
            if (!isSimulation)
            {
                CombatLog.Add("Invalid move (tile occupied)");
            }
            return;
        }

        if (isSimulation)
        {
            var simUnit = simUnits[ctx.attacker];

            simUnit.position = newPos;

            return;
        }

        MovementResolver.ExecuteMovePath(ctx.action.source, newPos, this);
        CombatLog.Add(ctx.action.source.unitName + " moved to " + newPos);
    }

    void ApplyPreActionEffects(ActionContext ctx)
    {
        List<StatusEffect> effects = CollectEffects(ctx);

        effects.Sort((a, b) => b.Priority.CompareTo(a.Priority));

        foreach (var effect in effects)
        {
            effect.OnBeforeAction(ctx);
        }
    }

    void ApplyPostActionEffects(ActionContext ctx)
    {
        //Future: cleanup, delayed effects, triggers
    }

    ActionContext BuildContext(CombatAction action)
    {
        return new ActionContext
        {
            attacker = action.source,
            originalTarget = action.target,
            finalTarget = action.target,

            baseDamage = 0,
            modifiedDamage = 0,

            action = action,
            temporaryEffects = action.temporaryEffects,
            definition = action.definition
        };
    }

    public void ApplyFinalResult(ActionContext ctx)
    {
        if (ctx.finalTarget == null) return;

        if (IsUnitDead(ctx.finalTarget)) return;

        int damage = Mathf.RoundToInt(ctx.modifiedDamage);

        if (isSimulation)
        {
            var simTarget = simUnits[ctx.finalTarget];

            ctx.simulatedDamage = damage;

            int prevHP = simTarget.currentHP;
            simTarget.currentHP -= damage;

            int deathThreshold = ctx.finalTarget.GetDeathThreshold();
            ctx.willKillTarget = prevHP > deathThreshold && simTarget.currentHP <= deathThreshold;
            return;
        }

        var target = ctx.finalTarget;

        int prev = target.currentHP;

        target.TakeDamage(damage);

        int actual = prev - target.currentHP;

        CombatLog.Add($"{target.unitName} takes {actual} damage. HP: {target.currentHP}");

        if (target.IsDead())
        {
            HandleUnitDeath(target);
        }
        else if (target.IsDying())
        {
            HandleUnitDying(target);
        }
        else if (target.IsDisabled())
        {
            HandleUnitDisabled(target);
        }
    }

    List<StatusEffect> CollectEffects(ActionContext ctx)
    {
        List<StatusEffect> allEffects = new List<StatusEffect>();

        //Runtime effects
        if (ctx.attacker != null)
            allEffects.AddRange(ctx.attacker.activeEffects);

        if (ctx.finalTarget != null)
            allEffects.AddRange(ctx.finalTarget.activeEffects);

        //Source-based effects
        if (ctx.attacker != null)
        {
            foreach (var source in ctx.attacker.effectSources)
            {
                allEffects.AddRange(source.GetEffects(ctx.attacker));
            }
        }

        if (ctx.finalTarget != null)
        {
            foreach (var source in ctx.finalTarget.effectSources)
            {
                allEffects.AddRange(source.GetEffects(ctx.finalTarget));
            }
        }

        //Temporary effects
        if (ctx.temporaryEffects != null)
            allEffects.AddRange(ctx.temporaryEffects);

        return allEffects;
    }

    bool ResolveHit(ActionContext ctx)
    {
        if (ctx.finalTarget == null) return false;

        int roll = Random.Range(1, 21);

        var attackType = ctx.definition.attackType;

        int accuracy = 0;

        switch (attackType)
        {
            case ActionDefinition.AttackType.Melee:
                accuracy = ctx.attacker.GetAccuracy(true);
                break;

            case ActionDefinition.AttackType.Ranged:
                accuracy = ctx.attacker.GetAccuracy(false);
                break;

            default:
                Debug.LogWarning($"ResolveHit called with non-attack action: {ctx.definition.actionID}");
                return false;
        }

        if (ctx.attacker.sourceCharacterData != null &&
            ctx.attacker.sourceCharacterData.HasItemEquipped(EquipmentSlot.MainHand))
        {
            ItemInstance weaponInstance = ctx.attacker.sourceCharacterData.GetEquippedItem(EquipmentSlot.MainHand);

            var weapon = weaponInstance != null ? ItemDatabase.Get(weaponInstance.definitionId) : null;
            if (weapon != null)
            {
                var classDef = ClassDatabase.Get(ctx.attacker.sourceCharacterData.classId);
                if (classDef != null)
                {
                    if (!classDef.IsProficientWith(weapon.weaponTag))
                    {
                        accuracy -= 4;
                        CombatLog.Add($"{ctx.attacker.unitName} is not proficient with {weapon.displayName} (-4 accuracy)");
                    }
                }
            }
        }

        int attackRating = ctx.attacker.attackRating;

        int attackTotal = roll + accuracy + attackRating + ctx.attackRollModifier;

        int defense = ctx.finalTarget.GetDefense();

        Debug.Log($"Attack Roll: {roll} + {accuracy} = {attackTotal} vs {defense}");

        bool hit = attackTotal >= defense;

        if (hit)
        {
            if (attackType == ActionDefinition.AttackType.Melee)
                AudioEventBus.Fire(AudioEventIds.Combat.MeleeHit);
            else if (attackType == ActionDefinition.AttackType.Ranged)
                AudioEventBus.Fire(AudioEventIds.Combat.RangedHit);
        }
        else
        {
            if (attackType == ActionDefinition.AttackType.Melee)
                AudioEventBus.Fire(AudioEventIds.Combat.MeleeMiss);
            else if (attackType == ActionDefinition.AttackType.Ranged)
                AudioEventBus.Fire(AudioEventIds.Combat.RangedMiss);
        }

        return hit;
    }

    void InitializeDamage(ActionContext ctx)
    {
       int baseDamage = RollBaseDamage(ctx);

        ctx.baseDamage = baseDamage;
        ctx.modifiedDamage = baseDamage;
    }

    int RollBaseDamage(ActionContext ctx)
    {
        //Try to read damage from equipped weapon
        if (ctx.attacker.sourceCharacterData != null &&
            ctx.attacker.sourceCharacterData.HasItemEquipped(EquipmentSlot.MainHand))
        {
            ItemInstance weaponInstance = ctx.attacker.sourceCharacterData.GetEquippedItem(EquipmentSlot.MainHand);

            var weapon = weaponInstance != null ? ItemDatabase.Get(weaponInstance.definitionId) : null;
            if (weapon != null)
            {
                int total = 0;
                for (int i = 0; i < weapon.damageDiceCount; i++)
                    total += Random.Range(1, weapon.damageDiceSides + 1);

                total += weapon.damageBonus;

                CombatLog.Add($"{ctx.attacker.unitName} rolls {weapon.displayName}: {total}");
                return total;
            }
        }

        //Fallback: unarmed d4
        return Random.Range(1, 5);
    }

    public Unit GetUnitAt(Vector2Int pos)
    {
        if (isSimulation)
        {
            foreach (var kvp in simUnits)
            {
                if (kvp.Value.isDead) continue;
                if (kvp.Value.position == pos) return kvp.Key;
            }
            return null;
        }

        return GridManager.Instance.GetUnitAt(pos);
    }

    void ResolveWindowBreak(ActionContext ctx)
    {
        if (!ctx.action.movePosition.HasValue) return;

        Vector2Int attackerPos = ctx.attacker.position;
        Vector2Int targetPos = ctx.action.movePosition.Value;

        //Must be adjacent
        if (!DistanceUtility.IsAdjacent(attackerPos, targetPos)) return;

        Vector2Int diff = targetPos - attackerPos;

        // Determine edge ownership and which edge to break
        // North edge: owned by the southern tile
        // East edge: owned by the western tile
        if (diff == Vector2Int.up)
        {
            GridManager.Instance.SetWindowBroken(attackerPos, true);
        }
        else if (diff == Vector2Int.down)
        {
            GridManager.Instance.SetWindowBroken(targetPos, true);
        }
        else if (diff == Vector2Int.right)
        {
            GridManager.Instance.SetWindowBroken(attackerPos, false);
        }
        else if (diff == Vector2Int.left)
        {
            GridManager.Instance.SetWindowBroken(targetPos, false);
        }

        CombatLog.Add($"{ctx.attacker.unitName} breaks the window at {targetPos}");
    }

    public void ResolveInterruptOAs(List<OARecord> queue)
    {
        //Sort by initiative descending: highest acts first
        queue.Sort((a, b) => b.priority.CompareTo(a.priority));

        foreach (OARecord record in queue)
        {
            //Re-check conditions, earlier OAs in this queue may have changed state
            if (record.attacker.IsDead()) continue;
            if (!record.attacker.oaState.CanTakeOA()) continue;
            if (record.target.IsDead()) continue;

            //Find the attacker's default melee action
            ActionDefinition meleeAction = record.attacker.GetAllActions()
                .Find(a => a.attackType == ActionDefinition.AttackType.Melee);

            if (meleeAction == null) continue;

            record.attacker.oaState.RecordOA();

            CombatAction oaAction = new CombatAction(
                record.attacker,
                record.target,
                meleeAction,
                manager
            );

            CombatLog.Add($"{record.attacker.unitName} takes an opportunity attack against {record.target.unitName}!");
            AudioEventBus.Fire(AudioEventIds.Combat.OATriggered);
            ExecuteAction(oaAction);
        }
    }

    private void CheckActionProvocation(ActionContext ctx)
    {
        var def = ctx.definition;

        bool isRanged = def.attackType == ActionDefinition.AttackType.Ranged;
        bool provokes = def.provokesOA;
        if (ctx.definition is PsionicPowerDefinition)
            AudioEventBus.Fire(AudioEventIds.Combat.PsionicManifest);
        if (!isRanged && !provokes) return;

        //Collect all enemy units threatening the actor's current tile(s)
        List<Unit> threateners = ThreatZoneManager.GetThreateningUnits(
            ctx.attacker.position, isEnemy: !ctx.attacker.isEnemy);

        if (threateners.Count == 0) return;

        List<OARecord> oaQueue = new List<OARecord>();

        foreach (Unit threatener in threateners)
        {
            //A threatener that cannot detect the mover cannot take an OA against them.
            if (!DetectionStateManager.CanTarget(threatener, ctx.attacker)) continue;
            
            //Skip if the threatener is hidden from the target and has not opted
            //in to taking OAs while hidden
            if (DetectionStateManager.Get(ctx.attacker, threatener) == DetectionState.Hidden
                && !threatener.oaState.reactWhileHidden)
                continue;
            
            if (threatener.oaState.CanTakeOA())
                oaQueue.Add(new OARecord(threatener, ctx.attacker, ctx.attacker.position));
        }

        if (oaQueue.Count > 0)
            ResolveInterruptOAs(oaQueue);
    }
}