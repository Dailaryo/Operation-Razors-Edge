using System.Collections.Generic;
using UnityEngine;

public static class EnemyFactory
{
    //Primary entry point.  Resolves definition, applies scaling, routes to
    //the correct construction pipeline, and returns a fully initialized Unit
    public static Unit CreateEnemy(string enemyId, int spawnLevel, int planetTier)
    {
        var def = EnemyDatabase.Get(enemyId);
        if (def == null) return null;

        ScaledStatBlock scaled = EnemyScaler.ScaleStatBlock(def, spawnLevel, planetTier);

        Unit unit = def.isClassed
            ? BuildClassedUnit(def, scaled, spawnLevel)
            : BuildClasslessUnit(def, scaled, spawnLevel);

        if (unit == null) return null;

        //Post-build: OA state, threat zone registration, XP
        unit.oaState = new OAState(1);
        unit.oaState.reactWhileHidden = true;
        ThreatZoneManager.RegisterUnit(unit);
        unit.xpValue = scaled.xpValue;

        Debug.Log($"[EnemyFactory] Built enemy: {unit.unitName} " +
                  $"| Level: {spawnLevel} | PlanetTier: {planetTier} " +
                  $"| HP: {unit.maxHP} | Attack: {unit.attackRating}");

        return unit;
    }

    //Convience wrapper for spawning multiple enemies of the same type
    public static List<Unit> CreateGroup(string enemyId, int count, int spawnLevel, int planetTier)
    {
        List<Unit> enemies = new List<Unit>();

        for (int i = 0; i < count; i++)
        {
            Unit unit = CreateEnemy(enemyId, spawnLevel, planetTier);
            if (unit != null)
                enemies.Add(unit);
        }

        return enemies;
    }

    //Private construction pipelines
    static Unit BuildClasslessUnit(EnemyDefinition def, ScaledStatBlock scaled, int spawnLevel)
    {
        var data = new CharacterData(def.enemyId, def.displayName,
            scaled.maxHP, scaled.attackRating, scaled.initiative);
        data.isEnemy = true;

        data.power = scaled.power;
        data.agility = scaled.agility;
        data.endurance = scaled.endurance;
        data.cognition = scaled.cognition;
        data.insight = scaled.insight;
        data.presence = scaled.presence;

        data.level = spawnLevel;
        data.moveSpeed = def.moveSpeed;
        data.size = def.size;
        data.lootTableId = def.lootTableId;

        //Enemy-specific actions on top of universal (already in baseActionIds from constructor)
        data.baseActionIds.AddRange(def.additionalActionIds);

        //Level-gated action unlocks
        foreach (var kvp in def.levelActionUnlocks)
        {
            if (kvp.Key <= spawnLevel)
                data.baseActionIds.AddRange(kvp.Value);
        }

        //Equipped items
        foreach (var kvp in def.equippedItemIds)
        {
            var instance = ItemInstanceFactory.CreateInstance(kvp.Value);
            data.TryEquipItem(kvp.Key, instance, out _);
        }

        var unit = new Unit(def.displayName, true, scaled.maxHP, scaled.attackRating, scaled.initiative);
        unit.sourceCharacterData = data;
        unit.actionSources.Clear();
        unit.actionSources.Add(new BaseActionSource(data.baseActionIds));

        //Feats
        if (def.featIds.Count > 0)
            unit.ApplyFeats();

        //Derive reach profile from size and equipped weapon
        data.reachProfile = ReachProfile.DeriveFrom(data);
        unit.sizeCategory = data.size;

        return unit;
    }

    static Unit BuildClassedUnit(EnemyDefinition def, ScaledStatBlock scaled, int spawnLevel)
    {
        var data = new CharacterData(def.enemyId, def.displayName,
            0, 0, def.baseInitiative);
        data.isEnemy = true;

        //Species substitute uses flat stat block instead of SpeciesDefinition lookup
        if (def.isStatBlockSpeciesSubstitute)
        {
            data.power = def.power;
            data.agility = def.agility;
            data.endurance = def.endurance;
            data.cognition = def.cognition;
            data.insight = def.insight;
            data.presence = def.presence;
            data.baseMaxHP = def.baseMaxHP;
            data.baseAttack = def.baseAttack;
        }
        else
        {
            data.speciesId = def.speciesId;
        }

        data.classId = def.classId;
        data.level = 1;
        data.moveSpeed = def.moveSpeed;
        data.size = def.size;
        data.lootTableId = def.lootTableId;

        //Iterate level up from 2 to spawnLevel, identical to player levelling
        for (int i = 2; i <= spawnLevel; i++)
            data.LevelUp();

        //Equipped itesm
        foreach (var kvp in def.equippedItemIds)
        {
            var instance = ItemInstanceFactory.CreateInstance(kvp.Value);
            data.TryEquipItem(kvp.Key, instance, out _);
        }

        var unit = new Unit(def.displayName, true, data.baseMaxHP, data.baseAttack, data.baseInitiative);
        unit.sourceCharacterData = data;
        unit.actionSources.Clear();
        unit.actionSources.Add(new BaseActionSource(data.baseActionIds));
        unit.attackRating = data.baseAttack;

        if (!def.isStatBlockSpeciesSubstitute)
            unit.ApplySpecies();

        unit.ApplyClass();
        unit.ApplyFeats();
        unit.ApplyEquipment();

        //Apply difficulty multiplier post construction
        unit.maxHP = Mathf.Max(1, Mathf.RoundToInt(unit.maxHP * scaled.difficultyModifier));
        unit.currentHP = unit.maxHP;
        unit.attackRating = Mathf.Max(1, Mathf.RoundToInt(unit.attackRating * scaled.difficultyModifier));

        data.reachProfile = ReachProfile.DeriveFrom(data);
        unit.sizeCategory = data.size;

        return unit;
    }
}
