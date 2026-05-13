using UnityEngine;
using System.Collections.Generic;

public class EliminationCondition : IObjectiveCondition
{
    public string enemyGroupTag; //null = all enemies

    public EliminationCondition(string enemyGroupTag = null)
    {
        this.enemyGroupTag = enemyGroupTag;
    }

    public bool Evaluate(CombatContext context)
    {
        foreach (Unit unit in context.enemyUnits)
        {
            if (enemyGroupTag != null && unit.groupTag != enemyGroupTag) continue;

            if (!unit.IsDead()) return false;
        }

        return true;
    }

    public bool IsFailed(CombatContext context)
    {
        return false;
    }
}
