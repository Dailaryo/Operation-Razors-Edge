using UnityEngine;
using System.Collections.Generic;

public static class BackgroundDatabase
{
    public static Dictionary<string, BackgroundDefinition> Definitions =
        new Dictionary<string, BackgroundDefinition>()
        {
            { BackgroundIds.Academic, new BackgroundDefinition(
                backgroundId: BackgroundIds.Academic,
                displayName: "Academic",
                description: "Implanted memories drawn from years of institutional research and technical study. " +
                             "Academics apply theoretical knowledge under pressure and excel at synthesizing " +
                             "information from disparate sources.",
                bonusClassSkills: new List<SkillType>
                {
                    SkillType.ComputerUse,
                    SkillType.Investigate,
                    SkillType.Knowledge
                }
            )},
            { BackgroundIds.Gladiator, new BackgroundDefinition(
                backgroundId: BackgroundIds.Gladiator,
                displayName:  "Gladiator",
                description:  "Implanted memory construct encoding the skills, reflexes, and psychological conditioning of an " +
                              "arena fighter. The implant provides muscle memory and confidence; the reality-check comes from " +
                              "first contact.",
                bonusFeatIds: new List<string>
                {
                    FeatIds.Brawl
                },
                bonusClassSkills: new List<SkillType>
                {
                    SkillType.Intimidate,
                    SkillType.Athletics,
                    SkillType.SenseMotive
                }
            )},
        };

    public static BackgroundDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out BackgroundDefinition background)) return background;

        Debug.LogWarning($"[BackgroundDatabase] Background not found: {id}");
        return null;
    }
}
