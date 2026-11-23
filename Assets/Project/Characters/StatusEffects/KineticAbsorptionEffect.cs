using UnityEngine;
using System.Collections.Generic;

public class KineticAbsorptionEffect : StatusEffect
{
    private bool triggered = false;

    public KineticAbsorptionEffect() : base("KineticAbsorption", -1)
    {
        isPersistent = true;
        instancedPassive = true;
    }

    public override void OnApply(Unit unit)
    {
        base.OnApply(unit);
        triggered = false;
    }

    // Called from Unit.TakeDamage() — needs a hook there
    public int GetAbsorption(Unit unit)
    {
        if (triggered) return 0;
        triggered = true;
        int absorption = unit.GetModifier(StatType.Cognition);
        CombatLog.Add($"{unit.unitName}: Kinetic Absorption absorbs {absorption} damage.");
        return absorption;
    }

    public void ResetForCombat() => triggered = false;

    public override List<StatModifier> GetStatModifiers(Unit unit)
        => new List<StatModifier>();

    public override void OnTurnEnd(Unit unit) { }
}