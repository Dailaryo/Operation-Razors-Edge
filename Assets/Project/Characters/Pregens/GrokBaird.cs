using UnityEngine;
using System.Collections.Generic;

public static class GrokBaird
{
    public static CharacterData Build()
    {
        var data = new CharacterData(
            id:         PregenIds.GrokBaird,
            name:       "Grok Baird",
            hp:         15,
            attack:     0,
            initiative: 0
        );

        // Identity
        data.speciesId    = SpeciesIds.Osteo;
        data.classId      = ClassIds.Dreadnought;
        data.backgroundId = BackgroundIds.Military;
        data.isEnemy      = false;
        data.level        = 1;

        // Ability Scores
        // 25-point buy, post-Osteo modifiers applied:
        // Power 14 (+4 racial) = 18    |  Agility 10 = 10
        // Endurance 16 = 16            |  Cognition 9 (-2 racial) = 7
        // Insight 12 = 12              |  Presence 10 (-2 racial) = 8
        data.power     = 18;
        data.agility   = 10;
        data.endurance = 16;
        data.cognition = 7;
        data.insight   = 12;
        data.presence  = 8;

        // Class Skills
        // Dreadnought base list: Athletics, Intimidate, Knowledge, Perception,
        // Survival, Acrobatics.
        // Military background selects Athletics and Survival (both already class
        // skills — background grants +1 competence on each via backgroundSkillBonuses).
        data.classSkills = new List<SkillType>
        {
            SkillType.Athletics,
            SkillType.Intimidate,
            SkillType.Knowledge,
            SkillType.Perception,
            SkillType.Survival,
            SkillType.Acrobatics,
        };

        // Background selections
        data.backgroundFeatChoice = FeatIds.PersonalFirearmsProficiency;
        data.backgroundSkillChoices = new List<SkillType>
        {
            SkillType.Athletics,
            SkillType.Survival,
        };

        // Background skill bonuses: Athletics and Survival already class skills
        // — background grants +1 competence on each.
        data.backgroundSkillBonuses = new List<StatModifier>
        {
            new StatModifier(
                StatType.SkillAthletics,
                1, 1f,
                BackgroundIds.Military + "_Athletics",
                StatModifier.BonusType.Competence),
            new StatModifier(
                StatType.SkillSurvival,
                1, 1f,
                BackgroundIds.Military + "_Survival",
                StatModifier.BonusType.Competence),
        };

        // Skill Ranks
        // Level 1: max(1, 3 + (-2)) = 1 rank (COG 7, mod -2, floor at 1)
        data.skillRanks = new Dictionary<SkillType, int>
        {
            { SkillType.Intimidate, 1 },
        };

        // Level 1 Action Unlocks
        // MeleeAttack delivered by class level table at level 1.
        data.unlockedActions = new List<string>
        {
            ActionIds.MeleeAttack,
        };

        // Feats at Level 1
        // Universal cadence level 1: Power Attack
        // Background bonus: Personal Firearms Proficiency
        // Species bonus: Simple Weapons Proficiency
        data.equippedFeatIds = new List<string>
        {
            FeatIds.PowerAttack,
            FeatIds.PersonalFirearmsProficiency,
            FeatIds.SimpleWeaponsProficiency,
        };

        // Intended Progression
        // intendedChoiceOptions drives LevelUp() path selection.
        // Bastion is the pregen's designated path.
        data.intendedChoiceOptions = new List<string>
        {
            ChoiceNodeIds.DreadnoughtBastion,
        };

        // intendedFeatProgression lists all feats in the order they will be taken,
        // grouped by source. LevelUp() consumes these in sequence per cadence.
        data.intendedFeatProgression = new List<string>
        {
            // Universal cadence: 1, 4, 7, 10, 13, 16, 19
            FeatIds.PowerAttack,                 // Level 1
            FeatIds.GreatCleave,                 // Level 4
            FeatIds.ArmorProficiencyMedium,      // Level 7
            FeatIds.CombatReflexes,              // Level 10
            FeatIds.ImprovedDamageThreshold,     // Level 13
            FeatIds.FrightfulPresence,           // Level 16
            FeatIds.Sunder,                      // Level 19

            // Class cadence: 3, 6, 9, 13, 16, 19
            FeatIds.Cleave,                      // Level 3
            FeatIds.ImprovedBullRush,            // Level 6
            FeatIds.ArmorProficiencyHeavy,       // Level 9
            FeatIds.Toughness,                   // Level 13
            FeatIds.AdvancedFirearmsProficiency, // Level 16
            FeatIds.Endurance,                   // Level 19

            // Background bonus
            FeatIds.PersonalFirearmsProficiency, // Level 1

            // Species bonus
            FeatIds.SimpleWeaponsProficiency,    // Level 1
        };

        return data;
    }
}
