using UnityEngine;
using System.Collections.Generic;

public class BackgroundDefinition
{
    //Identity
    public string backgroundId;
    public string displayName;
    public string description;

    //Bonus feats granted at creation
    public List<string> bonusFeatIds;

    public List<SkillType> bonusClassSkills;

    // Number of skills the player may choose from bonusClassSkills (-1 sentinel = full list)
    public int bonusSkillChoices;

    //Flat stat bonuses applied at creation
    public List<StatModifier> statBonuses;

    //Passive effect source - null if background has no ongoing effect
    public IEffectSource effectSource;

    public BackgroundDefinition(
        string backgroundId,
        string displayName,
        string description,
        List<string> bonusFeatIds = null,
        List<SkillType> bonusClassSkills = null,
        int bonusSkillChoices = -1,
        List<StatModifier> statBonuses = null,
        IEffectSource effectSource = null)
    {
        this.backgroundId = backgroundId;
        this.displayName = displayName;
        this.description = description;
        this.bonusFeatIds = bonusFeatIds ?? new List<string>();
        this.bonusClassSkills = bonusClassSkills ?? new List<SkillType>();
        this.bonusSkillChoices = bonusSkillChoices < 0
            ? this.bonusClassSkills.Count
            : bonusSkillChoices;
        this.statBonuses = statBonuses ?? new List<StatModifier>();
        this.effectSource = effectSource;
    }

    //Called once at character creation - mutates CharacterData only
    //Applies the player's chosen feat (backgroundFeatChoice) and chosen skills
    //(backgroundSkillChoices), rather than granting every option unconditionally.
    public void Apply(CharacterData data)
    {
        // Apply chosen feat (always exactly 1 if any feat options exist)
        if (!string.IsNullOrEmpty(data.backgroundFeatChoice))
        {
            if (!data.unlockedFeatIds.Contains(data.backgroundFeatChoice))
                data.unlockedFeatIds.Add(data.backgroundFeatChoice);
        }

        // Apply chosen skills (up to bonusSkillChoices from bonusClassSkills)
        foreach (var skill in data.backgroundSkillChoices)
        {
            if (!bonusClassSkills.Contains(skill)) continue; // guard against invalid choices

            if (data.classSkills.Contains(skill))
            {
                data.backgroundSkillBonuses.Add(new StatModifier(
                    SkillResolver.GetSkillStatType(skill),
                    1,
                    1f,
                    backgroundId + "_" + skill,
                    StatModifier.BonusType.Competence));
            }
            else
            {
                data.classSkills.Add(skill);
            }
        }
    }
}
