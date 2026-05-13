using UnityEngine;
using System.Collections.Generic;

public class EscalationWaveDefinition
{
    public string waveId;
    public TriggerCondition triggerCondition;
    public int triggerValue; // Meaning depends on triggerCondition:
                             //   TurnBased: the turn number to fire on
                             //   EnemyCount: fire when enemy count drops to this value
                             //   ObjectiveTriggered: unused (see triggerFlagId)
                             //   PlayerAction: unused (deferred)
    public string triggerFlagId; //ObjectiveTriggerd only, flag that arms this wave
    public List<EnemySpawnEntry> enemyComposition;
    public SpawnPattern spawnPattern;
    public SpawnEdge spawnEdge;
    public string spawnZoneId; //SpecificLocation and NearObjective only, null otherwise
    public int spawnLevel = 1;
    public int planetTier = 1;

    public enum TriggerCondition
    {
        TurnBased,
        EnemyCount,
        ObjectiveTriggered,
        PlayerAction //Stub: deferred to later milestone
    }

    public enum SpawnPattern
    {
        EdgeSpawn, //From the appropriate edge of the map
        RandomSpawn, //Random unoccupied tiles anywhere on the grid
        SpecificLocation, //Tiles registered under spawnZoneId in GridManager
        NearObjective //Tiles adjacent to the center of spawnZoneId tiles
    }

    public enum SpawnEdge
    {
        Right,
        Left,
        Top,
        Bottom
    }

    public struct EnemySpawnEntry
    {
        public string enemyId;
        public int count;

        public EnemySpawnEntry(string enemyId, int count)
        {
            this.enemyId = enemyId;
            this.count = count;
        }
    }

    public EscalationWaveDefinition(
        string waveId,
        TriggerCondition trigger,
        int triggerValue,
        List<EnemySpawnEntry> enemies,
        SpawnPattern spawn = SpawnPattern.EdgeSpawn,
        SpawnEdge edge = SpawnEdge.Right,
        string spawnZoneId = null,
        int level = 1,
        int tier = 1,
        string triggerFlagId = null)
    {
        this.waveId = waveId;
        triggerCondition = trigger;
        this.triggerValue = triggerValue;
        enemyComposition = enemies ?? new List<EnemySpawnEntry>();
        spawnPattern = spawn;
        spawnEdge = edge;
        this.spawnZoneId = spawnZoneId;
        spawnLevel = level;
        planetTier = tier;
        this.triggerFlagId = triggerFlagId;
    }
}
