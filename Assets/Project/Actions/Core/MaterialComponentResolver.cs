using UnityEngine;
using System.Collections.Generic;

//Pays PF1e-style material components.
//Squad:   the power's authored item from squad inventory (preferred), otherwise its value in credits.
//         Converted XP costs are always flat credits.
//Enemies: the encounter's material pool (EncounterData.materialPoolCredits), same credit value.
public static class MaterialComponentResolver
{
    //First instance of the power's authored component item in squad inventory, or null
    public static ItemInstance FindComponentItem(PsionicPowerDefinition power)
    {
        if (string.IsNullOrEmpty(power.materialItemId) || SquadManager.Instance == null) return null;
                return SquadManager.Instance.squadInventory.GetAll()
            .Find(i => i.definitionId == power.materialItemId);
    }

    //Squad: true if this action will consume the item rather than pay credits
    public static bool WillUseItem(CombatAction action, PsionicPowerDefinition power)
    {
        return !action.payMaterialWithCredits && FindComponentItem(power) != null;
    }

    public static bool CanPay(CombatAction action, PsionicPowerDefinition power, out string reason)
    {
        reason = null;
        int cost = power.GetMaterialCreditCost();
        if (cost <= 0) return true;

        if (action.source.isEnemy)
        {
            var pool = action.manager != null ? action.manager.enemyMaterialPool : null;
            if (pool != null && pool.CanAfford(cost)) return true;

            reason = "encounter material pool cannot cover the cost";
            return false;
        }

        if (WillUseItem(action, power)) return true;

        var balance = SquadManager.Instance != null ? SquadManager.Instance.creditBalance : null;
        if (balance != null && balance.CanAfford(cost)) return true;

        reason = string.IsNullOrEmpty(power.materialItemId)
            ? $"insufficient credits ({cost} needed)"
            : $"no component item in squad inventory and insufficient credits ({cost} needed)";
        return false;
    }

    //Consumes the component. Execution only, never during simulation.
    public static bool TryPay(CombatAction action, PsionicPowerDefinition power)
    {
        if (!CanPay(action, power, out string reason))
        {
            CombatLog.Add($"{action.source.unitName} cannot pay the material component for {power.DisplayName}: {reason}.");
            return false;
        }

        int cost = power.GetMaterialCreditCost();
        if (cost <= 0) return true;

        if (action.source.isEnemy)
        {
            action.manager.enemyMaterialPool.Spend(cost);
            CombatLog.Add($"{action.source.unitName} expends {cost} credits of material components.");
            return true;
        }

        if (WillUseItem(action, power))
        {
            SquadManager.Instance.squadInventory.RemoveItem(FindComponentItem(power));
            var def = ItemDatabase.Get(power.materialItemId);
            CombatLog.Add($"{action.source.unitName} consumes {(def != null ? def.displayName : power.materialItemId)} " +
                $"as a material component.");
            return true;
        }

        return SquadManager.Instance.SpendCredits(cost);
    }
}
