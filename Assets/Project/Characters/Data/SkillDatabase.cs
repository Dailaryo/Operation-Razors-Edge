using System;
using System.Collections.Generic;
using UnityEngine;

public static class SkillDatabase
{
    public static Dictionary<string, SkillDefinition> Definitions =
        new Dictionary<string, SkillDefinition>()
        {
            //Physical
            {SkillIds.Athletics, new SkillDefinition(
                skillId: SkillIds.Athletics,
                displayName: "Athletics",
                description: "Jumping, Swimming, Climbing, any physical activity involving physical power",
                govAtt: StatType.Power,
                trained: false
            )},
            {SkillIds.Acrobatics, new SkillDefinition(
                skillId: SkillIds.Acrobatics,
                displayName: "Acrobatics",
                description: "Tumbling, Balance, any physical activity requiring agility",
                govAtt: StatType.Agility,
                trained: false
            )},
            {SkillIds.Stealth, new SkillDefinition(
                skillId: SkillIds.Stealth,
                displayName: "Stealth",
                description: "The art of remaining unseen and unheard",
                govAtt: StatType.Agility,
                trained: false
            )},

            //Awareness
            {SkillIds.Perception, new SkillDefinition(
                skillId: SkillIds.Perception,
                displayName: "Perception",
                description: "The ability to notice and be aware of the world around you",
                govAtt: StatType.Insight,
                trained: false
            )},

            //Technical
            {SkillIds.ComputerUse, new SkillDefinition(
                skillId: SkillIds.ComputerUse,
                displayName: "Computer Use",
                description: "The art of using computers to find information or override security",
                govAtt: StatType.Cognition,
                trained: true
            )},
            {SkillIds.Demolitions, new SkillDefinition(
                skillId: SkillIds.Demolitions,
                displayName: "Demolitions",
                description: "The ability to use explosives without blowing off fingers or limbs",
                govAtt: StatType.Cognition,
                trained: true
            )},
            {SkillIds.DisableDevice, new SkillDefinition(
                skillId: SkillIds.DisableDevice,
                displayName: "Disable Device",
                description: "The art of rendering mechanical objects inert",
                govAtt: StatType.Cognition,
                trained: true
            )},
            {SkillIds.Repair, new SkillDefinition(
                skillId: SkillIds.Repair,
                displayName: "Repair",
                description: "The ability to return broken machines to functionality",
                govAtt: StatType.Cognition,
                trained: true
            )},
            {SkillIds.Navigate, new SkillDefinition(
                skillId: SkillIds.Navigate,
                displayName: "Navigate",
                description: "The art of plotting a course, whether through waves or stars",
                govAtt: StatType.Cognition,
                trained: false
            )},
            {SkillIds.Knowledge, new SkillDefinition(
                skillId: SkillIds.Knowledge,
                displayName: "Knowledge",
                description: "The ability to recall information about a particular subject",
                govAtt: StatType.Cognition,
                trained: true
            )},
            {SkillIds.Investigate, new SkillDefinition(
                skillId: SkillIds.Investigate,
                displayName: "Investigate",
                description: "The art of studying an object or space to glean more information from it",
                govAtt: StatType.Cognition,
                trained: true
            )},
            {SkillIds.Xenology, new SkillDefinition(
                skillId: SkillIds.Xenology,
                displayName: "Xenology",
                description: "The ability to identify and catalog alien life",
                govAtt: StatType.Cognition,
                trained: true
            )},

            //Medical
            {SkillIds.TreatInjury, new SkillDefinition(
                skillId: SkillIds.TreatInjury,
                displayName: "Treat Injury",
                description: "The art of mending wounds and healing",
                govAtt: StatType.Insight,
                trained: true
            )},
            {SkillIds.Survival, new SkillDefinition(
                skillId: SkillIds.Survival,
                displayName: "Survival",
                description: "The ability to 'live off the land'",
                govAtt: StatType.Insight,
                trained: false
            )},

            //Social
            {SkillIds.SenseMotive, new SkillDefinition(
                skillId: SkillIds.SenseMotive,
                displayName: "Sense Motive",
                description: "The art of reading people",
                govAtt: StatType.Insight,
                trained: false
            )},
            {SkillIds.Bluff, new SkillDefinition(
                skillId: SkillIds.Bluff,
                displayName: "Bluff",
                description: "The ability to deceive, whether through words or disguises",
                govAtt: StatType.Presence,
                trained: false
            )},
            {SkillIds.Diplomacy, new SkillDefinition(
                skillId: SkillIds.Diplomacy,
                displayName: "Diplomacy",
                description: "The art of using words to persuade or gather information from a crowd",
                govAtt: StatType.Presence,
                trained: false
            )},
            {SkillIds.Intimidate, new SkillDefinition(
                skillId: SkillIds.Intimidate,
                displayName: "Intimidate",
                description: "The ability to use fear as a social weapon",
                govAtt: StatType.Presence,
                trained: false
            )},

            //Other
            {SkillIds.ZeroGOperations, new SkillDefinition(
                skillId: SkillIds.ZeroGOperations,
                displayName: "Zero-G Operations",
                description: "The art of movement and surviving in free-fall or microgravity environments",
                govAtt: StatType.Agility,
                trained: true
            )}
        };

    public static SkillDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out SkillDefinition skill)) return skill;

        Debug.LogWarning($"[SkillDatabase] Skill not found: {id}");
        return null;
    }
}
