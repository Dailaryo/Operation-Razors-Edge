using System.Collections.Generic;
using UnityEngine;

public static class FieldZoneManager
{
    private static List<FieldZone> activeZones = new List<FieldZone>();
    private static int nextZoneIndex = 0;

    public static void PlaceZone(Vector2Int center, int radius, int duration,
        List<System.Func<StatusEffect>> effectFactories)
    {
        string zoneId = $"field_zone_{nextZoneIndex++}";
        var zone = new FieldZone(zoneId, center, radius, duration, effectFactories);
        activeZones.Add(zone);
        GridManager.Instance.RegisterZone(zoneId, zone.GetTiles());
        CombatLog.Add($"Field zone placed at {center} for {duration} turns.");
        RefreshVisuals();
    }

    public static void ProcessEndPhase(List<Unit> allUnits)
    {
        for (int i = activeZones.Count - 1; i >= 0; i--)
        {
            FieldZone zone = activeZones[i];
            List<Vector2Int> tiles = zone.GetTiles();

            foreach (Unit unit in allUnits)
            {
                if (unit.IsDead() || unit.IsDying()) continue;
                if (!tiles.Contains(unit.position)) continue;

                foreach (var factory in zone.effectFactories)
                {
                    var effect = factory();
                    if (!unit.activeEffects.Exists(e => e.GetType() == effect.GetType()))
                        unit.AddEffect(effect);
                }
            }

            zone.turnsRemaining--;

            if (zone.turnsRemaining <= 0)
            {
                CombatLog.Add($"Field zone at {zone.center} has expired.");
                activeZones.RemoveAt(i);
                RefreshVisuals();
            }
        }
    }

    public static bool IsTileInActiveZone(Vector2Int tile)
    {
        foreach (var zone in activeZones)
        {
            if (zone.GetTiles().Contains(tile)) return true;
        }
        return false;
    }

    public static List<FieldZone> GetActiveZones() => activeZones;

    public static void Reset()
    {
        activeZones.Clear();
        nextZoneIndex = 0;
        RefreshVisuals();
    }

    //Repaint zone tiles (view only; zones themselves are pure data)
    static void RefreshVisuals()
    {
        if (GridHighlighter.Instance != null)
            GridHighlighter.Instance.RefreshFieldZones();
    }
}