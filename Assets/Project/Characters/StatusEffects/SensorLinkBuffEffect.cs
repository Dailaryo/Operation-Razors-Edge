using UnityEngine;
using System.Collections.Generic;

public class SensorLinkBuffEffect : StatusEffect
{
    private int sharedPerception;

    public SensorLinkBuffEffect(int sharedPerception) : base("SensorLinkBuff", 1)
    {
        this.sharedPerception = sharedPerception;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        // Express as a flat bonus to Perception up to the shared value
        // If the ally already has a higher Perception result, no benefit
        int allyPerception = SkillResolver.GetSkillTotal(unit, SkillType.Perception);
        int bonus = Mathf.Max(0, sharedPerception - allyPerception);

        if (bonus == 0) return new List<StatModifier>();

        return new List<StatModifier>
        {
            new StatModifier(StatType.SkillPerception, bonus, 1f, "SensorLink",
                StatModifier.BonusType.Competence)
        };
    }

    public override void OnTurnEnd(Unit unit)
    {
        base.OnTurnEnd(unit);
    }
}