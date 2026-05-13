using UnityEngine;
using System.Collections.Generic;

public static class CharacterFactory
{
    //Builds a fully initialized player Unit from a SquadRuntime
    //This is the primary construction path for player characters
    public static Unit CreatePlayerUnit(SquadRuntime runtime)
    {
        var data = runtime.source;

        var unit = new Unit(
            data.name,
            false,
            data.baseMaxHP,
            data.baseAttack,
            data.baseInitiative
        );

        //Restore persistent HP from runtime
        unit.currentHP = runtime.currentHP;
        unit.sourceCharacterData = data;
        unit.attackRating = data.baseAttack;

        //Step 1: Base Universal actions
        if (data.baseActionIds.Count > 0)
            unit.actionSources.Add(new BaseActionSource(data.baseActionIds));

        //Step 2: Species - biological baseline
        unit.ApplySpecies();

        //Step 3: Class - BAB, saves, resource, etc
        unit.ApplyClass();

        //Step 4: Background
        unit.ApplyBackground();

        //Step 5: Feats
        unit.ApplyFeats();

        //Step 6: Equipment
        unit.ApplyEquipment();

        //Step. 7 Derive reach profile from size and equipped weapon
        data.reachProfile = ReachProfile.DeriveFrom(data);
        unit.sizeCategory = data.size;

        Debug.Log($"[CharacterFactory] Built player unit: {unit.unitName} " +
            $"| Species {data.speciesId} " +
            $"| Class: {data.classId} " +
            $"| Background: {data.backgroundId} " +
            $"| Level: {data.level} " +
            $"| Items: {data.equippedItemIds.Count}");

        return unit;
    }

    //Convienice overload for building from CharacterData directly
    //Used when no SquadRuntime exists
    public static Unit CreatePlayerUnit(CharacterData data)
    {
        var runtime = new SquadRuntime(data);
        return CreatePlayerUnit(runtime);
    }
}
