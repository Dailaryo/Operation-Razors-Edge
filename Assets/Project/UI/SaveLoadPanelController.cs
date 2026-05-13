using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveLoadPanelController : MonoBehaviour
{
    [Header("Slot Buttons")]
    [SerializeField] private Button[] slotButtons; // 5 slots
    [SerializeField] private TMP_Text[] slotLabels;

    [Header("Action Buttons")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button closeButton;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private TMP_Text confirmText;
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmNoButton;

    private int selectedSlot = -1;
    private System.Action pendingConfirmAction;

    void OnEnable()
    {
        RefreshSlots();

        for (int i = 0; i < slotButtons.Length; i++)
        {
            int capturedIndex = i;
            if (slotButtons[i] != null)
            {
                slotButtons[i].onClick.RemoveAllListeners();
                slotButtons[i].onClick.AddListener(() => OnSlotSelected(capturedIndex));
            }
        }

        if (saveButton != null)
        {
            saveButton.onClick.RemoveAllListeners();
            saveButton.onClick.AddListener(OnSavePressed);
        }

        if (loadButton != null)
        {
            loadButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(OnLoadPressed);
        }

        if (deleteButton != null)
        {
            deleteButton.onClick.RemoveAllListeners();
            deleteButton.onClick.AddListener(OnDeletePressed);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => UIManager.Instance.PopOverlay());
        }

        if (confirmYesButton != null)
        {
            confirmYesButton.onClick.RemoveAllListeners();
            confirmYesButton.onClick.AddListener(OnConfirmYes);
        }

        if (confirmNoButton != null)
        {
            confirmNoButton.onClick.RemoveAllListeners();
            confirmNoButton.onClick.AddListener(OnConfirmNo);
        }

        if (confirmPanel != null)
            confirmPanel.SetActive(false);

        selectedSlot = -1;
        RefreshActionButtons();
    }

    void RefreshSlots()
    {
        for (int i = 0; i < slotLabels.Length; i++)
        {
            if (slotLabels[i] != null)
                slotLabels[i].text = $"Slot {i + 1}: Empty";
        }
        // Full slot metadata (timestamps, character info) deferred to polish pass.
        // SaveManager currently uses a single file path rather than indexed slots —
        // multi-slot architecture is a future SaveManager change.
    }

    void OnSlotSelected(int index)
    {
        selectedSlot = index;
        RefreshActionButtons();
    }

    void RefreshActionButtons()
    {
        bool slotSelected = selectedSlot >= 0;

        if (saveButton != null) saveButton.interactable = slotSelected;
        if (loadButton != null) loadButton.interactable = slotSelected;
        if (deleteButton != null) deleteButton.interactable = slotSelected;
    }

    void OnSavePressed()
    {
        ShowConfirm($"Save to Slot {selectedSlot + 1}?", () =>
        {
            if (CampaignManager.Instance != null)
                SaveManager.Save(
                    CampaignManager.Instance.GetCampaignData(),
                    SquadManager.Instance.GetSquad());

            RefreshSlots();
        });
    }

    void OnLoadPressed()
    {
        ShowConfirm($"Load Slot {selectedSlot + 1}? Unsaved progress will be lost.", () =>
        {
            if (CampaignManager.Instance != null)
                SaveManager.Load(
                    CampaignManager.Instance.GetCampaignData(),
                    SquadManager.Instance.GetSquad());
        });
    }

    void OnDeletePressed()
    {
        ShowConfirm($"Delete Slot {selectedSlot + 1}?", () =>
        {
            SaveManager.DeleteSave();
            RefreshSlots();
        });
    }

    void ShowConfirm(string message, System.Action onConfirm)
    {
        if (confirmPanel == null) return;

        pendingConfirmAction = onConfirm;

        if (confirmText != null)
            confirmText.text = message;

        confirmPanel.SetActive(true);
    }

    void OnConfirmYes()
    {
        if (confirmPanel != null)
            confirmPanel.SetActive(false);

        pendingConfirmAction?.Invoke();
        pendingConfirmAction = null;
    }

    void OnConfirmNo()
    {
        if (confirmPanel != null)
            confirmPanel.SetActive(false);

        pendingConfirmAction = null;
    }
}