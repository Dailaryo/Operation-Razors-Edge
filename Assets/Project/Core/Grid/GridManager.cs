using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int width = 8;
    public int height = 5;

    private Dictionary<Vector2Int, Unit> grid = new Dictionary<Vector2Int, Unit>();

    private TileData[,] tileData;

    public const string EmergencyExtractionZoneId = "emergency_extraction_zone";

    private void Awake()
    {
        Instance = this;
        LoadMap();
    }

    public bool IsTileOccupied(Vector2Int pos)
    {
        return grid.ContainsKey(pos);
    }

    public bool IsWithinBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    public bool TryPlaceUnit(Unit unit, Vector2Int pos)
    {
        List<Vector2Int> footprint = GetFootprintTiles(unit, pos);

        //Verify all footprint tiles are in bounds and unoccupied before placing any
        foreach (Vector2Int tile in footprint)
        {
            if (!IsWithinBounds(tile)) return false;
            if (IsTileOccupied(tile)) return false;
        }

        //All clear: register every footprint tile
        foreach (Vector2Int tile in footprint)
        {
            grid[tile] = unit;
            tileData[tile.x, tile.y].occupant = unit;
        }

        unit.position = pos;
        return true;
    }

    public Unit GetUnitAt(Vector2Int pos)
    {
        if (grid.TryGetValue(pos, out Unit unit)) return unit;

        return null;
    }

    public void MoveUnit(Unit unit, Vector2Int newAnchor)
    {
        // Clear old footprint
        List<Vector2Int> oldFootprint = GetFootprintTiles(unit, unit.position);
        foreach (Vector2Int tile in oldFootprint)
        {
            if (grid.ContainsKey(tile) && grid[tile] == unit)
            {
                grid.Remove(tile);
                tileData[tile.x, tile.y].occupant = null;
            }
        }

        // Register new footprint
        List<Vector2Int> newFootprint = GetFootprintTiles(unit, newAnchor);
        foreach (Vector2Int tile in newFootprint)
        {
            grid[tile] = unit;
            tileData[tile.x, tile.y].occupant = unit;
        }

        unit.position = newAnchor;

        UnitViewManager.Instance.GetView(unit)?.UpdatePosition();
    }

    public void RemoveUnit(Unit unit)
    {
        List<Vector2Int> footprint = GetFootprintTiles(unit, unit.position);

        foreach (Vector2Int tile in footprint)
        {
            if (grid.ContainsKey(tile) && grid[tile] == unit)
            {
                grid.Remove(tile);
                tileData[tile.x, tile.y].occupant = null;
            }
        }
    }

    public void LoadMap(MapDefinition map = null)
    {
        if (map != null)
        {
            width = map.width;
            height = map.height;
        }

        tileData = new TileData[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map != null && map.tiles[x, y] != null)
                {
                    tileData[x, y] = map.tiles[x, y];
                }
                else
                {
                    TileData tile = new TileData();
                    tile.terrain = TileData.TerrainType.Open;
                    tile.northEdge = new EdgeData();
                    tile.eastEdge = new EdgeData();
                    tileData[x, y] = tile;
                }
            }
        }
    }

    public TileData GetTileData(Vector2Int pos)
    {
        if (!IsWithinBounds(pos)) return null;
        return tileData[pos.x, pos.y];
    }

    public EdgeData GetEdge(Vector2Int from, Vector2Int to)
    {
        Vector2Int diff = to - from;

        //Must be cardinal adjacent
        if (Mathf.Abs(diff.x) + Mathf.Abs(diff.y) != 1) return null;

        //North edge: owned by the southern tile (lower y)
        if (diff == Vector2Int.up)
        {
            TileData owner = GetTileData(from);
            return owner?.northEdge;
        }
        if (diff == Vector2Int.down)
        {
            TileData owner = GetTileData(to);
            return owner?.northEdge;
        }

        //East edge: owned by the western tile (lower x)
        if (diff == Vector2Int.right)
        {
            TileData owner = GetTileData(from);
            return owner?.eastEdge;
        }
        if (diff == Vector2Int.left)
        {
            TileData owner = GetTileData(to);
            return owner?.eastEdge;
        }

        return null;
    }

    public void SetDoorOpen(Vector2Int ownerPos, bool isNorthEdge, bool open)
    {
        TileData tile = GetTileData(ownerPos);
        if (tile == null) return;

        EdgeData edge = isNorthEdge ? tile.northEdge : tile.eastEdge;
        if (edge == null || edge.type != EdgeData.EdgeType.Door) return;

        edge.doorIsOpen = open;
    }

    public void SetWindowBroken(Vector2Int ownerPos, bool isNorthEdge)
    {
        TileData tile = GetTileData(ownerPos);
        if (tile == null) return;

        EdgeData edge = isNorthEdge ? tile.northEdge : tile.eastEdge;
        if (edge == null || edge.type != EdgeData.EdgeType.Window) return;

        edge.windowIsBroken = true;
    }

    public void SetTerrain(Vector2Int pos, TileData.TerrainType type)
    {
        TileData tile = GetTileData(pos);
        if (tile == null) return;

        tile.terrain = type;
    }

    //Zone Registry
    private Dictionary<string, List<Vector2Int>> zones = new Dictionary<string, List<Vector2Int>>();

    public void RegisterZone(string zoneId, List<Vector2Int> tiles)
    {
        zones[zoneId] = tiles;
    }

    public List<Unit> GetUnitsInZone(string zoneId)
    {
        List<Unit> result = new List<Unit>();

        if (!zones.TryGetValue(zoneId, out List<Vector2Int> tiles)) return result;

        foreach (Vector2Int tile in tiles)
        {
            Unit unit = GetUnitAt(tile);
            if (unit != null) result.Add(unit);
        }

        return result;
    }

    public bool IsUnitInZone(Unit unit, string zoneId)
    {
        if (!zones.TryGetValue(zoneId, out List<Vector2Int> tiles)) return false;

        foreach (Vector2Int tile in tiles)
        {
            if (tile == unit.position) return true;
        }

        return false;
    }

    public List<Vector2Int> GetZoneTiles(string zoneId)
    {
        if (zones.TryGetValue(zoneId, out List<Vector2Int> tiles))
            return tiles;
        return new List<Vector2Int>();
    }

    // Generates and registers a 2-column × 3-row extraction zone anchored
    // to the squad's entry edge. The zone covers the 3 rows nearest to the
    // vertical or horizontal centre of the entry edge, 2 tiles deep.
    // Returns the registered zone ID.
    public string GenerateExtractionZone(MapDefinition.EntryZone EntryZone)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();

        // Anchor the zone to the centre of the entry edge
        int centreRow = height / 2;
        int centreCol = width / 2;

        switch (EntryZone)
        {
            case MapDefinition.EntryZone.West:
                // 2 columns deep from left, 3 rows centred vertically
                for (int x = 0; x <= 1; x++)
                    for (int y = centreRow - 1; y <= centreRow + 1; y++)
                        if (IsWithinBounds(new Vector2Int(x, y)))
                            tiles.Add(new Vector2Int(x, y));
                break;

            case MapDefinition.EntryZone.East:
                // 2 columns deep from right, 3 rows centred vertically
                for (int x = width - 2; x <= width - 1; x++)
                    for (int y = centreRow - 1; y <= centreRow + 1; y++)
                        if (IsWithinBounds(new Vector2Int(x, y)))
                            tiles.Add(new Vector2Int(x, y));
                break;

            case MapDefinition.EntryZone.South:
                // 2 rows deep from bottom, 3 columns centred horizontally
                for (int y = 0; y <= 1; y++)
                    for (int x = centreCol - 1; x <= centreCol + 1; x++)
                        if (IsWithinBounds(new Vector2Int(x, y)))
                            tiles.Add(new Vector2Int(x, y));
                break;

            case MapDefinition.EntryZone.North:
                // 2 rows deep from top, 3 columns centred horizontally
                for (int y = height - 2; y <= height - 1; y++)
                    for (int x = centreCol - 1; x <= centreCol + 1; x++)
                        if (IsWithinBounds(new Vector2Int(x, y)))
                            tiles.Add(new Vector2Int(x, y));
                break;
        }

        RegisterZone(EmergencyExtractionZoneId, tiles);
        CombatLog.Add($"[Extraction] Emergency extraction zone generated at {EntryZone} edge ({tiles.Count} tiles).");
        return EmergencyExtractionZoneId;
    }

    private List<Vector2Int> GetFootprintTiles(Unit unit, Vector2Int anchor)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();

        int size = unit.sourceCharacterData != null
            ? unit.sourceCharacterData.reachProfile.footprintSize
            : 1;

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
