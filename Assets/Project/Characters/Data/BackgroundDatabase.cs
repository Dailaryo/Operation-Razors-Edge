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
                },
                bonusSkillChoices: 3
            )},
            { BackgroundIds.Gladiator, new BackgroundDefinition(
                backgroundId: BackgroundIds.Gladiator,
                displayName:  "Gladiator",
                description:  "Implanted memory construct encoding the skills, reflexes, and psychological conditioning of an " +
                              "arena fighter. The implant provides muscle memory and confidence; the reality-check comes from " +
                              "first contact.",
                bonusFeatIds: new List<string>
                {
                    FeatIds.Brawl,
                    FeatIds.CombatMartialArts
                },
                bonusClassSkills: new List<SkillType>
                {
                    SkillType.Intimidate,
                    SkillType.Athletics,
                    SkillType.SenseMotive
                },
                bonusSkillChoices: 1
            )},
            { BackgroundIds.Criminal, new BackgroundDefinition(
                backgroundId: BackgroundIds.Criminal,
                displayName:  "Criminal",
                description:  "Implanted memory construct encoding the operational habits, reflexes, and tradecraft of a " +
                              "career criminal. The instincts are genuine; the history is fabricated. Covers larceny, " +
                              "surveillance evasion, and the baseline social engineering that keeps you out of custody.",
                bonusFeatIds: new List<string>
                {
                    FeatIds.PersonalFirearmsProficiency
                },
                bonusClassSkills: new List<SkillType>
                {
                    SkillType.Stealth,
                    SkillType.DisableDevice,
                    SkillType.Bluff,
                    SkillType.Knowledge
                },
                bonusSkillChoices: 2
            )},
            { BackgroundIds.Military, new BackgroundDefinition(
                backgroundId: BackgroundIds.Military,
                displayName:  "Military",
                description:  "Implanted memory construct encoding the training, tactical vocabulary, and chain-of-command " +
                              "instincts of a career soldier. The discipline is genuine; the service record is fabricated. " +
                              "Unit-cohesion reflexes emerged from the implant as a side effect — whether by design or " +
                              "accident depends on who built it.",
                bonusFeatIds: new List<string>
                {
                    FeatIds.Brawl,
                    FeatIds.CombatMartialArts,
                    FeatIds.ArmorProficiencyLight,
                    FeatIds.PersonalFirearmsProficiency
                },
                bonusClassSkills: new List<SkillType>
                {
                    SkillType.Athletics,
                    SkillType.Demolitions,
                    SkillType.Stealth,
                    SkillType.Knowledge,
                    SkillType.Navigate,
                    SkillType.Survival
                },
                bonusSkillChoices: 2
            )},
            { BackgroundIds.Doctor, new BackgroundDefinition(
                backgroundId: BackgroundIds.Doctor,
                displayName:  "Doctor",
                description:  "Implanted memory construct encoding the expertise, reflexes, and clinical judgment of a " +
                              "practicing physician. The medical skills are genuine and deeply integrated. " +
                              "The residency, the patients, the years of training — fabricated.",
                bonusClassSkills: new List<SkillType>
                {
                    SkillType.ComputerUse,
                    SkillType.Knowledge,
                    SkillType.Investigate,
                    SkillType.TreatInjury,
                },
                bonusSkillChoices: 2
            )},
        };

    public static BackgroundDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out BackgroundDefinition background)) return background;

        Debug.LogWarning($"[BackgroundDatabase] Background not found: {id}");
        return null;
    }
}
