using UnityEngine;

public class MissionData
{
    public string missionName;
    public string description;

    public int enemyCount;
    public int rewardAttack;
    public int missionReward;
    //Survival
    public int turnLimit;

    public enum MissionObjectiveType
    {
        Elimination,
        Survival,
        TargetElimination
    }

    public MissionObjectiveType objectiveType;

    //Target
    public Unit targetUnit;

    public MissionData(string name, int enemies, string desc, int attackReward)
    {
        missionName = name;
        enemyCount = enemies;
        description = desc;
        rewardAttack = attackReward;

        missionReward = attackReward * 50; //TEMP

        //Default Mission
        objectiveType = MissionObjectiveType.Elimination;
    }
}
