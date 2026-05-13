using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StoragePanel : MonoBehaviour
{
    [Header("Storage List")]
    [SerializeField] private Transform storageContainer;
    [SerializeField] private GameObject storageEntryPrefab;

    [Header("Character Inventory List")]
    [SerializeField] private Transform characterContainer;
    [SerializeField] private GameObject characterEntryPrefab;

    [Header("Buttons")]
    [SerializeField] private Button moveToStorageButton;
    [SerializeField] private Button moveToCharacterButton;
    [SerializeField] private Button backButton;

    private ItemInstance selectedStorageItem;
    private ItemInstance selectedCharacterItem;

    void OnEnable()
    {
        Refresh();

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => UIManager.Instance.PopOverlay());
        }

        if (moveToStorageButton != null)
        {
            moveToStorageButton.onClick.RemoveAllListeners();
            moveToStorageButton.onClick.AddListener(OnMoveToStoragePressed);
        }

        if (moveToCharacterButton != null)
        {
            moveToCharacterButton.onClick.RemoveAllListeners();
            moveToCharacterButton.onClick.AddListener(OnMoveToCharacterPressed);
        }
    }

    public void Refresh()
    {
        RefreshStorageList();
        RefreshCharacterList();
        RefreshButtons();
    }

    void RefreshStorageList()
    {
        if (storageContainer == null) return;

        for (int i = storageContainer.childCount - 1; i >= 0; i--)
            Destroy(storageContainer.GetChild(i).gameObject);

        foreach (var item in SquadManager.Instance.squadInventory.GetAll())
        {
            GameObject obj = Instantiate(storageEntryPrefab, storageContainer);
            TMP_Text label = obj.GetComponentInChildren<TMP_Text>();
            var def = ItemDatabase.Get(item.definitionId);

            if (label != null)
                label.text = def != null ? def.displayName : item.definitionId;

            ItemInstance captured = item;
            var button = obj.GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(() =>
                {
                    selectedStorageItem = captured;
                    selectedCharacterItem = null;
                    RefreshButtons();
                });
        }
    }

    void RefreshCharacterList()
    {
        if (characterContainer == null) return;

        for (int i = characterContainer.childCount - 1; i >= 0; i--)
            Destroy(characterContainer.GetChild(i).gameObject);

        // Show equipped items from all squad members as moveable back to storage
        foreach (var character in SquadManager.Instance.GetSquad())
        {
            foreach (var kvp in character.equippedItemIds)
            {
                GameObject obj = Instantiate(characterEntryPrefab, characterContainer);
                TMP_Text label = obj.GetComponentInChildren<TMP_Text>();
                var def = ItemDatabase.Get(kvp.Value.definitionId);

                if (label != null)
                    label.text = $"{character.name}: {(def != null ? def.displayName : kvp.Value.definitionId)}";

                ItemInstance captured = kvp.Value;
                EquipmentSlot capturedSlot = kvp.Key;
                CharacterData capturedCharacter = character;

                var button = obj.GetComponent<Button>();
                if (button != null)
                    button.onClick.AddListener(() =>
                    {
                        selectedCharacterItem = captured;
                        selectedStorageItem = null;
                        RefreshButtons();
                    });
            }
        }
    }

    void RefreshButtons()
    {
        if (moveToStorageButton != null)
            moveToStorageButton.interactable = selectedCharacterItem != null;

        if (moveToCharacterButton != null)
            moveToCharacterButton.interactable = selectedStorageItem != null;
    }

    void OnMoveToStoragePressed()
    {
        if (selectedCharacterItem == null) return;

        // Find which character has this item equipped and unequip it
        foreach (var character in SquadManager.Instance.GetSquad())
        {
            foreach (var kvp in character.equippedItemIds)
            {
                if (kvp.Value.instanceId == selectedCharacterItem.instanceId)
                {
                    if (character.TryUnequipItem(kvp.Key, out string reason))
                    {
                        SquadManager.Instance.squadInventory.AddItem(selectedCharacterItem);
                        selectedCharacterItem = null;
                        Refresh();
                        return;
                    }
                    else
                    {
                        CombatLog.Add($"[StoragePanel] Cannot unequip: {reason}");
                        return;
                    }
                }
            }
        }
    }

    void OnMoveToCharacterPressed()
    {
        // Moving from storage to a character opens the equipment panel
        // so the player can choose which slot to equip into.
        // For this pass: log the intent and defer slot selection to EquipmentPanel.
        if (selectedStorageItem == null) return;

        CombatLog.Add("[StoragePanel] Select a character and use the Equipment panel to equip from storage.");
    }
}