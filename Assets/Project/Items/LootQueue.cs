using UnityEngine;
using System.Collections.Generic;

public static class LootQueue
{
    private static List<ItemInstance> pendingLoot = new List<ItemInstance>();

    public static bool HasLoot => pendingLoot.Count > 0;

    public static void Enqueue(ItemInstance item)
    {
        if (item != null)
            pendingLoot.Add(item);
    }

    public static void Enqueue(List<ItemInstance> items)
    {
        if (items == null) return;
        foreach (var item in items)
            Enqueue(item);
    }

    public static List<ItemInstance> GetAll()
    {
        return new List<ItemInstance>(pendingLoot);
    }

    public static void Claim(ItemInstance item, Unit claimant)
    {
        if (!pendingLoot.Contains(item)) return;
        pendingLoot.Remove(item);
        SquadManager.Instance.squadInventory.AddItem(item);
    }

    public static void Clear()
    {
        pendingLoot.Clear();
    }
}
