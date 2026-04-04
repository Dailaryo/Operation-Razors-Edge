using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int width = 8;
    public int height = 5;

    private Dictionary<Vector2Int, Unit> grid = new Dictionary<Vector2Int, Unit>();

    private void Awake()
    {
        Instance = this;
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
        if (!IsWithinBounds(pos)) return false;
        if (IsTileOccupied(pos)) return false;

        grid[pos] = unit;
        unit.position = pos;

        return true;
    }

    public Unit GetUnitAt(Vector2Int pos)
    {
        if (grid.TryGetValue(pos, out Unit unit)) return unit;

        return null;
    }

    public void MoveUnit(Unit unit, Vector2Int newPos)
    {
        //Remove old position
        if (grid.ContainsKey(unit.position))
        {
            grid.Remove(unit.position);
        }

        //Set new position
        grid[newPos] = unit;
        unit.position = newPos;
    }

    public void RemoveUnit(Unit unit)
    {
        if (grid.ContainsKey(unit.position))
            {
            grid.Remove(unit.position);
        }
    }
}
