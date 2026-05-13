using UnityEngine;
using System.Collections.Generic;

public class IntegratedTargetingEffect : StatusEffect
{
    public IntegratedTargetingEffect()
        : base("IntegratedTargeting", -1)
    {
        isPersistent = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        //Only grant the bonus when no enemy is threatening this unit's tile
        var threateners = ThreatZoneManager.GetThreateningUnits(
            unit.position, isEnemy: !unit.isEnemy);

        if (threateners.Count > 0)
            return new List<StatModifier>();

        return new List<StatModifier>
        {
            new StatModifier(
                StatType.Accuracy,
                1,
                1f,
                "IntegratedTargeting",
                StatModifier.BonusType.Equipment)
        };
    }

    public override void OnTurnEnd(Unit unit) { }
}
