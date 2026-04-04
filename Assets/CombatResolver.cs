using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatResolver
{
    CombatManager manager;

    int GetActionPriority(CombatAction action)
    {
        int baseInitiative = action.source.initiative;

        //FUTURE: ability modifier goes here
        int abilityModifier = 0;

        //Temp: keep Focus Shot behavior but isolate it
        int actionPriority = GetActionSpeed(action.actionType);

        return baseInitiative + abilityModifier + actionPriority;
    }

    int GetActionSpeed(ActionType type)
    {
        switch (type)
        {
            case ActionType.FocusShot: return -3;

            case ActionType.Defend: return 1;

            default: return 0;
        }
    }

    public CombatResolver(CombatManager manager)
    {
        this.manager = manager;
    }

    public IEnumerator ResolveTurn(List<CombatAction> actions)
    {
        SortActions(actions);

        yield return new WaitForSeconds(0.5f);

        yield return ExecuteActions(actions);
    }

    public void ResolveAttack(CombatAction action)
    {
        ActionContext ctx = new ActionContext
        {
            attacker = action.source,
            originalTarget = action.target,
            finalTarget = action.target,
            baseDamage = action.source.attack,
            modifiedDamage = action.source.attack + action.contextBonusDamage,
            actionType = action.actionType
        };

        //Combine persistant + temporary
        List<StatusEffect> allEffects = new List<StatusEffect>();

        foreach (Unit unit in manager.GetAllUnits())
        {
            allEffects.AddRange(unit.activeEffects);
        }

        //Add temporary effects from action
        allEffects.AddRange(action.temporaryEffects);

        //Sort by priority
        allEffects.Sort((a, b) => b.Priority.CompareTo(a.Priority));

        //Apply effects
        foreach (StatusEffect effect in allEffects)
        {
            effect.OnBeforeAction(ctx);
        }

        //Apply damage
        ApplyDamage(ctx);
    }

    void ApplyDamage(ActionContext ctx)
    {
        if (ctx.finalTarget == null || ctx.finalTarget.IsDead()) return; 
        
        int damage = Mathf.RoundToInt(ctx.modifiedDamage);

        ctx.finalTarget.TakeDamage(damage);
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

            //Stable tie-breaker
            return tieBreakers[b].CompareTo(tieBreakers[a]);
        });
    }

    IEnumerator ExecuteActions(List<CombatAction> actions)
    {
        foreach (CombatAction action in actions)
        {
            if (action.actionType == ActionType.Move)
            {
                //Re-calibrate range at execution time
                if (!ActionValidator.IsValid(action))
                {
                    CombatLog.Add("Action failed (out of range after movement)");
                    continue;
                }
            }

            if (!action.source.IsDead())
            {
                Debug.Log("Executing action from: " + action.source.unitName);
                    
                manager.SetCurrentUnit(action.source);
    
                action.Execute();
                 
                yield return new WaitForSeconds(0.75f);
            }
        }
    }
}