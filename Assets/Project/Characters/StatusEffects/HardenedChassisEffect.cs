using UnityEngine;
using System.Collections.Generic;

public class HardenedChassisEffect : StatusEffect
{
    private int resolveBonus;
    private int hpBonus;

    public HardenedChassisEffect(int resolveBonus, int hpBonus)
        : base("HardenedChassis", -1)
    {
        this.resolveBonus = resolveBonus;
        this.hpBonus = hpBonus;
        isPersistent = true;
    }

    public override void OnApply(Unit unit)
    {
        base.OnApply(unit);
        unit.maxHP += hpBonus;
        unit.currentHP += hpBonus;
    }

    public override void OnExpire(Unit unit)
    {
        unit.maxHP = System.Math.Max(1, unit.maxHP - hpBonus);
        unit.currentHP = System.Math.Min(unit.currentHP, unit.maxHP);
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Fortitude, resolveBonus, 1f, "HardenedChassis",
                StatModifier.BonusType.Equipment)
        };
    }

    public override void OnTurnEnd(Unit unit) { }
}