using UnityEngine;
using System.Collections.Generic;

public class PsionicPowerDefinition : ActionDefinition
{
    public int powerLevel;
    public PsionicDiscipline discipline;
    public int basePowerPointCost;
    public StatType keyAbility;
    public bool concentrationRequired;
    public List<AugmentationOption> augmentationOptions;

    public class AugmentationOption
    {
        public int additionalPowerpointCost;
        public string effectDescription; // authored intent; processor reads this later

        public AugmentationOption(int additionalCost, string description)
        {
            additionalPowerpointCost = additionalCost;
            effectDescription = description;
        }
    }

    public PsionicPowerDefinition(
        //ActionDefinition parameters
        string actionId,
        TargetType targetType,
        bool requiresTarget,
        int maxRange,
        int minRange = 0,
        int cooldown = 0,
        bool dealsDamage = false,
        TargetingMode targetingMode = TargetingMode.None,
        ActionCategory category = ActionCategory.Standard,
        List<ActionCategory> consumesCategories = null,
        List<IActionEffect> effects = null,
        AttackType attackType = AttackType.None,
        StatType scalingStat = StatType.Power,
        float scalingMultiplier = 1f,
        int resourceCost = 0,
        bool requiresLoS = true,
        //Psionic specific parameters
        int powerLevel = 1,
        PsionicDiscipline discipline = PsionicDiscipline.Telepathy,
        int basePowerPointCost = 1,
        StatType keyAbility = StatType.Insight,
        bool concentrationRequired = false,
        bool provokesOA = true,
        List<AugmentationOption> augmentationOptions = null
    ) : base(
        actionId, targetType, requiresTarget, minRange, maxRange,
        cooldown, dealsDamage, targetingMode, category, consumesCategories,
        effects, attackType, scalingStat, scalingMultiplier, resourceCost, requiresLoS
    )
    {
        this.powerLevel = powerLevel;
        this.discipline = discipline;
        this.basePowerPointCost = basePowerPointCost;
        this.keyAbility = keyAbility;
        this.concentrationRequired = concentrationRequired;
        this.provokesOA = provokesOA;
        this.augmentationOptions = augmentationOptions ?? new List<AugmentationOption>();
    }

    public int GetSaveDC(Unit manifester)
    {
        int keyMod = manifester.GetModifier(keyAbility);
        return 10 + powerLevel + keyMod;
    }
}
