using UnityEngine;
using System.Collections.Generic;

public class ConditionalSaveBonusEffect : StatusEffect
{
    private StatModifier modifier;
    private EffectDescriptor required;
    private bool permanent;

    //Applies only when the incoming effect carries ALL required descriptors.
    //duration -1 = permanent (feats, passives); otherwise rounds, or 0 with lastsUntilHub
    public ConditionalSaveBonusEffect(StatType saveStat, int flatBonus, EffectDescriptor required,
        string source, StatModifier.BonusType bonusType = StatModifier.BonusType.Untyped, int duration = -1)
        : base($"{source}_{saveStat}_conditional", duration)
    {
        this.required = required;
        modifier = new StatModifier(saveStat, flatBonus, 1f, source, bonusType);
        permanent = duration < 0;
    }

    //Permanent instances never tick down; timed ones use the default countdown
    public override void OnTurnEnd(Unit unit)
    {
        if (!permanent) base.OnTurnEnd(unit);
    }

    public override List<StatModifier> GetConditionalStatModifiers(Unit unit, EffectDescriptor incoming)
    {
        if ((incoming & required) != required) return new List<StatModifier>();
        return new List<StatModifier> { modifier };
    }
}
