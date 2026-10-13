using System.Collections.Generic;
using UnityEngine;

public class BastionProtectiveAuraBuffEffect : StatusEffect
{
    public BastionProtectiveAuraBuffEffect() : base("BastionProtectiveAuraBuff", 1) { }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.Defense, 2, 1f,
                "bastion_protective_aura", StatModifier.BonusType.Competence)
        };
    }
}
