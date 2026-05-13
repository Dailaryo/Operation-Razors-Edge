using UnityEngine;

public class ObjectiveDefinition
{
    public string objectiveId;
    public string description;

    public IObjectiveCondition condition;
    public IObjectiveCondition failCondition; //null = no independent fail condition

    public MissionReward reward;

    public bool visibleAtStart;
    public bool isPrimary;

    public string narrativeFlagOnComplete; //null = no flag
    public string narrativeFlagOnFail; //null = no flag

    public ObjectiveDefinition(
        string objectiveId,
        string description,
        IObjectiveCondition condition,
        MissionReward reward,
        bool visibleAtStart = true,
        bool isPrimary = false,
        IObjectiveCondition failCondition = null,
        string narrativeFlagOnComplete = null,
        string narrativeFlagOnFail = null)
    {
        this.objectiveId = objectiveId;
        this.description = description;
        this.condition = condition;
        this.reward = reward;
        this.visibleAtStart = visibleAtStart;
        this.isPrimary = isPrimary;
        this.failCondition = failCondition;
        this.narrativeFlagOnComplete = narrativeFlagOnComplete;
        this.narrativeFlagOnFail = narrativeFlagOnFail;
    }
}
