using TMPro;
using UnityEngine;

public class HubUIController : MonoBehaviour
{
    public static HubUIController Instance;

    [SerializeField] private RosterPanel rosterPanel;
    [SerializeField] private EquipmentPanel equipmentPanel;
    [SerializeField] private ShipInventoryPanel shipInventoryPanel;
    [Header("Debug (editor / development builds only)")]
    [SerializeField] private GameObject debugControlsRoot;
    [SerializeField] private TMP_InputField debugLevelInput;

    private CharacterData selectedCharacter;

    void Awake()
    {
        Instance = this;

        //D9: debug controls only exist in the editor and development builds
        if (debugControlsRoot != null)
            debugControlsRoot.SetActive(Debug.isDebugBuild);
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Show()
    {
        Refresh();
    }

    public void Hide() { }

    public void Refresh()
    {
        if (rosterPanel != null)
            rosterPanel.Refresh();

        if (shipInventoryPanel != null)
            shipInventoryPanel.Refresh();

        if (PersistentChromeController.Instance != null)
            PersistentChromeController.Instance.RefreshPortraits();
    }

    public void OnCharacterSelected(CharacterData character)
    {
        selectedCharacter = character;

        if (equipmentPanel != null)
            equipmentPanel.SetCharacter(character);
    }

    public CharacterData GetSelectedCharacter() => selectedCharacter;

    public void RefreshShipInventory()
    {
        if (shipInventoryPanel != null)
            shipInventoryPanel.Refresh();
    }

    public void OnDeployButtonPressed()
    {
        PlanetData planet = PlanetDatabase.Get(PlanetIds.PlanetStart);
        if (planet != null)
            GameFlowManager.Instance.SelectPlanet(planet);
    }


    //Debug controls (development builds only)

    //Called by the debug Set Level button onClick in the Inspector
    public void OnDebugSetSquadLevelPressed()
    {
        if (!Debug.isDebugBuild) return;

        if (debugLevelInput == null || !int.TryParse(debugLevelInput.text, out int level))
        {
            Debug.LogWarning("[HubUI] Enter a level from 1 to 20.");
            return;
        }

        SquadManager.Instance.DebugRebuildSquadToLevel(level);
        Refresh();
    }

    //Called by the debug Training button onClick in the Inspector (D7/D9).
    //Opens the Training planet through the normal exploration flow, mirroring OnDeployButtonPressed.
    public void OnDebugTrainingPressed()
    {
        if (!Debug.isDebugBuild) return;

        PlanetData planet = PlanetDatabase.Get(PlanetIds.Training);
        if (planet != null)
            GameFlowManager.Instance.SelectPlanet(planet);
    }
}