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
            if (runtime.isIncapacitated) continue;

            Unit unit = CharacterFactory.CreatePlayerUnit(runtime);
            if (unit.currentHP <= 0) continue;

            units.Add(unit);
        }

        return units;
    }

    //Apply combat results back to persistent data
    public void ApplyCombatResults(List<Unit> units)
    {
        foreach (var unit in units)
        {
            var runtime = runtimeSquad.Find(r => r.source == unit.sourceCharacterData);

            if (runtime != null)
            {
                runtime.hpDamage = Mathf.Max(0, unit.GetMaxHP() - unit.currentHP);

                //Effects lasting 1 hour+ carry between encounters until the Hub restore clears them (TEMP until time mechanics are implemented)
                runtime.persistentEffects.Clear();
                foreach (var effect in unit.activeEffects)
                    if (effect.lastsUntilHub && effect.maintainerCount == 0)
                        runtime.persistentEffects.Add(effect);

                if (unit.powerPoints != null)
                {
                    runtime.powerPointsSpent = unit.powerPoints.max - unit.powerPoints.current;
                    runtime.zeroLevelUsesSpent = unit.powerPoints.freeZeroLevelUsed;
                }
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

        foreach (var character in squad)
            character.hpDamage = 0;

        RechargeStoredPowerPoints();
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
            runtime.source.hpDamage = runtime.hpDamage;
            runtime.source.powerPointsSpent = runtime.powerPointsSpent;
            runtime.source.zeroLevelUsesSpent = runtime.zeroLevelUsesSpent;
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

    //Highest character level in the squad (1 if empty). Drills use it for spawnLevel 0 = "match the squad";
    //after a debug rebuild every member shares one level.
    public int GetHighestLevel()
    {
        int highest = 1;
        foreach (var character in squad)
            highest = Mathf.Max(highest, character.level);
        return highest;
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

    public void SetSnapshot(string characterId, CharacterSnapshot snapshot)
    {
        var runtime = runtimeSquad.Find(r => r.source.characterId == characterId);

        if (runtime == null)
        {
            Debug.LogWarning($"[SquadManager] SetSnapshot: no runtime for '{characterId}'.");
            return;
        }

        runtime.snapshot = snapshot;
    }

    public void CommitNewSnapshot()
    {
        foreach (var runtime in runtimeSquad)
        {
            runtime.snapshot = CharacterSnapshot.Capture(runtime.source);
        }
        CombatLog.Add("[SquadManager] Squad snapshots committed.");
    }

    public void SetIncapacitated(Unit unit, bool rollback = true)
    {
        var runtime = runtimeSquad.Find(r => r.source == unit.sourceCharacterData);

        if (runtime == null)
        {
            Debug.LogWarning($"[SquadManager] No runtime found for unit '{unit.unitName}'.");
            return;
        }

        runtime.isIncapacitated = true;
        runtime.source.isIncapacitated = true;
        runtime.source.incapacitatedWithoutRollback = !rollback;

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

            if (runtime.source.incapacitatedWithoutRollback)
                CombatLog.Add($"[SquadManager] {runtime.source.name} recovers.");
            else
            {
                runtime.snapshot.Restore(runtime.source);
                CombatLog.Add($"[SquadManager] {runtime.source.name} restored from snapshot.");
            }

            runtime.isIncapacitated = false;
            runtime.source.isIncapacitated = false;
            runtime.source.incapacitatedWithoutRollback = false;
            runtime.hpDamage = runtime.source.hpDamage;

            UpdateSquadInventoryCapacity();
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

    // DEBUG (D11): rebuilds every squad member to targetLevel through CharacterBuilder.BuildToLevel,
    // the same path pregen Build() and respec use. Pregens follow their PregenProgression route.
    // Hub only. Recreates the runtime squad so incapacitation snapshots match the new level.
    public void DebugRebuildSquadToLevel(int targetLevel)
    {
        if (GameFlowManager.Instance == null ||
            GameFlowManager.Instance.currentState != GameFlowManager.GameState.Hub)
        {
            Debug.LogWarning("[SquadManager] Debug squad level can only be set at the Hub.");
            return;
        }

        targetLevel = Mathf.Clamp(targetLevel, 1, 20);

        foreach (var character in squad)
        {
            character.xp = 0; //fresh start at the new level; BuildToLevel preserves in-level progress
            CharacterBuilder.BuildToLevel(character, targetLevel);
            character.NotifyChanged();
        }

        CreateRuntimeSquad();

        CombatLog.Add($"[Debug] Squad rebuilt to level {targetLevel}.");
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

    //Power Crystal and similar: stored power points refill at the Hub
    void RechargeStoredPowerPoints()
    {
        var instances = new List<ItemInstance>(squadInventory.GetAll());
        instances.AddRange(ShipInventory.GetAll());
        foreach (var character in squad)
            instances.AddRange(character.equippedItemIds.Values);

        foreach (var inst in instances)
        {
            var def = ItemDatabase.Get(inst.definitionId);
            if (def != null && def.storedPowerPoints > 0)
                inst.charges = def.storedPowerPoints;
        }
    }
}
