using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class JournalPanel : MonoBehaviour
{
    [Header("Journal Entries")]
    [SerializeField] private TMP_Text journalEntriesText;
    [SerializeField] private ScrollRect journalScrollRect;

    [Header("Narrative Flags")]
    [SerializeField] private TMP_Text narrativeFlagsText;

    [Header("Navigation")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button journalTabButton;
    [SerializeField] private Button flagsTabButton;

    [Header("Tab Panels")]
    [SerializeField] private GameObject journalTabPanel;
    [SerializeField] private GameObject flagsTabPanel;

    void OnEnable()
    {
        PopulateJournal();
        PopulateFlags();
        ShowJournalTab();

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => UIManager.Instance.PopOverlay());
        }

        if (journalTabButton != null)
        {
            journalTabButton.onClick.RemoveAllListeners();
            journalTabButton.onClick.AddListener(ShowJournalTab);
        }

        if (flagsTabButton != null)
        {
            flagsTabButton.onClick.RemoveAllListeners();
            flagsTabButton.onClick.AddListener(ShowFlagsTab);
        }
    }

    void PopulateJournal()
    {
        if (journalEntriesText == null) return;

        var entries = NarrativeJournal.GetEntries();
        journalEntriesText.text = entries.Count > 0
            ? string.Join("\n\n", entries)
            : "No journal entries recorded.";
    }

    void PopulateFlags()
    {
        if (narrativeFlagsText == null) return;

        var keys = NarrativeFlagManager.GetAllKeys();
        if (keys.Count == 0)
        {
            narrativeFlagsText.text = "No flags set.";
            return;
        }

        var lines = new List<string>();
        foreach (var key in keys)
            lines.Add($"{key}: {NarrativeFlagManager.GetFlag(key)}");

        narrativeFlagsText.text = string.Join("\n", lines);
    }

    void ShowJournalTab()
    {
        if (journalTabPanel != null) journalTabPanel.SetActive(true);
        if (flagsTabPanel != null) flagsTabPanel.SetActive(false);

        // Scroll to top
        Canvas.ForceUpdateCanvases();
        if (journalScrollRect != null)
            journalScrollRect.verticalNormalizedPosition = 1f;
    }

    void ShowFlagsTab()
    {
        if (journalTabPanel != null) journalTabPanel.SetActive(false);
        if (flagsTabPanel != null) flagsTabPanel.SetActive(true);
    }
}