using System.Collections.Generic;
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

    List<MissionData> availableMissions = new List<MissionData>();
    MissionData selectedMission;

    void Start()
    {
        SetupMissions();

        HideAllPanels();

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
        availableMissions.Add(new MissionData("Scout Encounter", 2, "Light enemy presence", 2));
        availableMissions.Add(new MissionData("Heavy Patrol", 3, "Stronger enemy force", 4));
    }

    void HideAllPanels()
    {
        missionPanel.SetActive(false);
        combatPanel.SetActive(false);
        resultPanel.SetActive(false);
    }

     public void SelectMission(int index)
    {
        if (index >= availableMissions.Count) return;

        selectedMission = availableMissions[index];

        CombatLog.Add("Selected Mission: " + selectedMission.missionName);

        StartMission();
    }

    public void EnterHub()
    {
        currentState = GameState.Hub;

        HideAllPanels();
        missionPanel.SetActive(true);

        CombatLog.Add("=== HUB ===");
        CombatLog.Add("Select Mission");

        for (int i = 0; i < availableMissions.Count; i++)
        {
            CombatLog.Add((i+1) + ": "
                + availableMissions[i].missionName
                + " - "
                + availableMissions[i].description);
        }
    }

    public void StartMission()
    {
        currentState=GameState.MissionStart;

        missionPanel.SetActive(false);

        CombatLog.Add("Mission Started");

        StartCombat();
    }

    public void StartCombat()
    {
        currentState = GameState.Combat;
        
        HideAllPanels();
        combatPanel.SetActive(true);

        CombatLog.Add("Combat Started");

        CombatManager combat = Object.FindAnyObjectByType<CombatManager>();

        if (combat != null)
        {
            combat.StartCombat(selectedMission, playerBonusAttack);
        }
        else
        {
            CombatLog.Add("CombatManager not found");
        }
    }

    public void EndCombat(bool victory)
    {
        currentState = GameState.MissionResult;

        HideAllPanels();

        ResultUIController resultUI = FindAnyObjectByType<ResultUIController>();

        if (resultUI != null)
        {
            int reward = selectedMission != null ? selectedMission.rewardAttack : 0;
            resultUI.ShowResult(victory, reward);
        }
    }

    public void ReturnToHub()
    {
        EnterHub();
    }
}
