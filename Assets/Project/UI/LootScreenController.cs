using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LootScreenController : MonoBehaviour
{
    [Header("Item List")]
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject lootItemEntryPrefab;

    [Header("Buttons")]
    [SerializeField] private Button selectAllButton;
    [SerializeField] private Button confirmButton;

    private List<LootItemEntry> entries = new List<LootItemEntry>();

    void OnEnable()
    {
        Populate();
    }

    void Populate()
    {
        ClearEntries();

        foreach (var item in LootQueue.GetAll())
        {
            GameObject obj = Instantiate(lootItemEntryPrefab, itemContainer);

            var entry = obj.GetComponent<LootItemEntry>();
            if (entry != null)
            {
                entry.Initialize(item);
                entries.Add(entry);
            }
        }

        if (selectAllButton != null)
        {
            selectAllButton.onClick.RemoveAllListeners();
            selectAllButton.onClick.AddListener(OnSelectAllPressed);
        }

        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(OnConfirmPressed);
        }
    }

    void OnSelectAllPressed()
    {
        foreach (var entry in entries)
            entry.SetChecked(true);
    }

    void OnConfirmPressed()
    {
        foreach (var entry in entries)
        {
            if (entry.IsChecked)
                SquadManager.Instance.squadInventory.AddItem(entry.Item);
        }

        LootQueue.Clear();
        UIManager.Instance.PopOverlay();
        GameFlowManager.Instance.ReturnToHub();
    }

    void ClearEntries()
    {
        if (itemContainer == null) return;

        for (int i = itemContainer.childCount - 1; i >= 0; i--)
            Destroy(itemContainer.GetChild(i).gameObject);

        entries.Clear();
    }
}