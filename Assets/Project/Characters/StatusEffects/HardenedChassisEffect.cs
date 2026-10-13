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

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        var mods = new List<StatModifier>
      {
          new StatModifier(StatType.Fortitude, resolveBonus, 1f, "HardenedChassis",
              StatModifier.BonusType.Equipment)
      };

        if (hpBonus > 0)
            mods.Add(new StatModifier(StatType.MaxHP, hpBonus, 1f, "HardenedChassis",
                StatModifier.BonusType.Equipment));

        return mods;
    }

    public override void OnTurnEnd(Unit unit) { }
}