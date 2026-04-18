using System.Collections.Generic;
using UnityEngine;

public static class MissionObjectiveEvaluator
{
    public static bool IsMissionSuccessful(
        MissionData mission,
        List<Unit> playerUnits,
        List<Unit> enemyUnits,
        TurnManager turnManager)
    {
        switch (mission.objectiveType)
        {
            case MissionData.MissionObjectiveType.Survival:
                return CheckSurvival(mission, turnManager);

            case MissionData.MissionObjectiveType.TargetElimination:
                return CheckTargetElimination(mission);

            case MissionData.MissionObjectiveType.Elimination:
            default:
                return CheckElimination(enemyUnits);
        }
    }

    static bool CheckElimination(List<Unit> enemies)
    {
        foreach (var unit in enemies)
        {
            if (!unit.IsDead()) return false;
        }
        return true;
    }

    static bool CheckSurvival(MissionData mission, TurnManager turnManager)
    {
        return turnManager.turnCount >= mission.turnLimit;
    }

    static bool CheckTargetElimination(MissionData mission)
    {
        if (mission.targetUnit == null)
        {
            Debug.LogError("TargetElimination mission missing targetUnit!");
            return false;
        }

        return mission.targetUnit.IsDead();
    }
}
