using System.Collections.Generic;
using UnityEngine;

public class FrightfulPresenceEffect : StatusEffect
{
    public FrightfulPresenceEffect() : base("FrightfulPresence", 0)
    {
        isPersistent = true;
    }

    // Called by CombatResolver.HandleUnitDeath when the bearer lands the killing blow
    public void TriggerOnKill(Unit attacker, List<Unit> allUnits, CombatResolver resolver)
    {
        if (attacker.sourceCharacterData == null) return;

        int level = attacker.sourceCharacterData.level;
        int preMod = attacker.GetModifier(StatType.Presence);
        int dc = 10 + (level / 2) + preMod;
        int range = 2; // 10 feet = 2 tiles

        foreach (Unit unit in allUnits)
        {
            if (unit == attacker) continue;
            if (!unit.isEnemy || unit.IsDead()) continue;
            if (DistanceUtility.MovementCostDistance(unit.position, attacker.position) > range) continue;

            int saveRoll = unit.RollSave(Unit.SaveType.Focus);
            CombatLog.Add($"{unit.unitName} Frightful Presence save: {saveRoll} vs DC {dc}");

            if (saveRoll < dc)
            {
                var effect = new ShakenEffect();
                effect.sourceLevel = level;
                unit.AddEffect(effect);
                CombatLog.Add($"{unit.unitName} is Shaken by {attacker.unitName}'s Frightful Presence!");
            }
        }
    }
}
