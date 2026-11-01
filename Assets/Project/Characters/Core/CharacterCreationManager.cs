using UnityEngine;
using System.Collections.Generic;

public class CharacterCreationManager : MonoBehaviour
{
    public static CharacterCreationManager Instance;

    void Awake()
    {
        Instance = this;
    }

    //Called by GameFlowManager.NewGame() and EnterHub() (empty squad) to populate the roster with the six pregens
    public void StartNewGame()
    {
        SquadManager.Instance.ClearRoster();
        LoadPregenSquad();
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

    // Populates the roster with the six pregens, in PregenIds order.
    // Each Build() returns a fresh CharacterData, so XP and level-ups never
    // mutate the reference copies held in PregenDatabase.Definitions.
    // Do not use PregenDatabase.Get() here — it returns the shared static instance.
    void LoadPregenSquad()
    {
        SquadManager.Instance.AddCharacter(AlexandraIvanova.Build());
        SquadManager.Instance.AddCharacter(RogDarrel.Build());
        SquadManager.Instance.AddCharacter(MeharelNueva.Build());
        SquadManager.Instance.AddCharacter(GrokBaird.Build());
        SquadManager.Instance.AddCharacter(HarveyJones.Build());
        SquadManager.Instance.AddCharacter(AnyhaMcRae.Build());
    }
}
