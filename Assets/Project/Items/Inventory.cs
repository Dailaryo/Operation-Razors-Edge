using UnityEngine;
using System.Collections.Generic;

public class Inventory
{
    private List<ItemInstance> items = new List<ItemInstance>();

    public float maxCarryWeight;

    public Inventory(float maxCarryWeight)
    {
        this.maxCarryWeight = maxCarryWeight;
    }

    public void AddItem(ItemInstance item)
    {
        items.Add(item);
    }

    public bool RemoveItem(ItemInstance item)
    {
        return items.Remove(item);
    }

    public bool Contains(ItemInstance item)
    {
        return items.Contains(item);
    }

    public List<ItemInstance> GetAll()
    {
        return new List<ItemInstance>(items); //return a copy, callers should not mutate
    }

    public float GetTotalWeight()
    {
        float total = 0f;

        foreach (var instance in items)
        {
            var item = ItemDatabase.Get(instance.definitionId);
            if (item != null) total += item.weight;
        }

        return total;
    }

    public bool CanCarry(ItemInstance instance)
    {
        var item = ItemDatabase.Get(instance.definitionId);
        if (item == null) return false;

        return GetTotalWeight() + item.weight <= maxCarryWeight;
    }
}
