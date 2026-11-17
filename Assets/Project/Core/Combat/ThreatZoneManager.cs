using UnityEngine;
using System.Collections.Generic;

public static class ThreatZoneManager
{
    private static Dictionary<Unit, HashSet<Vector2Int>> threatCache
        = new Dictionary<Unit, HashSet<Vector2Int>>();

    //Called by CombatManager.StartCombat() for every unit.
    //Also call for reinforcement units in PlaceReinforcements).
    public static void RegisterUnit(Unit unit)
    {
        threatCache[unit] = ComputeThreatenedTiles(unit);
    }

    //Call after any state change that could affect threat:
    //movement, disarm, relevant status effect applied/removed.
    public static void UpdateUnit(Unit unit)
    {
        if (!threatCache.ContainsKey(unit)) return;
        threatCache[unit] = ComputeThreatenedTiles(unit);
    }

    //Called by CombatResolver.HandleUnitDeath
    public static void UnregisterUnit(Unit unit)
    {
        threatCache.Remove(unit);
    }

    //Returns the cahced set of tiles threatened by this unit.
    //Used by GridHighlighter and the interrupt pipeline.
    public static HashSet<Vector2Int> GetThreatenedTiles(Unit unit)
    {
        if (threatCache.TryGetValue(unit, out var tiles))
            return tiles;

        return new HashSet<Vector2Int>();
    }

    // Returns all units on the specified team that currently threaten the given tile.
    // isEnemy: true = return enemy units threatening this tile,
    //          false = return player units threatening this tile.
    public static List<Unit> GetThreateningUnits(Vector2Int tile, bool isEnemy)
    {
        List<Unit> result = new List<Unit>();

        foreach (var kvp in threatCache)
        {
            Unit unit = kvp.Key;
            HashSet<Vector2Int> tiles = kvp.Value;

            if (unit.isEnemy != isEnemy) continue;
            if (!unit.CanAct()) continue;

            if (tiles.Contains(tile))
                result.Add(unit);
        }

        return result;
    }

    //Clears all cached data.  Call when combat ends to prevent stale data
    //persisting into the next combat session
    public static void Reset()
    {
        threatCache.Clear();
    }

    private static HashSet<Vector2Int> ComputeThreatenedTiles(Unit unit)
    {
        HashSet<Vector2Int> result = new HashSet<Vector2Int>();

        //Training targets (Static / Mindless) never act, so they threaten nothing and make no OAs
        if (unit.isInvulnerableTarget) return result;

        //A unit with no melee action available threatens nothing
        bool hasMeleeAction = unit.GetAllActions()
            .Exists(a => a.attackType == ActionDefinition.AttackType.Melee);

        if (!hasMeleeAction) return result;

        //Unarmed without Combat Martial Arts threatens nothing, so no OAs (d20).
        //DEFERRED until weapons are implemented: every pregen is unarmed today, so enabling this would remove most squad OAs
        //if (!unit.CanThreatenUnarmed()) return result;

        //Get reach profile, fall back to default if no CharacterData
        ReachProfile reach = unit.sourceCharacterData != null
            ? unit.sourceCharacterData.reachProfile
            : ReachProfile.Default;

        //Build footprint tile list
        List<Vector2Int> footprint = GetFootprintTiles(unit);

        //Chebyshev pre-filter ceiling: any candidate beyond this is provably out of reach
        int chebyshevCeiling = reach.maxReach;

        //Check every tile on the map
        int width = GridManager.Instance.width;
        int height = GridManager.Instance.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int candidate = new Vector2Int(x, y);

                //Fast pre-filter: skip if every footprint tile is too far by Chebyshev
                bool passesPreFilter = false;
                foreach (Vector2Int fp in footprint)
                {
                    if (DistanceUtility.Chebyshev(fp, candidate) <= chebyshevCeiling)
                    {
                        passesPreFilter = true;
                        break;
                    }
                }
                if (!passesPreFilter) continue;

                //Full distance check: include if within reach range from any footprint tile
                foreach (Vector2Int fp in footprint)
                {
                    int dist = DistanceUtility.MovementCostDistance(fp, candidate);
                    if (dist >= reach.minReach && dist <= reach.maxReach)
                    {
                        result.Add(candidate);
                        break;
                    }
                }
            }
        }

        return result;
    }

    private static List<Vector2Int> GetFootprintTiles(Unit unit)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();

        int size = unit.sourceCharacterData != null
            ? unit.sourceCharacterData.reachProfile.footprintSize
            : 1;

        Vector2Int anchor = unit.position;

        for (int dx = 0; dx < size; dx++)
        {
            for (int dy = 0; dy < size; dy++)
            {
                tiles.Add(new Vector2Int(anchor.x + dx, anchor.y + dy));
            }
        }

        return tiles;
    }
}
