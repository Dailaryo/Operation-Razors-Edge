using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MissionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Button button;

    private int missionIndex;

    public void Initialize(EncounterData mission, int index, GameFlowManager manager)
    {
        missionIndex = index;

        label.text =
            $"{mission.encounterName}\n" +
            $"{mission.primaryObjective?.description ?? "No objective"}\n" +
            $"Reward: {mission.primaryObjective?.reward?.credits ?? 0} credits";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            //manager.SelectEncounter(missionIndex);
        });
    }
}
