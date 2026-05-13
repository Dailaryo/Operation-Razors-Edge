using UnityEngine;
using System.Collections.Generic;

public class SquadManager : MonoBehaviour
{
    public static SquadManager Instance;

    public CreditBalance creditBalance = new CreditBalance();

    public Inventory squadInventory = new Inventory(0f);

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
        CreateRuntimeSquad();
    }

    //Create combat Units from CharacterData
    public List<Unit> CreateCombatUnits()
    {
        List<Unit> units = new List<Unit>();

        foreach (var runtime in runtimeSquad)
        {
            if (runtime.currentHP <= 0 || runtime.isIncapacitated) continue;

            Unit unit = CharacterFactory.CreatePlayerUnit(runtime);
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

        UpdateSquadInventoryCapacity();

        foreach (var character in squad)
        {
            var runtime = new SquadRuntime(character);
            runtime.snapshot = CharacterSnapshot.Capture(character);
            runtimeSquad.Add(runtime);
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

    // Awards individually scaled XP amounts per character.
    // xpByCharacterId maps characterId -> XP amount to grant.
    // Characters not present in the map receive no XP — this is intentional,
    // not an error, since incapacitated characters are excluded by the calculator.
    public void GrantXPToSquadScaled(Dictionary<string, int> xpByCharacterId)
    {
        foreach (CharacterData character in squad)
        {
            if (!xpByCharacterId.TryGetValue(character.characterId, out int amount)) continue;
            if (amount <= 0) continue;

            int beforeLevel = character.level;

            character.GainXP(amount);

            CombatLog.Add($"{character.name} gained {amount} XP " +
                      $"({character.xp}/{character.xpToNextLevel})");

            if (character.level > beforeLevel)
                CombatLog.Add($"{character.name} leveled up to {character.level}!");
        }
    }

    public void SetSquad(List<CharacterData> newSquad)
    {
        squad = new List<CharacterData>(newSquad);
        CreateRuntimeSquad();
    }

    public List<CharacterData> GetSquad()
    {
        return squad;
    }

    public void AddCharacter(CharacterData data)
    {
        if (squad.Count >= 6)
        {
            Debug.LogWarning("[SquadManager] Roster is full. Cannot add character.");
            return;
        }

        squad.Add(data);
        UpdateSquadInventoryCapacity();
    }

    public void RemoveCharacter(string characterId)
    {
        if (GameFlowManager.Instance == null ||
            GameFlowManager.Instance.currentState != GameFlowManager.GameState.Hub)
        {
            Debug.LogWarning("[SquadManager] Characters can only be removed at the Hub.");
            return;
        }

        var character = squad.Find(c => c.characterId == characterId);
            
        if (character == null)
        {
            Debug.LogWarning($"[SquadManager] Character '{characterId}' not found in roster.");
            return;
        }

        ShipInventory.DepositFromCharacter(character);
        squad.Remove(character);
        UpdateSquadInventoryCapacity();

        //Clear any corresponding runtime entry
        runtimeSquad.RemoveAll(r => r.source.characterId == characterId);

        CombatLog.Add($"[SquadManager] {character.name} removed from roster");
    }

    public List<CharacterData> GetRoster()
    {
        return squad;
    }

    public Dictionary<string, CharacterSnapshot> GetSnapshots()
    {
        var result = new Dictionary<string, CharacterSnapshot>();
        foreach (var runtime in runtimeSquad)
        {
            if (runtime.snapshot != null)
                result[runtime.source.characterId] = runtime.snapshot;
        }
        return result;
    }

    public void CommitNewSnapshot()
    {
        foreach (var runtime in runtimeSquad)
        {
            runtime.snapshot = CharacterSnapshot.Capture(runtime.source);
        }
        CombatLog.Add("[SquadManager] Squad snapshots committed.");
    }

    public void SetIncapacitated(Unit unit)
    {
        var runtime = runtimeSquad.Find(r => r.source == unit.sourceCharacterData);

        if (runtime == null)
        {
            Debug.LogWarning($"[SquadManager] No runtime found for unit '{unit.unitName}'.");
            return;
        }

        runtime.isIncapacitated = true;
        runtime.source.isIncapacitated = true;

        UpdateSquadInventoryCapacity();

        CombatLog.Add($"{unit.unitName} has been incapacitated.");

        OnCharacterIncapacitated?.Invoke(unit);
    }

    public event System.Action<Unit> OnCharacterIncapacitated;

    public void RestoreIncapacitatedCharacters()
    {
        foreach (var runtime in runtimeSquad)
        {
            if (!runtime.isIncapacitated) continue;

            runtime.snapshot.Restore(runtime.source);
            runtime.isIncapacitated = false;
            runtime.source.isIncapacitated = false;
            runtime.currentHP = runtime.source.currentHP;

            UpdateSquadInventoryCapacity();

            CombatLog.Add($"[SquadManager] {runtime.source.name} restored from snapshot.");
        }
    }

    public float GetTotalFieldCapacity()
    {
        float total = 0f;

        foreach (var runtime in runtimeSquad)
        {
            if (!runtime.isIncapacitated)
                total += runtime.source.GetMaxCarryWeight();
        }

        return total;
    }

    public void ClearRoster()
    {
        squad.Clear();
        runtimeSquad.Clear();
    }

    public void UpdateSquadInventoryCapacity()
    {
        squadInventory.maxCarryWeight = GetTotalFieldCapacity();
    }

    public void AddCredits(int amount)
    {
        creditBalance.Add(amount);
        CombatLog.Add($"[Economy] +{amount} credits. Balance: {creditBalance.credits}");
    }

    public bool SpendCredits(int cost)
    {
        bool success = creditBalance.Spend(cost);
        if (success)
            CombatLog.Add($"[Economy] -{cost} credits. Balance: {creditBalance.credits}");
        else
            CombatLog.Add($"[Economy] Insufficient credits. Need {cost}, have {creditBalance.credits}");
        return success;
    }
}
