using UnityEngine;
using System.Collections.Generic;

public static class EquipmentValidator
{
    // Validates all equipped items against the character's current build.
    // Any items that fail validation are unequipped and deposited in squad inventory.
    // Returns a list of display names of items that were unequipped, for UI feedback.
    // NOTE: Prerequisite checking is stubbed — no items currently define prerequisites.
    // Flesh out IsItemValid() when ItemDefinition gains class/stat/feat requirement fields.
    public static List<string> ValidateAndUnequip(CharacterData data)
    {
        var unequipped = new List<string>();
        var slotsToUnequip = new List<EquipmentSlot>();

        foreach (var kvp in data.equippedItemIds)
        {
            if (!IsItemValid(data, kvp.Value))
                slotsToUnequip.Add(kvp.Key);
        }

        foreach (var slot in slotsToUnequip)
        {
            var instance = data.equippedItemIds[slot];
            var itemDef = ItemDatabase.Get(instance.definitionId);
            string displayName = itemDef != null ? itemDef.displayName : instance.definitionId;

            data.equippedItemIds.Remove(slot);
            SquadManager.Instance.squadInventory.AddItem(instance);

            unequipped.Add(displayName);
            CombatLog.Add($"[EquipmentValidator] {displayName} unequipped from " +
                          $"{data.name} — prerequisites no longer met.");
        }

        return unequipped;
    }

    //Stub: always returns true until ItemDefinition has prerequisite fields
    private static bool IsItemValid(CharacterData data, ItemInstance item)
    {
        // TODO: Check class restrictions, ability score minimums, feat prerequisites
        // once ItemDefinition gains those fields.
        return true;
    }
}
