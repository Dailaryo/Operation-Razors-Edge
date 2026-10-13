using UnityEngine;
using System.Collections.Generic;

//Empath Aura of Calm (Paladin-aura model): always on while the bearer is conscious. Fear conditions on the
//bearer and same-side units within 30 ft. are suppressed, not removed: they keep ticking and resume if still
//running when the unit leaves the aura or the bearer drops.
//Deferred: sleep/unconscious conditions (none exist yet) should also end it.
public static class AuraOfCalm
{
    public const int Range = 6;  //30 ft.
    public const EffectDescriptor Suppressed = EffectDescriptor.Fear;

    private static List<Unit> bearers = new List<Unit>();

    //Called by CombatManager.StartCombat
    public static void Initialize(List<Unit> allUnits)
    {
        bearers.Clear();
        foreach (var unit in allUnits)
            Register(unit);
    }

    //Also called for units that join mid-combat (reinforcements)
    public static void Register(Unit unit)
    {
        var data = unit.sourceCharacterData;
        if (data == null || bearers.Contains(unit)) return;

        if (data.unlockedPassiveIds.Contains(PassiveIds.EmpathAuraOfCalm))
            bearers.Add(unit);
    }

    public static bool Suppresses(Unit unit, StatusEffect effect)
    {
        if ((effect.descriptors & Suppressed) == 0) return false;

        foreach (var bearer in bearers)
        {
            if (bearer.isEnemy != unit.isEnemy) continue;
            if (!bearer.CanAct()) continue; //dead, dying or stabilized = unconscious

            if (bearer == unit ||
                DistanceUtility.MovementCostDistance(bearer.position, unit.position) <= Range)
                return true;
        }

        return false;
    }
}