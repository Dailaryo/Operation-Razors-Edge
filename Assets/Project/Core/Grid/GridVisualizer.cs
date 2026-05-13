using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    public GameObject tilePrefab;

    public float tileSize = 1.1f;

    public static GridVisualizer Instance;

    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();

    private void Start()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        GridManager grid = GridManager.Instance;

        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                ////Debug.Log($"Spawning tile at {x},{y}");

                Vector3 worldPos = GridToWorld(x, y);

                GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.identity);

                Tile tileComponent = tile.GetComponent<Tile>();
                tileComponent.gridPosition = new Vector2Int(x, y);

                tiles[tileComponent.gridPosition] = tileComponent;
                
                //Apply terrain color
                TileData tileData = GridManager.Instance.GetTileData(tileComponent.gridPosition);
                if (tileData != null) tileComponent.SetTerrainColor(tileData.terrain);

                tile.name = $"Tile_{x}_{y}";
                tile.transform.SetParent(transform, true);

                ////Debug.Log("World pos: + worldPos");
            }
        }

        ////Debug.Log("Grid size: " + grid.width + " x " + grid.height);
    }

    Vector3 GridToWorld(int x, int y)
    {
        return new Vector3(x * tileSize, 0, y * tileSize);
    }

    public Tile GetTileAt(Vector2Int pos)
    {
        if (tiles.TryGetValue(pos, out Tile tile)) return tile;

        return null;
    }

    public Dictionary<Vector2Int, Tile> GetAllTiles()
    {
        return tiles;
    }

    public void RefreshTerrainColors()
    {
        foreach (var kvp in tiles)
        {
            TileData tileData = GridManager.Instance.GetTileData(kvp.Key);
            if (tileData != null)
                kvp.Value.SetTerrainColor(tileData.terrain);
        }
    }

    void Awake()
    {
        Instance = this;
    }
}
