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
        };

    public static BackgroundDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out BackgroundDefinition background)) return background;

        Debug.LogWarning($"[BackgroundDatabase] Background not found: {id}");
        return null;
    }
}
