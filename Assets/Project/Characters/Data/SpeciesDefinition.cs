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

    //Action and Effect sources
    public IActionSource actionSource;
    public IEffectSource effectSource;

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

    public void Apply(CharacterData data)
    {
        foreach (var featId in bonusFeatIds)
        {
            if (!data.unlockedFeatIds.Contains(featId))
                data.unlockedFeatIds.Add(featId);
        }
    }
}
