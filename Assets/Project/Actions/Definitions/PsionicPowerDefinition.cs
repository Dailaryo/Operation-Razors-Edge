using UnityEngine;
using System.Collections.Generic;

public class PsionicPowerDefinition : ActionDefinition
{
    public int powerLevel;
    public PsionicDiscipline discipline;
    public int basePowerPointCost;
    public StatType keyAbility;
    public bool concentrationRequired;   // Maintained via ConcentrationEffect on the manifester.
                                         // Author applied effects with duration 0: they last exactly
                                         // as long as concentration.
                                         // Caps ("up to N rounds/level") use concentrationMaxRounds.
    public List<AugmentationOption> augmentationOptions;

    //d20 range bands in 5-ft tiles, scaled by manifester level
    public enum PowerRange { Fixed, Personal, Touch, Close, Medium, Long }
    public Unit.SaveType? saveType;          // null = no save
    public bool allowsPowerResistance;
    public EffectDescriptor descriptors;
    public string tierGroupId;   // null = not tiered. All tiers of one power share a group (e.g. "MindThrust")
    public int tier;             // 1 = I, 2 = II, ... Each tier is its own PowerDatabase entry
    public string materialItemId;  // Material component: ItemIds entry. Consumed from squad inventory if
                                   // present, otherwise paid as a flat credit cost equal to its baseValue
    public int creditCost;         // Converted XP costs: flat credit cost, no item. Ignored when materialItemId is set
    public PowerRange rangeBand;     // Fixed = use maxRange as authored
    public System.Func<int, int> concentrationMaxRounds; // ML -> max rounds while concentrating; null = no cap
    public List<string> variantPowerIds;                 // Other manifestation modes (own entries). Usable whenever this
                                                         // power is known; never learned on their own (not on class lists)

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
        List<AugmentationOption> augmentationOptions = null,
        Unit.SaveType? saveType = null,
        bool allowsPowerResistance = false,
        EffectDescriptor descriptors = EffectDescriptor.None,
        string tierGroupId = null,
        int tier = 0,
        string materialItemId = null,
        int creditCost = 0,
        PowerRange rangeBand = PowerRange.Fixed,
        System.Func<int, int> concentrationMaxRounds = null,
        List<string> variantPowerIds = null
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
        this.saveType = saveType;
        this.allowsPowerResistance = allowsPowerResistance;
        this.descriptors = descriptors;
        this.tierGroupId = tierGroupId;
        this.tier = tier;
        this.materialItemId = materialItemId;
        this.creditCost = creditCost;
        this.rangeBand = rangeBand;
        this.concentrationMaxRounds = concentrationMaxRounds;
        this.variantPowerIds = variantPowerIds ?? new List<string>();
    }

    public int GetSaveDC(Unit manifester)
    {
        int keyMod = manifester.GetModifier(GetKeyAbility(manifester));
        return 10 + powerLevel + keyMod;
    }

    public StatType GetKeyAbility(Unit manifester)
    {
        if (manifester.sourceCharacterData != null)
        {
            var classDef = ClassDatabase.Get(manifester.sourceCharacterData.classId);
            if (classDef != null && classDef.isPsionicClass)
                return classDef.psionicKeyAbility;
        }
        return keyAbility; // fallback for classless manifesters (enemies)
    }

    public bool IsTiered => !string.IsNullOrEmpty(tierGroupId);

    //True if this is a lower tier of the same tiered power as 'other'
    public bool IsLowerTierOf(PsionicPowerDefinition other)
    {
        return IsTiered && other != null
            && other.tierGroupId == tierGroupId
            && tier < other.tier;
    }

    //Credit value of the material component: the authored item's baseValue, or the flat
    //creditCost for converted XP costs. 0 = no material cost
    public int GetMaterialCreditCost()
    {
        if (!string.IsNullOrEmpty(materialItemId))
        {
            var item = ItemDatabase.Get(materialItemId);
            return item != null ? item.baseValue : 0;
        }
        return creditCost;
    }

    public override int GetMaxRange(Unit source)
    {
        int ml = GetManifesterLevel(source);

        switch (rangeBand)
        {
            case PowerRange.Personal: return 0;
            case PowerRange.Touch: return 1;
            case PowerRange.Close: return 5 + ml / 2;   // 25 ft + 5 ft per 2 levels
            case PowerRange.Medium: return 20 + 2 * ml;  // 100 ft + 10 ft per level
            case PowerRange.Long: return 80 + 8 * ml;  // 400 ft + 40 ft per level
            default: return maxRange;
        }
    }

    public int GetConcentrationMaxRounds(Unit manifester)
    {
        return concentrationMaxRounds != null ? concentrationMaxRounds(GetManifesterLevel(manifester)) : 0;
    }

    public static int GetManifesterLevel(Unit unit)
    {
        return Mathf.Max(1, unit != null ? unit.GetEffectiveManifesterLevel() : 1);
    }
}
