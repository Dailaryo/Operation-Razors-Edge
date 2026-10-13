using UnityEngine;
using System.Collections.Generic;

public static class PowerProgression
{
    public const int PowerLevelCount = 10; //0-level through 9th

    //Powers known allowed per power level: Powers Discovered row + path bonus slots.
    //Recalculated from data every time (nothing stored), so respec and path choices stay in sync.
    public static int[] GetAllowance(CharacterData data)
    {
        var allowance = new int[PowerLevelCount];

        var classDef = ClassDatabase.Get(data.classId);
        if (classDef == null || !classDef.isPsionicClass) return allowance;

        if (classDef.powersDiscovered != null &&
            classDef.powersDiscovered.TryGetValue(data.level, out int[] row))
        {
            for (int i = 0; i < row.Length && i < PowerLevelCount; i++)
                allowance[i] += row[i];
        }

        for (int lvl = 1; lvl <= data.level; lvl++)
        {
            if (!classDef.levelTable.TryGetValue(lvl, out var entries)) continue;

            foreach (var entry in entries)
            {
                if (entry.prerequisiteOptionId != null &&
                    !data.takenChoiceOptionIds.Contains(entry.prerequisiteOptionId))
                    continue;

                foreach (int slotLevel in entry.bonusPowerSlots)
                    if (slotLevel >= 0 && slotLevel < PowerLevelCount)
                        allowance[slotLevel]++;
            }
        }

        return allowance;
    }

    public static int[] GetKnownCounts(CharacterData data)
    {
        var counts = new int[PowerLevelCount];

        foreach (var id in data.knownPowerIds)
        {
            var power = PowerDatabase.Get(id);
            if (power != null && power.powerLevel >= 0 && power.powerLevel < PowerLevelCount)
                counts[power.powerLevel]++;
        }

        return counts;
    }

    //Everything a known power makes usable: itself, its variant modes, and (undercasting) every lower tier
    public static List<PsionicPowerDefinition> GetManifestable(PsionicPowerDefinition known)
    {
        var result = new List<PsionicPowerDefinition> { known };

        foreach (var variantId in known.variantPowerIds)
        {
            var variant = PowerDatabase.Get(variantId);
            if (variant != null) result.Add(variant);
        }

        if (!known.IsTiered) return result;

        var tiers = new List<PsionicPowerDefinition>();
        foreach (var power in PowerDatabase.Definitions.Values)
            if (power.IsLowerTierOf(known))
                tiers.Add(power);

        tiers.Sort((a, b) => a.tier.CompareTo(b.tier));
        result.AddRange(tiers);
        return result;
    }

    public static bool HasPendingPicks(CharacterData data)
    {
        var allowance = GetAllowance(data);
        var known = GetKnownCounts(data);

        for (int i = 0; i < PowerLevelCount; i++)
            if (known[i] < allowance[i]) return true;

        return false;
    }

    //Shared Validation: used by the pregen plan applier now and the level-up UI later
    public static bool CanLearn(CharacterData data, string powerId, out string reason)
    {
        reason = null;

        var power = PowerDatabase.Get(powerId);
        if (power == null) { reason = "power not found"; return false; }

        var classDef = ClassDatabase.Get(data.classId);
        if (classDef == null || !classDef.powerListIds.Contains(powerId))
        { reason = "not on class power list"; return false; }
        
        if (data.knownPowerIds.Contains(powerId)) { reason = "already known"; return false; }

        //A known higher tier already grants this one
        foreach (var knownId in data.knownPowerIds)
        {
            var known = PowerDatabase.Get(knownId);
            if (power.IsLowerTierOf(known))
            { reason = $"granted by known {known.actionID}"; return false; }
        }

        if (power.powerLevel < 0 || power.powerLevel >= PowerLevelCount)
        { reason = $"invalid power level {power.powerLevel}"; return false; }

        if (GetKnownCounts(data)[power.powerLevel] >= GetAllowance(data)[power.powerLevel])
        { reason = $"no open level-{power.powerLevel} slot"; return false; }

        return true;
    }

    public static bool TryLearn(CharacterData data, string powerId, out string reason)
    {
        if (!CanLearn(data, powerId, out reason)) return false;

        var power = PowerDatabase.Get(powerId);

        //Undercasting replacement: lower tiers are now redundant, so their slots open for new picks
        data.knownPowerIds.RemoveAll(id => PowerDatabase.Get(id)?.IsLowerTierOf(power) == true);

        data.knownPowerIds.Add(powerId);
        return true;
    }
}
