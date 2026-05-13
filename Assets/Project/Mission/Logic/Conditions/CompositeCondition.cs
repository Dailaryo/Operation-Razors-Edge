using UnityEngine;
using System.Collections.Generic;

public class CompositeCondition : IObjectiveCondition
{
    public enum Mode { All, Any }

    public List<IObjectiveCondition> conditions;
    public Mode mode;

    public CompositeCondition(List<IObjectiveCondition> conditions, Mode mode)
    {
        this.conditions = conditions;
        this.mode = mode;
    }

    public bool Evaluate(CombatContext context)
    {
        if (mode == Mode.All)
        {
            foreach (var condition in conditions)
            {
                if (!condition.Evaluate(context)) return false;
            }
            return true;
        }
        else
        {
            foreach (var condition in conditions)
            {
                if (condition.Evaluate(context)) return true;
            }
            return false;
        }
    }

    public bool IsFailed(CombatContext context)
    {
        if (mode == Mode.All)
        {
            foreach (var condition in conditions)
            {
                if (condition.IsFailed(context)) return true;
            }
            return false;
        }
        else
        {
            foreach (var condition in conditions)
            {
                if (!condition.IsFailed(context)) return false;
            }
            return true;
        }
    }
}
