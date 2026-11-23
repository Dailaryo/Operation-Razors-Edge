using System.Collections.Generic;
using UnityEngine;

public class ArtilleristSuppressionFireEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation) return;

        if (ctx.attacker == null || ctx.finalTarget == null) return;

        Vector2Int attackerPos = resolver.GetPosition(ctx.attacker);
        Vector2Int targetPos   = resolver.GetPosition(ctx.finalTarget);

        List<Vector2Int> lineTiles = GetLineTiles(attackerPos, targetPos);

        int level  = ctx.attacker.sourceCharacterData?.level ?? 1;
        int powMod = ctx.attacker.GetModifier(StatType.Power);
        int saveDC = 10 + (level / 2) + powMod;

        foreach (Unit unit in resolver.GetAllUnits())
        {
            if (resolver.IsUnitDead(unit)) continue;
            if (unit.isEnemy == ctx.attacker.isEnemy) continue;
            if (unit == ctx.attacker) continue;

            Vector2Int unitPos = resolver.GetPosition(unit);

            if (!lineTiles.Contains(unitPos)) continue;

            // Primary target is hit directly by the attack roll — Slowed unconditionally on hit
            if (unit == ctx.finalTarget)
            {
                if (!resolver.IsSimulation)
                {
                    unit.AddEffect(new SlowedEffect());
                    CombatLog.Add($"{unit.unitName} is Slowed by Suppression Fire.");
                }
                continue;
            }

            // Other units on the line make a save
            var saveRoll = unit.RollSave(Unit.SaveType.Resolve);
            CombatLog.Add($"{unit.unitName} Suppression Fire save: {saveRoll} vs DC {saveDC}");

            if (saveRoll < saveDC && !resolver.IsSimulation)
            {
                unit.AddEffect(new SlowedEffect());
                CombatLog.Add($"{unit.unitName} is Slowed by Suppression Fire.");
            }
        }
    }

    private List<Vector2Int> GetLineTiles(Vector2Int from, Vector2Int to)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        Vector2Int current = from;

        while (current != to)
        {
            int dx = to.x - current.x;
            int dy = to.y - current.y;

            int stepX = dx != 0 ? (dx > 0 ? 1 : -1) : 0;
            int stepY = dy != 0 ? (dy > 0 ? 1 : -1) : 0;

            current = new Vector2Int(current.x + stepX, current.y + stepY);
            tiles.Add(current);
        }

        return tiles;
    }
}
