using UnityEngine;
using System.Collections.Generic;

public static class LootResolver
{
    //Rolls a loot table, auto-converts credstick entries to credits,
    //and returns item drops as ItemInstances for the loot UI queue.
    public static List<ItemInstance> ResolveLoot(string tableId)
    {
        List<ItemInstance> itemDrops = new List<ItemInstance>();

        if (!LootTableDatabase.Definitions.TryGetValue(tableId, out LootTable table))
        {
            Debug.LogWarning($"[LootResolver] Table not found: {tableId}");
            return itemDrops;
        }

        List<LootEntry> rolled = table.Roll();

        foreach (var entry in rolled)
        {
            if (entry.entryType == LootEntryType.Credstick)
            {
                SquadManager.Instance.AddCredits(entry.credstickValue);
            }
            else if (entry.entryType == LootEntryType.Item)
            {
                if (string.IsNullOrEmpty(entry.itemDefinitionId))
                {
                    Debug.LogWarning("[LootResolver] Item entry has null or empty itemDefinitionId. Skipping.");
                    continue;
                }

                if (!ItemDatabase.Definitions.ContainsKey(entry.itemDefinitionId))
                {
                    Debug.LogWarning($"[LootResolver] Item definition not found: {entry.itemDefinitionId}. Skipping.");
                    continue;
                }

                ItemInstance item = ItemInstanceFactory.CreateInstance(entry.itemDefinitionId);
                itemDrops.Add(item);
            }
        }

        return itemDrops;
    }
}
