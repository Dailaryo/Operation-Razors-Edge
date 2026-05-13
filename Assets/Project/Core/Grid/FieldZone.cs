using System.Collections.Generic;
using UnityEngine;

public class FieldZone
{
    public string zoneId;
    public Vector2Int center;
    public int radius;
    public int turnsRemaining;
    public List<System.Func<StatusEffect>> effectFactories;

    public FieldZone(string zoneId, Vector2Int center, int radius,
        int duration, List<System.Func<StatusEffect>> effectFactories)
    {
        this.zoneId = zoneId;
        this.center = center;
        this.radius = radius;
        this.turnsRemaining = duration;
        this.effectFactories = effectFactories;
    }

    public List<Vector2Int> GetTiles()
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        for (int dx = -radius; dx <= radius; dx++)
            for (int dy = -radius; dy <= radius; dy++)
            {
                Vector2Int tile = new Vector2Int(center.x + dx, center.y + dy);
                if (GridManager.Instance.IsWithinBounds(tile))
                    tiles.Add(tile);
            }
        return tiles;
    }
}