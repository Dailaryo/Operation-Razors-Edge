using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EncounterResultController : MonoBehaviour
{
    [Header("Result Header")]
    [SerializeField] private TMP_Text resultHeaderText;

    [Header("Objectives")]
    [SerializeField] private Transform objectiveContainer;
    [SerializeField] private GameObject objectiveEntryPrefab;

    [Header("Rewards")]
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private TMP_Text creditsText;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    [SerializeField] private CombatManager combatManager;

    private CombatEndState lastEndState;

    void OnEnable()
    {
        GameFlowManager.OnStateChanged += HandleStateChanged;
    }

    void OnDisable()
    {
        GameFlowManager.OnStateChanged -= HandleStateChanged;
    }

    void HandleStateChanged(GameFlowManager.GameState state)
    {
        if (state != GameFlowManager.GameState.MissionResult) return;

        Populate();
    }

    void Populate()
    {
        //Result header
        lastEndState = GetLastEndState();

        resultHeaderText.text = lastEndState switch
        {
            CombatEndState.FullVictory => "VICTORY",
            CombatEndState.PartialVictory => "PARTIAL VICTORY",
            CombatEndState.Retreat => "MISSION ABANDONED",
            CombatEndState.Defeat => "DEFEAT",
            _ => "RESULT"
        };

        //Objectives: populated from CombatManager.GetMissionState()
        ClearObjectives();

        MissionState missionState = combatManager != null ? combatManager.GetMissionState() : null;

        if (missionState != null)
        {
            foreach (var kvp in missionState.objectiveCompleted)
            {
                AddObjectiveEntry(kvp.Key, kvp.Value);
            }
        }

        //XP and credits, read from squad after GameFlowManager has already applied them
        int totalXP = 0;
        foreach (var character in SquadManager.Instance.GetSquad())
            totalXP += character.xp;

        if (xpText  != null)
            xpText.text = $"XP Awarded";

        if (creditsText != null)
            creditsText.text = $"Credits: {SquadManager.Instance.creditBalance.credits}";

        //Continue button
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinuePressed);
        }
    }

    void AddObjectiveEntry(string objectiveId, bool completed)
    {
        if (objectiveEntryPrefab == null || objectiveContainer == null) return;

        GameObject obj = Instantiate(objectiveEntryPrefab, objectiveContainer);
        TMP_Text label = obj.GetComponentInChildren<TMP_Text>();

        if (label!= null)
            label.text = $"{objectiveId}: {(completed ? "Achieved" : "Failed")}";
    }

    void ClearObjectives()
    {
        if (objectiveContainer == null) return;

        for (int i = objectiveContainer.childCount - 1; i >= 0; i--)
            Destroy(objectiveContainer.GetChild(i).gameObject);
    }

    void OnContinuePressed()
    {
        if (LootQueue.HasLoot && lastEndState != CombatEndState.Retreat)
        {
            UIManager.Instance.PushOverlay(ScreenId.Loot);
        }
        else
        {
            GameFlowManager.Instance.ReturnToPlanetSide();
        }
    }

    // GameFlowManager doesn't currently expose last end state publicly.
    // ResultUIController uses a parameter passed directly — we read from
    // the existing ResultUIController if present, otherwise default to FullVictory.
    CombatEndState GetLastEndState()
    {
        return GameFlowManager.Instance != null
            ? GameFlowManager.Instance.LastCombatEndState
            : CombatEndState.FullVictory;
    }
}
