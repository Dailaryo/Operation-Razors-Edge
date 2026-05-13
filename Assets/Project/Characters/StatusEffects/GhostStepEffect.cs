using UnityEngine;
using System.Collections.Generic;

public class GhostStepEffect : StatusEffect
{
    public GhostStepEffect() : base("GhostStep", -1)
    {
        isPersistent = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier>
        {
            new StatModifier(StatType.SkillStealth, 2, 1f, "GhostStep",
                StatModifier.BonusType.Competence)
        };
    }

    public override void OnTurnEnd(Unit unit) { }
}