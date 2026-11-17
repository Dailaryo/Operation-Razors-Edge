using UnityEngine;
using System.Collections.Generic;

public class ExtractionMissionManager
{
    public List<string> extractionZoneIds;
    public int extractionDeadline;  //The turn on which extraction resolves.  Always > 0. Units are killed or extracted based on zone presence
    
    private bool deadlineResolved = false;
    public bool isEmergencyExtraction;

    public ExtractionMissionManager(List<string> zoneIds, int deadline)
    {
        extractionZoneIds = zoneIds ?? new List<string>();
        extractionDeadline = deadline;
    }

    // Returns true on the exact turn the deadline fires, and only once.
    // Called by MissionObjectiveEvaluator each turn before end state resolution.
    public bool ShouldResolveThisTurn(int turn)
    {
        if (deadlineResolved) return false;
        return turn >= extractionDeadline;
    }

    // Resolves the extraction deadline:
    // - Units in any extraction zone are extracted into MissionState
    // - Units not in a zone and still alive are killed
    // Safe to call only once per combat — guarded by deadlineResolved.
    public void ResolveDeadline(List<Unit> playerUnits, MissionState state, GridManager grid)
    {
        if (deadlineResolved) return;
        deadlineResolved = true;

        foreach (Unit unit in playerUnits)
        {
            if (unit.IsDead()) continue;

            if (IsInExtractionZone(unit, grid))
            {
                unit.isExtracted = true;
                grid.RemoveUnit(unit);
                ThreatZoneManager.UnregisterUnit(unit); //off the board: no threat zone, no OAs
                state.extractedUnits.Add(unit);
                CombatLog.Add($"[Extraction] {unit.unitName} extracted successfully.");
            }
            else
            {
                CombatLog.Add($"[Extraction] {unit.unitName} did not reach the extraction zone in time.");

                //Standard defeat path: OnDeath -> CombatManager.ProcessDefeat (incapacitated until the Hub, snapshot rollback)
                unit.Kill();
            }
        }
    }

    bool IsInExtractionZone(Unit unit, GridManager grid)
    {
        foreach (string zone in extractionZoneIds)
        {
            if (grid.IsUnitInZone(unit, zone)) return true;
        }
        return false;
    }
}
