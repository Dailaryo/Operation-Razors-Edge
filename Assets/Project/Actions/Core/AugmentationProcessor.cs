using UnityEngine;
using System.Collections.Generic;

public static class AugmentationProcessor
{
    public class AugmentationResult
    {
        public bool isValid;
        public int totalPowerPointCost;
        public bool usesFreeZeroLevel;
        public List<PsionicPowerDefinition.AugmentationOption> appliedOptions;
        public string failureReason;
    }

    public const int MaximizePowerCost = 6;

    //3.5 rule: total PP spent on a single power cannot exceed (effective) manifester level
    public static int GetManifestationLimit(Unit manifester)
    {
        return manifester.GetEffectiveManifesterLevel();
    }

    //d20 Modern: 0-level powers cost nothing while free uses remain (3 + manifester level per rest)
    //committedFreeUses = free uses already claimed by earlier planned actions this turn
    public static bool UsesFreeZeroLevel(Unit manifester, PsionicPowerDefinition power, int committedFreeUses = 0)
    {
        if (power.powerLevel != 0) return false;
        if (manifester.powerPoints == null) return false;
        return manifester.powerPoints.FreeZeroLevelRemaining - committedFreeUses > 0;
    }

    public static bool RequiresCredits(Unit manifester, PsionicPowerDefinition power)
    {
        return power.creditCost > 0 && !manifester.isEnemy;
    }

    public static int GetTotalCost(
        PsionicPowerDefinition power,
        List<PsionicPowerDefinition.AugmentationOption> options,
        bool usesFreeZeroLevel, bool maximized = false)
    {
        int total = usesFreeZeroLevel ? 0 : power.basePowerPointCost;

        if (options != null)
        {
            foreach (var option in options)
                total += option.additionalPowerpointCost;
        }

        if (maximized) total += MaximizePowerCost;

        return total;
    }

    public static AugmentationResult Validate(
        Unit manifester,
        PsionicPowerDefinition power,
        List<PsionicPowerDefinition.AugmentationOption> requestedOptions,
        int committedPowerPoints = 0,
        int committedFreeUses = 0,
        bool maximized = false)
    {
        var result = new AugmentationResult
        {
            appliedOptions = new List<PsionicPowerDefinition.AugmentationOption>()
        };

        if (manifester.sourceCharacterData == null)
        {
            result.isValid = false;
            result.failureReason = "No character data on manifester";
            return result;
        }

        if (manifester.powerPoints == null)
        {
            result.isValid = false;
            result.failureReason = "Manifester has no power point pool";
            return result;
        }

        bool freeUse = UsesFreeZeroLevel(manifester, power, committedFreeUses);
        int totalCost = GetTotalCost(power, requestedOptions, freeUse, maximized);

        //Enforce manifester level cap
        int limit = GetManifestationLimit(manifester);
        //House rule: Maximize Power's +6 doesn't count toward the manifestation limit (3.5 says it does)
        if (totalCost - (maximized ? MaximizePowerCost : 0) > limit)
        {
            result.isValid = false;
            result.failureReason = $"Total PP cost ({totalCost}) exceeds manifester limit ({limit})";
            return result;
        }

        //Validate power point availablility
        if (!manifester.powerPoints.CanAfford(totalCost + committedPowerPoints))
        {
            result.isValid = false;
            result.failureReason = $"Insufficient power points " +
                $"({manifester.powerPoints.current} available, {committedPowerPoints} committed, {totalCost} needed)";
            return result;
        }

        //Material component: credit pool
        if (RequiresCredits(manifester, power))
        {
            var balance = SquadManager.Instance != null ? SquadManager.Instance.creditBalance : null;
            if (balance == null || !balance.CanAfford(power.creditCost))
            {
                result.isValid = false;
                result.failureReason = $"Insufficient credits for material component " +
                    $"({(balance != null ? balance.credits : 0)} available, {power.creditCost} needed)";
                return result;
            }
        }

        result.isValid = true;
        result.totalPowerPointCost = totalCost;
        //A maximized 0-level power never spends a free use: with one available it costs only Maximize's +6 PP
        result.usesFreeZeroLevel = freeUse && !maximized;
        if (requestedOptions != null)
            result.appliedOptions.AddRange(requestedOptions);
        return result;
    }

    //Convenience: no augmentation, just validates and returns base cost
    public static AugmentationResult ValidateBase(Unit manifester, PsionicPowerDefinition power)
    {
        return Validate(manifester, power,
            new List<PsionicPowerDefinition.AugmentationOption>());
    }
}
