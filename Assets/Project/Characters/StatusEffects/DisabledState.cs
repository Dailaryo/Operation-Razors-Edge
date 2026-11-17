using UnityEngine;

public class DisabledState : StatusEffect
{
    public DisabledState() : base("Disabled", 0)
    {
        isPersistent = true;
    }

    public override void OnApply(Unit unit)
    {
        base.OnApply(unit);
        CombatLog.Add($"{unit.unitName} is disabled (0 HP): one move or standard action per round, half speed. A standard action costs 1 HP.");
    }

    public override void OnTurnEnd(Unit unit)
    {
        //Transition to dying if HP dropped below 0 during this turn
        if (unit.currentHP < 0)
        {
            unit.RemoveEffect<DisabledState>();
            unit.AddEffect(new DyingState());
        }
    }

    public override void OnExpire(Unit unit)
    {
        //Intentionally empty: removed externally by CombatResolver
        //when HP changes move the unit out of the disabled range
    }
}
