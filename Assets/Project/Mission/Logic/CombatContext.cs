using UnityEngine;
using System.Collections.Generic;

public class CombatContext
{
    public List<Unit> playerUnits;
    public List<Unit> enemyUnits;
    public int turnCount;
    public GridManager gridManager;
    public MissionState missionState;

    public CombatContext(
        List<Unit> playerUnits,
        List<Unit> enemyUnits,
        int turnCount,
        GridManager gridManager,
        MissionState missionState)
    {
        this.playerUnits = playerUnits;
        this.enemyUnits = enemyUnits;
        this.turnCount = turnCount;
        this.gridManager = gridManager;
        this.missionState = missionState;
    }
}
