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
    private Stack<ScreenId> overlayStack = new Stack<ScreenId>();
    private ScreenId currentScreen = ScreenId.None;

    //Overlays that hide the persistent chrome (mainbar + sidebar)
    private static readonly HashSet<ScreenId> ChromeHidingOverlays = new HashSet<ScreenId>
    {
        ScreenId.LevelUp,
        ScreenId.Settings,
        ScreenId.SaveLoad,
        ScreenId.SystemMap,
        ScreenId.CharacterDetail,
        ScreenId.Inventory,
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

        //Deactivate all overlays
        while (overlayStack.Count > 0)
        {
            ScreenId overlayId = overlayStack.Pop();
            if (screens.TryGetValue(overlayId, out var overlay))
                overlay.SetActive(false);
        }

        currentScreen = id;

        if (screens.TryGetValue(id, out var next))
            next.SetActive(true);

        if (chrome != null)
        {
            chrome.SetChromeVisible(true);
            chrome.UpdateContext(id);
        }
    }

    public void PushOverlay(ScreenId id)
    {
        overlayStack.Push(id);

        if (screens.TryGetValue(id, out var panel))
            panel.SetActive(true);

        if (chrome != null && ChromeHidingOverlays.Contains(id))
            chrome.SetChromeVisible(false);
    }

    public void PopOverlay()
    {
        if (overlayStack.Count == 0) return;

        ScreenId id = overlayStack.Pop();

        if (screens.TryGetValue(id, out var panel))
            panel.SetActive(false);

        //Restore chorme if no remaining overlays are chrome-hiding
        if (chrome != null && !AnyActiveOverlayHidesChrome())
            chrome.SetChromeVisible(true);
    }

    public ScreenId GetCurrentScreen() => currentScreen;
    public bool IsOverlayActive(ScreenId id) => overlayStack.Contains(id);

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

    //Helpers
    bool AnyActiveOverlayHidesChrome()
    {
        foreach (ScreenId id in overlayStack)
        {
            if (ChromeHidingOverlays.Contains(id)) return true;
        }
        return false;
    }
}
