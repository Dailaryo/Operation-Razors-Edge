using UnityEngine;
using System.Collections.Generic;

public static class LootTableDatabase
{
    public static Dictionary<string, LootTable> Definitions =
        new Dictionary<string, LootTable>()
        {
            {
                LootTableIds.BasicDrone,
                new LootTable(LootTableIds.BasicDrone, new List<LootEntry>
                {
                    new LootEntry(LootEntryType.Credstick, credstickValue: 25, dropWeight: 60f),
                    new LootEntry(LootEntryType.Credstick, credstickValue: 50, dropWeight: 30f),
                    new LootEntry(LootEntryType.Credstick, credstickValue: 100, dropWeight: 10f),
                })
            },
        };
}
