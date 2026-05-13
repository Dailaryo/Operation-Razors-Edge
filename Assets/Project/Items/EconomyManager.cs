using UnityEngine;
using System.Collections.Generic;

public static class EconomyManager
{
    //Returns the buy price for an item definition with no mods installed
    //Mods are not pre-installed at point of purchase, the buyer installs them separately
    public static int GetBuyPrice(string itemDefinitionId)
    {
        var item = ItemDatabase.Get(itemDefinitionId);
        if (item == null) return 0;

        return item.baseValue;
    }

    //Returns the sell price for an ItemInstance, including installed mod values.
    //Trade Goods sell at 100% of total value, everything else at 50%.
    public static int GetSellPrice(ItemInstance instance)
    {
        var item = ItemDatabase.Get(instance.definitionId);
        if (item == null) return 0;

        int totalValue = item.baseValue;

        foreach (string modId in instance.installedModIds)
        {
            var mod = ItemModDatabase.Get(modId);
            if (mod != null)
                totalValue += mod.baseValue;
            else
                Debug.LogWarning($"[EconomyManager] Mod definition not found: {modId}. Skipping in value calculation.");
        }

        return item.itemType == ItemType.TradeGood ? totalValue : Mathf.RoundToInt(totalValue * 0.5f);
    }

    //Returns the filtered stock list for a merchant.
    //If techTierFilter is true, filters by the current planet's CivilizationData.
    //Returns empty list if merchant is not found or planet context is unavailable when required.
    public static List<ItemDefinition> GetAvailableStock(string merchantId)
    {
        List<ItemDefinition> result = new List<ItemDefinition>();

        var merchant = MerchantDatabase.Get(merchantId);
        if (merchant == null) return result;

        if (!merchant.techTierFilter)
        {
            //No filtering: return full stock pool as ItemDefintions
            foreach (string itemId in merchant.stockPool)
            {
                var item = ItemDatabase.Get(itemId);
                if (item != null)
                    result.Add(item);
                else
                    Debug.LogWarning($"[EconomyManager] Stock item not found in ItemDatabase: {itemId}");
            }

            return result;
        }

        //Tech tier filtering: requires current planet context
        if (GameFlowManager.Instance == null)
        {
            Debug.LogWarning("[EconomyManager] GameFlowManager not available for tech tier filtering.");
            return result;
        }

        //Find the current planet by looking up the active mission's planet
        //Deferred: planet-side merchant access tied to exploration milestone
        //For now, filter against first accessible planet in the current act
        CivilizationData civ = GetCurrentCivilizationData();

        if (civ == null)
        {
            Debug.LogWarning("[EconomyManager] No civilization data available for tech tier filtering.");
            return result;
        }

        foreach (string itemId in merchant.stockPool)
        {
            var item = ItemDatabase.Get(itemId);
            if (item == null) continue;

            int localTier = civ.baselineTiers.GetTierForCategory(item.techCategory);

            if (item.techTier <= localTier)
                result.Add(item);
        }

        return result;
    }

    //Executes a buy transaction.  Validates stock availablility, affordability, then
    //deducts credits, creates and ItemInstance, and deposits it to the squad inventory
    public static bool BuyItem(string merchantId, string itemDefinitionId, out string reason)
    {
        var merchant = MerchantDatabase.Get(merchantId);
        if (merchant == null)
        {
            reason = $"Merchant '{merchantId}' not found.";
            return false;
        }

        List<ItemDefinition> availableStock = GetAvailableStock(merchantId);
        bool inStock = availableStock.Exists(i => i.itemId == itemDefinitionId);

        if (!inStock)
        {
            reason = "Item is not available from this merchant.";
            return false;
        }

        int price = GetBuyPrice(itemDefinitionId);

        if (!SquadManager.Instance.creditBalance.CanAfford(price))
        {
            reason = $"Insufficient credits. Need {price}, have {SquadManager.Instance.creditBalance.credits}.";
            return false;
        }

        SquadManager.Instance.SpendCredits(price);

        ItemInstance instance = ItemInstanceFactory.CreateInstance(itemDefinitionId);
        SquadManager.Instance.squadInventory.AddItem(instance);

        CombatLog.Add($"[Economy] Bought '{itemDefinitionId}' from {merchant.displayName} for {price} credits.");

        reason = null;
        return true;

    }

    //Executes a sell transaction.  Validates merchant accepts the item type, then
    //grants credits and removes the instance from squad inventory.
    public static bool SellItem(string merchantId, ItemInstance instance, out string reason)
    {
        var merchant = MerchantDatabase.Get(merchantId);
        if (merchant == null)
        {
            reason = $"Merchant '{merchantId}' not found.";
            return false;
        }

        if (!ItemDatabase.Definitions.TryGetValue(instance.definitionId, out ItemDefinition item))
        {
            reason = $"Item definition '{instance.definitionId}' not found.";
            return false;
        }

        if (!merchant.buysAll && !merchant.acceptedItemTypes.Contains(item.itemType))
        {
            reason = $"{merchant.displayName} does not buy {item.itemType} items.";
            return false;
        }

        //Check squad inventory, items being sold must be in squad inventory,
        //not equipped on a character
        if (!SquadManager.Instance.squadInventory.Contains(instance))
        {
            reason = "Item is not in squad inventory. Unequip it before selling.";
            return false;
        }

        int price = GetSellPrice(instance);

        SquadManager.Instance.squadInventory.RemoveItem(instance);
        SquadManager.Instance.AddCredits(price);

        CombatLog.Add($"[Economy] Sold '{item.displayName}' to {merchant.displayName} for {price} credits.");

        reason = null;
        return true;
    }

    // Returns CivilizationData for the current planet context.
    private static CivilizationData GetCurrentCivilizationData()
    {
        if (ExplorationController.Instance != null &&
            ExplorationController.Instance.currentPlanet != null)
        {
            return CampaignManager.Instance.GetCivilizationForPlanet(
                ExplorationController.Instance.currentPlanet.planetId);
        }

        // Fallback: hub context — use first accessible planet in current act
        if (CampaignManager.Instance == null) return null;
        var missions = CampaignManager.Instance.GetAvailableEncounters();
        if (missions == null || missions.Count == 0) return null;
        return CampaignManager.Instance.GetCivilizationForEncounter(missions[0].encounterId);
    }
}
