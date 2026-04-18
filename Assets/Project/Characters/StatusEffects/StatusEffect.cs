using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    public string effectName;
    public int duration;
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
        duration--;
    }

    public virtual void OnBeforeAction(ActionContext ctx) { }
    public virtual void OnAfterAction(ActionContext ctx) { }

    public bool IsExpired()
    {
        return duration <= 0;
    }

    public virtual List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>();
    }
}
