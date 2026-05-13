using UnityEngine;
using System.Collections.Generic;

public class PhantomProtocolEffect : StatusEffect
{
    public PhantomProtocolEffect() : base("PhantomProtocol", -1)
    {
        isPersistent = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.SkillStealth, 4, 1f, "PhantomProtocol",
                StatModifier.BonusType.Competence)
        };
    }

    public override void OnTurnEnd(Unit unit) { }
}