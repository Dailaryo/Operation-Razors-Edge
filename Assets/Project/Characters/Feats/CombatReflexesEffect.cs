using UnityEngine;

public class CombatReflexesEffect : StatusEffect
{
    public CombatReflexesEffect() : base("CombatReflexes", 0)
    {
        isPersistent = true;
    }

    public override void OnApply(Unit unit)
    {
        base.OnApply(unit);

        if (unit.oaState == null) return;

        int agilityMod = unit.GetModifier(StatType.Agility);
        unit.oaState.maxPerRound = Mathf.Max(1, agilityMod);
    }
}
