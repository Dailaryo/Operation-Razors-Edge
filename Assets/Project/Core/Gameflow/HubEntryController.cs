using System.Collections.Generic;
using UnityEngine;

public class HubEntryController : MonoBehaviour
{
    public static HubEntryController Instance;

    void Awake()
    {
        Instance = this;
    }

    //Called by GammeflowManager.EnterHub() after RestoreIncpacitatedCharacters()
    public void RunHubEntryComparison()
    {
        var squad = SquadManager.Instance.GetSquad();
        var snapshots = SquadManager.Instance.GetSnapshots();

        bool anyChanges = false;

        foreach (var character in squad)
        {
            if (!snapshots.TryGetValue(character.characterId, out CharacterSnapshot snapshot))
            {
                CombatLog.Add($"[HubEntry] No snapshot found for {character.name} — skipping comparison.");
                continue;
            }

            var comparison = SnapshotComparisonData.Compare(snapshot, character);

            bool characterChanged = false;
            foreach (var entry in comparison.entries)
            {
                if (entry.hasChanged)
                {
                    if (!characterChanged)
                    {
                        CombatLog.Add($"[HubEntry] Changes for {character.name}:");
                        characterChanged = true;
                        anyChanges = true;
                    }
                    CombatLog.Add($"  {entry.fieldName}: {entry.snapshotValue} -> {entry.currentValue}");
                }
            }

            if (!characterChanged)
                CombatLog.Add($"[HubEntry] {character.name}: no changes.");
        }

        if (!anyChanges)
            CombatLog.Add("[HubEntry] All characters unchanged since last hub visit.");

        //UI acknowledgement deferred: commit immediately for now
        ConfirmAndCommit();
    }

    //Called by player confirmation
    public void ConfirmAndCommit()
    {
        SquadManager.Instance.CommitNewSnapshot();
        CombatLog.Add("[HubEntry] Snapshots committed.");
    }
}
