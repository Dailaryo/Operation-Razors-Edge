using UnityEngine;
using System.Collections.Generic;

public static class ItemModManager
{
    public static bool InstallMod(ItemInstance item, string modId, out string reason)
    {
        if (GameFlowManager.Instance == null ||
            GameFlowManager.Instance.currentState != GameFlowManager.GameState.Hub)
        {
            reason = "Item mods can only be changed at the Hub";
            return false;
        }

        var itemDef = ItemDatabase.Get(item.definitionId);
        if (itemDef == null)
        {
            reason = $"Item definition '{item.definitionId}' not found";
            return false;
        }

        var modDef = ItemModDatabase.Get(modId);
        if (modDef == null)
        {
            reason = $"Mod '{modId}' not found in ItemModDatabase";
            return false;
        }

        if (!modDef.IsCompatibleWith(itemDef))
        {
            reason = $"'{modDef.displayName}' is not compatible with '{itemDef.displayName}'.";
            return false;
        }

        if (item.installedModIds.Count >= itemDef.modSlotCount)
        {
            reason = $"'{itemDef.displayName}' has no available mod slots ({item.installedModIds.Count}/{itemDef.modSlotCount})";
            return false;
        }

        if (item.installedModIds.Contains(modId))
        {
            reason = $"'{modDef.displayName}' is already installed on this item";
            return false;
        }

        item.installedModIds.Add(modId);
        reason = null;
        return true;
    }

    public static bool RemoveMod(ItemInstance item, string modId, out string reason)
    {
        if (GameFlowManager.Instance == null ||
            GameFlowManager.Instance.currentState != GameFlowManager.GameState.Hub)
        {
            reason = "Item mods can only be changed at the Hub";
            return false;
        }

        if (!item.installedModIds.Contains(modId))
        {
            reason = $"Mod '{modId}' is not installed on this item";
            return false;
        }

        item.installedModIds.Remove(modId);
        reason = null;
        return true;
    }
}
