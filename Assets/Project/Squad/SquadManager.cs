using UnityEngine;
using System.Collections.Generic;

public class SquadManager : MonoBehaviour
{
    public static SquadManager Instance;

    private List<CharacterData> squad = new List<CharacterData>();
    private List<SquadRuntime> runtimeSquad = new List<SquadRuntime>();

    void Awake()
    {
        Instance = this;
        InitializeSquad();
    }

    void InitializeSquad()
    {
        squad.Clear();

        //TEMP: placeholder until UI hookup
        squad.AddRange(GetDebugSquad());
    }

    List<CharacterData> GetDebugSquad()
    {
        return new List<CharacterData>
        {
            CreateTestCharacter("Unit A"),
            CreateTestCharacter("Unit B"),
            CreateTestCharacter("Unit C"),
            CreateTestCharacter("Unit D"),
            CreateTestCharacter("Unit E"),
            CreateTestCharacter("Unit F"),
        };
    }

    //Create combat Units from CharacterData
    public List<Unit> CreateCombatUnits()
    {
        List<Unit> units = new List<Unit>();

        foreach (var runtime in runtimeSquad)
        {
            if (runtime.currentHP <= 0) continue;
            
            Unit unit = new Unit(
                runtime.source.name,
                false,
                runtime.source.baseMaxHP,
                runtime.source.baseAttack,
                runtime.source.baseInitiative
            );

            //Apply persistant HP
            unit.currentHP = runtime.currentHP;
            unit.sourceCharacterData = runtime.source;
            unit.attackRating = runtime.source.baseAttack;
            unit.actionSources.Clear();

            // Base abilities
            unit.actionSources.Add(new BaseActionSource(runtime.source.baseActionIds));

            // Unlocked abilities
            unit.actionSources.Add(new BaseActionSource(runtime.source.unlockedActions));


            units.Add(unit);
        }

        return units;
    }

    //Apply combat results back to persistent data
    public void ApplyCombatResults(List<Unit> units)
    {
        foreach (var unit in units)
        {
            var runtime = runtimeSquad.Find(r => r.source.name == unit.unitName);

            if (runtime != null)
            {
                runtime.currentHP = unit.currentHP;
            }
        }
    }

    //Called when returning to hub
    public void ResetSquadToFull()
    {
        foreach (var runtime in runtimeSquad)
        {
            runtime.HealToFull();
        }

        CommitRuntimeToCharacters();
    }

    public void CreateRuntimeSquad()
    {
        runtimeSquad.Clear();

        foreach (var character in squad)
        {
            runtimeSquad.Add(new SquadRuntime(character));
        }
    }

    public void CommitRuntimeToCharacters()
    {
        foreach (var runtime in runtimeSquad)
        {
            runtime.source.currentHP = runtime.currentHP;
        }
    }

    public void GrantXPToSquad(int amount)
    {
        foreach (var character in squad)
        {
            int before = character.xp;
            int beforeLevel = character.level;
            
            character.GainXP(amount);

            CombatLog.Add($"{character.name} gained {amount} XP ({character.xp}/{character.xpToNextLevel})");

            if (character.level > beforeLevel)
            {
                CombatLog.Add($"{character.name} leveled up to {character.level}!");
            }
        }
    }

    CharacterData CreateTestCharacter(string name)
    {
        var c = new CharacterData(name.ToLower(), name, 100, 10, 5);

        //Core actions only
        c.baseActionIds.Add(ActionIds.MeleeAttack);
        c.baseActionIds.Add(ActionIds.RangedAttack);
        c.baseActionIds.Add(ActionIds.Move);
        c.baseActionIds.Add(ActionIds.FightingDefensively);
        c.baseActionIds.Add(ActionIds.TotalDefense);

        return c;
    }

    public void SetSquad(List<CharacterData> newSquad)
    {
        squad = new List<CharacterData>(newSquad);
        CreateRuntimeSquad();
    }
}
