using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ExplorationUIController : MonoBehaviour
{
    [Header("Zone Lists")]
    [SerializeField] private Transform socialZoneContainer;
    [SerializeField] private Transform neutralZoneContainer;
    [SerializeField] private Transform hostileZoneContainer;
    [SerializeField] private GameObject zoneEntryPrefab;

    [Header("Stealth Toggle")]
    [SerializeField] private Toggle stealthToggle;
    [SerializeField] private TMP_Text stealthNoteText;


    [Header("Debug (editor / development builds only)")]
    [SerializeField] private Button debugStealthButton;
    [SerializeField] private TMP_Text debugStealthLabel;
    [SerializeField] private Button debugXpButton;          //"Grant XP": shows the entry row
    [SerializeField] private GameObject debugXpEntryRoot;   //Holds the input field and the Grant button
    [SerializeField] private TMP_InputField debugXpInput;
    [SerializeField] private Button debugXpGrantButton;
    [SerializeField] private Button debugRestoreButton;      //"Restore Squad": Training-only full rest

    [Header("Encounter Prompt")]
    [SerializeField] private GameObject encounterPromptPanel;
    [SerializeField] private TMP_Text encounterNameText;
    [SerializeField] private TMP_Text encounterDescriptionText;
    [SerializeField] private Transform encounterOptionContainer;
    [SerializeField] private GameObject encounterOptionPrefab;

    [Header("Navigation")]
    [SerializeField] private Button departButton;

    void OnEnable()
    {
        Refresh();

        if (stealthToggle != null)
        {
            stealthToggle.onValueChanged.RemoveAllListeners();
            stealthToggle.onValueChanged.AddListener(OnStealthToggled);
        }

        if (departButton != null)
        {
            departButton.onClick.RemoveAllListeners();
            departButton.onClick.AddListener(OnDepartPressed);
        }

        if (encounterPromptPanel != null)
            encounterPromptPanel.SetActive(false);

        //Debug controls only exist in the editor and development builds
        if (debugStealthButton != null)
        {
            debugStealthButton.gameObject.SetActive(Debug.isDebugBuild);
            debugStealthButton.onClick.RemoveAllListeners();
            debugStealthButton.onClick.AddListener(OnDebugStealthPressed);
        }

        UpdateDebugStealthLabel();

        if (debugXpButton != null)
        {
            debugXpButton.gameObject.SetActive(Debug.isDebugBuild);
            debugXpButton.onClick.RemoveAllListeners();
            debugXpButton.onClick.AddListener(OnDebugXpPressed);
        }

        if (debugXpGrantButton != null)
        {
            debugXpGrantButton.onClick.RemoveAllListeners();
            debugXpGrantButton.onClick.AddListener(OnDebugXpGrantPressed);
        }

        if (debugXpEntryRoot != null)
            debugXpEntryRoot.SetActive(false);

        if (debugRestoreButton != null)
        {
            debugRestoreButton.onClick.RemoveAllListeners();
            debugRestoreButton.onClick.AddListener(OnDebugRestorePressed);
        }
    }

    public void Refresh()
    {
        if (ExplorationController.Instance == null) return;

        ClearZoneLists();

        var zones = ExplorationController.Instance.GetAvailableZones();

        bool anySocialZone = false;

        foreach (var zone in zones)
        {
            Transform container = zone.zoneType switch
            {
                ZoneDefinition.ZoneType.Social => socialZoneContainer,
                ZoneDefinition.ZoneType.Neutral => neutralZoneContainer,
                ZoneDefinition.ZoneType.Hostile => hostileZoneContainer,
                _ => neutralZoneContainer
            };

            if (zone.zoneType == ZoneDefinition.ZoneType.Social)
                anySocialZone = true;

            AddZoneEntry(container, zone);
        }

        // Hide stealth toggle when all zones are social
        if (stealthToggle != null)
            stealthToggle.gameObject.SetActive(!anySocialZone);

        if (stealthNoteText != null)
        {
            stealthNoteText.gameObject.SetActive(ExplorationController.Instance.SquadStealthMode);
            //Keep the checkbox in step with the controller without firing onValueChanged
            stealthToggle.SetIsOnWithoutNotify(ExplorationController.Instance.SquadStealthMode);
        }

        //Restore Squad is a Training-only debug control
        if (debugRestoreButton != null)
            debugRestoreButton.gameObject.SetActive(Debug.isDebugBuild && IsOnTrainingPlanet());
    }

    void AddZoneEntry(Transform container, ZoneDefinition zone)
    {
        if (container == null || zoneEntryPrefab == null) return;

        GameObject obj = Instantiate(zoneEntryPrefab, container);
        TMP_Text label = obj.GetComponentInChildren<TMP_Text>();

        if (label != null)
            label.text = zone.displayName;

        string capturedId = zone.zoneId;
        var button = obj.GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(() => OnZoneSelected(capturedId));
    }

    void ClearZoneLists()
    {
        ClearContainer(socialZoneContainer);
        ClearContainer(neutralZoneContainer);
        ClearContainer(hostileZoneContainer);
    }

    void ClearContainer(Transform container)
    {
        if (container == null) return;
        for (int i = container.childCount - 1; i >= 0; i--)
            Destroy(container.GetChild(i).gameObject);
    }

    void OnZoneSelected(string zoneId)
    {
        if (ExplorationController.Instance == null) return;

        ExplorationController.Instance.EnterZone(zoneId);

        var events = ExplorationController.Instance.GetAvailableZoneEvents(zoneId);

        if (events.Count == 0)
        {
            CombatLog.Add("[ExplorationUI] No events available in this zone.");
            return;
        }

        ShowEncounterPrompt(ZoneDatabase.Get(zoneId), events);
    }

    //Zone event picker (G11): one option per available event, plus Back. Replaces the old events[0] auto-trigger.
    void ShowEncounterPrompt(ZoneDefinition zone, List<ZoneEventDefinition> events)
    {
        if (encounterPromptPanel == null || encounterOptionContainer == null || encounterOptionPrefab == null)
        {
            Debug.LogWarning("[ExplorationUI] Encounter prompt is not wired; triggering the first event.");
            ExplorationController.Instance.TriggerEncounter(events[0].zoneEventId);
            return;
        }

        ClearContainer(encounterOptionContainer);

        if (encounterNameText != null)
            encounterNameText.text = zone != null ? zone.displayName : "Choose an event";

        if (encounterDescriptionText != null)
            encounterDescriptionText.text = events.Count == 1
                ? "1 event available."
                : $"{events.Count} events available.";

        foreach (var zoneEvent in events)
        {
            string capturedId = zoneEvent.zoneEventId;
            AddEncounterOption(zoneEvent.displayName + GetEventSummary(zoneEvent),
                () => OnEncounterOptionSelected(capturedId));
        }

        AddEncounterOption("Back", HideEncounterPrompt);

        encounterPromptPanel.SetActive(true);
    }

    void AddEncounterOption(string text, UnityEngine.Events.UnityAction onClick)
    {
        GameObject obj = Instantiate(encounterOptionPrefab, encounterOptionContainer);

        TMP_Text label = obj.GetComponentInChildren<TMP_Text>();
        if (label != null)
            label.text = text;

        var button = obj.GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(onClick);
    }

    //Combat events append their encounter's description (e.g. a drill's goal) after the name
    string GetEventSummary(ZoneEventDefinition zoneEvent)
    {
        if (zoneEvent.zoneEventType != ZoneEventDefinition.ZoneEventType.Combat) return "";
        if (string.IsNullOrEmpty(zoneEvent.combatEncounterId)) return "";

        var encounter = EncounterDatabase.Get(zoneEvent.combatEncounterId);

        return encounter != null && !string.IsNullOrEmpty(encounter.description)
            ? $" — {encounter.description}"
            : "";
    }

    void OnEncounterOptionSelected(string zoneEventId)
    {
        HideEncounterPrompt();

        if (ExplorationController.Instance != null)
            ExplorationController.Instance.TriggerEncounter(zoneEventId);
    }

    void HideEncounterPrompt()
    {
        if (encounterPromptPanel != null)
            encounterPromptPanel.SetActive(false);
    }

    void OnStealthToggled(bool value)
    {
        if (ExplorationController.Instance == null) return;

        ExplorationController.Instance.SetStealthMode(value);

        //SetStealthMode can refuse (social zone), so mirror what actually took effect
        bool active = ExplorationController.Instance.SquadStealthMode;
        stealthToggle.SetIsOnWithoutNotify(active);

        if (stealthNoteText != null)
            stealthNoteText.gameObject.SetActive(active);
    }

    void OnDepartPressed()
    {
        if (ExplorationController.Instance != null)
            ExplorationController.Instance.DepartPlanet();
    }

    void OnDebugStealthPressed()
    {
        if (ExplorationController.Instance == null) return;

        ExplorationController.Instance.CycleDebugStealthOverride();
        UpdateDebugStealthLabel();
    }

    void UpdateDebugStealthLabel()
    {
        if (debugStealthLabel == null || ExplorationController.Instance == null) return;

        debugStealthLabel.text = ExplorationController.Instance.DebugStealthOverride switch
        {
            ExplorationController.DebugStealthResult.ForceSuccess => "Stealth: Force Success",
            ExplorationController.DebugStealthResult.ForceFail => "Stealth: Force Fail",
            _ => "Stealth: Roll"
        };
    }

    //Debug: show or hide the XP entry row
    void OnDebugXpPressed()
    {
        if (debugXpEntryRoot != null)
            debugXpEntryRoot.SetActive(!debugXpEntryRoot.activeSelf);
    }

    void OnDebugXpGrantPressed()
    {
        if (debugXpInput == null || !int.TryParse(debugXpInput.text, out int amount) || amount <= 0)
        {
            Debug.LogWarning("[ExplorationUI] Enter a positive whole number of XP");
            return;
        }

        SquadManager.Instance.DebugGrantXP(amount);

        debugXpInput.text = "";
        if (debugXpEntryRoot != null)
            debugXpEntryRoot.SetActive(false);
    }

    //Debug: Training-planet full rest (Restore Squad)
    void OnDebugRestorePressed()
    {
        if (SquadManager.Instance == null) return;

        SquadManager.Instance.DebugRestoreSquad();

        if (PersistentChromeController.Instance != null)
            PersistentChromeController.Instance.RefreshPortraits();
    }

    static bool IsOnTrainingPlanet()
    {
        var explorer = ExplorationController.Instance;
        return explorer != null && explorer.currentPlanet != null &&
               explorer.currentPlanet.planetId == PlanetIds.Training;
    }
}