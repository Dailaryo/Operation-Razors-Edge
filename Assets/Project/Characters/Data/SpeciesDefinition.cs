using UnityEngine;
using System.Collections.Generic;

public class SpeciesDefinition
{
    //Identity
    public string speciesId;
    public string displayName;
    public string description;

    //Bonus feats (if applicable)
    public List<string> bonusFeatIds;
    public List<string> passiveIds;

    //Flat stat bonuses applied at creation
    public List<StatModifier> statBonuses;

    //Awareness checks against units of this species (every current genotype is Human)
    public KnowledgeCategory knowledgeCategory = KnowledgeCategory.Human;

    //Action and Effect sources
    public IActionSource actionSource;
    public IEffectSource effectSource;

    //Optional hooks for species-specific behavior at character creation / level-up
    public System.Action<CharacterData> onApply;
    public System.Action<CharacterData> onLevelUp;

    public SpeciesDefinition(
        string speciesId,
        string displayName,
        string description,
        List<string> bonusFeatIds = null,
        List<string> passiveIds = null,
        List<StatModifier> statBonuses = null,
        IActionSource actionSource = null,
        IEffectSource effectSource = null)
    {
        this.speciesId = speciesId;
        this.displayName = displayName;
        this.description = description;
        this.bonusFeatIds = bonusFeatIds ?? new List<string> ();
        this.passiveIds = passiveIds ?? new List<string> ();
        this.statBonuses = statBonuses ?? new List<StatModifier> ();
        this.actionSource = actionSource;
        this.effectSource = effectSource;
    }

    public virtual void Apply(CharacterData data)
    {
        foreach (var featId in bonusFeatIds)
        {
            if (!data.unlockedFeatIds.Contains(featId))
                data.unlockedFeatIds.Add(featId);
        }

        onApply?.Invoke(data);
    }

    // Called by CharacterData.LevelUp() after classDef.ApplyLevelUp().
    // Default invokes onLevelUp if set. Override in species with level-scaling traits.
    public virtual void ApplyLevelUp(CharacterData data)
    {
        onLevelUp?.Invoke(data);
    }

    //Sum of this species' modifiers to one core attribute (applied at the data level)
    public int GetAttributeBonus(StatType attribute)
    {
        int total = 0;
        foreach (var mod in statBonuses)
        {
            if (mod.stat == attribute)
                total += mod.flatBonus;
        }
        return total;
    }
}
