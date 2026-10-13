using UnityEngine;
using System.Collections.Generic;

public static class PowerPointManager
{
    //State Container, held on Unit.powerPoints
    public class PowerPointState
    {
        public int max;
        public int current;
        public int freeZeroLevelMax;
        public int freeZeroLevelUsed;
        public int FreeZeroLevelRemaining => Mathf.Max(0, freeZeroLevelMax - freeZeroLevelUsed);

        public PowerPointState(int max)
        {
            this.max = max;
            current = max;
        }

        public ItemInstance crystal; //Equipped bound Power Crystal: its stored PP are spent first

        public int CrystalPoints => crystal != null ? crystal.charges : 0;

        public bool CanAfford(int cost) => current + CrystalPoints >= cost;

        public void Spend(int cost)
        {
            if (crystal != null)
            {
                int fromCrystal = Mathf.Min(crystal.charges, cost);
                crystal.charges -= fromCrystal;
                cost -= fromCrystal;
            }

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
            freeZeroLevelUsed = 0;
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

        //3.5 rule: bonus PP = key ability modifier × manifester level × 1/2 (rounded down)
        int keyMod = unit.GetModifier(classDef.psionicKeyAbility);
        int abilityBonus = Mathf.Max(0, keyMod * data.manifesterLevel / 2);

        return Mathf.Max(0, basePoints + abilityBonus);
    }

    //Initialization (calleed during Unit setup, alongside ApplyClass)
    public static void Initialize(Unit unit, int spent = 0, int zeroLevelUsed = 0)
    {
        int max = CalculateMaxPowerPoints(unit);
        if (max <= 0) return;

        unit.powerPoints = new PowerPointState(max);

        unit.powerPoints.crystal = FindEquippedCrystal(unit.sourceCharacterData);

        if (spent > 0)
            unit.powerPoints.current = Mathf.Max(0, max - spent);

        //d20 Modern: 3 + manifester level free 0-level manifestations per rest
        unit.powerPoints.freeZeroLevelMax = 3 + unit.sourceCharacterData.manifesterLevel;
        unit.powerPoints.freeZeroLevelUsed = Mathf.Min(zeroLevelUsed, unit.powerPoints.freeZeroLevelMax);

        Debug.Log($"{unit.unitName}: Power points initialized — {unit.powerPoints.current}/{max} PP, " +
        $"{unit.powerPoints.FreeZeroLevelRemaining} free 0-level uses");
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
        CombatLog.Add($"{unit.unitName}: Spent {cost} PP ({unit.powerPoints.current}/{unit.powerPoints.max} remaining" +
    (unit.powerPoints.crystal != null ? $", crystal {unit.powerPoints.CrystalPoints})" : ")"));
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

    //The character's own equipped item with stored power points (Power Crystal), if any
    private static ItemInstance FindEquippedCrystal(CharacterData data)
    {
        foreach (var inst in data.equippedItemIds.Values)
        {
            if (inst.ownerCharacterId != data.characterId) continue;

            var def = ItemDatabase.Get(inst.definitionId);
            if (def != null && def.storedPowerPoints > 0) return inst;
        }

        return null;
    }
}