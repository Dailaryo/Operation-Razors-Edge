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
            stealthNoteText.gameObject.SetActive(
                ExplorationController.Instance.SquadStealthMode && !anySocialZone);
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

        // For now auto-trigger the first available event.
        // A full zone event picker UI can be added in a later pass.
        ExplorationController.Instance.TriggerEncounter(events[0].zoneEventId);
    }

    void OnStealthToggled(bool value)
    {
        if (ExplorationController.Instance == null) return;

        ExplorationController.Instance.SetStealthMode(value);

        if (stealthNoteText != null)
            stealthNoteText.gameObject.SetActive(value);
    }

    void OnDepartPressed()
    {
        if (ExplorationController.Instance != null)
            ExplorationController.Instance.DepartPlanet();
    }
}