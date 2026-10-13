using System.Collections.Generic;
using UnityEngine;

public class EncounterData
{
    public string encounterName;
    public string encounterId;
    public string mapId = MapIds.Default;
    public string description;

    public int xpReward;
    public int encounterLevel;

    //Credits enemies draw on for material components this encounter. Authored per encounter for balance
    public int materialPoolCredits = 0;

    public int enemyCount;
    public string enemyTypeName;

    public bool isRequired;
    public bool isFinalEncounter;

    //Faction association: narrative only, no mechanical effect (yet, maybe)
    public string factionId;
    public string factionContext;

    //Objectives
    public ObjectiveDefinition primaryObjective;
    public List<ObjectiveDefinition> secondaryObjectives;
    public ObjectiveDefinition failureObjective; //null = no fallback on primary failure

    public List<EscalationWaveDefinition> escalationWaves;

    public enum RetryPolicy
    {
        AlwaysRetryable, // Optional side encounters — always available until act advances
        FullVictoryRequired, // Core narrative encounters — only FullVictory counts as done; partial/retreat leaves it retryable
        AnyEndCounts // One-shot encounters — any non-Defeat outcome marks it done, no retry
    }

    public RetryPolicy retryPolicy;

    public EncounterData(string id, string name, int enemies, string desc)
    {
        encounterId = id;
        encounterName = name;
        enemyCount = enemies;
        enemyTypeName = "Enemies";
        description = desc;

        isRequired = false;
        isFinalEncounter = false;

        factionId = "";
        factionContext = "";

        secondaryObjectives = new List<ObjectiveDefinition>();
        escalationWaves = new List<EscalationWaveDefinition>();

        retryPolicy = RetryPolicy.AlwaysRetryable;
    }

    public ExtractionMissionConfig extractionConfig;

    public class ExtractionMissionConfig
    {
        public List<string> extractionZoneIds;
        public int extractionDeadline; //Must be > 0

        public ExtractionMissionConfig(List<string> zoneIds, int deadline)
        {
            extractionZoneIds = zoneIds ?? new List<string>();
            extractionDeadline = Mathf.Max(1, deadline);
        }
    }
}
