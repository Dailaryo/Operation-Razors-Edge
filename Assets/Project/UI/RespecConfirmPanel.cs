using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RespecConfirmPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text characterInfoText;
    [SerializeField] private TMP_Text respecCostText;
    [SerializeField] private TMP_Text creditBalanceText;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private CharacterData targetCharacter;

    void OnEnable()
    {
        // Use selected character from hub
        targetCharacter = HubUIController.Instance != null
            ? HubUIController.Instance.GetSelectedCharacter()
            : null;

        Populate();

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(OnConfirmPressed);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(OnCancelPressed);
        }
    }

    void Populate()
    {
        if (targetCharacter == null) return;

        if (characterInfoText != null)
            characterInfoText.text = $"{targetCharacter.name} — Level {targetCharacter.level}";

        int cost = RespecCostCalculator.Calculate(targetCharacter);
        int balance = SquadManager.Instance.creditBalance.credits;

        if (respecCostText != null)
            respecCostText.text = cost == 0
                ? "Cost: 0 credits (first respec)"
                : $"Cost: {cost} credits";

        if (creditBalanceText != null)
            creditBalanceText.text = $"Balance: {balance} credits";

        if (warningText != null)
            warningText.text = "This will reset all feat, skill, and progression choices.\nSpecies and background are preserved.";

        if (confirmButton != null)
            confirmButton.interactable = balance >= cost;
    }

    void OnConfirmPressed()
    {
        if (targetCharacter == null) return;

        if (!RespecManager.Instance.TryBeginRespec(targetCharacter, out string failReason))
        {
            CombatLog.Add($"[Respec] Cannot respec: {failReason}");
            return;
        }

        UIManager.Instance.PopOverlay();
        UIManager.Instance.PushOverlay(ScreenId.LevelUp);
        LevelUpUIController.Instance.BeginLevelUp(targetCharacter);
    }

    void OnCancelPressed()
    {
        UIManager.Instance.PopOverlay();
    }
}
