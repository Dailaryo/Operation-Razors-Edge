using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    int playerBonusAttack =0;
    
    //Controls high-level game flow between hub, missions and combat
    public enum GameState
    {
        Hub,
        MissionStart,
        Combat,
        MissionResult
    }

    public GameState currentState;

    public GameObject missionPanel;
    public GameObject combatPanel;
    public GameObject resultPanel;
    public GameObject objectivePanel;

    List<MissionData> availableMissions = new List<MissionData>();
    MissionData selectedMission;
    private MissionData lastMission;

    private bool lastVictory;

    [SerializeField] GameObject gridRoot;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private Transform missionButtonContainer;
    [SerializeField] private GameObject missionButtonPrefab;
    [SerializeField] private GameObject briefingPanel;
    [SerializeField] private TextMeshProUGUI briefingObjectiveText;
    [SerializeField] private TextMeshProUGUI briefingDetailsText;

    void Start()
    {
        SetupMissions();

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

            EnterHub();
    }

    void Update()
    {
        if (currentState == GameState.Hub)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
               SelectMission(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectMission(1);
            }
        }
    }

    void SetupMissions()
    {
        //Elimination (default)
        var m1 = new MissionData("Scout Encounter", 2, "Eliminate all enemies", 2);
        m1.objectiveType = MissionData.MissionObjectiveType.Elimination;

        //Survival
        var m2 = new MissionData("Hold Position", 3, "Survive 3 turns", 2);
        m2.objectiveType = MissionData.MissionObjectiveType.Survival;
        m2.turnLimit = 3;

        //Target Elimination
        var m3 = new MissionData("Assassination", 4, "Eliminate priority targer", 3);
        m3.objectiveType = MissionData.MissionObjectiveType.TargetElimination;

        availableMissions.Add(m1);
        availableMissions.Add(m2);
        availableMissions.Add(m3);
    }

    void HideAllPanels()
    {
        missionPanel.SetActive(false);
        combatPanel.SetActive(false);
        resultPanel.SetActive(false);
        objectivePanel.SetActive(false);
        briefingPanel.SetActive(false);
    }

     public void SelectMission(int index)
    {
        if (index >= availableMissions.Count) return;

        selectedMission = availableMissions[index];

        CombatLog.Add("Selected Mission: " + selectedMission.missionName);

        ShowMissionBriefing();
    }

    public void EnterHub()
    {
        currentState = GameState.Hub;

        HideAllPanels();
        missionPanel.SetActive(true);

        PopulateMissionButtons();

        gridRoot.SetActive(false);

        CombatLog.Add("=== HUB ===");
        CombatLog.Add("Select Mission");

        for (int i = 0; i < availableMissions.Count; i++)
        {
            CombatLog.Add((i + 1) + ": "
                + availableMissions[i].missionName
                + " - "
                + availableMissions[i].description);
        }

        SquadManager.Instance.ResetSquadToFull();
    }

    public void StartMission()
    {
        currentState=GameState.MissionStart;

        missionPanel.SetActive(false);

        CombatLog.Add("Mission Started");

        SquadManager.Instance.CreateRuntimeSquad();

        StartCombat();
    }

    public void StartCombat()
    {
        currentState = GameState.Combat;
        
        HideAllPanels();
        combatPanel.SetActive(true);

        gridRoot.SetActive(true);

        CombatLog.Add("Combat Started");

        if (combatManager != null)
        {
            var playerUnits = SquadManager.Instance.CreateCombatUnits();
            var enemyUnits = EnemyFactory.CreateBasicEnemies(selectedMission.enemyCount);

            combatManager.StartCombat(selectedMission, playerUnits, enemyUnits, playerBonusAttack);
        }
        else
        {
            CombatLog.Add("CombatManager not found");
        }

        lastMission = selectedMission;
    }

    public void EndCombat(bool victory)
    {
        lastVictory = victory;

        currentState = GameState.MissionResult;

        HideAllPanels();

        gridRoot.SetActive(false);

        ResultUIController resultUI = FindAnyObjectByType<ResultUIController>();

        if (resultUI != null)
        {
            int reward = CalculateReward(lastVictory, lastMission);

            resultUI.ShowResult(victory, reward);
        }
    }

    public void ReturnToHub()
    {
        EnterHub();
    }

    void HandleCombatEnded(List<Unit> playerUnits)
    {
        SquadManager.Instance.ApplyCombatResults(playerUnits);

        int xpReward = 100; //TEMP for testing

        SquadManager.Instance.GrantXPToSquad(xpReward);
    }

     int CalculateReward(bool victory, MissionData mission)
    {
        int totalReward = 0;

        //Base: enemy reward (TEMP)
        int enemyReward = mission.enemyCount * 20;

        totalReward += enemyReward;

        //Mission success bonus
        if (victory)
        {
            totalReward += mission.missionReward;
            CombatLog.Add("[Reward] Mission Success Bonus Granted");
        }
        else
        {
            CombatLog.Add("[Reward] Partial reward (failure)");
        }

        CombatLog.Add($"[Reward] Total: {totalReward}");

        return totalReward;
    }

    void PopulateMissionButtons()
    {
        //Clear existing
        foreach (Transform child in missionButtonContainer)
        {
            Destroy(child.gameObject);
        }

        //Create new buttons
        for (int i = 0; i < availableMissions.Count; i++)
        {
            var mission = availableMissions[i];

            GameObject obj = Instantiate(missionButtonPrefab, missionButtonContainer);

            var ui = obj.GetComponent<MissionButtonUI>();

            ui.Initialize(mission, i, this);
        }
    }

    void ShowMissionBriefing()
    {
        currentState = GameState.MissionStart;

        HideAllPanels();
        briefingPanel.SetActive(true);

        //Objective text
        switch (selectedMission.objectiveType)
        {
            case MissionData.MissionObjectiveType.Elimination:
                briefingObjectiveText.text = "Eliminate all enemies";
                break;

            case MissionData.MissionObjectiveType.Survival:
                briefingObjectiveText.text = $"Survive {selectedMission.turnLimit} turns";
                break;

            case MissionData.MissionObjectiveType.TargetElimination:
                briefingObjectiveText.text = "Eliminate the priority target";
                break;
        }

        //Details
        briefingDetailsText.text =
        $"Enemies: {selectedMission.enemyCount}\n" +
        $"Reward: {selectedMission.missionReward}";
    }
}
