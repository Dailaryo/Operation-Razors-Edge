using UnityEngine;
using System.Collections.Generic;

//Items a class hands out at a level (Telepath Power Crystal). Bound to the character and created once:
//if the character already owns one (equipped, squad inventory or ship), nothing new is made, so a respec
//rebuild never duplicates it. Goes into the first free Consumable slot, otherwise squad inventory.
public static class ClassItemGrants
{
    public static void Grant(CharacterData data, string classId, string itemId)
    {
        if (FindOwned(data, itemId) != null) return;

        var item = ItemDatabase.Get(itemId);
        if (item == null) return;

        var instance = ItemInstanceFactory.CreateInstance(itemId);
        instance.ownerCharacterId = data.characterId;
        instance.grantedByClassId = classId;
        instance.charges = item.storedPowerPoints;

        for (int i = (int)EquipmentSlot.Consumable1; i <= (int)EquipmentSlot.Consumable5; i++)
        {
            var slot = (EquipmentSlot)i;
            if (data.equippedItemIds.ContainsKey(slot)) continue;

            if (data.TryEquipItem(slot, instance, out _))
            {
                CombatLog.Add($"{data.name} gains {item.displayName} ({slot}).");
                return;
            }
        }

        if (!data.isEnemy && SquadManager.Instance != null)
        {
            SquadManager.Instance.squadInventory.AddItem(instance);
            CombatLog.Add($"{data.name} gains {item.displayName} (consumable slots full: placed in squad inventory).");
        }
    }

    //The character's own copy of a class item, wherever it is
    public static ItemInstance FindOwned(CharacterData data, string itemId)
    {
        foreach (var inst in data.equippedItemIds.Values)
            if (IsOwnedCopy(data, inst, itemId)) return inst;

        if (SquadManager.Instance != null)
            foreach (var inst in SquadManager.Instance.squadInventory.GetAll())
                if (IsOwnedCopy(data, inst, itemId)) return inst;

        foreach (var inst in ShipInventory.GetAll())
            if (IsOwnedCopy(data, inst, itemId)) return inst;

        return null;
    }

    //True while the character's class still grants this item at their level (path gates respected)
    public static bool StillGranted(CharacterData data, ItemInstance instance)
    {
        if (data.classId != instance.grantedByClassId) return false;

        var classDef = ClassDatabase.Get(data.classId);
        if (classDef == null) return false;

        foreach (var kvp in classDef.levelTable)
        {
            if (kvp.Key > data.level) continue;

            foreach (var entry in kvp.Value)
            {
                if (entry.prerequisiteOptionId != null &&
                    !data.takenChoiceOptionIds.Contains(entry.prerequisiteOptionId))
                    continue;

                if (entry.grantedItemIds.Contains(instance.definitionId)) return true;
            }
        }

        return false;
    }

    //Respec: class items the rebuilt character no longer earns are lost, wherever they are
    public static void RemoveUnearned(CharacterData data)
    {
        var slots = new List<EquipmentSlot>();
        foreach (var kvp in data.equippedItemIds)
            if (IsUnearned(data, kvp.Value)) slots.Add(kvp.Key);

        foreach (var slot in slots)
        {
            CombatLog.Add($"[Respec] {data.name} loses {data.equippedItemIds[slot].definitionId}.");
            data.equippedItemIds.Remove(slot);
        }

        if (SquadManager.Instance != null)
            foreach (var inst in SquadManager.Instance.squadInventory.GetAll())
                if (IsUnearned(data, inst)) SquadManager.Instance.squadInventory.RemoveItem(inst);

        foreach (var inst in ShipInventory.GetAll())
            if (IsUnearned(data, inst)) ShipInventory.RemoveItem(inst);
    }

    private static bool IsOwnedCopy(CharacterData data, ItemInstance inst, string itemId)
    {
        return inst.definitionId == itemId && inst.ownerCharacterId == data.characterId;
    }

    private static bool IsUnearned(CharacterData data, ItemInstance inst)
    {
        return !string.IsNullOrEmpty(inst.grantedByClassId)
            && inst.ownerCharacterId == data.characterId
            && !StillGranted(data, inst);
    }
}
