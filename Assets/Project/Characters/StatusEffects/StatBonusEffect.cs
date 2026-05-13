using UnityEngine;
using System.Collections.Generic;

public class StatBonusEffect : StatusEffect
{
    private StatModifier modifier;

    public StatBonusEffect(StatType stat, int flatbonus, string source,
        StatModifier.BonusType bonusType = StatModifier.BonusType.Untyped)
        : base($"{source}_{stat}_bonus", -1)  //duration -1 = permanent until removed
    {
        modifier = new StatModifier(stat, flatbonus, 1f, source, bonusType);
    }

    //Permanent effects never tick down
    public override void OnTurnEnd(Unit unit) { }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        return new List<StatModifier> { modifier };
    }
}
