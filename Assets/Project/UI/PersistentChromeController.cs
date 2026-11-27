using UnityEngine;
using System.Collections.Generic;

public class PersistentChromeController : MonoBehaviour
{
    public static PersistentChromeController Instance;

    [Header("Chrome Root Objects")]
    [SerializeField] private GameObject mainbar;
    [SerializeField] private GameObject sidebar;

    [Header("Portrait Strip")]
    [SerializeField] private Transform portraitContainer;
    [SerializeField] private GameObject portraitSlotPrefab;

    [Header("Sidebar Buttons")]
    [SerializeField] private UnityEngine.UI.Button squadButton;
    [SerializeField] private UnityEngine.UI.Button inventoryButton;
    [SerializeField] private UnityEngine.UI.Button journalButton;
    [SerializeField] private UnityEngine.UI.Button respecButton;
    [SerializeField] private UnityEngine.UI.Button missionBriefingButton;
    [SerializeField] private UnityEngine.UI.Button saveLoadButton;
    [SerializeField] private UnityEngine.UI.Button optionsButton;

    private List<SquadPortraitUI> portraitSlots = new List<SquadPortraitUI>();
    private ScreenId currentContext = ScreenId.None;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        SquadManager.Instance?.GetSquad();
        GameFlowManager.OnLevelUpPending += HandleLevelUpPending;
    }

    void OnDisable()
    {
        GameFlowManager.OnLevelUpPending -= HandleLevelUpPending;
    }

    //Called by UIManager.PushOverlay() and PopOverlay()
    public void SetChromeVisible(bool visible)
    {
        if (mainbar != null) mainbar.SetActive(visible);
        if (sidebar != null) sidebar.SetActive(visible);
    }

    //Called by UIManager.ShowScreen() after every screen transition
    public void UpdateContext(ScreenId screen)
    {
        currentContext = screen;

        bool inHub = screen == ScreenId.Hub;
        bool inCombat = screen == ScreenId.Combat;

        //Hub-only sidebar buttons
        if (respecButton != null)
            respecButton.gameObject.SetActive(inHub);
        if (missionBriefingButton != null)
            missionBriefingButton.gameObject.SetActive(inHub);

        //Inventory greyed out during combat, available otherwise
        if (inventoryButton != null)
            inventoryButton.interactable = !inCombat;

        RefreshPortraits();
    }

    //Portrait Strip

    public void RefreshPortraits()
    {
        if (SquadManager.Instance == null) return;

        var squad = SquadManager.Instance.GetSquad();

        //Rebuild portrait slots if squad size changed
        if (portraitSlots.Count != squad.Count)
            RebuildPortraitStrip(squad);
        else
            UpdatePortraitData(squad);
    }

    void RebuildPortraitStrip(List<CharacterData> squad)
    {
        //Clear existing
        foreach (Transform child in portraitContainer)
            Destroy(child.gameObject);

        portraitSlots.Clear();

        foreach (var character in squad)
        {
            GameObject obj = Instantiate(portraitSlotPrefab, portraitContainer);
            var portrait = obj.GetComponent<SquadPortraitUI>();
            if (portrait != null)
            {
                portrait.Bind(character);
                portraitSlots.Add(portrait);
            }
        }
    }

    void UpdatePortraitData(List<CharacterData> squad)
    {
        for (int i = 0; i < portraitSlots.Count; i++)
        {
            if (i < squad.Count)
                portraitSlots[i].Bind(squad[i]);
        }
    }

    //Event Handlers
    void HandleLevelUpPending(CharacterData character)
    {
        //Find the portrait for this character and enable its Level Up button
        foreach (var portrait in portraitSlots)
        {
            if (portrait.BoundCharacter == character)
            {
                portrait.SetLevelUpAvailable(true);
                return;
            }
        }
    }

    //Sidebar Button Wiring
    //These are called by the Inspector onClick events on each sidebar button.
    public void OnSquadButtonPressed()
    {
        UIManager.Instance.PushOverlay(ScreenId.CharacterDetail);
    }

    public void OnInventoryButtonPressed()
    {
        UIManager.Instance.PushOverlay(ScreenId.Inventory);
    }

    public void OnJournalButtonPressed()
    {
        UIManager.Instance.PushOverlay(ScreenId.Journal);
    }

    public void OnRespecButtonPressed()
    {
        UIManager.Instance.PushOverlay(ScreenId.Respec);
    }

    public void OnMissionBriefingButtonPressed()
    {
        UIManager.Instance.ShowScreen(ScreenId.MissionBrief);
    }

    public void OnSaveLoadButtonPressed()
    {
        UIManager.Instance.PushOverlay(ScreenId.SaveLoad);
    }

    public void OnOptionsButtonPressed()
    {
        UIManager.Instance.PushOverlay(ScreenId.Settings);
    }

    //Quits the game. Application.Quit does nothing in the Editor, so Play Mode is stopped instead
    public void OnExitGamePressed()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
