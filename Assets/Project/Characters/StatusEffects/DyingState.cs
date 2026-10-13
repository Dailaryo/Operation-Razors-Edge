using UnityEngine;

public class DyingState : StatusEffect
{
    public DyingState() : base("Dying", 0)
    {
        isPersistent = true;
    }

    public override void OnApply(Unit unit)
    {
        base.OnApply(unit);
        CombatLog.Add($"{unit.unitName} is dying!");
    }

    public override void OnTurnEnd(Unit unit)
    {
        //Lose 1 HP per round
        unit.currentHP -= 1;
        CombatLog.Add($"{unit.unitName} loses 1 HP while dying. HP: {unit.currentHP}");

        //Check if HP loss crossed the death threshold
        if (unit.IsDead())
        {
            CombatLog.Add($"{unit.unitName} has died.");
            unit.Kill();
            return;
        }

        //Attempt natural stabilization: Resolve save DC 20
        int total = unit.RollSave(Unit.SaveType.Resolve);

        if (total >= 20)
        {
            CombatLog.Add($"{unit.unitName} stabilizes naturally (Resolve {total} vs DC 20).");
            unit.RemoveEffect<DyingState>();
            unit.AddEffect(new StabilizedState());
        }
        else
        {
            CombatLog.Add($"{unit.unitName} fails to stabilize (Resolve {total} vs DC 20).");
        }
    }

    public override void OnExpire(Unit unit)
    {
        //Intentionally empty: removal is handled externally by RemoveEffect<DyingState>()
        //or by CombatResolver when death threshold is crossed
    }
}
