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
        List<StatModifier> statBonuses = null,
        IEffectSource effectSource = null)
    {
        this.backgroundId = backgroundId;
        this.displayName = displayName;
        this.description = description;
        this.bonusFeatIds = bonusFeatIds ?? new List<string>();
        this.bonusClassSkills = bonusClassSkills ?? new List<SkillType>();
        this.statBonuses = statBonuses ?? new List<StatModifier>();
        this.effectSource = effectSource;
    }

    //Called once at character creation - mutates CharacterData only
    public void Apply(CharacterData data)
    {
        foreach (var featId in bonusFeatIds)
        {
            if (!data.unlockedFeatIds.Contains(featId))
                data.unlockedFeatIds.Add(featId);
        }

        foreach (var skill in bonusClassSkills)
        {
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
