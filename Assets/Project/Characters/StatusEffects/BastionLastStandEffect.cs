using UnityEngine;

public class BastionLastStandEffect : StatusEffect
{
    private bool triggered = false;

    public BastionLastStandEffect() : base("BastionLastStand", 0)
    {
        isPersistent = true;
    }

    public bool TryTrigger(Unit unit, int incomingDamage)
    {
        if (triggered) return false;
        if (unit.currentHP - incomingDamage > 0) return false;

        triggered = true;
        CombatLog.Add($"{unit.unitName} Last Stand — holds at 1 HP!");
        return true;
    }
}
