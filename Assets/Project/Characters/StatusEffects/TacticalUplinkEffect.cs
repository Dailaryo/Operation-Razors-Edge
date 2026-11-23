using System.Collections.Generic;
using UnityEngine;

public class TacticalUplinkEffect : StatusEffect
{
    private int bonus;
    private int range;

    public TacticalUplinkEffect(int bonus) : base("TacticalUplink", -1)
    {
        this.bonus = bonus;
        this.range = bonus == 1 ? 3 : 5; // Uplink I = 3 tiles, Uplink II = 5 tiles
        isPersistent = true;
    }

    public override void OnEndPhaseAura(Unit source, List<Unit> allUnits)
    {
        int multiplier = source.activeEffects.Exists(e => e is HighCommandActiveEffect) ? 2 : 1;
        int effectiveBonus = bonus * multiplier;

        foreach (Unit unit in allUnits)
        {
            if (unit.IsDead() || unit.IsDying()) continue;
            if (unit.isEnemy != source.isEnemy) continue;

            int dist = DistanceUtility.MovementCostDistance(source.position, unit.position);
            if (dist > range) continue;

            // Apply a short-duration accuracy buff — refreshed each turn
            var existing = unit.activeEffects.Find(e => e is TacticalUplinkBuffEffect) as TacticalUplinkBuffEffect;
            if (existing != null)
            {
                existing.bonus = effectiveBonus;
                existing.duration = 1; // Refresh duration
            }
            else
            {
                unit.AddEffect(new TacticalUplinkBuffEffect(effectiveBonus));
            }
        }
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
        => new List<StatModifier>();

    public override void OnTurnEnd(Unit unit) { }
}