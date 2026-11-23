using System.Collections.Generic;
using UnityEngine;

public class JuggernautUnstoppableChargeEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation) return;

        if (ctx.attacker == null || ctx.finalTarget == null) return;

        Vector2Int start = resolver.GetPosition(ctx.attacker);
        Vector2Int end   = resolver.GetPosition(ctx.finalTarget);

        // Build the cardinal/diagonal path from attacker to one tile short of target
        List<Vector2Int> path = BuildPath(start, end);

        if (!resolver.IsSimulation)
        {
            // Displace any unit on each intermediate tile via forced movement
            Vector2Int dir = end - start;
            int mag = Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
            Vector2Int normalized = mag > 0
                ? new Vector2Int(
                    dir.x != 0 ? dir.x / Mathf.Abs(dir.x) : 0,
                    dir.y != 0 ? dir.y / Mathf.Abs(dir.y) : 0)
                : Vector2Int.zero;

            foreach (Vector2Int tile in path)
            {
                Unit occupant = resolver.GetUnitAt(tile);
                if (occupant != null && occupant != ctx.attacker && occupant != ctx.finalTarget)
                {
                    resolver.ResolveForcedMove(occupant, normalized, 1);
                    CombatLog.Add($"{ctx.attacker.unitName} displaces {occupant.unitName}!");
                }
            }

            // Move attacker to the tile adjacent to the target
            if (path.Count > 0)
            {
                Vector2Int destination = path[path.Count - 1];
                ctx.movePosition = destination;
                resolver.ResolveMove(ctx);
            }
        }

        // The melee attack is handled by the action's dealsDamage flag and ResolveHit
        // in CombatResolver.ExecuteAction — no additional damage call needed here
        CombatLog.Add($"{ctx.attacker.unitName} Unstoppable Charge toward {ctx.finalTarget.unitName}!");
    }

    private List<Vector2Int> BuildPath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = start;

        while (true)
        {
            int dx = end.x - current.x;
            int dy = end.y - current.y;

            if (dx == 0 && dy == 0) break;

            int stepX = dx != 0 ? (dx > 0 ? 1 : -1) : 0;
            int stepY = dy != 0 ? (dy > 0 ? 1 : -1) : 0;

            current = new Vector2Int(current.x + stepX, current.y + stepY);

            // Stop one tile short — that's the attacker's landing tile
            if (current == end) break;

            path.Add(current);
        }

        // The last tile added is where the attacker lands (adjacent to target)
        if (current != start && current != end)
            path.Add(current);
        else if (DistanceUtility.IsAdjacent(start, end))
        {
            // Already adjacent — no movement, just the attack
            path.Clear();
        }

        return path;
    }
}
