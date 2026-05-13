using UnityEngine;
using System.Collections.Generic;

public static class AugmentationProcessor
{
    public class AugmentationResult
    {
        public bool isValid;
        public int totalPowerPointCost;
        public List<PsionicPowerDefinition.AugmentationOption> appliedOptions;
        public string failureReason;
    }

    //d20 rule: total PP spent on a single power cannot exceed manifester level + 1
    public static int GetManifestationLimit(CharacterData data)
    {
        return data.manifesterLevel + 1;
    }

    public static AugmentationResult Validate(
        Unit manifester,
        PsionicPowerDefinition power,
        List<PsionicPowerDefinition.AugmentationOption> requestedOptions)
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

        //Calculate total cost
        int totalCost = power.basePowerPointCost;
        foreach (var option in requestedOptions)
        {
            totalCost += option.additionalPowerpointCost;
        }

        //Enforce manifester level cap
        int limit = GetManifestationLimit(manifester.sourceCharacterData);
        if (totalCost > limit)
        {
            result.isValid = false;
            result.failureReason = $"Total PP cost ({totalCost}) exceeds manifester limit ({limit})";
            return result;
        }

        //Validate power point availablility
        if (manifester.powerPoints == null || !manifester.powerPoints.CanAfford(totalCost))
        {
            result.isValid = false;
            result.failureReason = $"Insufficient power points " +
                $"({manifester.powerPoints?.current ?? 0}/{totalCost} needed)";
            return result;
        }

        result.isValid = true;
        result.totalPowerPointCost = totalCost;
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
