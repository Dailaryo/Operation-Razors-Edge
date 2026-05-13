using UnityEngine;
using System.Collections.Generic;

public class RandomEncounterTable
{
    public string tableId;
    private List<RandomEncounterEntry> entries = new List<RandomEncounterEntry>();

    // Zone type distribution weights — these determine how likely each
    // encounter category is to be selected, relative to zone type.
    // Higher weight = more likely. A weight of 0 means never selected.
    private static readonly Dictionary<ZoneDefinition.ZoneType, Dictionary<ZoneEventDefinition.ZoneEventType, int>>
        ZoneTypeWeights = new Dictionary<ZoneDefinition.ZoneType, Dictionary<ZoneEventDefinition.ZoneEventType, int>>
    {
        {
            ZoneDefinition.ZoneType.Social, new Dictionary<ZoneEventDefinition.ZoneEventType, int>
            {
                {ZoneEventDefinition.ZoneEventType.Combat, 0  },
                {ZoneEventDefinition.ZoneEventType.SkillCheck, 40 },
                {ZoneEventDefinition.ZoneEventType.Merchant, 30 },
                {ZoneEventDefinition.ZoneEventType.Narrative, 30  }
            }
        },
        {
            ZoneDefinition.ZoneType.Neutral, new Dictionary<ZoneEventDefinition.ZoneEventType, int>
            {
                {ZoneEventDefinition.ZoneEventType.Combat, 20  },
                {ZoneEventDefinition.ZoneEventType.SkillCheck, 30 },
                {ZoneEventDefinition.ZoneEventType.Merchant, 30 },
                {ZoneEventDefinition.ZoneEventType.Narrative, 30  }
            }
        },
        {
            ZoneDefinition.ZoneType.Hostile, new Dictionary<ZoneEventDefinition.ZoneEventType, int>
            {
                {ZoneEventDefinition.ZoneEventType.Combat, 70  },
                {ZoneEventDefinition.ZoneEventType.SkillCheck, 20 },
                {ZoneEventDefinition.ZoneEventType.Merchant, 10 },
                {ZoneEventDefinition.ZoneEventType.Narrative, 20  }
            }
        }
    };

    public RandomEncounterTable(string tableId)
    {
        this.tableId = tableId;
    }

    public void AddEntry(RandomEncounterEntry entry)
    {
        entries.Add(entry);
    }

    // Rolls the table for a single encounter, respecting budget and zone type distribution.
    // Returns null if no valid entry exists within the budget for the given zone type.
    public RandomEncounterEntry Roll(int budget, ZoneDefinition.ZoneType zone)
    {
        if (entries.Count == 0) return null;

        //Step 1: filter to entries that fit within budget
        List<RandomEncounterEntry> affordable = new List<RandomEncounterEntry>();
        foreach (RandomEncounterEntry entry in entries)
        {
            if (entry.pointCost <= budget)
                affordable.Add(entry);
        }

        if (affordable.Count == 0) return null;

        //Step 2: build weighted pool, factoring both entry weight and
        //zone type distribution weight for that encounter typw
        List<RandomEncounterEntry> pool = new List<RandomEncounterEntry>();
        List<int> weights = new List<int>();
        int totalWeight = 0;

        Dictionary<ZoneEventDefinition.ZoneEventType, int> distributionWeights;
        if (!ZoneTypeWeights.TryGetValue(zone, out distributionWeights))
        {
            //Unknown zone type: treat all encounter types equally
            distributionWeights = new Dictionary<ZoneEventDefinition.ZoneEventType, int>
            {
                { ZoneEventDefinition.ZoneEventType.Combat,     25 },
                { ZoneEventDefinition.ZoneEventType.SkillCheck, 25 },
                { ZoneEventDefinition.ZoneEventType.Merchant,   25 },
                { ZoneEventDefinition.ZoneEventType.Narrative,  25 }
            };
        }

        foreach (RandomEncounterEntry entry in affordable)
        {
            int distributionWeight;
            if (!distributionWeights.TryGetValue(entry.encounterType, out distributionWeight))
                distributionWeight = 0;

            //A ditribution weight of 0 means this typ never appears in this zone type
            if (distributionWeight == 0) continue;

            int combinedWeight = entry.weight * distributionWeight;
            pool.Add(entry);
            weights.Add(combinedWeight);
            totalWeight += combinedWeight;
        }

        if (pool.Count == 0 || totalWeight == 0) return null;

        //Step 3: weighted random draw
        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;

        for (int i = 0; i < pool.Count; i++)
        {
            cumulative += weights[i];
            if (roll < cumulative)
                return pool[i];
        }

        //Fallback: should not be reached under normal conditions
        return pool[pool.Count - 1];
    }

    //Entry Definition

    public class RandomEncounterEntry
    {
        public string encounterId;
        public ZoneEventDefinition.ZoneEventType encounterType;
        public int weight; //Relative draw weight among entries of the same type
        public int pointCost; //Budget consumed if this entry is selected

        public RandomEncounterEntry(
            string encounter,
            ZoneEventDefinition.ZoneEventType type,
            int weight,
            int cost)
        {
            encounterId = encounter;
            encounterType = type;
            this.weight = weight;
            pointCost = cost;
        }
    }
}
