using System.Collections.Generic;
using UnityEngine;

public class ZoneEventDefinition
{
    public string zoneEventId;
    public string displayName;
    public ZoneEventType zoneEventType;

    // Set on completion — written to NarrativeFlagManager
    public string narrativeFlagOnComplete;  // null = no flag
    public string narrativeFlagOnFail;      // null = no flag

    // Unlock condition — NarrativeFlag id; null = always available
    public string unlockCondition;

    // Whether this zone event disappears after first completion
    public bool repeatable;

    // When true, SquadStealthMode is checked on combat entry and a stealth
    // check is rolled. False means the zone event is never avoidable by stealth.
    public bool requiresStealth = false;

    // DC for the squad stealth check. 0 = computed at runtime from zone danger level.
    public int stealthDC = 0;

    // --- Type-specific payload fields ---

    // ZoneEventType.Combat: references a EncounterData id in EncounterDatabase
    public string combatEncounterId;

    // ZoneEventType.SkillCheck: references a SkillCheckDefinition id in SkillCheckDatabase
    public string skillCheckId;

    // ZoneEventType.Merchant: references a MerchantDefinition id in MerchantDatabase
    public string merchantId;

    // EZoneEventType.Narrative
    public string dialogueTreeId;

    public enum ZoneEventType
    {
        Combat,
        SkillCheck,
        Merchant,
        Narrative
    }

    public ZoneEventDefinition(string id, string name, ZoneEventType type)
    {
        zoneEventId = id;
        displayName = name;
        zoneEventType = type;
        repeatable = false;
    }

    //ZoneEventType.Combat: enemy spawn list drives CombatManager enemy construction
    public List<EnemySpawnEntry> enemySpawnEntries = new List<EnemySpawnEntry>();

    public class EnemySpawnEntry
    {
        public string enemyId;
        public int spawnLevel;
        public int count;

        public EnemySpawnEntry(string enemy, int level, int count)
        {
            enemyId = enemy;
            spawnLevel = level;
            this.count = count;
        }
    }
}