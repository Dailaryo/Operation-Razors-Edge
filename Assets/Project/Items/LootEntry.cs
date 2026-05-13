using UnityEngine;

public enum LootEntryType { Item, Credstick }

public class LootEntry
{
    public LootEntryType entryType;
    public string itemDefinitionId;
    public int credstickValue;
    public float dropWeight;
    public bool guaranteed;
    public bool valueHidden;

    public LootEntry(
        LootEntryType entryType,
        string itemDefinitionId = null,
        int credstickValue = 0,
        float dropWeight = 1f,
        bool guaranteed = false,
        bool valueHidden = true)
    {
        this.entryType = entryType;
        this.itemDefinitionId = itemDefinitionId;
        this.credstickValue = credstickValue;
        this.dropWeight = dropWeight;
        this.guaranteed = guaranteed;
        this.valueHidden = valueHidden;
    }
}
