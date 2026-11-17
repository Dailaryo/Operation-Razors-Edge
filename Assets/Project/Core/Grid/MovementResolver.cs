using UnityEngine;
using System.Collections.Generic;

public static class MovementResolver
{
    //Returns all tiles reachable by the unit within its MoveSpeed budget.
    //Accounts for diagonal cost, terrain MoveCost, edge barriers, and occupancy.
    public static HashSet<Vector2Int> GetReachableTiles(Unit unit, CombatResolver resolver)
    {
        Vector2Int start = resolver.GetPosition(unit);
        int budget = resolver.GetMoveSpeed(unit);

        //Dijkstra over the tile grid
        //Key: tile position, Value: cheapest cost to reach it
        Dictionary<Vector2Int, int> costSoFar = new Dictionary<Vector2Int, int>();

        //Min-priority queue implemented as a sorted list of (cost, position)
        List<(int cost, Vector2Int pos, bool nextDiagCheap)> frontier =
            new List<(int, Vector2Int, bool)>();

        costSoFar[start] = 0;
        frontier.Add((0, start, true));

        HashSet<Vector2Int> reachable = new HashSet<Vector2Int>();

        while (frontier.Count > 0)
        {
            //Pop lowest cost entry
            frontier.Sort((a, b) => a.cost.CompareTo(b.cost));
            var (currentCost, current, nextDiagCheap) = frontier[0];
            frontier.RemoveAt(0);

            //Skip if we've already found a cheaper path here
            if (currentCost > costSoFar.GetValueOrDefault(current, int.MaxValue)) continue;

            //All 8 neighbors
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    Vector2Int neighbor = new Vector2Int(current.x + dx, current.y + dy);

                    if (!GridManager.Instance.IsWithinBounds(neighbor)) continue;

                    //Check edge traversal
                    if (!CanTraverse(current, neighbor, dx, dy)) continue;

                    // Can't stop here if occupied
                    bool canStop = IsAnchorClear(unit, neighbor, resolver);

                    // Can pass through if ally or large size difference
                    bool canPassThrough = CanPassThrough(unit, neighbor, resolver);

                    if (!canPassThrough) continue;  // Can't even enter this tile

                    //Get tile surface cost
                    TileData tileData = GridManager.Instance.GetTileData(neighbor);
                    if (tileData == null) continue;
                    if (tileData.MoveCost == int.MaxValue) continue;

                    //Compute step cost using diagonal rule
                    bool diagCheapCopy = nextDiagCheap;
                    int stepCost = DistanceUtility.DiagonalMoveCost(current, neighbor, ref diagCheapCopy);
                    int totalCost = currentCost + stepCost * tileData.MoveCost;

                    if (totalCost > budget) continue;

                    if (!costSoFar.ContainsKey(neighbor) || totalCost < costSoFar[neighbor])
                    {
                        costSoFar[neighbor] = totalCost;
                        if (canStop) reachable.Add(neighbor);
                        frontier.Add((totalCost, neighbor, diagCheapCopy));
                    }
                }
            }
        }

        //Remove starting tile: unit is already there
        reachable.Remove(start);

        return reachable;
    }

    //Returns true if the unit can reach the destination within MoveSpeed.
    public static bool IsReachable(Unit unit, Vector2Int destination, CombatResolver resolver)
    {
        if (!GridManager.Instance.IsWithinBounds(destination)) return false;
        if (resolver.IsTileOccupied(destination, unit)) return false;

        HashSet<Vector2Int> reachable = GetReachableTiles(unit, resolver);
        return reachable.Contains(destination);
    }

    //Returns an ordered path from unit position to destination
    //Stub for future move-animation sequencing.  Returns empty list if unreachable.
    public static List<Vector2Int> GetPath(Unit unit, Vector2Int destination, CombatResolver resolver)
    {
        Vector2Int start = resolver.GetPosition(unit);
        int budget = resolver.GetMoveSpeed(unit);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> costSoFar = new Dictionary<Vector2Int, int>();
        List<(int cost, Vector2Int pos, bool nextDiagCheap)> frontier =
            new List<(int, Vector2Int, bool)>();

        costSoFar[start] = 0;
        frontier.Add((0, start, true));

        while (frontier.Count > 0)
        {
            frontier.Sort((a, b) => a.cost.CompareTo(b.cost));
            var (currentCost, current, nextDiagCheap) = frontier[0];
            frontier.RemoveAt(0);

            if (current == destination) break;

            if (currentCost > costSoFar.GetValueOrDefault(current, int.MaxValue)) continue;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    Vector2Int neighbor = new Vector2Int(current.x + dx, current.y + dy);

                    if (!GridManager.Instance.IsWithinBounds(neighbor)) continue;
                    if (!CanTraverse(current, neighbor, dx, dy)) continue;
                    if (!CanPassThrough(unit, neighbor, resolver)) continue;

                    TileData tileData = GridManager.Instance.GetTileData(neighbor);
                    if(tileData == null) continue;
                    if (tileData.MoveCost == int.MaxValue) continue;

                    bool diagCheapCopy = nextDiagCheap;
                    int stepCost = DistanceUtility.DiagonalMoveCost(current, neighbor, ref diagCheapCopy);
                    int totalCost = currentCost + stepCost * tileData.MoveCost;

                    if (totalCost > budget) continue;

                    if (!costSoFar.ContainsKey(neighbor) || totalCost < costSoFar[neighbor])
                    {
                        costSoFar[neighbor] = totalCost;
                        cameFrom[neighbor] = current;
                        frontier.Add((totalCost, neighbor, diagCheapCopy));
                    }
                }
            }
        }

        //Reconstruct path
        if (!cameFrom.ContainsKey(destination)) return new List<Vector2Int>();

        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int step = destination;

        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }

        path.Reverse();
        return path;
    }

    //Checks whether movement from 'from' to 'to' is permitted by edge barriers.
    //For cardinal moves: checks the singel shared edge.
    //For diagonal moves: blocked only if BOTH intermediate edges block movement.
    private static bool CanTraverse(Vector2Int from, Vector2Int to, int dx, int dy)
    {
        bool isDiagonal = dx != 0 && dy!= 0;

        if (!isDiagonal)
        {
            EdgeData edge = GridManager.Instance.GetEdge(from, to);
            return edge == null || !edge.BlocksMovement;
        }

        //Diagonal: check the two intermediate cardinal edges
        //Intermediate tile A shares the cardinal-x edge with 'from'
        //Intermediate tile B shares the cardinal-y edge with 'from'
        Vector2Int intermediateX = new Vector2Int(from.x + dx, from.y);
        Vector2Int intermediateY = new Vector2Int(from.x, from.y + dy);

        EdgeData edgeX = GridManager.Instance.GetEdge(from, intermediateX);
        EdgeData edgeY = GridManager.Instance.GetEdge(from, intermediateY);

        bool blockedX = edgeX != null && edgeX.BlocksMovement;
        bool blockedY = edgeY != null && edgeY.BlocksMovement;

        //Blocked only if BOTH intermediate edges block
        //One open gap is sufficient
        return !(blockedX && blockedY);
    }

    private static bool CanPassThrough(Unit mover, Vector2Int pos, CombatResolver resolver)
    {
        Unit occupant = resolver.GetUnitAt(pos);
        if (occupant == null) return true;
        if (occupant.IsDead()) return true;

        //Allies can always be passed through
        if (occupant.isEnemy == mover.isEnemy) return true;

        //Enemies: allow if size deifference is 3 or more categories
        int sizeDiff = Mathf.Abs((int)mover.sizeCategory - (int)occupant.sizeCategory);
        return sizeDiff >= 3;
    }

    private static bool IsAnchorClear(Unit unit, Vector2Int anchor, CombatResolver resolver)
    {
        int size = unit.sourceCharacterData != null
            ? unit.sourceCharacterData.reachProfile.footprintSize
            : 1;

        for (int dx = 0; dx < size; dx++)
        {
            for (int dy = 0; dy < size; dy++)
            {
                Vector2Int tile = new Vector2Int(anchor.x + dx, anchor.y + dy);

                if (!GridManager.Instance.IsWithinBounds(tile)) return false;
                if (resolver.IsTileOccupied(tile, unit)) return false;
            }
        }

        return true;
    }

    // Walks the unit along its path one tile at a time.
    // In live mode only — simulation still uses direct position assignment in ResolveMove().
    public static bool ExecuteMovePath(Unit mover, Vector2Int destination, CombatResolver resolver, List<Vector2Int> walked = null)
    {
        List<Vector2Int> path = GetPath(mover, destination, resolver);

        if (path.Count == 0)
        {
            mover.provokeImmunity = false; //clear regardless of how the move ended
            return false;
        }

        Vector2Int previousTile = mover.position;
        AudioEventBus.Fire(AudioEventIds.Movement.UnitMoveStart);
        foreach (Vector2Int nextTile in path)
        {
            GridManager.Instance.MoveUnit(mover, nextTile);
            walked?.Add(nextTile);
            AudioEventBus.Fire(AudioEventIds.Movement.UnitMoveStep);
            ThreatZoneManager.UpdateUnit(mover);

            // Re-check detection after each step so a unit moving into LoS
            // of an opponent is detected immediately rather than next turn.
            List<Unit> allUnits = resolver.GetAllUnits();
            List<Unit> opponents = allUnits.FindAll(u => u.isEnemy != mover.isEnemy && !u.IsDead());
            DetectionStateManager.RunPassiveSweep(opponents, new List<Unit> { mover });

            // Check for OA provocation from the tile just left
            if (!mover.provokeImmunity)
            {
                List<Unit> threateners = ThreatZoneManager.GetThreateningUnits(
                    previousTile, isEnemy: !mover.isEnemy);

                List<OARecord> oaQueue = new List<OARecord>();

                foreach (Unit threatener in threateners)
                {
                    if (!DetectionStateManager.CanTarget(threatener, mover))
                        continue;

                    if (DetectionStateManager.Get(mover, threatener) == DetectionState.Hidden
                        && !threatener.oaState.reactWhileHidden)
                        continue;

                    if (threatener.oaState.CanTakeOA())
                        oaQueue.Add(new OARecord(threatener, mover, previousTile));
                }

                if (oaQueue.Count > 0)
                {
                    resolver.ResolveInterruptOAs(oaQueue);

                    if (mover.IsDead())
                        break;
                }

                // Overwatch Specialist — trigger OA if mover enters an active field zone tile
                if (FieldZoneManager.IsTileInActiveZone(nextTile))
                {
                    List<OARecord> overwatchQueue = new List<OARecord>();

                    foreach (Unit watcher in allUnits)
                    {
                        if (watcher.isEnemy == mover.isEnemy) continue;
                        if (watcher.IsDead() || watcher.IsDying()) continue;
                        if (!watcher.oaState.CanTakeOA()) continue;
                        if (!watcher.activeEffects.Exists(e => e is OverwatchSpecialistEffect)) continue;
                        if (!DetectionStateManager.CanTarget(watcher, mover)) continue;

                        overwatchQueue.Add(new OARecord(watcher, mover, nextTile));
                    }

                    if (overwatchQueue.Count > 0)
                    {
                        resolver.ResolveInterruptOAs(overwatchQueue);
                        if (mover.IsDead()) break;
                    }
                }
            }

            previousTile = nextTile;
        }
        AudioEventBus.Fire(AudioEventIds.Movement.UnitMoveEnd);

        // Clear provoke immunity regardless of how the move ended
        mover.provokeImmunity = false;

        return true;
    }
}
