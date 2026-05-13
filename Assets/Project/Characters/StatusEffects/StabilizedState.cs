using UnityEngine;

public class StabilizedState : StatusEffect
{
    public StabilizedState() : base("Stabilized", 0)
    {
        isPersistent = true;
    }

    public override void OnApply(Unit unit)
    {
        base.OnApply(unit);
        CombatLog.Add($"{unit.unitName} is stabilized but unconscious.");
    }

    public override void OnTurnEnd(Unit unit)
    {
        //No per-turn HP loss. Stabilized characters simply wait for healing.
    }

    public override void OnExpire(Unit unit)
    {
        //Intentionally empty: removed externally when healing restores HP above 0
    }
}
