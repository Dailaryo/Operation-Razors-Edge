using UnityEngine;

public class JuggernautGroundbreakerEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation) return;

        if (ctx.attacker == null) return;

        int level = ctx.attacker.sourceCharacterData?.level ?? 1;
        int powMod = ctx.attacker.GetModifier(StatType.Power);
        int dc = 10 + (level / 2) + powMod;

        foreach (Unit unit in resolver.GetAllUnits())
        {
            if (resolver.IsUnitDead(unit)) continue;
            if (unit.isEnemy == ctx.attacker.isEnemy) continue;

            int dist = DistanceUtility.MovementCostDistance(
                resolver.GetPosition(ctx.attacker),
                resolver.GetPosition(unit));

            if (dist > 1) continue;

            var saveRoll = unit.RollSave(Unit.SaveType.Resolve);
            CombatLog.Add($"{unit.unitName} Groundbreaker save: {saveRoll} vs DC {dc}");

            if (saveRoll < dc)
            {
                if (!resolver.IsSimulation)
                {
                    // TODO: Replace with ProneEffect when authored (M22)
                    unit.AddEffect(new SlowedEffect());
                    CombatLog.Add($"{unit.unitName} is knocked prone by Groundbreaker!");
                }
            }
            else
            {
                CombatLog.Add($"{unit.unitName} resists Groundbreaker.");
            }
        }
    }
}
