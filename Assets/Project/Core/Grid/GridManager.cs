using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int width = 10;
    public int height = 8;
    // Entry edge of the currently loaded map. Set by LoadMap; West when no map is given.
    public MapDefinition.EntryZone squadEntryZone = MapDefinition.EntryZone.West;


    // Enemies spawn on the edge opposite the squad's entry. Defaults to East (opposite West).
    public MapDefinition.EntryZone EnemyEntryZone
    {
        get
        {
            switch (squadEntryZone)
            {
                case MapDefinition.EntryZone.East: return MapDefinition.EntryZone.West;
                case MapDefinition.EntryZone.South: return MapDefinition.EntryZone.North;
                case MapDefinition.EntryZone.North: return MapDefinition.EntryZone.South;
                case MapDefinition.EntryZone.West:
                default: return MapDefinition.EntryZone.East;
            }
        }
    }

    private Dictionary<Vector2Int, Unit> grid = new Dictionary<Vector2Int, Unit>();

    private TileData[,] tileData;

    public const string EmergencyExtractionZoneId = "emergency_extraction_zone";
    public const string EnemySpawnZoneId = "enemy_spawn";

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

        squadEntryZone = map != null ? map.squadEntryZone : MapDefinition.EntryZone.West;

        //Zones belong to the map that registered them: clear the registry, then register this map's own zones.
        //(The emergency extraction zone is generated after LoadMap by GameFlowManager.StartCombat.)
        zones.Clear();
        if (map != null && map.enemySpawnTiles.Count > 0)
            RegisterZone(EnemySpawnZoneId, new List<Vector2Int>(map.enemySpawnTiles));

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

    // Spawn tiles for a map edge, ordered from the edge inward one line at a time.
    // Within each line, tiles run from the centre outward: centre, +1, -1, +2, -2...
    // Impassable tiles are skipped. Occupancy and footprint are not checked here;
    // TryPlaceUnit handles both, so callers just try each tile in order
    public List<Vector2Int> GetSpawnTiles(MapDefinition.EntryZone edge)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();

        bool verticalEdge = edge != MapDefinition.EntryZone.South && edge != MapDefinition.EntryZone.North; // anything not South/North is treated as West/East
        int depthCount = verticalEdge ? width : height; //how many lines from the edge inward
        int lineLength = verticalEdge ? height : width; //how many tiles along each line
        int center = lineLength / 2;

        for (int depth = 0; depth < depthCount; depth++)
        {
            for (int offset = 0; offset < lineLength; offset++)
            {
                for (int sign = 1; sign >= -1; sign -= 2)
                {
                    if (offset == 0 && sign == -1) continue; //center tile only once

                    int along = center + (offset * sign);
                    if (along < 0 || along >= lineLength) continue;

                    Vector2Int pos;
                    switch (edge)
                    {
                        case MapDefinition.EntryZone.East: pos = new Vector2Int(width - 1 - depth, along); break;
                        case MapDefinition.EntryZone.South: pos = new Vector2Int(along, depth); break;
                        case MapDefinition.EntryZone.North: pos = new Vector2Int(along, height - 1 - depth); break;
                        case MapDefinition.EntryZone.West:
                        default: pos = new Vector2Int(depth, along); break;
                    }

                    if (tileData[pos.x, pos.y].terrain == TileData.TerrainType.Impassable) continue;

                    tiles.Add(pos);
                }
            }
        }

        return tiles;
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
