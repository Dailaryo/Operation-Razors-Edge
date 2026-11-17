using UnityEngine;
using System.Collections.Generic;

public class UseConsumableEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        //Planning simulations never consume items (and have no item choice to read)
        if (resolver.IsSimulation) return;

        if (ctx.attacker == null) return;

        if (string.IsNullOrEmpty(ctx.consumableItemId))
        {
            Debug.LogWarning("UseConsumableEffect: no consumableItemId set on context");
            return;
        }

        var item = ItemDatabase.Get(ctx.consumableItemId);
        if (item == null) return;

        if (item.itemType != ItemType.Consumable)
        {
            Debug.LogWarning($"UseConsumableEffect: item '{item.displayName}' is not a consumable");
            return;
        }

        // Find which consumable slot holds this item
        var charData = ctx.attacker.sourceCharacterData;

        if (charData == null)
        {
            Debug.LogWarning($"UseConsumableEffect: {ctx.attacker.unitName} has no CharacterData");
            return;
        }

        EquipmentSlot? foundSlot = null;

        for (int i = (int)EquipmentSlot.Consumable1; i <= (int)EquipmentSlot.Consumable5; i++)
        {
            EquipmentSlot slot = (EquipmentSlot)i;
            if (charData.equippedItemIds.TryGetValue(slot, out ItemInstance slotItemId)
                && slotItemId.definitionId == ctx.consumableItemId)
            {
                foundSlot = slot;
                break;
            }
        }

        if (foundSlot == null)
        {
            Debug.LogWarning($"UseConsumableEffect: {ctx.attacker.unitName} does not have '{item.displayName}' in any consumable slot");
            return;
        }

        // Apply the item's effects
        if (item.effectSource != null)
        {
            foreach (var effect in item.effectSource.GetEffects(ctx.attacker))
            {
                ctx.attacker.AddEffect(effect);
            }
        }

        // Remove from the slot — item is consumed, goes nowhere
        charData.equippedItemIds.Remove(foundSlot.Value);

        CombatLog.Add($"{ctx.attacker.unitName} uses {item.displayName}");
    }
}
