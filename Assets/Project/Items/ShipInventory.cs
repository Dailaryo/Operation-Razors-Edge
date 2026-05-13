using UnityEngine;
using System.Collections.Generic;

public static class ShipInventory
{
    private static List<ItemInstance> items = new List<ItemInstance>();

    public static void AddItem(ItemInstance item)
    {
        items.Add(item);
    }

    public static bool RemoveItem(ItemInstance item)
    {
        return items.Remove(item);
    }

    public static bool Contains(ItemInstance item)
    {
        return items.Contains(item);
    }

    public static List<ItemInstance> GetAll()
    {
        return new List<ItemInstance>(items); //Return a copy, callers should not mutate
    }

    //Should only be called when deleting an existing character
    public static void DepositFromCharacter(CharacterData data)
    {
        //Transfer equipped items
        var slotsToDeposit = new List<EquipmentSlot>();
        foreach (var kvp in data.equippedItemIds)
        {
            slotsToDeposit.Add(kvp.Key);
        }
        foreach (var slot in slotsToDeposit)
        {
            items.Add(data.equippedItemIds[slot]);
            data.equippedItemIds.Remove(slot);
        }
    }

    public static void Clear()
    {
        items.Clear();
    }
}
