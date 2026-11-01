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
        unit.isInvulnerableTarget = def.isInvulnerableTarget; //D6-B; set here so both pipelines get it
        unit.attackPattern = def.attackPattern;               //D4; shared, read-only reference to the authored list

        Debug.Log($"[EnemyFactory] Built enemy: {unit.unitName} " +
          $"| Level: {spawnLevel} | PlanetTier: {planetTier} " +
          $"| HP: {unit.GetMaxHP()} | Defense: {unit.GetDefense()} | BAB: {unit.GetBAB()} " +
          $"| Resolve: {unit.GetSaveValue(Unit.SaveType.Resolve)} | Response: {unit.GetSaveValue(Unit.SaveType.Response)} " +
          $"| Focus: {unit.GetSaveValue(Unit.SaveType.Focus)}");

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
            scaled.maxHP, scaled.initiative);
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
        data.psionicResistance = def.psionicResistance; //innate PR; PsionicResistanceCalculator takes the higher of this and any granted PR

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

        var unit = new Unit(def.displayName, true, scaled.maxHP, scaled.initiative);
        unit.sourceCharacterData = data;
        unit.actionSources.Clear();
        unit.actionSources.Add(new BaseActionSource(data.baseActionIds));


        //Progression profile (D2-A): BAB and saves at spawnLevel, same curves and stat types as Unit.ApplyClass.
        //A null progression adds nothing, so the save is the attribute modifier alone (pre-milestone behaviour).
        var profileEffects = new List<StatusEffect>();

        if (def.babProgression.HasValue)
            profileEffects.Add(new StatBonusEffect(StatType.BaseAttackBonus,
                ClassDefinition.CalculateBAB(def.babProgression.Value, spawnLevel), def.enemyId + "_bab"));

        if (def.resolveProgression.HasValue)
            profileEffects.Add(new StatBonusEffect(StatType.Fortitude,
                ClassDefinition.CalculateSaveBonus(def.resolveProgression.Value, spawnLevel), def.enemyId + "_resolve"));

        if (def.responseProgression.HasValue)
            profileEffects.Add(new StatBonusEffect(StatType.Evasion,
                ClassDefinition.CalculateSaveBonus(def.responseProgression.Value, spawnLevel), def.enemyId + "_response"));

        if (def.focusProgression.HasValue)
            profileEffects.Add(new StatBonusEffect(StatType.Willpower,
                ClassDefinition.CalculateSaveBonus(def.focusProgression.Value, spawnLevel), def.enemyId + "_focus"));

        if (profileEffects.Count > 0)
            unit.effectSources.Add(new SimpleEffectSource(profileEffects));

        //Flat offsets (D1 / D2-C): on top of the profile, using each modifier's own bonus type for stacking.
        //Only flatBonus is used; StatModifier.multiplier is ignored here (StatBonusEffect is flat-only).
        if (def.statBonuses.Count > 0)
        {
            var offsetEffects = new List<StatusEffect>();

            foreach (var mod in def.statBonuses)
                offsetEffects.Add(new StatBonusEffect(mod.stat, mod.flatBonus,
                    string.IsNullOrEmpty(mod.source) ? def.enemyId : mod.source, mod.bonusType));

            unit.effectSources.Add(new SimpleEffectSource(offsetEffects));
        }

        //Damage reduction setting (R2): its own source; Unit.GetDamageReduction takes the highest DR present
        if (def.damageReduction > 0)
            unit.effectSources.Add(new SimpleEffectSource(new List<StatusEffect>
            {
                new DamageReductionEffect(def.damageReduction)
            }));

        //Feats: ApplyFeats reads equippedFeatIds, so copy the authored feats onto the data first
        data.equippedFeatIds.AddRange(def.featIds);
        if (data.equippedFeatIds.Count > 0)
            unit.ApplyFeats();

        //Equipment: item action/effect sources and mods, same order as the player pipeline
        unit.ApplyEquipment();

        //HP last: max HP is only known once every source is applied
        unit.currentHP = unit.GetMaxHP();

        //Derive reach profile from size and equipped weapon
        data.reachProfile = ReachProfile.DeriveFrom(data);
        unit.sizeCategory = data.size;

        return unit;
    }

    static Unit BuildClassedUnit(EnemyDefinition def, ScaledStatBlock scaled, int spawnLevel)
    {
        var data = new CharacterData(def.enemyId, def.displayName, 0, def.baseInitiative);
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

        // DEFERRED (G5, until the first classed enemy is authored): this pipeline does not match
        // CharacterBuilder.BuildToLevel. It starts at level 1 with 0 HP and loops LevelUp from 2,
        // so it skips species.Apply, ApplyClassSkills and all level-1 class grants. It also ignores
        // def.additionalActionIds, def.levelActionUnlocks and def.featIds, applies no progression
        // profile, statBonuses, damageReduction or psionicResistance, and multiplies HP by the
        // difficulty modifier after the build. Rebuild through BuildToLevel when fixed.

        //Iterate level up from 2 to spawnLevel, identical to player levelling
        for (int i = 2; i <= spawnLevel; i++)
            data.LevelUp();

        //Equipped itesm
        foreach (var kvp in def.equippedItemIds)
        {
            var instance = ItemInstanceFactory.CreateInstance(kvp.Value);
            data.TryEquipItem(kvp.Key, instance, out _);
        }

        var unit = new Unit(def.displayName, true, data.baseMaxHP, data.baseInitiative);
        unit.sourceCharacterData = data;
        unit.actionSources.Clear();
        unit.actionSources.Add(new BaseActionSource(data.baseActionIds));

        if (!def.isStatBlockSpeciesSubstitute)
            unit.ApplySpecies();

        unit.ApplyClass();
        unit.ApplyFeats();
        unit.ApplyEquipment();

        //Apply difficulty multiplier post construction
        unit.maxHP = Mathf.Max(1, Mathf.RoundToInt(unit.maxHP * scaled.difficultyModifier));
        unit.currentHP = unit.GetMaxHP();

        data.reachProfile = ReachProfile.DeriveFrom(data);
        unit.sizeCategory = data.size;

        return unit;
    }
}
