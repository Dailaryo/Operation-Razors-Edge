using UnityEngine;

public class MissionData
{
    public string missionName;
    public int enemyCount;
    public string description;

    public int rewardAttack;

    public MissionData(string name, int enemies, string desc, int attackReward)
    {
        missionName = name;
        enemyCount = enemies;
        description = desc;
        rewardAttack = attackReward;
    }
}
