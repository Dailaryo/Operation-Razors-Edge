using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

public class ZoneRandomEncounterManager : MonoBehaviour
{
    public static ZoneRandomEncounterManager Instance;

    private Dictionary<string, RandomEncounterTable> tables
        = new Dictionary<string, RandomEncounterTable>();

    private HashSet<string> randomlyGeneratedIds = new HashSet<string>();

    void Awake()
    {
        Instance = this;
    }

    //Called during content authoring setup to register a zone's encounter table.
    //Safe to call multiple times, later registrations overwrite earlier ones.
    public void RegisterTable(string zoneId, RandomEncounterTable table)
    {
        if (string.IsNullOrEmpty(zoneId) || table == null)
        {
            Debug.LogWarning("[RandomEncounter] Attempted to register null table or empty zoneId.");
            return;
        }

        tables[zoneId] = table;
    }

    //Attempts to generate a random encounter for the given zone and squad state.
    //Returns and EncounterDefinition if a valid entry was rolled, null otherwise.
    //Null is a valid non-error result, it means the zone has no random table,
    //or nothing in the table fit the budget
    public ZoneEventDefinition TryGetRandomEncounter(string zoneId, List<CharacterData> squad)
    {
        RandomEncounterTable table;
        if (!tables.TryGetValue(zoneId, out table))
        {
            CombatLog.Add($"[RandomEncounter] No table registered for zone '{zoneId}'.");
            return null;
        }

        ZoneDefinition zone = ZoneDatabase.Get(zoneId);
        if (zone == null)
        {
            Debug.LogWarning($"[RandomEncounter] Zone '{zoneId}' not found in ZoneDatabase.");
            return null;
        }

        int budget = EncounterBudgetCalculator.Calculate(zone, squad);
        CombatLog.Add($"[RandomEncounter] Zone '{zone.displayName}' — encounter budget: {budget}.");

        RandomEncounterTable.RandomEncounterEntry entry = table.Roll(budget, zone.zoneType);
        if (entry == null)
        {
            CombatLog.Add($"[RandomEncounter] No valid encounter found within budget for '{zone.displayName}'.");
            return null;
        }

        ZoneEventDefinition encounter = ZoneEventDatabase.Get(entry.encounterId);
        if (encounter == null)
        {
            Debug.LogWarning($"[RandomEncounter] Rolled encounter '{entry.encounterId}' not found in EncounterDatabase.");
            return null;
        }

        CombatLog.Add($"[RandomEncounter] Rolled encounter: {encounter.displayName}.");
        randomlyGeneratedIds.Add(encounter.zoneEventId);
        return encounter;
    }

    //Return true if a random table exists for the given zone.
    public bool HasTable(string zoneId)
    {
        return tables.ContainsKey(zoneId);
    }

    public bool WasRandomlyGenerated(string encounterId)
    {
        return randomlyGeneratedIds.Contains(encounterId);
    }
}
