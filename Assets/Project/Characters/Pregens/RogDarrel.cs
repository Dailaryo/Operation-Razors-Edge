using UnityEngine;
using System.Collections.Generic;

public static class RogDarrel
{
    public static CharacterData Build()
    {
        var data = new CharacterData(
            id: PregenIds.RogDarrel,
            name: "Rog Darrel",
            hp: 13,
            attack: 0,
            initiative: -1
        );

        // Identity
        data.speciesId = SpeciesIds.Pentian;
        data.classId = ClassIds.HelixWarrior;
        data.backgroundId = BackgroundIds.Gladiator;
        data.isEnemy = false;
        data.level = 1;

        // Ability Scores
        // 25-point buy, post-Pentian modifiers applied:
        // Power 16 (+6 racial) = 22   |  Agility 10 (-2 racial) = 8
        // Endurance 14 (+2 racial) = 16 |  Cognition 9 (-2 racial) = 7
        // Insight 13 = 13              |  Presence 9 (-2 racial) = 7
        data.power = 22;
        data.agility = 8;
        data.endurance = 16;
        data.cognition = 7;
        data.insight = 13;
        data.presence = 7;

        // Class Skills
        // Helix Warrior base list; Gladiator adds SenseMotive as new class skill.
        // Intimidate and Athletics already on class list — background grants +1 competence.
        data.classSkills = new List<SkillType>
        {
            SkillType.Athletics,
            SkillType.Intimidate,
            SkillType.Survival,
            SkillType.Perception,
            SkillType.SenseMotive,
            SkillType.ZeroGOperations,
        };

        // Background skill bonuses: Intimidate and Athletics already class skills
        data.backgroundSkillBonuses = new List<StatModifier>
        {
            new StatModifier(
                StatType.SkillIntimidate,
                1, 1f,
                BackgroundIds.Gladiator + "_Intimidate",
                StatModifier.BonusType.Competence),
            new StatModifier(
                StatType.SkillAthletics,
                1, 1f,
                BackgroundIds.Gladiator + "_Athletics",
                StatModifier.BonusType.Competence),
        };

        // Skill Ranks
        // Level 1: max(1, 3 + (-2)) = 1 rank (COG 7, mod -2, floor at 1)
        data.skillRanks = new Dictionary<SkillType, int>
        {
            { SkillType.Intimidate, 1 },
        };

        // Level 1 Unlocks
        // AdaptivePhysiology delivered by class level table at level 1.
        // Mutations purchased at level-up; none active at level 1 yet.
        // RapidHealing purchased from the 2 MP budget at character creation.
        data.unlockedActions = new List<string>
        {
            ActionIds.MeleeAttack,
        };

        // Feats
        // Level 1 universal (cadence 1): Power Attack
        // Level 1 background bonus: Brawl
        data.equippedFeatIds = new List<string>
        {
            FeatIds.PowerAttack,
            FeatIds.Brawl,
        };

        // Intended Progression
        // Records full planned path so LevelUp() makes correct choices automatically.
        data.intendedChoiceOptions = new List<string>
        {
            ChoiceNodeIds.HelixWarriorBerserker,
            ChoiceNodeIds.HelixEnhancedMutationRapidHealing,
            ChoiceNodeIds.HelixImprovedMutationMusculature,
            ChoiceNodeIds.HelixGreaterMutationRapidHealing,
        };

        data.intendedFeatProgression = new List<string>
        {
            // Universal cadence: 1, 4, 7, 10, 13, 16, 19
            FeatIds.PowerAttack,               // Level 1
            FeatIds.Cleave,                    // Level 4
            FeatIds.GreatCleave,               // Level 7
            FeatIds.ImprovedCombatMartialArts, // Level 10
            FeatIds.ImprovedKnockoutPunch,     // Level 13
            FeatIds.AdvancedCombatMartialArts, // Level 16
            FeatIds.BlindFight,                // Level 19

            // Class cadence: 2, 6, 9, 13, 16, 19
            FeatIds.ImprovedBrawl,             // Level 2
            FeatIds.CombatMartialArts,         // Level 6
            FeatIds.KnockoutPunch,             // Level 9
            FeatIds.ImprovedGrapple,           // Level 13
            FeatIds.CombatReflexes,            // Level 16
            FeatIds.Streetfighting,            // Level 19

            // Background bonus
            FeatIds.Brawl,                     // Level 1
        };

        return data;
    }
}