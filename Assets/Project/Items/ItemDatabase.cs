using UnityEngine;
using System.Collections.Generic;

public static class ItemDatabase
{
    public static Dictionary<string, ItemDefinition> Definitions =
        new Dictionary<string, ItemDefinition>()
        {
            // --- Class feature items ---
            { ItemIds.PowerCrystal, new ItemDefinition(
                itemId:       ItemIds.PowerCrystal,
                displayName:  "Power Crystal",
                description:  "A crystal attuned to the telepath who created it. Stores 5 power points, " +
                              "spent before the telepath's own. Recharges at the Hub. Only its creator can use it.",
                itemType:     ItemType.Utility,
                slot:         EquipmentSlot.Consumable1,   //slotless: occupies a consumable slot, never consumed
                techCategory: TechCategory.Consumables)
                { storedPowerPoints = 5 } },
        };
    public static ItemDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out ItemDefinition item)) return item;

        Debug.LogWarning($"[ItemDatabase] Item not found: {id}");
        return null;
    }

}
