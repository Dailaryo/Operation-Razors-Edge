using UnityEngine;

public class HubUIController : MonoBehaviour
{
    public static HubUIController Instance;

    [SerializeField] private RosterPanel rosterPanel;
    [SerializeField] private EquipmentPanel equipmentPanel;
    [SerializeField] private ShipInventoryPanel shipInventoryPanel;

    private CharacterData selectedCharacter;

    void Awake()
    {
        Instance = this;
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
}