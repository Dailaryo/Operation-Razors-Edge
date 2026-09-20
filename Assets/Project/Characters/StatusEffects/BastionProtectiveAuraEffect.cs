using System.Collections.Generic;
using UnityEngine;

public class BastionProtectiveAuraEffect : StatusEffect
{
    public BastionProtectiveAuraEffect() : base("BastionProtectiveAura", 0)
    {
        isPersistent = true;
    }

    public void ApplyToNearbyAllies(Unit source, List<Unit> allUnits)
    {
        if (source.currentHP <= 0) return;

        bool apotheosis = source.sourceCharacterData != null &&
            source.sourceCharacterData.unlockedPassiveIds.Contains(PassiveIds.BastionApotheosis);

        int range = apotheosis ? 6 : 1;

        foreach (Unit unit in allUnits)
        {
            if (unit == source) continue;
            if (unit.IsDead() || unit.IsDying()) continue;
            if (unit.isEnemy != source.isEnemy) continue;

            int dist = DistanceUtility.MovementCostDistance(source.position, unit.position);
            if (dist > range) continue;

            var existing = unit.activeEffects
                .Find(e => e is BastionProtectiveAuraBuffEffect) as BastionProtectiveAuraBuffEffect;

            if (existing != null)
                existing.duration = 1;
            else
                unit.AddEffect(new BastionProtectiveAuraBuffEffect());
        }
    }

    public override void OnTurnEnd(Unit unit) { }
}
