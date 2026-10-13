using UnityEngine;
using System.Collections.Generic;
using static ClassDefinition;

public static class InfiltratorClass
{
    public static ClassDefinition Create()
    {
        var def = new ClassDefinition(
            classId:          ClassIds.Infiltrator,
            diplayName:       "Infiltrator",
            statBias:         StatType.Agility,
            babProgression:   BABProgression.Half,
            hpPerLevel:       8,
            skillRanksPerLevel: 7,
            resolveSave:      SaveProgression.Poor,
            responseSave:     SaveProgression.Good,
            focusSave:        SaveProgression.Poor,
            featCadence:      new List<int> { 3, 6, 9, 13, 16, 19 },
            classSkills: new List<SkillType>
            {
                SkillType.Acrobatics,
                SkillType.Athletics,
                SkillType.Bluff,
                SkillType.DisableDevice,
                SkillType.Investigate,
                SkillType.Knowledge,
                SkillType.Perception,
                SkillType.SenseMotive,
                SkillType.Stealth,
            },
            levelTable: new Dictionary<int, List<ClassLevelEntry>>
            {
                // ── TIER 1 (LEVELS 1–10) ────────────────────────────────

                { 1, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        actionUnlockIds: new List<string> { ActionIds.RangedAttack },
                        passiveIds:      new List<string> { PassiveIds.InfiltratorSweep })
                }},

                { 2, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.InfiltratorImprovised })
                }},

                { 4, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.InfiltratorImprovedEvasion })
                }},

                { 5, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds:  new List<string> { PassiveIds.InfiltratorSkillMastery },
                        choicNodeId: ChoiceNodeIds.InfiltratorSkillMasteryNode)
                }},

                { 7, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.InfiltratorImprovisedDmg })
                }},

                { 8, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.InfiltratorImprovedSweep })
                }},

                { 10, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds:  new List<string> { PassiveIds.InfiltratorWithoutATrace },
                        choicNodeId: ChoiceNodeIds.InfiltratorPath)
                }},

                // ── TIER 2 (LEVELS 11–20) — ALL THREE PATHS ─────────────

                { 11, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ghost
                        passiveIds: new List<string> { PassiveIds.InfiltratorGhostShadowStep },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorGhost),

                    new ClassLevelEntry(                          // Operative
                        actionUnlockIds: new List<string> { ActionIds.InfiltratorExposeWeakness },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorOperative),

                    new ClassLevelEntry(                          // Phantom
                        actionUnlockIds: new List<string> { ActionIds.InfiltratorPsionicFeint },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorPhantom),
                }},

                { 12, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ghost
                        passiveIds: new List<string> { PassiveIds.InfiltratorGhostDeathBlow },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorGhost),

                    new ClassLevelEntry(                          // Operative
                        passiveIds: new List<string> { PassiveIds.InfiltratorOperativeReadTheRoom },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorOperative),

                    new ClassLevelEntry(                          // Phantom
                        passiveIds: new List<string> { PassiveIds.InfiltratorPhantomBlurredPresence },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorPhantom),
                }},

                { 13, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ghost
                        actionUnlockIds: new List<string> { ActionIds.InfiltratorVanish },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorGhost),

                    new ClassLevelEntry(                          // Operative
                        actionUnlockIds: new List<string> { ActionIds.InfiltratorDisablingStrike },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorOperative),

                    new ClassLevelEntry(                          // Phantom
                        actionUnlockIds: new List<string> { ActionIds.InfiltratorMentalCloak },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorPhantom),
                }},

                { 14, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ghost
                        passiveIds: new List<string> { PassiveIds.InfiltratorGhostShadowStepII },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorGhost),

                    new ClassLevelEntry(                          // Operative
                        passiveIds: new List<string> { PassiveIds.InfiltratorOperativeExploitOpening },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorOperative),

                    new ClassLevelEntry(                          // Phantom
                        passiveIds: new List<string> { PassiveIds.InfiltratorPhantomEchoStep },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorPhantom),
                }},

                { 15, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ghost
                        passiveIds: new List<string> { PassiveIds.InfiltratorGhostPerfectStrike },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorGhost),

                    new ClassLevelEntry(                          // Operative
                        passiveIds: new List<string> { PassiveIds.InfiltratorOperativeTacticalBlind },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorOperative),

                    new ClassLevelEntry(                          // Phantom
                        passiveIds: new List<string> { PassiveIds.InfiltratorPhantomMindFog },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorPhantom),
                }},

                { 16, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ghost
                        actionUnlockIds: new List<string> { ActionIds.InfiltratorSpectralForm },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorGhost),

                    new ClassLevelEntry(                          // Operative
                        actionUnlockIds: new List<string> { ActionIds.InfiltratorChokePoint },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorOperative),

                    new ClassLevelEntry(                          // Phantom
                        actionUnlockIds: new List<string> { ActionIds.InfiltratorPsychicDisplacement },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorPhantom),
                }},

                { 17, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ghost
                        passiveIds: new List<string> { PassiveIds.InfiltratorGhostAssassination },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorGhost),

                    new ClassLevelEntry(                          // Operative
                        passiveIds: new List<string> { PassiveIds.InfiltratorOperativeSurgicalPrecision },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorOperative),

                    new ClassLevelEntry(                          // Phantom
                        passiveIds: new List<string> { PassiveIds.InfiltratorPhantomGhostwalk },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorPhantom),
                }},

                { 18, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ghost
                        passiveIds: new List<string> { PassiveIds.InfiltratorGhostShadowWalk },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorGhost),

                    new ClassLevelEntry(                          // Operative
                        passiveIds: new List<string> { PassiveIds.InfiltratorOperativeControlTempo },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorOperative),

                    new ClassLevelEntry(                          // Phantom
                        passiveIds: new List<string> { PassiveIds.InfiltratorPhantomStrike },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorPhantom),
                }},

                { 19, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ghost
                        actionUnlockIds: new List<string> { ActionIds.InfiltratorDeathFromShadows },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorGhost),

                    new ClassLevelEntry(                          // Operative
                        actionUnlockIds: new List<string> { ActionIds.InfiltratorCalculatedTakedown },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorOperative),

                    new ClassLevelEntry(                          // Phantom
                        actionUnlockIds: new List<string> { ActionIds.InfiltratorPhaseOut },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorPhantom),
                }},

                { 20, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ghost
                        passiveIds: new List<string> { PassiveIds.InfiltratorGhostApotheosis },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorGhost),

                    new ClassLevelEntry(                          // Operative
                        passiveIds: new List<string> { PassiveIds.InfiltratorOperativeApotheosis },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorOperative),

                    new ClassLevelEntry(                          // Phantom
                        passiveIds: new List<string> { PassiveIds.InfiltratorPhantomApotheosis },
                        prerequisiteOptionId: ChoiceNodeIds.InfiltratorPhantom),
                }},
            }
        );

        // Bonus feats (d20 Modern Infiltrator list, unchanged)
        def.bonusFeatIds = new List<string>
        {
            FeatIds.Acrobatic, FeatIds.Alertness, FeatIds.ArmorProficiencyLight, FeatIds.Athletic,
            FeatIds.Attentive, FeatIds.Brawl, FeatIds.Cautious, FeatIds.DefensiveMartialArts, FeatIds.Dodge,
            FeatIds.ElusiveTarget, FeatIds.Meticulous, FeatIds.Mobility, FeatIds.Nimble, FeatIds.Renown,
            FeatIds.Run, FeatIds.Stealthy,
        };

        return def;
    }
}
