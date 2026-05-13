using UnityEngine;
using System.Collections.Generic;

public class CharacterCreationManager : MonoBehaviour
{
    public static CharacterCreationManager Instance;

    void Awake()
    {
        Instance = this;
    }

    //Called by GameFlowManager.NewGame() to populate the roster
    public void StartNewGame()
    {
        SquadManager.Instance.ClearRoster();
        LoadDebugSquad();
    }

    //Creates a replacement character at current minimum squad level
    //Called from Hub UI when player deletes a character and creates a new one
    public void CreateReplacementCharacter(CharacterData data)
    {
        int minLevel = GetMinimumSquadLevel();

        //Bring the new character up to minimum level without triggering level-up UI
        while (data.level < minLevel)
        {
            data.level++;

            var classDef = ClassDatabase.Get(data.classId);
            if (classDef != null) classDef.ApplyLevelUp(data, data.level);
        }

        SquadManager.Instance.AddCharacter(data);
    }
    int GetMinimumSquadLevel()
    {
        int min = int.MaxValue;

        foreach (var character in SquadManager.Instance.GetRoster())
        {
            if (character.level < min)
                min = character.level;
        }

        return min == int.MaxValue ? 1 : min;
    }

    // TEMP: Replaces GetDebugSquad() from SquadManager
    // Remove once pre-gen characters and CharacterCreationUI are implemented
    void LoadDebugSquad()
    {
        SquadManager.Instance.AddCharacter(CreateTestCharacter("Unit A"));
        SquadManager.Instance.AddCharacter(CreateTestCharacter("Unit B"));
        SquadManager.Instance.AddCharacter(CreateTestCharacter("Unit C"));
        SquadManager.Instance.AddCharacter(CreateTestCharacter("Unit D"));
        SquadManager.Instance.AddCharacter(CreateTestCharacter("Unit E"));
        SquadManager.Instance.AddCharacter(CreateTestCharacter("Unit F"));
    }

    CharacterData CreateTestCharacter(string name)
    {
        var c = new CharacterData(name.ToLower(), name, 100, 10, 5);
        c.classId = ClassIds.Soldier;

        if (name == "Unit A")
        {
            c.equippedFeatIds.Add(FeatIds.PowerAttack);
            c.equippedFeatIds.Add(FeatIds.Toughness);
        }

        return c;
    }

    // TODO: Pre-generated character factory methods
    // Implement once class and background content is authored
    // Each method should call SquadManager.Instance.AddCharacter()
    // Alexandra — Delphin genotype, squad commander
    // Rog       — Pentian genotype, melee DPS
    // Grok      — Osteo genotype, tank
    // Meharel   — Cryptian genotype, sniper
    // Harley    — Radix genotype, medic
    // Anyha     — Phytonian, psionic specialist
}
