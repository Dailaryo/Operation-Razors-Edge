using UnityEngine;
using System.Collections.Generic;

public static class PowerPointManager
{
    //State Container, held on Unit.powerPoints
    public class PowerPointState
    {
        public int max;
        public int current;

        public PowerPointState(int max)
        {
            this.max = max;
            current = max;
        }

        public bool CanAfford(int cost) => current >= cost;

        public void Spend(int cost)
        {
            current -= cost;
            if (current < 0) current = 0;
        }

        public void Restore(int amount)
        {
            current = Mathf.Min(current + amount, max);
        }

        public void RestoreFull()
        {
            current = max;
        }
    }

    //Max pool calculation

    public static int CalculateMaxPowerPoints(Unit unit)
    {
        if (unit.sourceCharacterData == null) return 0;

        var data = unit.sourceCharacterData;
        var classDef = ClassDatabase.Get(data.classId);

        if (classDef == null || !classDef.isPsionicClass) return 0;

        //Base points from class progression table
        int basePoints = 0;
        if (classDef.powerPointProgression != null &&
            classDef.powerPointProgression.TryGetValue(data.level, out int tableValue))
        {
            basePoints = tableValue;
        }

        //Key attribute modifer bonus (modifier * manifester level)
        int keyMod = unit.GetModifier(classDef.psionicKeyAbility);
        int abilityBonus = Mathf.Max(0, keyMod * data.level);

        return Mathf.Max(0, basePoints + abilityBonus);
    }

    //Initialization (calleed during Unit setup, alongside ApplyClass)
    public static void Initialize(Unit unit)
    {
        int max = CalculateMaxPowerPoints(unit);
        if (max <= 0) return;

        unit.powerPoints = new PowerPointState(max);
        Debug.Log($"{unit.unitName}: Power points initialized — {max} PP");
    }

    //Expenditure
    public static bool TrySpend(Unit unit, int cost)
    {
        if (unit.powerPoints == null)
        {
            Debug.LogWarning($"{unit.unitName}: TrySpend called but powerPoints is null");
            return false;
        }

        if (!unit.powerPoints.CanAfford(cost))
        {
            CombatLog.Add($"{unit.unitName}: Not enough power points ({unit.powerPoints.current}/{cost} needed)");
            return false;
        }

        unit.powerPoints.Spend(cost);
        CombatLog.Add($"{unit.unitName}: Spent {cost} PP ({unit.powerPoints.current}/{unit.powerPoints.max} remaining)");
        return true;
    }

    //Restoration (rest/meditation)
    public static void RestoreFull(Unit unit)
    {
        if (unit.powerPoints == null) return;
        unit.powerPoints.RestoreFull();
        CombatLog.Add($"{unit.unitName}: Power points fully restored ({unit.powerPoints.max} PP)");
    }

    public static void RestorePartial(Unit unit, int amount)
    {
        if (unit.powerPoints == null) return;
        unit.powerPoints.Restore(amount);
        CombatLog.Add($"{unit.unitName}: Restored {amount} PP ({unit.powerPoints.current}/{unit.powerPoints.max} remaining)");
    }
}