using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndingUIController : MonoBehaviour
{
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI summaryText;
    [SerializeField] private TextMeshProUGUI journalText;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private GameObject creditsOverlay;
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private Button creditsCloseButton;

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
        if (state == GameFlowManager.GameState.Ending)
        {
            EndingData ending = CampaignManager.Instance != null
                ? CampaignManager.Instance.GetEnding()
                : null;

            if (ending != null)
                ShowEnding(ending);
        }
    }

    public void ShowEnding(EndingData ending)
    {
        if (endingPanel != null)
            endingPanel.SetActive(true);

        if (titleText != null)
            titleText.text = ending.title;

        if (summaryText != null)
            summaryText.text = ending.summary;

        var entries = NarrativeJournal.GetEntries();
        if (journalText != null)
            journalText.text = entries.Count > 0
                ? string.Join("\n\n", entries)
                : "No journal entries recorded.";
    }

    public void OnNewGamePressed()
    {
        if (endingPanel != null)
            endingPanel.SetActive(false);

        GameFlowManager.Instance.NewGame();
    }

    void OnCreditsPressed()
    {
        if (creditsOverlay == null) return;

        creditsOverlay.SetActive(true);

        if (creditsText != null)
            creditsText.text = "Operation Razor's Edge\n\nDesign & Development: Paul";
    }

    void OnCreditsClosePressed()
    {
        if (creditsOverlay != null)
            creditsOverlay.SetActive(false);
    }
}