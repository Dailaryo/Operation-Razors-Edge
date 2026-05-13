using UnityEngine;
using System.Collections.Generic;

public class ResilienceEffect : StatusEffect
{
    private bool triggered = false;

    public ResilienceEffect() : base("Resilience", -1)
    {
        isPersistent = true;
    }

    public override void OnApply(Unit unit)
    {
        base.OnApply(unit);
        triggered = false;
    }

    public bool TryTrigger(Unit unit, int incomingDamage)
    {
        if (triggered) return false;
        if (unit.currentHP - incomingDamage > 0) return false; // Would survive anyway

        triggered = true;
        CombatLog.Add($"{unit.unitName}: Resilience — survives at 1 HP!");
        return true;
    }

    public void ResetForCombat() => triggered = false;

    public override List<StatModifier> GetStatModifiers(Unit unit)
        => new List<StatModifier>();

    public override void OnTurnEnd(Unit unit) { }
}