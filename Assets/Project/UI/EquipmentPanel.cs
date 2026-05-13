using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class EquipmentPanel : MonoBehaviour
{
    [Header("Equipment Slots")]
    [SerializeField] private Transform slotContainer;
    [SerializeField] private GameObject slotEntryPrefab;

    [Header("Inventory List")]
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private GameObject inventoryEntryPrefab;

    [Header("Item Description")]
    [SerializeField] private TMP_Text itemDescriptionText;

    [Header("Action Buttons")]
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;
    [SerializeField] private Button backButton;

    private CharacterData currentCharacter;
    private ItemInstance selectedInventoryItem;
    private EquipmentSlot selectedSlot;

    void OnEnable()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => UIManager.Instance.PopOverlay());
        }

        if (equipButton != null)
        {
            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(OnEquipPressed);
        }

        if (unequipButton != null)
        {
            unequipButton.onClick.RemoveAllListeners();
            unequipButton.onClick.AddListener(OnUnequipPressed);
        }
    }

    public void SetCharacter(CharacterData character)
    {
        currentCharacter = character;
        Refresh();
    }

    public void Refresh()
    {
        if (currentCharacter == null) return;

        RefreshSlots();
        RefreshInventory();
        RefreshButtons();
    }

    void RefreshSlots()
    {
        if (slotContainer == null) return;

        for (int i = slotContainer.childCount - 1; i >= 0; i--)
            Destroy(slotContainer.GetChild(i).gameObject);

        foreach (EquipmentSlot slot in System.Enum.GetValues(typeof(EquipmentSlot)))
        {
            GameObject obj = Instantiate(slotEntryPrefab, slotContainer);
            TMP_Text label = obj.GetComponentInChildren<TMP_Text>();

            if (currentCharacter.HasItemEquipped(slot))
            {
                var instance = currentCharacter.GetEquippedItem(slot);
                var def = ItemDatabase.Get(instance.definitionId);
                string itemName = def != null ? def.displayName : instance.definitionId;

                if (label != null)
                    label.text = $"{slot}: {itemName}";

                EquipmentSlot capturedSlot = slot;
                var button = obj.GetComponent<Button>();
                if (button != null)
                    button.onClick.AddListener(() => OnSlotSelected(capturedSlot));
            }
            else
            {
                if (label != null)
                    label.text = $"{slot}: (empty)";
            }
        }
    }

    void RefreshInventory()
    {
        if (inventoryContainer == null) return;

        for (int i = inventoryContainer.childCount - 1; i >= 0; i--)
            Destroy(inventoryContainer.GetChild(i).gameObject);

        foreach (var item in SquadManager.Instance.squadInventory.GetAll())
        {
            GameObject obj = Instantiate(inventoryEntryPrefab, inventoryContainer);
            TMP_Text label = obj.GetComponentInChildren<TMP_Text>();
            var def = ItemDatabase.Get(item.definitionId);

            if (label != null)
                label.text = def != null ? def.displayName : item.definitionId;

            ItemInstance capturedItem = item;
            var button = obj.GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(() => OnInventoryItemSelected(capturedItem));
        }
    }

    void RefreshButtons()
    {
        if (equipButton != null)
            equipButton.interactable = selectedInventoryItem != null;

        if (unequipButton != null)
            unequipButton.interactable = currentCharacter != null &&
                                         currentCharacter.HasItemEquipped(selectedSlot);
    }

    void OnSlotSelected(EquipmentSlot slot)
    {
        selectedSlot = slot;
        selectedInventoryItem = null;

        if (currentCharacter.HasItemEquipped(slot))
        {
            var instance = currentCharacter.GetEquippedItem(slot);
            var def = ItemDatabase.Get(instance.definitionId);
            ShowItemDescription(def);
        }

        RefreshButtons();
    }

    void OnInventoryItemSelected(ItemInstance item)
    {
        selectedInventoryItem = item;
        var def = ItemDatabase.Get(item.definitionId);
        ShowItemDescription(def);
        RefreshButtons();
    }

    void ShowItemDescription(ItemDefinition def)
    {
        if (itemDescriptionText == null) return;

        if (def == null)
        {
            itemDescriptionText.text = "";
            return;
        }

        var lines = new List<string>();
        lines.Add(def.displayName);

        if (def.statModifiers != null)
        {
            foreach (var mod in def.statModifiers)
                lines.Add($"  {mod.stat}: {(mod.flatBonus >= 0 ? "+" : "")}{mod.flatBonus}");
        }

        itemDescriptionText.text = string.Join("\n", lines);
    }

    void OnEquipPressed()
    {
        if (currentCharacter == null || selectedInventoryItem == null) return;
        OnEquipItem(selectedSlot, selectedInventoryItem);
    }

    void OnUnequipPressed()
    {
        if (currentCharacter == null) return;
        OnUnequipItem(selectedSlot);
    }

    public void OnEquipItem(EquipmentSlot slot, ItemInstance itemInstance)
    {
        if (currentCharacter == null) return;

        if (currentCharacter.TryEquipItem(slot, itemInstance, out string reason))
        {
            SquadManager.Instance.squadInventory.RemoveItem(itemInstance);
            selectedInventoryItem = null;
            Refresh();
        }
        else
        {
            CombatLog.Add($"[EquipmentPanel] Cannot equip: {reason}");
        }
    }

    public void OnUnequipItem(EquipmentSlot slot)
    {
        if (currentCharacter == null) return;

        ItemInstance itemInstance = currentCharacter.GetEquippedItem(slot);

        if (currentCharacter.TryUnequipItem(slot, out string reason))
        {
            SquadManager.Instance.squadInventory.AddItem(itemInstance);
            Refresh();
        }
        else
        {
            CombatLog.Add($"[EquipmentPanel] Cannot unequip: {reason}");
        }
    }
}