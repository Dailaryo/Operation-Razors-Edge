using UnityEngine;

public class ArtilleristDevastationEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation) return;

        if (ctx.attacker == null || ctx.finalTarget == null) return;

        Vector2Int epicenter = resolver.GetPosition(ctx.finalTarget);

        int level  = ctx.attacker.sourceCharacterData?.level ?? 1;
        int powMod = ctx.attacker.GetModifier(StatType.Power);
        int dc = 10 + (level / 2) + powMod;

        CombatLog.Add($"{ctx.attacker.unitName} calls Devastation on {epicenter}!");

        foreach (Unit unit in resolver.GetAllUnits())
        {
            if (resolver.IsUnitDead(unit)) continue;
            if (unit.isEnemy == ctx.attacker.isEnemy) continue;

            Vector2Int unitPos = resolver.GetPosition(unit);
            int dist = DistanceUtility.MovementCostDistance(epicenter, unitPos);
            if (dist > 5) continue;

            var saveRoll = unit.RollSave(Unit.SaveType.Resolve);
            CombatLog.Add($"{unit.unitName} Devastation save: {saveRoll} vs DC {dc}");

            if (saveRoll < dc)
            {
                if (!resolver.IsSimulation)
                {
                    var slowed = new SlowedEffect();
                    slowed.duration = 3;
                    unit.AddEffect(slowed);

                    var shaken = new ShakenEffect();
                    shaken.duration = 3;
                    shaken.sourceLevel = level;
                    unit.AddEffect(shaken);

                    CombatLog.Add($"{unit.unitName} is Slowed and Shaken by Devastation!");
                }
            }
            else
            {
                CombatLog.Add($"{unit.unitName} resists Devastation.");
            }
        }
    }
}
