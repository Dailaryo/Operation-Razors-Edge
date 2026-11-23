using UnityEngine;
using System.Collections.Generic;

//Multi-target powers ("one or more creatures, no two of which can be more than 30 ft. apart").
//The planned target is the anchor. Further units on the anchor's side are added nearest-first
//while every pair stays within maxSpread tiles, up to maxTargets and an optional combined-HD budget.
//Works for hostile and ally powers alike: the anchor's side decides who is picked.
//Inner effects run once per target with ctx.finalTarget swapped, so each target saves separately.
//Place it outermost: MultiTarget > TargetLimit > SaveGate > effects.
public class MultiTargetEffect : IActionEffect
{
    public const int DefaultSpread = 6; //30 ft.

    private List<IActionEffect> inner;
    private System.Func<ActionContext, int> maxTargets; //null = no count limit
    private int maxSpread;
    private int hdBudget; //0 = no combined-HD limit

    public MultiTargetEffect(List<IActionEffect> inner, System.Func<ActionContext, int> maxTargets = null,
        int maxSpread = DefaultSpread, int hdBudget = 0)
    {
        this.inner = inner ?? new List<IActionEffect>();
        this.maxTargets = maxTargets;
        this.maxSpread = maxSpread;
        this.hdBudget = hdBudget;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        Unit anchor = ctx.finalTarget;
        if (anchor == null || ctx.attacker == null) return;

        foreach (var target in SelectTargets(ctx, resolver, anchor))
        {
            ctx.finalTarget = target;
            foreach (var effect in inner) effect.Execute(ctx, resolver);
        }

        //Restore: the resolver reads finalTarget after the effects run
        ctx.finalTarget = anchor;
    }

    private List<Unit> SelectTargets(ActionContext ctx, CombatResolver resolver, Unit anchor)
    {
        var selected = new List<Unit>();
        int limit = maxTargets != null ? Mathf.Max(1, maxTargets(ctx)) : int.MaxValue;
        int hdUsed = 0;

        if (!TryAdd(anchor, selected, ref hdUsed))
        {
            if (!resolver.IsSimulation)
                CombatLog.Add($"{anchor.unitName} is unaffected by {ctx.definition.DisplayName} ({BudgetText(anchor, hdUsed)}).");
            return selected;
        }

        Vector2Int sourcePos = resolver.GetPosition(ctx.attacker);
        Vector2Int anchorPos = resolver.GetPosition(anchor);
        int range = ctx.definition.GetMaxRange(ctx.attacker);

        var candidates = new List<Unit>();
        foreach (var unit in resolver.GetAllUnits())
        {
            if (unit == anchor || resolver.IsUnitDead(unit)) continue;
            if (unit.isEnemy != anchor.isEnemy) continue; //same side as anchor
            if (!DetectionStateManager.CanTarget(ctx.attacker, unit)) continue;

            Vector2Int pos = resolver.GetPosition(unit);
            if (DistanceUtility.MovementCostDistance(sourcePos, pos) > range) continue;
            if (DistanceUtility.MovementCostDistance(anchorPos, pos) > maxSpread) continue;
            if (ctx.definition.requiresLoS && !HasLoS(ctx.attacker, sourcePos, pos)) continue;

            candidates.Add(unit);
        }

        candidates.Sort((a, b) =>
            DistanceUtility.MovementCostDistance(anchorPos, resolver.GetPosition(a))
                .CompareTo(DistanceUtility.MovementCostDistance(anchorPos, resolver.GetPosition(b))));

        foreach (var unit in candidates)
        {
            if (selected.Count >= limit) break;

            Vector2Int pos = resolver.GetPosition(unit);
            bool withinSpread = selected.TrueForAll(s =>
                DistanceUtility.MovementCostDistance(resolver.GetPosition(s), pos) <= maxSpread);
            if (!withinSpread) continue;

            if (!TryAdd(unit, selected, ref hdUsed) && !resolver.IsSimulation)
                CombatLog.Add($"{unit.unitName} is unaffected by {ctx.definition.DisplayName} ({BudgetText(unit, hdUsed)}).");
        }

        return selected;
    }

    private bool TryAdd(Unit unit, List<Unit> selected, ref int hdUsed)
    {
        int hd = HitDice(unit);
        if (hdBudget > 0 && hdUsed + hd > hdBudget) return false;

        hdUsed += hd;
        selected.Add(unit);
        return true;
    }

    private static int HitDice(Unit unit) => unit.sourceCharacterData != null ? unit.sourceCharacterData.level : 0;

    private string BudgetText(Unit unit, int hdUsed) => $"Hit Dice limit: {HitDice(unit)} HD, {hdUsed} of {hdBudget} used";

    //Same rule as ActionValidator: blindsight within range bypasses line of sight
    private static bool HasLoS(Unit source, Vector2Int from, Vector2Int to)
    {
        bool blindsight = source.HasBlindSight &&
            DistanceUtility.MovementCostDistance(from, to) <= source.BlindSightRange;
        return blindsight || LoSResolver.HasLoS(from, to, GridManager.Instance);
    }
}
