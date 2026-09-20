using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    public string effectName;
    public int duration;

    public bool isPersistent = false;
    public int sourceLevel = 0;
    public Unit Target { get; private set; }
    public int Priority { get; protected set; } = 0;

    public StatusEffect(string name, int turns)
    {
        effectName = name;
        duration = turns;
    }

    public virtual void OnApply(Unit unit)
    {
        Target = unit;
    }

    public virtual void OnExpire(Unit unit)
    {
        // Override in subclasses
    }

    //Default behavior: decrease duration each turn
    public virtual void OnTurnEnd(Unit unit)
    {
        if (!isPersistent)
            duration--;
    }

    public virtual void OnBeforeAction(ActionContext ctx) { }
    public virtual void OnAfterAction(ActionContext ctx) { }

    public bool IsExpired()
    {
        if (isPersistent) return false;
        return duration <= 0;
    }

    public virtual List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>();
    }

    //Conditions that must ALL be true for this effect to fire.
    //Null or empty = always fires
    public ConditionType[] Conditions { get; protected set; } = null;

    // Returns true if all declared conditions pass, or if no conditions
    // are declared. Call this at the top of OnBeforeAction/OnAfterAction
    // in subclasses that use the condition system.
    public bool ConditionsMet(ActionContext ctx)
    {
        if (Conditions == null || Conditions.Length == 0) return true;
        return ConditionEvaluator.EvaluateAll(Conditions, ctx);
    }
}
