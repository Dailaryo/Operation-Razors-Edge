using UnityEngine;
using System.Collections.Generic;

public static class EnemyDatabase
{
    public static Dictionary<string, EnemyDefinition> Definitions =
        new Dictionary<string, EnemyDefinition>()
        {
            {
                EnemyIds.BasicDrone,
                new EnemyDefinition(
                    enemy: EnemyIds.BasicDrone,
                    name: "Drone"
                )
                {
                    baseMaxHP       = 60,
                    baseAttack      = 12,
                    baseInitiative  = 4,
                    moveSpeed       = 6,
                    power           = 12,
                    agility         = 10,
                    endurance       = 10,
                    cognition       = 1,
                    insight         = 1,
                    presence        = 1,
                    xpValue         = 50,
                    lootTableId     = LootTableIds.BasicDrone,
                    additionalActionIds = new List<string> { ActionIds.RangedAttack },
                }
            },
        };

    public static EnemyDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out EnemyDefinition enemy)) return enemy;

        Debug.LogWarning($"[EnemyDatabase] Enemy not found: {id}");
        return null;
    }
}
