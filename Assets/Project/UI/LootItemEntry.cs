using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LootItemEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text itemLabel;
    [SerializeField] private Toggle checkToggle;

    public ItemInstance Item { get; private set; }
    public bool IsChecked => checkToggle != null && checkToggle.isOn;

    public void Initialize(ItemInstance item)
    {
        Item = item;

        if (checkToggle != null)
            checkToggle.isOn = false;

        if (itemLabel != null)
        {
            var def = ItemDatabase.Get(item.definitionId);
            string statSummary = "";

            if (def != null && def.statModifiers != null && def.statModifiers.Count > 0)
            {
                var parts = new System.Collections.Generic.List<string>();
                foreach (var mod in def.statModifiers)
                    parts.Add($"{mod.stat} {(mod.flatBonus >= 0 ? "+" : "")}{mod.flatBonus}");
                statSummary = $" ({string.Join(", ", parts)})";
            }

            itemLabel.text = $"{(def != null ? def.displayName : item.definitionId)}{statSummary}";
        }
    }

    public void SetChecked(bool value)
    {
        if (checkToggle != null)
            checkToggle.isOn = value;
    }
}