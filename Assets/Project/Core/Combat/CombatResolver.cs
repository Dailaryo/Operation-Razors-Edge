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

        Debug.Log($"{action.source.unitName} initiative: {baseInitiative} + {actionPriority} + {total}");

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
            if (!action.source.IsDead())
            {
                //Debug.Log("Executing action from: " + action.source.unitName);
                    
                manager.SetCurrentUnit(action.source);
    
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
        }

        action.OnResolved();
    }

    public List<Unit> GetAllUnits()
    {
        return manager.GetAllUnits();
    }

    void HandleUnitDeath(Unit unit)
    {
        //Remove from grid
        GridManager.Instance.RemoveUnit(unit);

        CombatLog.Add($"{unit.unitName} has been defeated");
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

        GridManager.Instance.MoveUnit(ctx.action.source, newPos);

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
            if (simTarget.currentHP < 0) simTarget.currentHP = 0;

            ctx.willKillTarget = prevHP > 0 && simTarget.currentHP <= 0;
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

        int attackRating = ctx.attacker.attackRating;

        int attackTotal = roll + accuracy + attackRating;

        int defense = 10 + ctx.finalTarget.GetDefense();

        Debug.Log($"Attack Roll: {roll} + {accuracy} = {attackTotal} vs {defense}");

        return attackTotal >= defense;
    }

    void InitializeDamage(ActionContext ctx)
    {
        var attackType = ctx.definition.attackType;

        int baseDamage = RollBaseDamage();

        ctx.baseDamage = baseDamage;
        ctx.modifiedDamage = baseDamage;
    }

    int RollBaseDamage()
    {
        return Random.Range(1, 7); //TEMP: d6
    }
}