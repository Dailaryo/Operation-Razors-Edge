using UnityEngine;
using System.Collections.Generic;

public class MissionState
{
    public Dictionary<string, bool> objectiveCompleted = new Dictionary<string, bool>();
    public Dictionary<string, bool> objectiveFailed = new Dictionary<string, bool>();

    public bool primaryObjectiveFailed;
    public bool failureObjectiveActive;

    public List<Unit> extractedUnits = new List<Unit>();

    public CombatEndState currentEndState = CombatEndState.InProgress;
}
