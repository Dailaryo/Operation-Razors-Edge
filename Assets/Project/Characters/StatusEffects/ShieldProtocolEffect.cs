using UnityEngine;
using System.Collections.Generic;

public class ShieldProtocolEffect : StatusEffect
{
    public ShieldProtocolEffect() : base("ShieldProtocol", 1) { }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Evasion,   2, 1f, "ShieldProtocol", StatModifier.BonusType.Equipment),
            new StatModifier(StatType.Fortitude, 2, 1f, "ShieldProtocol", StatModifier.BonusType.Equipment),
            new StatModifier(StatType.Willpower, 2, 1f, "ShieldProtocol", StatModifier.BonusType.Equipment),
        };
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}