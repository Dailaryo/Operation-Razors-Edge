using UnityEngine;
using System.Collections.Generic;

public class ShipInventoryPanel : MonoBehaviour
{
    //Called by HubUIController.Show() and after any inventory change
    public void Refresh()
    {
        var items = ShipInventory.GetAll();
        DisplayItems(items);
    }

    void DisplayItems(List<ItemInstance> itemIds)
    {
        // TODO Milestone 27: Populate item list UI elements
        // Each entry should show item display name and wire the select callback below
    }

    //Transfer item from ship inventory to squad inventory
    //Wire to each item's select button in Milestone 27
    public void OnTransferToSquad(ItemInstance itemId)
    {
        if (!SquadManager.Instance.squadInventory.CanCarry(itemId))
        {
            CombatLog.Add("[ShipInventoryPanel] Not enough squad carry capacity");
            return;
        }

        if (ShipInventory.RemoveItem(itemId))
        {
            SquadManager.Instance.squadInventory.AddItem(itemId);
            Refresh();
        }
    }

    //Transfer item from squad inventory to ship inventory
    //Wire to deposit button in Milestone 27
    public void OnDepositToShip(ItemInstance itemId)
    {
        if (SquadManager.Instance.squadInventory.RemoveItem(itemId))
        {
            ShipInventory.AddItem(itemId);
            Refresh();
        }
    }
}
