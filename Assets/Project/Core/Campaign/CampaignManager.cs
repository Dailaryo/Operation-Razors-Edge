using System.Collections.Generic;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{
    public static CampaignManager Instance;

    private CampaignData campaign;

    private List<EndingData> endings;

    [SerializeField] private SquadManager squadManager;

    void Awake()
    {
        Instance = this;
        campaign = BuildCampaign();
        endings = BuildEndings();
    }

    //Returns encounters available on the current planet that are not locked
    public List<EncounterData> GetAvailableEncounters()
    {
        ActData currentAct = campaign.GetCurrentAct();
        if (currentAct == null) return new List<EncounterData>();

        List<EncounterData> available = new List<EncounterData>();

        foreach (string planetId in currentAct.planetIds)
        {
            PlanetData planet = campaign.GetPlanet(planetId);
            if (planet == null) continue;
            if (planet.accessibility == PlanetData.PlanetAccessibility.Locked) continue;
            if (planet.accessibility == PlanetData.PlanetAccessibility.Inaccessible) continue;

            foreach (string encounterId in planet.encounterIds)
            {
                EncounterData encounter = campaign.GetEncounter(encounterId);
                if (encounter != null) available.Add(encounter);
            }
        }

        return available;
    }

    //Called after a encounter ends - writes outcome flags and checks act advance
    public void RecordEncounterOutcome(EncounterData encounter, CombatEndState endState)
    {
        bool stampsComplete = false;

        if (endState == CombatEndState.Defeat)
        {
            NarrativeFlagManager.SetFlag(encounter.encounterId + "_failed", 1);
            CombatLog.Add($"[Campaign] Flag set: {encounter.encounterId}_failed");
        }
        else
        {
            switch (encounter.retryPolicy)
            {
                case EncounterData.RetryPolicy.FullVictoryRequired:
                    if (endState == CombatEndState.FullVictory)
                        stampsComplete = true;
                    else
                    {
                        NarrativeFlagManager.SetFlag(encounter.encounterId + "_incomplete", 1);
                        CombatLog.Add($"[Campaign] Flag set: {encounter.encounterId}_incomplete");
                    }
                    break;

                case EncounterData.RetryPolicy.AnyEndCounts:
                    stampsComplete = true;
                    break;

                case EncounterData.RetryPolicy.AlwaysRetryable:
                    if (endState == CombatEndState.FullVictory)
                        stampsComplete = true;
                    break;
            }
        }

        if (stampsComplete)
        {
            NarrativeJournal.AddEntry(
                encounter.encounterId + "_complete",
                $"{encounter.encounterName} — completed."
            );
            CombatLog.Add($"[Campaign] Flag set: {encounter.encounterId}_complete");
        }

        TryAdvanceAct();
        SaveManager.Save(campaign, squadManager.GetSquad());
    }

    void TryAdvanceAct()
    {
        ActData currentAct = campaign.GetCurrentAct();
        if (currentAct == null) return;

        //Check if all required encounters in this act are complete
        foreach (string planetId in currentAct.planetIds)
        {
            PlanetData planet = campaign.GetPlanet(planetId);
            if (planet == null) continue;

            foreach (string encounterId in planet.encounterIds)
            {
                EncounterData encounter = campaign.GetEncounter(encounterId);
                if (encounter == null) continue;
                if (encounter.isRequired && !NarrativeFlagManager.HasFlag(encounter.encounterId + "_complete")) return;
            }
        }

        //All required encounters done: advance act
        int nextIndex = campaign.currentActIndex + 1;

        if (nextIndex < campaign.acts.Count && campaign.acts[nextIndex].IsUnlocked())
        {
            campaign.currentActIndex = nextIndex;
            CombatLog.Add($"[Campaign] Advanced to Act {nextIndex}: {campaign.acts[nextIndex].actName}");

            //Lock missable content from the act we just left
            int previousIndex = campaign.currentActIndex - 1;
            if (previousIndex >= 0)
            {
                ActData previousAct = campaign.acts[previousIndex];
                foreach (string planetId in previousAct.planetIds)
                {
                    PlanetData planet = campaign.GetPlanet(planetId);
                    if (planet == null) continue;

                    planet.accessibility = PlanetData.PlanetAccessibility.Left;

                    foreach (string encounterId in planet.encounterIds)
                    {
                        EncounterData encounter = campaign.GetEncounter(encounterId);
                        if (encounter == null) continue;
                        
                        if (!encounter.isRequired && !NarrativeFlagManager.HasFlag(encounter.encounterId + "_complete"))
                        {
                            NarrativeFlagManager.SetFlag(encounter.encounterId + "_locked", 1);
                            CombatLog.Add($"[Campaign] Optional encounter locked: {encounter.encounterName}");
                        }
                    }
                }
            }
        }
    }

    CampaignData BuildCampaign()
    {
        CampaignData data = new CampaignData();

        foreach (var actKvp in ActDatabase.Definitions)
        {
            ActData act = actKvp.Value;

            foreach (string planetId in act.planetIds)
            {
                PlanetData planet = PlanetDatabase.Get(planetId);
                if (planet == null) continue;

                //Resolve encounterIds to EncounterData and add to runtime planet
                foreach (string encounterId in planet.encounterIds)
                {
                    EncounterData encounter = EncounterDatabase.Get(encounterId);
                    if (encounter != null)
                        data.encounters[encounterId] = encounter;
                }

                data.planets.Add(planet);
            }

            data.acts.Add(act);
        }

        return data;
    }

    public int GetPlanetTierForEncounter(EncounterData encounter)
    {
        foreach (var planet in campaign.planets)
        {
            if (planet.encounterIds.Contains(encounter.encounterId))
                return planet.difficultyTier;
        }

        Debug.LogWarning($"[CampaignManager] Could not find planet tier for encounter: {encounter.encounterId}");
        return 1; // Default to tier 1 if not found
    }

    void Start()
    {
        //Load saved state over the freshly built campaign structure
        if (SaveManager.SaveExists())
        {
            SaveManager.Load(campaign, squadManager.GetSquad());
        }
    }

    public EndingData GetEnding()
    {
        return EndingResolver.Resolve(endings);
    }

    List<EndingData> BuildEndings()
    {
        return new List<EndingData>(EndingDatabase.Definitions);
    }

    public FactionData GetFaction(string factionId)
    {
        var faction = FactionDatabase.Get(factionId);

        if (faction == null)
            Debug.LogWarning($"[CampaignManager] Faction not found: {factionId}");

        return faction;
    }

    public CivilizationData GetCivilizationForEncounter(string encounterId)
    {
        foreach (var planet in campaign.planets)
        {
            if (planet.encounterIds.Contains(encounterId))
                return campaign.GetCivilizationForPlanet(planet.planetId);
        }

        Debug.LogWarning($"[CampaignManager] Could not find civilization for encounter: {encounterId}");
        return null;
    }

    public CampaignData GetCampaignData()
    {
        return campaign;
    }

    public CivilizationData GetCivilizationForPlanet(string planetId)
    {
        return campaign.GetCivilizationForPlanet(planetId);
    }
}
