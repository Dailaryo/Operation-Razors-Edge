using UnityEngine;
using System.Collections.Generic;

public class LootTable
{
    public string tableId;
    public List<LootEntry> entries;

    public LootTable(string tableId, List<LootEntry> entries = null)
    {
        this.tableId = tableId;
        this.entries = entries ?? new List<LootEntry>();
    }

    public List<LootEntry> Roll()
    {
        List<LootEntry> results= new List<LootEntry>();

        //Always include guaranteed entries
        foreach (var entry in entries)
        {
            if (entry.guaranteed) results.Add(entry);
        }

        //Weighted random selection from non-guaranteed entries
        List<LootEntry> pool = new List<LootEntry>();
        float totalWeight = 0f;

        foreach (var entry in entries)
        {
            if (!entry.guaranteed && entry.dropWeight > 0f)
            {
                pool.Add(entry);
                totalWeight += entry.dropWeight;
            }
        }

        if (pool.Count == 0 || totalWeight <= 0f) return results;

        float roll = Random.Range(0f, totalWeight);
        float accumulated = 0f;

        foreach (var entry in pool)
        {
            accumulated += entry.dropWeight;
            if (roll < accumulated)
            {
                results.Add(entry);
                break;
            }
        }

        return results;
    }
}
