using UnityEngine;
using System.Collections.Generic;

public class CybertakerEffect : StatusEffect
{
    public CybertakerEffect()
        : base("Cybertaker", -1)
    {
        isPersistent = true;
    }

    public override void OnApply(Unit unit)
    {
        base.OnApply(unit);
        if (unit.sourceCharacterData != null)
            unit.sourceCharacterData.cyberneticCapacityBonus += 1;
    }

    public override void OnExpire(Unit unit)
    {
        if (unit.sourceCharacterData != null)
            unit.sourceCharacterData.cyberneticCapacityBonus -= 1;
    }

    public override void OnTurnEnd(Unit unit) { }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>();
    }
}
