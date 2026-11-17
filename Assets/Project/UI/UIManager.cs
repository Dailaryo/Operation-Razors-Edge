using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [System.Serializable]
    public struct ScreenEntry
    {
        public ScreenId id;
        public GameObject panel;
    }

    [SerializeField] private List<ScreenEntry> screenList;
    [SerializeField] private PersistentChromeController chrome;

    private Dictionary<ScreenId, GameObject> screens = new Dictionary<ScreenId, GameObject>();
    //Only one overlay is ever open (standing UI rule): opening another replaces it; Back returns to the main screen
    private ScreenId currentOverlay = ScreenId.None;
    private ScreenId currentScreen = ScreenId.None;

    //Overlays that hide the persistent chrome (mainbar + sidebar).
    //Character Detail and Inventory keep the mainbar visible so the squad cards stay usable.
    private static readonly HashSet<ScreenId> ChromeHidingOverlays = new HashSet<ScreenId>
    {
        ScreenId.LevelUp,
        ScreenId.Settings,
        ScreenId.SaveLoad,
        ScreenId.SystemMap,
        ScreenId.Journal
    };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var entry in screenList)
        {
            if (entry.panel != null)
                screens[entry.id] = entry.panel;
        }
    }

    void OnEnable()
    {
        GameFlowManager.OnStateChanged += HandleStateChanged;
    }

    void OnDisable()
    {
        GameFlowManager.OnStateChanged -= HandleStateChanged;
    }

    //Public Interface
    public void ShowScreen(ScreenId id)
    {
        //Deactivate current screen
        if (currentScreen != ScreenId.None && screens.TryGetValue(currentScreen, out var current))
            current.SetActive(false);

        //Close the open overlay, if any
        CloseCurrentOverlay();

        currentScreen = id;

        if (screens.TryGetValue(id, out var next))
            next.SetActive(true);

        if (chrome != null)
        {
            chrome.SetChromeVisible(true);
            chrome.UpdateContext(id);
        }
    }

    //Opens an overlay. Any overlay already open is closed first: only one is ever open
    public void PushOverlay(ScreenId id)
    {
        if (currentOverlay == id) return;

        CloseCurrentOverlay();

        currentOverlay = id;

        if (screens.TryGetValue(id, out var panel))
            panel.SetActive(true);

        if (chrome != null)
            chrome.SetChromeVisible(!ChromeHidingOverlays.Contains(id));
    }

    //Back: closes the open overlay and returns to the main screen with its chrome
    public void PopOverlay()
    {
        if (currentOverlay == ScreenId.None) return;

        CloseCurrentOverlay();

        if (chrome != null)
            chrome.SetChromeVisible(true);
    }

    public ScreenId GetCurrentScreen() => currentScreen;
    public bool IsOverlayActive(ScreenId id) => currentOverlay == id;

    void CloseCurrentOverlay()
    {
        if (currentOverlay == ScreenId.None) return;

        if (screens.TryGetValue(currentOverlay, out var panel))
            panel.SetActive(false);

        currentOverlay = ScreenId.None;
    }

    //State mapping
    void HandleStateChanged(GameFlowManager.GameState state)
    {
        switch (state)
        {
            case GameFlowManager.GameState.Hub:
                ShowScreen(ScreenId.Hub);
                break;
            case GameFlowManager.GameState.MissionBrief:
                ShowScreen(ScreenId.MissionBrief);
                break;
            case GameFlowManager.GameState.Combat:
                ShowScreen(ScreenId.Combat);
                break;
            case GameFlowManager.GameState.MissionResult:
                ShowScreen(ScreenId.EncounterResult);
                break;
            case GameFlowManager.GameState.PlanetSide:
                ShowScreen(ScreenId.PlanetSide);
                break;
            case GameFlowManager.GameState.SystemMap:
                ShowScreen(ScreenId.SystemMap);
                break;
            case GameFlowManager.GameState.Ending:
                ShowScreen(ScreenId.Ending);
                break;
        }
    }
}
