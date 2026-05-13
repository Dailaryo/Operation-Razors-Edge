using UnityEngine;
using System.Collections.Generic;

public class ExplorationController : MonoBehaviour
{
    public static ExplorationController Instance;

    public PlanetData currentPlanet;

    public bool SquadStealthMode { get; private set; } = false;

    void Awake()
    {
        Instance = this;
    }

    //Planet Entry

    //Called by GameFlowManager.EnterPlanetSide()
    public void EnterPlanet(PlanetData planet)
    {
        currentPlanet = planet;
        currentPlanet.currentZone = null;

        RefreshUnlockedZones();

        CombatLog.Add($"[Exploration] Entered planet: {planet.planetName}");
        LogAvailableZones();
    }

    // Evaluates each zone's unlockCondition against NarrativeFlagManager.
    // Zones with no unlock condition are always added.
    // Only adds zones that aren't already in the unlocked list.
    void RefreshUnlockedZones()
    {
        foreach (string zoneId in currentPlanet.zoneIds)
        {
            if (currentPlanet.unlockedZones.Contains(zoneId)) continue;

            ZoneDefinition zone = ZoneDatabase.Get(zoneId);
            if (zone == null) continue;

            if (string.IsNullOrEmpty(zone.unlockCondition) ||
                NarrativeFlagManager.HasFlag(zone.unlockCondition))
            {
                currentPlanet.unlockedZones.Add(zoneId);
                CombatLog.Add($"[Exploration] Zone unlocked: {zone.displayName}");
            }
        }
    }

    //Zone Navigation

    //Returns zones currently accessible to the player on the active planet
    public List<ZoneDefinition> GetAvailableZones()
    {
        if (currentPlanet == null) return new List<ZoneDefinition>();

        List<ZoneDefinition> result = new List<ZoneDefinition>();

        foreach (string zoneId in currentPlanet.unlockedZones)
        {
            ZoneDefinition zone = ZoneDatabase.Get(zoneId);
            if (zone != null) result.Add(zone);
        }

        return result;
    }

    //Called when the player selects a zone from the navigation menu.
    public void EnterZone(string zoneId)
    {
        ZoneDefinition zone = ZoneDatabase.Get(zoneId);
        if (zone == null)
        {
            Debug.LogWarning($"[Exploration] Zone not found: {zoneId}");
            return;
        }

        currentPlanet.currentZone = zoneId;
        CombatLog.Add($"[Exploration] Entered zone: {zone.displayName}");
        AudioEventBus.Fire(AudioEventIds.Exploration.ZoneEntered);
    }

    //Returns zone events available in the given zone, filtered by
    //unlock conditions and completion state.
    public List<ZoneEventDefinition> GetAvailableZoneEvents(string zoneId)
    {
        ZoneDefinition zone = ZoneDatabase.Get(zoneId);
        if (zone == null) return new List<ZoneEventDefinition>();

        List<ZoneEventDefinition> result = new List<ZoneEventDefinition>();

        foreach (string eventId in zone.encounterIds)
        {
            ZoneEventDefinition zoneEvent = ZoneEventDatabase.Get(eventId);
            if (zoneEvent == null) continue;

            //Skip completed non-repeatable zone events
            if (!zoneEvent.repeatable &&
                currentPlanet.completedEncounters.Contains(eventId)) continue;

            //Skip zoneEvents whose unlock condition hasn't been met
            if (!string.IsNullOrEmpty(zoneEvent.unlockCondition) &&
                !NarrativeFlagManager.HasFlag(zoneEvent.unlockCondition)) continue;

            result.Add(zoneEvent);
        }

        //If authored content is exhausted and a random table exists for this zone,
        //apped one randomly generated zoneEvent as a supplement
        if (result.Count == 0 &&
            HasExhaustedAuthoredContent(zoneId) &&
            ZoneRandomEncounterManager.Instance != null &&
            ZoneRandomEncounterManager.Instance.HasTable(zoneId))
        {
            List<CharacterData> squad = SquadManager.Instance.GetSquad();
            ZoneEventDefinition randomEncounter =
                ZoneRandomEncounterManager.Instance.TryGetRandomEncounter(zoneId, squad);

            if (randomEncounter != null)
                result.Add(randomEncounter);
        }

        return result;
    }

    //Encounter Dispatch

    //Called when the player selects an zoneEvent.  Routes to the
    //correct subsystem based on EncounterType
    public void TriggerEncounter(string zoneEventId)
    {
        ZoneEventDefinition encounter = ZoneEventDatabase.Get(zoneEventId);
        if (encounter == null)
        {
            Debug.LogWarning($"[Exploration] Encounter not found: {zoneEventId}");
            return;
        }

        CombatLog.Add($"[Exploration] Triggering encounter: {encounter.displayName}");
        AudioEventBus.Fire(AudioEventIds.Exploration.EncounterTriggered);
        switch (encounter.zoneEventType)
        {
            case ZoneEventDefinition.ZoneEventType.Combat:
                DispatchCombatEncounter(encounter);
                break;

            case ZoneEventDefinition.ZoneEventType.SkillCheck:
                DispatchSkillCheckEncounter(encounter);
                break;

            case ZoneEventDefinition.ZoneEventType.Merchant:
                DispatchMerchantEncounter(encounter);
                break;

            case ZoneEventDefinition.ZoneEventType.Narrative:
                DispatchNarrativeEncounter(encounter);
                break;

            default:
                Debug.LogWarning($"[Exploration] Unhandled ZoneEventType: {encounter.zoneEventType}");
                break;
        }
    }

    void DispatchCombatEncounter(ZoneEventDefinition zoneEvent)
    {
        if (string.IsNullOrEmpty(zoneEvent.combatEncounterId))
        {
            Debug.LogWarning($"[Exploration] Combat encounter has no combatEncounterId: {zoneEvent.zoneEventId}");
            return;
        }

        bool surpriseRound = false;

        if (SquadStealthMode && zoneEvent.requiresStealth)
        {
            surpriseRound = ResolveExplorationStealth(zoneEvent);
            SquadStealthMode = false; //Stealth mode ends when combat begins.
        }

        //Set routing flag before handing off to GameFlowManager
        GameFlowManager.Instance.SetZoneEncounter(true, zoneEvent.zoneEventId);
        GameFlowManager.Instance.LaunchZoneCombat(zoneEvent, surpriseRound);
    }

    void DispatchSkillCheckEncounter(ZoneEventDefinition encounter)
    {
        if (string.IsNullOrEmpty(encounter.skillCheckId))
        {
            CombatLog.Add($"[Exploration] Skill check encounter '{encounter.zoneEventId}' has no skillCheckId authored yet.");
            return;
        }

        //Build context for Exploration caller
        var context = new SkillCheckContext
        {
            checkId = encounter.skillCheckId,
            attacker = GetLeadUnit(),
            callerContext = SkillCheckContext.CallerContext.Exploration,
            showPreview = true,
            onResult = (result) => OnSkillCheckResult(encounter, result)
        };

        SkillCheckExecutor.Execute(context);
    }

    void DispatchMerchantEncounter(ZoneEventDefinition encounter)
    {
        if (string.IsNullOrEmpty(encounter.merchantId))
        {
            CombatLog.Add($"[Exploration] Merchant encounter '{encounter.zoneEventId}' has no merchantId authored.");
            return;
        }

        // EconomyManager.GetAvailableStock() accepts merchantId directly.
        // The stub in EconomyManager.GetCurrentCivilizationData() is replaced
        // here once UI wiring passes the planet context — deferred to UI milestone.
        CombatLog.Add($"[Exploration] Opening merchant: {encounter.merchantId}");

        // Merchant UI hookup deferred — log confirms routing is correct
        CompleteEncounter(encounter.zoneEventId, true);
    }

    void DispatchNarrativeEncounter(ZoneEventDefinition encounter)
    {
        if (string.IsNullOrEmpty(encounter.dialogueTreeId))
        {
            CombatLog.Add($"[Exploration] Narrative encounter '{encounter.zoneEventId}' has no dialogueTreeId authored yet.");
            CompleteEncounter(encounter.zoneEventId, true);  // stub auto-completes
            return;
        }

        //Store zoneEventId for use in the completion callback
        string encounterId = encounter.zoneEventId;

        // Subscribe to OnConversationEnded before starting — unsubscribe immediately
        // after firing so repeat conversations don't double-complete
        System.Action<string> onEnded = null;
        onEnded = (treeId) =>
        {
            DialogueController.Instance.OnConversationEnded -= onEnded;
            CompleteEncounter(encounterId, true);
        };

        DialogueController.Instance.OnConversationEnded += onEnded;
        DialogueController.Instance.StartConversation(encounter.dialogueTreeId);
    }

    //Encounter Completion

    // Called by GameFlowManager.EndCombat() for zone combat,
    // and internally for non-combat encounters.
    public void CompleteEncounter(string encounterId, bool success)
    {
        ZoneEventDefinition encounter = ZoneEventDatabase.Get(encounterId);
        if (encounter == null) return;

        // Random encounters sourced from ZoneRandomEncounterManager are transient —
        // completing them does not permanently mark them in completedEncounters,
        // but narrative flags still stamp normally so consequences persist.
        bool isRandomEncounter =
            ZoneRandomEncounterManager.Instance != null &&
            ZoneRandomEncounterManager.Instance.WasRandomlyGenerated(encounterId);

        //Mark completed if non-repeatable
        if (!isRandomEncounter && !encounter.repeatable &&
            !currentPlanet.completedEncounters.Contains(encounterId))
        {
            currentPlanet.completedEncounters.Add(encounterId);
        }

        //Stamp narrative flags
        if (success && !string.IsNullOrEmpty(encounter.narrativeFlagOnComplete))
            NarrativeFlagManager.SetFlag(encounter.narrativeFlagOnComplete, 1);

        if (!success && !string.IsNullOrEmpty(encounter.narrativeFlagOnFail))
            NarrativeFlagManager.SetFlag(encounter.narrativeFlagOnFail, 1);

        // Re-evaluate zone unlocks — completion may have unlocked new zones
        RefreshUnlockedZones();

        CombatLog.Add($"[Exploration] Encounter complete: {encounter.displayName} — " +
                      $"{(success ? "Success" : "Failed")}");

        SaveCurrentPlanetState();
    }

    //Planet Departure

    // Called when the player chooses to leave the planet from the zone menu.
    // Side encounters not yet completed are locked per the realignment rules.
    public void DepartPlanet()
    {
        if (currentPlanet == null) return;

        CombatLog.Add($"[Exploration] Departing {currentPlanet.planetName}.");

        currentPlanet.currentZone = null;

        SquadStealthMode = false;

        GameFlowManager.Instance.ReturnToHub();
    }

    //Save Integration
    void SaveCurrentPlanetState()
    {
        // Triggers a full save through CampaignManager so planet state
        // (completedEncounters, unlockedZones) is persisted immediately
        // after each encounter rather than only on hub return.
        if (CampaignManager.Instance != null)
            SaveManager.Save(
                CampaignManager.Instance.GetCampaignData(),
                SquadManager.Instance.GetSquad());
    }

    //Helpers

    // Returns the first living squad member as the skill check performer.
    // Full party-selection UI deferred to UI milestone.
    public Unit GetLeadUnit()
    {
        var squad = SquadManager.Instance.CreateCombatUnits();
        return squad.Count > 0 ? squad[0] : null;
    }

    void LogAvailableZones()
    {
        var zones = GetAvailableZones();
        if (zones.Count == 0)
        {
            CombatLog.Add("[Exploration] No zones currently available.");
            return;
        }

        CombatLog.Add($"[Exploration] Available zones ({zones.Count}):");
        foreach (var zone in zones)
            CombatLog.Add($"  - {zone.displayName} [{zone.zoneType}]");
    }

    void OnSkillCheckResult(ZoneEventDefinition encounter, SkillCheckResult result)
    {
        CombatLog.Add($"[Exploration] Skill check result for '{encounter.displayName}': " +
                  $"{(result.success ? $"Success (Tier {result.tier})" : "Failed")} " +
                  $"— {result.total} vs {result.dc}");

        // A tier > 0 result counts as success; tier 0 is failure
        bool success = result.tier > 0;

        CompleteEncounter(encounter.zoneEventId, success);
    }

    //Returns true when all non-repeatable authored encounters in the zone
    //have been completed.  Repeatable encounters neveer count as exhausted
    bool HasExhaustedAuthoredContent(string zoneId)
    {
        ZoneDefinition zone = ZoneDatabase.Get(zoneId);
        if (zone == null) return true;

        foreach (string encounterId in zone.encounterIds)
        {
            ZoneEventDefinition encounter = ZoneEventDatabase.Get(encounterId);
            if (encounter == null) continue;

            //A repeatable encounter means authored content is neve exhausted
            if (encounter.repeatable) return false;

            //A non-repeatable encounter that hasn't been completed yet
            if (!currentPlanet.completedEncounters.Contains(encounterId)) return false;
        }

        return true;
    }

    //Called by the zone navigation UI to toggle squad stealth.
    //Only meaningful in Neutral and Hostile zones, Social zones ignore it.
    public void SetStealthMode(bool enabled)
    {
        if (currentPlanet == null) return;

        string zoneId = currentPlanet.currentZone;
        if (!string.IsNullOrEmpty(zoneId))
        {
            ZoneDefinition zone = ZoneDatabase.Get(zoneId);
            if (zone != null && zone.zoneType == ZoneDefinition.ZoneType.Social)
            {
                CombatLog.Add("[Exploration] Stealth mode has no effect in social zones.");
                return;
            }
        }

        SquadStealthMode = enabled;
        CombatLog.Add($"[Exploration] Squad stealth mode: {(enabled ? "ON" : "OFF")}");
    }

    //rolls a squad stealth check against the encoutner's stealthDC.
    //Uses the weakestlink rule: the lowest Stealth total in the squad
    //Returns true if the entire squad passes and a surpirse round should fire
    bool ResolveExplorationStealth(ZoneEventDefinition encounter)
    {
        List<Unit> combatUnits = SquadManager.Instance.CreateCombatUnits();

        if (combatUnits.Count == 0) return false;

        //Compute the stealthDC: authored value is set, otherwise 10 + danger level.
        ZoneDefinition zone = currentPlanet?.currentZone != null
            ? ZoneDatabase.Get(currentPlanet.currentZone)
            : null;

        int dc = encounter.stealthDC > 0
            ? encounter.stealthDC
            : 10 + (zone != null ? zone.dangerLevel : 0);

        // Everyone rolls — the lowest total result determines whether the squad passes.
        // A single bad roll by any member exposes the whole squad.
        int lowestTotal = int.MaxValue;
        Unit weakestLink = combatUnits[0];

        foreach (Unit unit in combatUnits)
        {
            SkillCheckResult result = SkillResolver.Resolve(unit, SkillType.Stealth, dc);

            CombatLog.Add($"[Exploration] {unit.unitName} stealth roll: {result.total} vs DC {dc}");

            if (result.total < lowestTotal)
            {
                lowestTotal = result.total;
                weakestLink = unit;
            }
        }

        bool success = lowestTotal >= dc;

        CombatLog.Add($"[Exploration] Squad stealth check: weakest result {lowestTotal} by {weakestLink.unitName} " +
              $"vs DC {dc} — {(success ? "SUCCESS" : "FAIL")}");

        if (success)
        {
            // Pre-initialise all squad members as Hidden vs all enemies.
            // DetectionStateManager.Initialize() will be called in StartCombat()
            // with this hidden list immediately after.
            CombatLog.Add("[Exploration] Squad approaches undetected — surprise round granted!");
            AudioEventBus.Fire(AudioEventIds.Exploration.StealthApproachSuccess);
            return true;
        }

        CombatLog.Add("[Exploration] Squad detected on approach — no surprise round.");
        AudioEventBus.Fire(AudioEventIds.Exploration.StealthApproachFail);
        return false;
    }
}
