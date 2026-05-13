using UnityEngine;
using System.Collections.Generic;

public static class MapDatabase
{
    public static Dictionary<string, MapDefinition> Definitions =
        new Dictionary<string, MapDefinition>()
        {
            { MapIds.Default, BuildDefaultMap() },
            { MapIds.TestRoom, BuildTestRoomMap() },
        };

    // Flat open 8x5 — matches the original hardcoded grid
    static MapDefinition BuildDefaultMap()
    {
        return new MapDefinition(MapIds.Default, 8, 5);
    }

    // 8x5 map with a room containing a wall, a door, and a window
    // Room: columns 2-5, rows 1-3
    // North wall: north edge of tiles y=3, x=2..4 (east edge of x=5 closes corner)
    // West wall: east edge of tiles x=1, y=1..3
    // East wall: east edge of tiles x=4, y=1..3 — with a window at y=2
    // South wall: north edge of tiles y=0, x=2..4 — with a door at x=3
    static MapDefinition BuildTestRoomMap()
    {
        var map = new MapDefinition(MapIds.TestRoom, 8, 5);

        // Difficult terrain patch outside the room
        map.SetTerrain(0, 2, TileData.TerrainType.Difficult);
        map.SetTerrain(0, 3, TileData.TerrainType.Difficult);
        map.SetTerrain(1, 2, TileData.TerrainType.Difficult);

        // High ground in the top-right corner
        map.SetTerrain(6, 3, TileData.TerrainType.HighGround);
        map.SetTerrain(7, 3, TileData.TerrainType.HighGround);
        map.SetTerrain(6, 4, TileData.TerrainType.HighGround);
        map.SetTerrain(7, 4, TileData.TerrainType.HighGround);

        // North wall of room: north edge of y=3, x=2,3,4,5
        map.SetNorthEdge(2, 3, EdgeData.EdgeType.Wall);
        map.SetNorthEdge(3, 3, EdgeData.EdgeType.Wall);
        map.SetNorthEdge(4, 3, EdgeData.EdgeType.Wall);
        map.SetNorthEdge(5, 3, EdgeData.EdgeType.Wall);

        // South wall of room: north edge of y=0, x=2,3,4,5
        // Door at x=3 (north edge of tile (3,0))
        map.SetNorthEdge(2, 0, EdgeData.EdgeType.Wall);
        map.SetNorthEdge(3, 0, EdgeData.EdgeType.Door);
        map.SetNorthEdge(4, 0, EdgeData.EdgeType.Wall);
        map.SetNorthEdge(5, 0, EdgeData.EdgeType.Wall);

        // West wall of room: east edge of x=1, y=1,2,3
        map.SetEastEdge(1, 1, EdgeData.EdgeType.Wall);
        map.SetEastEdge(1, 2, EdgeData.EdgeType.Wall);
        map.SetEastEdge(1, 3, EdgeData.EdgeType.Wall);

        // East wall of room: east edge of x=5, y=1,2,3
        // Window at y=2
        map.SetEastEdge(5, 1, EdgeData.EdgeType.Wall);
        map.SetEastEdge(5, 2, EdgeData.EdgeType.Window);
        map.SetEastEdge(5, 3, EdgeData.EdgeType.Wall);

        return map;
    }

    public static MapDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out MapDefinition map)) return map;

        Debug.LogWarning($"[MapDatabase] Map not found: {id}");
        return null;
    }
}
