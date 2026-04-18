using UnityEngine;
using TMPro;

public class MissionUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI progressText;

    private MissionData mission;
    private TurnManager turnManager;

    public void Initialize(MissionData missionData, TurnManager tm)
    {
        mission = missionData;
        turnManager = tm;

        UpdateObjectiveText();
        UpdateProgress();
    }

    void Update()
    {
        if (mission == null || turnManager == null) return;

        UpdateProgress();
    }

    void UpdateObjectiveText()
    {
        switch (mission.objectiveType)
        {
            case MissionData.MissionObjectiveType.Elimination:
                objectiveText.text = "Objective: Eliminate all enemies";
                break;

            case MissionData.MissionObjectiveType.Survival:
                objectiveText.text = $"Objective: SUrvive {mission.turnLimit} turns";
                break;

            case MissionData.MissionObjectiveType.TargetElimination:
                objectiveText.text = "Objective: Eliminate priority target";
                break;
        }
    }

    void UpdateProgress()
    {
        switch (mission.objectiveType)
        {
            case MissionData.MissionObjectiveType.Elimination:
                progressText.text = "";
                break;

            case MissionData.MissionObjectiveType.Survival:
                progressText.text = $"Turn: {turnManager.turnCount}/{mission.turnLimit}";
                break;

            case MissionData.MissionObjectiveType.TargetElimination:
                if (mission.targetUnit != null)
                {
                    bool alive = !mission.targetUnit.IsDead();
                    progressText.text = alive ? "Target: Alive" : "Target Eliminated";
                }
                break;
        }
    }
}
