using System.Collections.Generic;
using UnityEngine;

public class MapDefinition
{
    public string mapId;
    public int width;
    public int height;
    public TileData[,] tiles;

    //TEMP: Edge only at the moment, will scale up as maps are authored
    public enum EntryZone
    {
        West,
        East,
        South,
        North
    }

    public EntryZone squadEntryZone = EntryZone.West;

    //Authored enemy spawn tiles, registered as GridManager.EnemySpawnZoneId on load. Tried before the
    //opposite-edge fill; escalation waves can target them with SpawnPattern.SpecificLocation.
    public List<Vector2Int> enemySpawnTiles = new List<Vector2Int>();

    public MapDefinition(string mapId, int width, int height)
    {
        this.mapId = mapId;
        this.width = width;
        this.height = height;
        this.tiles = new TileData[width, height];

        //Intialize all tiles to Open with blank edges
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileData tile = new TileData();
                tile.terrain = TileData.TerrainType.Open;
                tile.northEdge = new EdgeData();
                tile.eastEdge = new EdgeData();
                tiles[x, y] = tile;
            }
        }
    }

    //Convenience: set terrain on a single tile
    public void SetTerrain(int x, int y, TileData.TerrainType terrain)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return;
        tiles[x, y].terrain = terrain;
    }

    //Convenience: set a north edge on a tile
    public void SetNorthEdge(int x, int y, EdgeData.EdgeType type)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return;
        tiles[x, y].northEdge.type = type;
    }

    //Convenience: set a east edge on a tile
    public void SetEastEdge(int x, int y, EdgeData.EdgeType type)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return;
        tiles[x, y].eastEdge.type = type;
    }
}
