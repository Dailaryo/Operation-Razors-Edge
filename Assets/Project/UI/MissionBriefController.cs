using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MissionBriefController : MonoBehaviour
{
    [Header("Mission Info")]
    [SerializeField] private TMP_Text missionNameText;
    [SerializeField] private TMP_Text missionDescriptionText;
    [SerializeField] private TMP_Text objectivesText;

    [Header("Squad Info")]
    [SerializeField] private TMP_Text squadCompositionText;

    [Header("Buttons")]
    [SerializeField] private Button deployButton;
    [SerializeField] private Button abortButton;

    void OnEnable()
    {
        Populate();

        if (deployButton != null)
        {
            deployButton.onClick.RemoveAllListeners();
            deployButton.onClick.AddListener(OnDeployPressed);
        }

        if (abortButton != null)
        {
            abortButton.onClick.RemoveAllListeners();
            abortButton.onClick.AddListener(OnAbortPressed);
        }
    }

    void Populate()
    {
        PlanetData planet = GameFlowManager.Instance.SelectedPlanet;

        if (missionNameText != null)
            missionNameText.text = planet != null ? planet.planetName : "Mission Briefing";

        if (missionDescriptionText != null)
            missionDescriptionText.text = planet != null ? $"Prepare for deployment to {planet.planetName}." : "";

        if (objectivesText != null)
            objectivesText.text = "";

        if (squadCompositionText != null)
        {
            var lines = new System.Collections.Generic.List<string>();
            foreach (var character in SquadManager.Instance.GetSquad())
                lines.Add($"{character.name} — {character.classId} Lvl {character.level}");
            squadCompositionText.text = string.Join("\n", lines);
        }
    }

    void OnDeployPressed()
    {
        GameFlowManager.Instance.EnterPlanetSide();
    }

    void OnAbortPressed()
    {
        GameFlowManager.Instance.ReturnToHub();
    }
}