using UnityEngine;
using System.Collections.Generic;

public class EscalationHandler
{
    private EncounterData encounter;
    private List<EscalationWaveDefinition> triggeredWaves = new List<EscalationWaveDefinition>();

    public EscalationHandler(EncounterData encounter)
    {
        this.encounter = encounter;
    }

    // Called at the end of each turn by CombatManager.ProcessEndPhase().
    // Returns an EscalationResult containing units to spawn and their intended
    // grid positions. CombatManager is responsible for actual grid placement.
    public EscalationResult CheckEscalation(int currentTurn, int currentEnemyCount)
    {
        var result = new EscalationResult();

        if (encounter.escalationWaves == null) return result;

        foreach (var wave in encounter.escalationWaves)
        {
            if (triggeredWaves.Contains(wave)) continue;
            if (!EvaluateTrigger(wave, currentTurn, currentEnemyCount)) continue;

            triggeredWaves.Add(wave);

            List<Unit> spawned = SpawnWave(wave);
            List<Vector2Int> positions = GetSpawnPositions(wave, spawned.Count);

            for (int i = 0; i < spawned.Count; i++)
            {
                if (i >= positions.Count)
                {
                    CombatLog.Add($"[Escalation] Wave '{wave.waveId}' — no spawn tile for unit {i + 1}, discarding.");
                    continue;
                }

                result.units.Add(spawned[i]);
                result.positions.Add(positions[i]);
            }

            CombatLog.Add($"[Escalation] Wave '{wave.waveId}' triggered — {spawned.Count} reinforcements incoming.");
        }

        return result;
    }

    //Trigger Evaluation
    bool EvaluateTrigger(EscalationWaveDefinition wave, int currentTurn, int currentEnemyCount)
    {
        switch (wave.triggerCondition)
        {
            case EscalationWaveDefinition.TriggerCondition.TurnBased:
                return currentTurn == wave.triggerValue;

            case EscalationWaveDefinition.TriggerCondition.EnemyCount:
                return currentEnemyCount <= wave.triggerValue;

            case EscalationWaveDefinition.TriggerCondition.ObjectiveTriggered:
                if (string.IsNullOrEmpty(wave.triggerFlagId)) return false;
                return NarrativeFlagManager.HasFlag(wave.triggerFlagId);

            case EscalationWaveDefinition.TriggerCondition.PlayerAction:
                Debug.LogWarning($"[Escalation] Wave '{wave.waveId}' uses PlayerAction trigger — deferred, skipping.");
                return false;

            default:
                return false;
        }
    }

    //Unit Spawning
    List<Unit> SpawnWave(EscalationWaveDefinition wave)
    {
        var units = new List<Unit>();

        foreach (var entry in wave.enemyComposition)
        {
            //spawnLevel 0 = match the squad (highest squad level), same rule as zone event spawn entries
            int level = wave.spawnLevel > 0 ? wave.spawnLevel : SquadManager.Instance.GetHighestLevel();
            var spawned = EnemyFactory.CreateGroup(entry.enemyId, entry.count, level, wave.planetTier);
            units.AddRange(spawned);
        }

        return units;
    }

    //Spawn Position Resolution
    List<Vector2Int> GetSpawnPositions(EscalationWaveDefinition wave, int count)
    {
        switch (wave.spawnPattern)
        {
            case EscalationWaveDefinition.SpawnPattern.EdgeSpawn:
                return GetEdgeSpawnPositions(wave, count);

            case EscalationWaveDefinition.SpawnPattern.RandomSpawn:
                return GetRandomSpawnPositions(count);

            case EscalationWaveDefinition.SpawnPattern.SpecificLocation:
                return GetZoneSpawnPositions(wave, wave.spawnZoneId, count);

            case EscalationWaveDefinition.SpawnPattern.NearObjective:
                return GetNearObjectivePositions(wave, wave.spawnZoneId, count);

            default:
                return GetEdgeSpawnPositions(wave, count);
        }
    }

    //Pre-defined edge: the same centred, edge-inward fill as the initial placement (GridManager.GetSpawnTiles),
    //skipping occupied tiles
    List<Vector2Int> GetEdgeSpawnPositions(EscalationWaveDefinition wave, int count)
    {
        MapDefinition.EntryZone edge;
        switch (wave.spawnEdge)
        {
            case EscalationWaveDefinition.SpawnEdge.Left: edge = MapDefinition.EntryZone.West; break;
            case EscalationWaveDefinition.SpawnEdge.Top: edge = MapDefinition.EntryZone.North; break;
            case EscalationWaveDefinition.SpawnEdge.Bottom: edge = MapDefinition.EntryZone.South; break;
            case EscalationWaveDefinition.SpawnEdge.Right:
            default: edge = MapDefinition.EntryZone.East; break;
        }

        var positions = new List<Vector2Int>();

        foreach (Vector2Int pos in GridManager.Instance.GetSpawnTiles(edge))
        {
            if (positions.Count >= count) break;
            if (!GridManager.Instance.IsTileOccupied(pos)) positions.Add(pos);
        }

        return positions;
    }

    //Random unoccupied tiles across the map
    List<Vector2Int> GetRandomSpawnPositions(int count)
    {
        var candidates = new List<Vector2Int>();

        for (int x = 0; x < GridManager.Instance.width; x++)
        {
            for (int y = 0; y < GridManager.Instance.height; y++)
            {
                var pos = new Vector2Int(x, y);
                if (!GridManager.Instance.IsTileOccupied(pos))
                    candidates.Add(pos);
                
            }
        }

        //Fisher-Yates shuffle then take first 'count'
        for (int i = candidates.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
        }

        return candidates.GetRange(0, Mathf.Min(count, candidates.Count));
    }

    //Tiles registered under spawnZoneId in GridManager, filtered to unoccupied
    List<Vector2Int> GetZoneSpawnPositions(EscalationWaveDefinition wave, string zoneId, int count)
    {
        if (string.IsNullOrEmpty(zoneId))
        {
            Debug.LogWarning("[Escalation] SpecificLocation wave has no spawnZoneId — falling back to EdgeSpawn.");
            return GetEdgeSpawnPositions(wave, count);
        }

        var zoneTiles = GridManager.Instance.GetZoneTiles(zoneId);
        var positions = new List<Vector2Int>();

        foreach (var tile in zoneTiles)
        {
            if (!GridManager.Instance.IsTileOccupied(tile))
                positions.Add(tile);

            if (positions.Count >= count) break;
        }

        return positions;
    }

    //Unoccupied tiles adjacent to the center of the named zone
    List<Vector2Int> GetNearObjectivePositions(EscalationWaveDefinition wave, string zoneId, int count)
    {
        if (string.IsNullOrEmpty(zoneId))
        {
            Debug.LogWarning("[Escalation] NearObjective wave has no spawnZoneId — falling back to EdgeSpawn.");
            return GetEdgeSpawnPositions(wave, count);
        }

        var zoneTiles = GridManager.Instance.GetZoneTiles(zoneId);
        if (zoneTiles.Count == 0) return GetEdgeSpawnPositions(wave, count);

        //Calculate center
        float cx = 0f, cy = 0f;
        foreach (var tile in zoneTiles) { cx += tile.x; cy += tile.y; }
        cx /= zoneTiles.Count;
        cy /= zoneTiles.Count;

        Vector2Int center = new Vector2Int(Mathf.RoundToInt(cx), Mathf.RoundToInt(cy));

        //Collect unoccupied cardinal and diagonal neighbors, sorted by distanceto center
        var candidates = new List<Vector2Int>();
        int searchRadius = 2;

        for (int dx = -searchRadius; dx <= searchRadius; dx++)
        {
            for (int dy = -searchRadius; dy <= searchRadius; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                var candidate = center + new Vector2Int(dx, dy);

                if (!GridManager.Instance.IsWithinBounds(candidate)) continue;
                if (GridManager.Instance.IsTileOccupied(candidate)) continue;

                candidates.Add(candidate);
            }
        }

        candidates.Sort((a, b) =>
        {
            float da = Vector2Int.Distance(a, center);
            float db = Vector2Int.Distance(b, center);
            return da.CompareTo(db);
        });

        return candidates.GetRange(0, Mathf.Min(count, candidates.Count));
    }

    //Result Container
    public class EscalationResult
    {
        public List<Unit> units = new List<Unit>();
        public List<Vector2Int> positions = new List<Vector2Int>();

        public bool HasReinforcements => units.Count > 0;
    }
}
