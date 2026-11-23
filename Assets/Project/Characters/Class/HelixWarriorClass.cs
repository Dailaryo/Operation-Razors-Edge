using UnityEngine;
using System.Collections.Generic;
using static ClassDefinition;

public static class HelixWarriorClass
{
    public static ClassDefinition Create()
    {
        var def = new ClassDefinition(
            classId: ClassIds.HelixWarrior,
            diplayName: "Helix Warrior",
            statBias: StatType.Power,
            babProgression: BABProgression.ThreeQuarters,
            hpPerLevel: 10,
            skillRanksPerLevel: 3,
            resolveSave: SaveProgression.Good,
            responseSave: SaveProgression.Poor,
            focusSave: SaveProgression.Poor,
            featCadence: new List<int> { 2, 6, 9, 13, 16, 19 },
            classSkills: new List<SkillType>
            {
                SkillType.Athletics,
                SkillType.Intimidate,
                SkillType.Survival,
                SkillType.Perception,
                SkillType.SenseMotive,
                SkillType.ZeroGOperations,
            },
            levelTable: new Dictionary<int, List<ClassLevelEntry>>
            {
                // ── TIER 1 (LEVELS 1–10) ────────────────────────────────

                { 1, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.HelixAdaptivePhysiology })
                }},

                { 3, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(choicNodeId: ChoiceNodeIds.HelixEnhancedMutation)
                }},

                { 4, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.HelixPowerfulBuild })
                }},

                { 5, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(choicNodeId: ChoiceNodeIds.HelixImprovedMutation)
                }},

                { 7, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string> { PassiveIds.HelixBrutalStrike },
                        actionUnlockIds: new List<string> { ActionIds.BrutalStrike })
                }},

                { 8, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(choicNodeId: ChoiceNodeIds.HelixGreaterMutation)
                }},

                { 10, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(choicNodeId: ChoiceNodeIds.HelixWarriorPath)
                }},

                // ── TIER 2 (LEVELS 11–20) — ALL THREE PATHS ─────────────

                { 11, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Berserker
                        passiveIds: new List<string> { PassiveIds.HelixRampage },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorBerserker),

                    new ClassLevelEntry(                          // Apex Predator
                        passiveIds: new List<string> { PassiveIds.HelixHuntingInstinct },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorApexPredator),

                    new ClassLevelEntry(                          // Iron Flesh
                        passiveIds: new List<string> { PassiveIds.HelixHardenedHide },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorIronFlesh),
                }},

                { 12, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Berserker
                        passiveIds: new List<string> { PassiveIds.HelixBloodlust },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorBerserker),

                    new ClassLevelEntry(                          // Apex Predator
                        actionUnlockIds: new List<string> { ActionIds.HelixPredatoryLeap },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorApexPredator),

                    new ClassLevelEntry(                          // Iron Flesh
                        passiveIds: new List<string> { PassiveIds.HelixReactivePlating },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorIronFlesh),
                }},

                { 13, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Berserker
                        passiveIds: new List<string> { PassiveIds.HelixSavageMomentum },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorBerserker),

                    new ClassLevelEntry(                          // Apex Predator
                        passiveIds: new List<string> { PassiveIds.HelixLethalTempo },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorApexPredator),

                    new ClassLevelEntry(                          // Iron Flesh
                        passiveIds: new List<string> { PassiveIds.HelixWoundSuppression },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorIronFlesh),
                }},

                { 14, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Berserker
                        passiveIds: new List<string> { PassiveIds.HelixKillingBlow },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorBerserker),

                    new ClassLevelEntry(                          // Apex Predator
                        actionUnlockIds: new List<string> { ActionIds.HelixMarkPrey },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorApexPredator),

                    new ClassLevelEntry(                          // Iron Flesh
                        passiveIds: new List<string> { PassiveIds.HelixFortressBody },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorIronFlesh),
                }},

                { 15, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Berserker
                        choicNodeId: ChoiceNodeIds.HelixEnhancedMutation,
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorBerserker),

                    new ClassLevelEntry(                          // Apex Predator
                        passiveIds: new List<string> { PassiveIds.HelixExposedWeakness },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorApexPredator),

                    new ClassLevelEntry(                          // Iron Flesh
                        choicNodeId: ChoiceNodeIds.HelixImprovedMutation,
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorIronFlesh),
                }},

                { 16, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Berserker
                        actionUnlockIds: new List<string> { ActionIds.HelixFrenziedStrike },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorBerserker),

                    new ClassLevelEntry(                          // Apex Predator
                        passiveIds: new List<string> { PassiveIds.HelixAmbushStriker },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorApexPredator),

                    new ClassLevelEntry(                          // Iron Flesh — DR 4/— upgrade
                        passiveIds: new List<string> { PassiveIds.HelixHardenedHide },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorIronFlesh),
                }},

                { 17, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Berserker
                        passiveIds: new List<string> { PassiveIds.HelixDevastatingCleave },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorBerserker),

                    new ClassLevelEntry(                          // Berserker: 2nd Greater Mutation
                        choicNodeId: ChoiceNodeIds.HelixGreaterMutation,
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorBerserker),

                    new ClassLevelEntry(                          // Apex Predator
                        passiveIds: new List<string> { PassiveIds.HelixApexSpeed },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorApexPredator),

                    new ClassLevelEntry(                          // Iron Flesh
                        passiveIds: new List<string> { PassiveIds.HelixUnbreakable },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorIronFlesh),
                }},

                { 18, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Berserker
                        choicNodeId: ChoiceNodeIds.HelixImprovedMutation,
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorBerserker),

                    new ClassLevelEntry(                          // Apex Predator
                        actionUnlockIds: new List<string> { ActionIds.HelixShadowstep },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorApexPredator),

                    new ClassLevelEntry(                          // Iron Flesh
                        passiveIds: new List<string> { PassiveIds.HelixLivingWall },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorIronFlesh),
                }},

                { 19, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Berserker
                        actionUnlockIds: new List<string> { ActionIds.HelixBerserkApotheosis },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorBerserker),

                    new ClassLevelEntry(                          // Apex Predator
                        actionUnlockIds: new List<string> { ActionIds.HelixPredatorApotheosis },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorApexPredator),

                    new ClassLevelEntry(                          // Iron Flesh
                        actionUnlockIds: new List<string> { ActionIds.HelixIronApotheosis },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorIronFlesh),
                }},

                { 20, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Berserker
                        actionUnlockIds: new List<string> { ActionIds.HelixApexMutationBerserker },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorBerserker),

                    new ClassLevelEntry(                          // Apex Predator
                        actionUnlockIds: new List<string> { ActionIds.HelixApexMutationApex },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorApexPredator),

                    new ClassLevelEntry(                          // Iron Flesh
                        actionUnlockIds: new List<string> { ActionIds.HelixApexMutationIron },
                        prerequisiteOptionId: ChoiceNodeIds.HelixWarriorIronFlesh),
                }},
            }
        );

        def.resource = new HelixWarriorResourceDefinition();

        // Bonus feats (d20 Future Helix Warrior list; Surface Vehicle Operation dropped)
        // Adapted: unarmed entry tier (Brawl, Combat Martial Arts, Improved Brawl, Knockout Punch)
        // added so the list's Improved/Advanced Combat Martial Arts are reachable.
        def.bonusFeatIds = new List<string>
        {
            FeatIds.AdvancedCombatMartialArts, FeatIds.AdvancedFirearmsProficiency, FeatIds.ArmorProficiencyLight,
            FeatIds.ArmorProficiencyMedium, FeatIds.ArmorProficiencyHeavy, FeatIds.ArmorProficiencyPowered,
            FeatIds.Athletic, FeatIds.BlindFight, FeatIds.CombatExpertise, FeatIds.CombatReflexes,
            FeatIds.CombatThrow, FeatIds.DeadlyAim, FeatIds.DefensiveMartialArts, FeatIds.ElusiveTarget,
            FeatIds.ExoticFirearmsProficiency, FeatIds.FarShot, FeatIds.GreatFortitude,
            FeatIds.ImprovedCombatMartialArts, FeatIds.ImprovedCombatThrow, FeatIds.ImprovedDisarm,
            FeatIds.ImprovedTrip, FeatIds.IronWill, FeatIds.LightningReflexes, FeatIds.NervePinch,
            FeatIds.UltraImmuneSystem, FeatIds.WeaponFocus, FeatIds.WhirlwindAttack, FeatIds.ZeroGTraining,
            // Adaptations
            FeatIds.Brawl, FeatIds.CombatMartialArts, FeatIds.ImprovedBrawl, FeatIds.KnockoutPunch,
        };
        def.pathBonusFeatIds = new Dictionary<string, List<string>>
        {
            { ChoiceNodeIds.HelixWarriorBerserker, new List<string>
                { FeatIds.ImprovedKnockoutPunch, FeatIds.ImprovedGrapple, FeatIds.Streetfighting } },
        };

        return def;
    }

    private class HelixWarriorResourceDefinition : ClassDefinition.ResourceDefinition
    {
        public HelixWarriorResourceDefinition() : base(
            resourceId: "helix_warrior_mp",
            displayName: "Mutation Points",
            regenMode: RegenMode.Permanent,
            baseMax: 2)
        { }

        public override int GetMaxAtLevel(int level)
        {
            if (level >= 20) return 16;
            if (level >= 18) return 14;
            if (level >= 15) return 12;
            if (level >= 12) return 10;
            if (level >= 10) return 8;
            if (level >= 7) return 6;
            if (level >= 4) return 4;
            return 2;
        }
    }
}