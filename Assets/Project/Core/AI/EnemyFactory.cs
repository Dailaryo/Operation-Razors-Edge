using System.Collections.Generic;
using UnityEngine;

public static class EnemyFactory
{
    public static List<Unit> CreateBasicEnemies(int count)
    {
        List<Unit> enemies = new List<Unit>();

        for (int i = 0; i < count; i++)
        {
            var drone = new Unit("Drone", true, 60, 12, 4);

            drone.actionSources.Clear();

            drone.actionSources.Add(new BaseActionSource(new List<string>
            {
                ActionIds.Move,
                ActionIds.MeleeAttack,
                ActionIds.RangedAttack
            }));

            enemies.Add(drone);
        }

        return enemies;
    }
}
