using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;
    
    //Controls high-level game flow between hub, missions and combat
    public enum GameState
    {
        Hub,
        SystemMap,
        MissionBrief,
        PlanetSide,
        Combat,
        MissionResult,
        Ending
    }

    public GameState currentState;

    PlanetData selectedPlanet;
    public PlanetData SelectedPlanet => selectedPlanet;

    // Tracks whether the current combat originated from a zone event (true)
    // or was launched directly as a standalone encounter (false).
    // Controls post-combat routing in EndCombat().
    private bool isZoneEncounter;

    EncounterData selectedEncounter;
    private EncounterData lastEncounter;
    public EncounterData SelectedEncounter => selectedEncounter;

    private CombatEndState lastCombatEndState;
    public CombatEndState LastCombatEndState => lastCombatEndState;

    private ZoneEventDefinition activeZoneEvent;

    [SerializeField] GameObject gridRoot;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private CampaignManager campaignManager;
    [SerializeField] private CharacterCreationManager characterCreationManager;

    void Start()
    {
        HideAllPanels();

        gridRoot.SetActive(false);

        if (combatManager != null)
        {
            combatManager.OnCombatEnded += HandleCombatEnded;
        }
        else
        {
            Debug.LogError("CombatManager not assigned in GameFlowManager");
        }

        //Startup order: build the squad, load the save onto it, then enter the Hub.
        //Load needs the roster to exist (it matches characters by id), and EnterHub's restore and heal
        //must run after it: launch always lands at the Hub, which is a full restore.
        if (SquadManager.Instance.GetSquad().Count == 0)
            CharacterCreationManager.Instance.StartNewGame();

        if (campaignManager != null)
            campaignManager.LoadSaveIfPresent();

        EnterHub();
    }

    void HideAllPanels()
    {
        
    }

    public void EnterHub()
    {
        // Temp: auto-populate squad if empty (no new game flow yet)
        if (SquadManager.Instance.GetSquad().Count == 0)
            CharacterCreationManager.Instance.StartNewGame();

        currentState = GameState.Hub;

        if (HubUIController.Instance != null)
            HubUIController.Instance.Show();

        gridRoot.SetActive(false);

        CombatLog.Add("=== HUB ===");

        SquadManager.Instance.RestoreIncapacitatedCharacters();
        SquadManager.Instance.ResetSquadToFull();

        if (HubEntryController.Instance != null)
            HubEntryController.Instance.RunHubEntryComparison();

        CheckLevelUpPending();
        OnStateChanged?.Invoke(currentState);
        AudioEventBus.Fire(AudioEventIds.GameFlow.HubEntered);
    }

    public void StartCombat(bool surpriseRound = false)
    {
        currentState = GameState.Combat;

        gridRoot.SetActive(true);

        // Load map for this encounter
        var mapDef = MapDatabase.Get(selectedEncounter.mapId);
        if (mapDef != null)
            GridManager.Instance.LoadMap(mapDef);
        else
            GridManager.Instance.LoadMap();

        // Register the emergency extraction zone based on the map's entry edge.
        // This zone is always generated so emergency extraction is available on any map.
        GridManager.Instance.GenerateExtractionZone(
            mapDef != null ? mapDef.squadEntryZone : MapDefinition.EntryZone.West);

        //G20: redraw tiles at this map's size (Rebuild also applies terrain colors)
        if (GridVisualizer.Instance != null)
            GridVisualizer.Instance.Rebuild();

        CombatLog.Add("Combat Started");

        if (combatManager != null)
        {
            var playerUnits = SquadManager.Instance.CreateCombatUnits();
            SquadManager.Instance.SetLiveCombatUnits(playerUnits);
            //Training encounters live on no campaign planet; tier 1 without the lookup warning
            int planetTier = selectedEncounter.isTraining
                ? 1
                : campaignManager.GetPlanetTierForEncounter(selectedEncounter);
            var enemyUnits = new List<Unit>();

            if (activeZoneEvent != null && activeZoneEvent.enemySpawnEntries.Count > 0)
            {
                foreach (var entry in activeZoneEvent.enemySpawnEntries)
                {
                    //spawnLevel 0 = match the squad (highest squad level), so drills follow the debug level control
                    int level = entry.spawnLevel > 0 ? entry.spawnLevel : SquadManager.Instance.GetHighestLevel();
                    var spawned = EnemyFactory.CreateGroup(entry.enemyId, entry.count, level, planetTier);
                    enemyUnits.AddRange(spawned);
                }
            }
            else
            {
                //Fallback: hub path or zone event without authored spawn entries
                enemyUnits = EnemyFactory.CreateGroup(EnemyIds.BasicDrone, selectedEncounter.enemyCount, 1, planetTier);
            }

            //Exploration stealth is the only surprise source so far, and it is always the squad's.
            //DEFERRED: enemy ambushes (SurpriseSide.Enemies) need an encounter-level source when the first one is authored.
            var surpriseSide = surpriseRound ? CombatManager.SurpriseSide.Squad : CombatManager.SurpriseSide.None;

            combatManager.StartCombat(selectedEncounter, playerUnits, enemyUnits, surpriseSide);
        }
        else
        {
            CombatLog.Add("CombatManager not found");
        }

        lastEncounter = selectedEncounter;
        OnStateChanged?.Invoke(currentState);
    }

    public void EndCombat(CombatEndState endState)
    {
        lastCombatEndState = endState;

        currentState = GameState.MissionResult;

        //D8: drills leave flags, journal, act progress and the save file untouched
        if (!lastEncounter.isTraining)
            campaignManager.RecordEncounterOutcome(lastEncounter, endState);
        else
            CombatLog.Add($"[Training] {lastEncounter.encounterName} ended ({endState}): campaign state unchanged.");

        gridRoot.SetActive(false);

        // Handle post-combat loot
        if (endState == CombatEndState.Retreat)
        {
            // Squad is extracting — no time to loot
            LootQueue.Clear();
        }
        // Otherwise loot screen will be shown by UI pass (Milestone 26 deferred)
        // LootQueue.HasLoot can be checked here to gate the screen when it exists

        // Route to ending screen if this was the final mission and player won
        if (endState == CombatEndState.FullVictory && lastEncounter.isFinalEncounter)
        {
            currentState = GameState.Ending;

            EndingData ending = campaignManager.GetEnding();

            activeZoneEvent = null;
            OnStateChanged?.Invoke(currentState);
            return;
        }
        
        // Route back to PlanetSide if this was a zone encounter
        if (isZoneEncounter)
        {
            isZoneEncounter = false;

            bool success = endState != CombatEndState.Defeat;

            //D8: drills don't mark completion, stamp zone flags or save planet state
            if (ExplorationController.Instance != null && !lastEncounter.isTraining)
                ExplorationController.Instance.CompleteEncounter(activeEncounterId, success);

            ReturnToPlanetSide();
            OnStateChanged?.Invoke(currentState);
            return;
        }

        CheckLevelUpPending();

        if (endState == CombatEndState.Defeat)
            AudioEventBus.Fire(AudioEventIds.GameFlow.CombatDefeat);
        else
            AudioEventBus.Fire(AudioEventIds.GameFlow.CombatVictory);
        OnStateChanged?.Invoke(currentState);
    }

    public void ReturnToHub()
    {
        EnterHub();
    }

    void HandleCombatEnded(List<Unit> playerUnits)
    {
        SquadManager.Instance.ApplyCombatResults(playerUnits);

        //Drills grant no XP and resolve no objective rewards
        if (lastEncounter.isTraining)
        {
            CombatLog.Add("[Training] Drill ended: no XP or rewards granted.");
            return;
        }

        int baseXP = lastEncounter.xpReward > 0 ? lastEncounter.xpReward : 100;
        int encounterLevel = lastEncounter.encounterLevel > 0 ? lastEncounter.encounterLevel : 1;

        Dictionary<string, int> scaledXP = EncounterXPCalculator.CalculateForSquad(
            baseXP,
            encounterLevel,
            SquadManager.Instance.GetSquad());

        SquadManager.Instance.GrantXPToSquadScaled(scaledXP);

        if (lastCombatEndState == CombatEndState.Defeat) return;

        MissionState missionState = combatManager.GetMissionState();
        if (missionState == null) return;

        // Resolve primary objective reward if completed
        if (lastEncounter.primaryObjective != null &&
            missionState.objectiveCompleted.ContainsKey(lastEncounter.primaryObjective.objectiveId))
        {
            CombatLog.Add($"[Reward] Primary complete: {lastEncounter.primaryObjective.description}");
            ResolveMissionReward(lastEncounter.primaryObjective.reward);
        }

        // Resolve completed secondary objective rewards
        if (lastEncounter.secondaryObjectives != null)
        {
            foreach (var objective in lastEncounter.secondaryObjectives)
            {
                if (missionState.objectiveCompleted.ContainsKey(objective.objectiveId))
                {
                    CombatLog.Add($"[Reward] Secondary complete: {objective.description}");
                    ResolveMissionReward(objective.reward);
                }
            }
        }
    }

    void ResolveMissionReward(MissionReward reward)
    {
        if (reward == null || reward.IsEmpty()) return;

        //Flat credit grant
        if (reward.credits > 0)
            SquadManager.Instance.AddCredits(reward.credits);

        //Specific item grants: deposited in squad inventory
        foreach (string itemId in reward.itemRewards)
        {
            if (!ItemDatabase.Definitions.ContainsKey(itemId))
            {
                Debug.LogWarning($"[Reward] Item definition not found: {itemId}. Skipping.");
                continue;
            }

            ItemInstance item = ItemInstanceFactory.CreateInstance(itemId);
            SquadManager.Instance.squadInventory.AddItem(item);
            CombatLog.Add($"[Reward] Item granted: {itemId}");
        }

        //Loot table roll: deposited in squad inventory
        if (!string.IsNullOrEmpty(reward.lootTableId))
        {
            List<ItemInstance> lootItems = LootResolver.ResolveLoot(reward.lootTableId);
            foreach (var item in lootItems)
            {
                SquadManager.Instance.squadInventory.AddItem(item);
            }

            if (lootItems.Count >0)
                CombatLog.Add($"[Reward] {lootItems.Count} item(s) from loot table deposited in inventory");
        }
    }

    void Awake()
    {
        Instance = this;
    }

    public void NewGame()
    {
        NarrativeFlagManager.ClearAll();
        NarrativeJournal.ClearAll();
        SaveManager.DeleteSave();
        DifficultySettings.Current = DifficultySettings.DifficultyLevel.Normal;
        ShipInventory.Clear();
        SquadManager.Instance.squadInventory = new Inventory(0f);
        SquadManager.Instance.creditBalance = new CreditBalance();

        CombatLog.Add("[Gameflow] New Game started");

        CharacterCreationManager.Instance.StartNewGame();

        EnterHub();
    }

    public void CheckLevelUpPending()
    {
        foreach (var character in SquadManager.Instance.GetSquad())
        {
            if (character.levelUpPending)
            {
                OnLevelUpPending?.Invoke(character);
                return; //Handle one at a time, the UI will re-trigger for each pending level.
            }
        }
    }

    public static event System.Action<CharacterData> OnLevelUpPending;
    public static event System.Action<GameState> OnStateChanged;

    public void EnterSystemMap()
    {
        currentState = GameState.SystemMap;

        HideAllPanels();
        //SystemMap UI panel hookup deferred

        if (HubUIController.Instance != null)
            HubUIController.Instance.Hide();

        CombatLog.Add("=== SYSTEM MAP ===");
        CombatLog.Add("Select a destination.");
        OnStateChanged?.Invoke(currentState);
    }

    public void SelectPlanet(PlanetData planet)
    {
        selectedPlanet = planet;
        CombatLog.Add($"[GameFlow] Planet selected: {planet.planetName}");
        EnterPlanetSide();
    }

    void ShowPlanetBriefing()
    {
        currentState = GameState.MissionBrief;

        HideAllPanels();
        //Briefing panel hookup deferred

        CombatLog.Add("=== MISSION BRIEF ===");
        CombatLog.Add($"Planet: {selectedPlanet.planetName}");
        CombatLog.Add("Confirm deployment to enter planet-side.");
        OnStateChanged?.Invoke(currentState);
    }

    public void EnterPlanetSide()
    {
        currentState = GameState.PlanetSide;
        SquadManager.Instance.CreateRuntimeSquad();

        HideAllPanels();
        // PlanetSide zone navigation panel hookup deferred

        CombatLog.Add("=== PLANET SIDE ===");
        CombatLog.Add($"Arrived at {selectedPlanet.planetName}. Select a zone.");

        if (ExplorationController.Instance != null)
            ExplorationController.Instance.EnterPlanet(selectedPlanet);
        else
            Debug.LogWarning("[GameFlow] ExplorationController not found on EnterPlanetSide");
        OnStateChanged?.Invoke(currentState);
    }

    public void ReturnToPlanetSide()
    {
        currentState= GameState.PlanetSide;

        HideAllPanels() ;
        gridRoot.SetActive(false);
        // PlanetSide zone navigation panel hookup deferred

        CombatLog.Add("=== PLANET SIDE ===");
        CombatLog.Add($"Returned to {selectedPlanet?.planetName}. Select a zone.");
        OnStateChanged?.Invoke(currentState);
    }

    // Called by ExplorationController before launching zone combat.
    // Stores the active encounter id so EndCombat() can pass it back.
    private string activeEncounterId;

    public void SetZoneEncounter(bool isZone, string encounterId)
    {
        isZoneEncounter = isZone;
        activeEncounterId = encounterId;
    }

    // Launches combat from a zone event using an authored EncounterData.
    // Mirrors StartEncounter() but does not create a new runtime squad —
    // the squad is already active from entering PlanetSide.
    public void LaunchZoneCombat(ZoneEventDefinition zoneEvent, bool surpriseRound = false)
    {
        activeZoneEvent = zoneEvent;
        selectedEncounter = EncounterDatabase.Get(zoneEvent.combatEncounterId);

        if (selectedEncounter == null)
            Debug.LogError($"[GameFlow] EncounterDatabase returned null for id: {zoneEvent.combatEncounterId}");

        CombatLog.Add($"[GameFlow] Zone combat launched: {selectedEncounter.encounterName}");

        StartCombat(surpriseRound);
    }
}
