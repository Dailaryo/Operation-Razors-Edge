using UnityEngine;
using TMPro;

public class MissionUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private TextMeshProUGUI progressText;

    private EncounterData mission;
    private TurnManager turnManager;
    private CombatManager combatManager;

    private int extractionDeadline = 0;

    public void Initialize(EncounterData missionData, TurnManager tm, CombatManager cm)
    {
        mission = missionData;
        turnManager = tm;
        combatManager = cm;

        if (combatManager != null)
            combatManager.OnSecondaryObjectiveRevealed += HandleObjectiveRevealed;

        UpdateObjectiveText();
        UpdateProgress();
    }

    public void SetExtractionDeadline(int deadline)
    {
        extractionDeadline = deadline;
    }

    void Update()
    {
        if (mission == null || turnManager == null) return;

        UpdateProgress();
    }

    void UpdateObjectiveText()
    {
        if (mission.primaryObjective != null)
            objectiveText.text = $"Objective: {mission.primaryObjective.description}";
        else
            objectiveText.text = "Objective: Unknown";
    }

    void UpdateProgress()
    {
        string text = $"Turn: {turnManager.turnCount}";

        if (extractionDeadline > 0)
        {
            int turnsRemaining = Mathf.Max(0, extractionDeadline - turnManager.turnCount);
            text += $"\nExtraction: {turnsRemaining} turn(s)";
        }

        progressText.text = text;
    }

    private void OnDestroy()
    {
        if (combatManager != null)
            combatManager.OnSecondaryObjectiveRevealed -= HandleObjectiveRevealed;
    }

    void HandleObjectiveRevealed(ObjectiveDefinition objective)
    {
        CombatLog.Add($"[UI] Secondary objective now visible: {objective.description}");
        UpdateObjectiveText();
    }
}