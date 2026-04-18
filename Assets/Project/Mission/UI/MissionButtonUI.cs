using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MissionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Button button;

    private int missionIndex;

    public void Initialize(MissionData mission, int index, GameFlowManager manager)
    {
        missionIndex = index;

        string objectiveText = "";

        switch (mission.objectiveType)
        {
            case MissionData.MissionObjectiveType.Elimination:
                objectiveText = "Eliminate All";
                break;

            case MissionData.MissionObjectiveType.Survival:
                objectiveText = $"Survive {mission.turnLimit} Turns";
                break;

            case MissionData.MissionObjectiveType.TargetElimination:
                objectiveText = "Eliminate Target";
                break;
        }

        label.text =
            $"{mission.missionName}\n" +
            $"{objectiveText}\n" +
            $"Reward: {mission.missionReward}";

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(() =>
        {
            manager.SelectMission(missionIndex);
        });
    }
}
