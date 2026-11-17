using System.Collections.Generic;
using UnityEngine;

public static class MissionObjectiveEvaluator
{
    public static CombatEndState EvaluateAll(
        EncounterData encounter,
        MissionState state,
        CombatContext context,
        ExtractionMissionManager extraction = null)
    {
        //1. Evaluate secondary objectives
        if (encounter.secondaryObjectives != null)
        {
            foreach (var objective in encounter.secondaryObjectives)
            {
                if (IsResolved(objective.objectiveId, state)) continue;

                if (objective.condition.Evaluate(context))
                {
                    state.objectiveCompleted[objective.objectiveId] = true;
                    StampFlag(objective.narrativeFlagOnComplete);
                    CombatLog.Add($"[Objective] Complete: {objective.description}");
                }
                else if (objective.failCondition != null && objective.failCondition.IsFailed(context))
                {
                    state.objectiveFailed[objective.objectiveId] = true;
                    StampFlag(objective.narrativeFlagOnFail);
                    CombatLog.Add($"[Objective] Failed: {objective.description}");
                }
            }
        }

        //2. Check primary fail condition
        if (!state.primaryObjectiveFailed && encounter.primaryObjective != null)
        {
            if (encounter.primaryObjective.failCondition != null &&
                encounter.primaryObjective.failCondition.IsFailed(context))
            {
                state.primaryObjectiveFailed = true;
                StampFlag(encounter.primaryObjective.narrativeFlagOnFail);
                CombatLog.Add($"[Objective] Primary failed: {encounter.primaryObjective.description}");

                if (encounter.failureObjective != null)
                {
                    state.failureObjectiveActive = true;
                    CombatLog.Add($"[Objective] Failure objective activated: {encounter.failureObjective.description}");
                }
            }
        }

        //3. Check primary success
        if (!state.primaryObjectiveFailed && encounter.primaryObjective != null)
        {
            if (!IsResolved(encounter.primaryObjective.objectiveId, state) &&
                encounter.primaryObjective.condition.Evaluate(context))
            {
                state.objectiveCompleted[encounter.primaryObjective.objectiveId] = true;
                StampFlag(encounter.primaryObjective.narrativeFlagOnComplete);
                CombatLog.Add($"[Objective] Primary complete: {encounter.primaryObjective.description}");
            }
        }

        //4. Evaluate failure objective if active
        if (state.failureObjectiveActive && encounter.failureObjective != null)
        {
            string failObjId = encounter.failureObjective.objectiveId;

            if (!IsResolved(failObjId, state))
            {
                if (encounter.failureObjective.condition.Evaluate(context))
                {
                    state.objectiveCompleted[failObjId] = true;
                    StampFlag(encounter.failureObjective.narrativeFlagOnComplete);
                    CombatLog.Add($"[Objective] Failure objective complete: {encounter.failureObjective.description}");
                }
                else if (encounter.failureObjective.failCondition != null &&
                    encounter.failureObjective.failCondition.IsFailed(context))
                {
                    state.objectiveFailed[failObjId] = true;
                    StampFlag(encounter.failureObjective.narrativeFlagOnFail);
                    CombatLog.Add($"[Objective] Failure objective failed: {encounter.failureObjective.description}");
                }
            }
        }

        //5. Resolve extraction deadline
        if (extraction != null && extraction.ShouldResolveThisTurn(context.turnCount))
        {
            extraction.ResolveDeadline(context.playerUnits, state, context.gridManager);
            CombatLog.Add("[Extraction] Deadline resolved.");
        }

        //6. Determine CombatEndState
        bool allEnemiesDead = AllDead(context.enemyUnits);
        bool allPlayersDead = AllDead(context.playerUnits);
        bool primaryComplete = encounter.primaryObjective != null &&
            state.objectiveCompleted.ContainsKey(encounter.primaryObjective.objectiveId);

        if (allEnemiesDead && primaryComplete)
        {
            state.currentEndState = CombatEndState.FullVictory;
        }
        else if (allEnemiesDead && !primaryComplete)
        {
            state.currentEndState = CombatEndState.PartialVictory;
        }
        else if (primaryComplete && encounter.endsWhenPrimaryComplete && !allPlayersDead)
        {
            //G19: Survival-type primary met; the encounter ends with enemies still standing
            state.currentEndState = CombatEndState.FullVictory;
        }
        else if (allPlayersDead && state.extractedUnits.Count > 0)
        {
            state.currentEndState = CombatEndState.Retreat;
        }
        else if (allPlayersDead)
        {
            state.currentEndState = CombatEndState.Defeat;
        }
        else
        {
            state.currentEndState = CombatEndState.InProgress;
        }

        return state.currentEndState;
    }

    static bool IsResolved(string objectiveId, MissionState state)
    {
        return state.objectiveCompleted.ContainsKey(objectiveId) ||
            state.objectiveFailed.ContainsKey(objectiveId);
    }

    static void StampFlag(string flagId)
    {
        if (!string.IsNullOrEmpty(flagId))
            NarrativeFlagManager.SetFlag(flagId, 1);
    }

    //Out of the fight: dead, or extracted from the map
    static bool AllDead(List<Unit> units)
    {
        foreach (Unit unit in units)
        {
            if (!unit.IsDead() && !unit.isExtracted) return false;
        }
        return true;
    }
}
