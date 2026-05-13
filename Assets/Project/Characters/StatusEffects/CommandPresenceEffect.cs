using UnityEngine;
using System.Collections.Generic;

public class CommandPresenceEffect : StatusEffect
{
    public CommandPresenceEffect() : base("CommandPresence", -1)
    {
        isPersistent = true;
    }

    public override List<StatModifier> GetStatModifiers(Unit unit)
    {
        int presenceMod = Mathf.Max(0, unit.GetModifier(StatType.Presence));
        return new List<StatModifier>
        {
            new StatModifier(StatType.Initiative, presenceMod, 1f, "CommandPresence",
                StatModifier.BonusType.Untyped)
        };
    }

    public override void OnTurnEnd(Unit unit) { }
}