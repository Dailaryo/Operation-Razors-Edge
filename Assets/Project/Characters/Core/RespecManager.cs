using UnityEngine;

public class RespecManager : MonoBehaviour
{
    public static RespecManager Instance;

    public bool IsInRespec {  get; private set; }

    private CharacterData respecTarget;
    private CharacterBackup respecBackup;
    private int respecCost;
    private int targetLevel;

    void Awake()
    {
        Instance = this;
    }

    public bool TryBeginRespec(CharacterData data, out string failReason)
    {
        if (GameFlowManager.Instance == null ||
            GameFlowManager.Instance.currentState != GameFlowManager.GameState.Hub)
        {
            failReason = "Respec is only available at the Hub";
            return false;
        }

        if (IsInRespec)
        {
            failReason = "A respec is already in progress.";
            return false;
        }

        int cost = RespecCostCalculator.Calculate(data);

        if (cost > 0 && !SquadManager.Instance.creditBalance.CanAfford(cost))
        {
            failReason = $"Insufficient credits. Respec costs {cost}, " +
                         $"balance is {SquadManager.Instance.creditBalance.credits}.";
            return false;
        }

        //All checks passed: commit to starting the respec
        respecTarget = data;
        respecBackup = CharacterBackup.Capture(data);
        respecCost = cost;
        targetLevel = data.level;

        if (cost > 0)
            SquadManager.Instance.SpendCredits(cost);

        IsInRespec = true;

        //Rebuild from scratch; pregens re-follow their plan automatically
        CharacterBuilder.BuildToLevel(data, targetLevel);

        failReason = null;
        return true;
    }

    public void FinalizeRespec()
    {
        if (!IsInRespec)
        {
            Debug.LogWarning("[RespecManager] FinalizeRespec called but no respec is in progress.");
            return;
        }

        ClassItemGrants.RemoveUnearned(respecTarget);
        var unequippedItems = EquipmentValidator.ValidateAndUnequip(respecTarget);

        if (unequippedItems.Count > 0)
            CombatLog.Add($"[Respec] {unequippedItems.Count} item(s) unequipped due to " +
                          $"changed prerequisites: {string.Join(", ", unequippedItems)}");

        respecTarget.respecCount++;

        SquadManager.Instance.CommitNewSnapshot();

        CombatLog.Add($"[Respec] {respecTarget.name} respec finalized. " +
                      $"Total respecs: {respecTarget.respecCount}");

        Cleanup();
    }

    public void CancelRespec()
    {
        if (!IsInRespec)
        {
            Debug.LogWarning("[RespecManager] CancelRespec called but no respec is in progress.");
            return;
        }

        //Full reset before restoring, to avoid snapshot gaps on partially re-leveled characters
        respecTarget.ResetToLevelOne();
        respecBackup.Restore(respecTarget);

        //Refund credits if any were spent
        if (respecCost > 0)
            SquadManager.Instance.AddCredits(respecCost);

        CombatLog.Add($"[Respec] {respecTarget.name} respec cancelled. " +
                      $"{respecCost} credits refunded.");

        Cleanup();
    }

    private void Cleanup()
    {
        IsInRespec = false;
        respecTarget = null;
        respecBackup = null;
        respecCost = 0;
        targetLevel = 0;
    }
}
