using UnityEngine;
using System.Collections.Generic;

public static class HarveyJones
{
    public static CharacterData Build()
    {
        var data = new CharacterData(
            id:         PregenIds.HarveyJones,
            name:       "Harvey Jones",
            hp:         10,   // 1d8 average (8) + END mod (+2)
            attack:     0,    // +0 BAB at level 1
            initiative: 0     // AGI mod 0; Improved Initiative feat adds +4 at level 3
        );

        // Identity
        data.speciesId    = SpeciesIds.Radix;
        data.classId      = ClassIds.FieldMedic;
        data.backgroundId = BackgroundIds.Doctor;
        data.isEnemy      = false;
        data.level        = 1;

        // Ability Scores
        // 25-point buy, post-racial modifiers applied
        // Power 9 = 9 (-1)   | Agility 10 = 10 (0)
        // Endurance 12 +2 Radix = 14 (+2) | Cognition 14 = 14 (+2)
        // Insight 16 = 16 (+3) | Presence 10 -2 Radix = 8 (-1)
        data.power      = 9;
        data.agility    = 10;
        data.endurance  = 14;
        data.cognition  = 14;
        data.insight    = 16;
        data.presence   = 8;

        // Class Skills
        // Field Medic base list — Doctor background adds no new skills
        // (both chosen skills, TreatInjury and Knowledge, are already class skills)
        data.classSkills = new List<SkillType>
        {
            SkillType.ComputerUse,
            SkillType.Knowledge,
            SkillType.TreatInjury,
            SkillType.Perception,
            SkillType.SenseMotive,
            SkillType.Survival,
        };

        // Background Skill Bonuses
        // Doctor background: chose TreatInjury and Knowledge — both already class skills,
        // so both grant +1 competence bonus per BackgroundDefinition.Apply() logic
        data.backgroundSkillChoices = new List<SkillType>
        {
            SkillType.TreatInjury,
            SkillType.Knowledge,
        };

        data.backgroundSkillBonuses = new List<StatModifier>
        {
            new StatModifier(
                StatType.SkillTreatInjury,
                1, 1f,
                BackgroundIds.Doctor + "_TreatInjury",
                StatModifier.BonusType.Competence),
            new StatModifier(
                StatType.SkillKnowledge,
                1, 1f,
                BackgroundIds.Doctor + "_Knowledge",
                StatModifier.BonusType.Competence),
        };

        // Skill Ranks
        // Level 1: 3 (class) + 2 (COG mod) = 5 ranks
        data.skillRanks = new Dictionary<SkillType, int>
        {
            { SkillType.TreatInjury, 1 },
            { SkillType.Knowledge,   1 },
            { SkillType.Perception,  1 },
            { SkillType.SenseMotive, 1 },
            { SkillType.Survival,    1 },
        };

        // Level 1 Unlocks
        // Set directly — bypasses LevelUp() to avoid triggering feat cadence
        data.unlockedActions = new List<string>
        {
            ActionIds.TreatInjury,
            ActionIds.HealAlly,
        };

        data.unlockedPassiveIds = new List<string>
        {
            PassiveIds.FieldMedicMedicalSpecialist1,
        };

        // Feats
        // Level 1 universal cadence: Medical Expert
        data.equippedFeatIds = new List<string>
        {
            FeatIds.MedicalExpert,
        };

        // Starting Equipment
        // Medkit assignment deferred until the item system is fully implemented —
        // no equippedItemIds block for now.

        // Intended Progression
        // Path selection at level 10: Trauma Surgeon
        data.intendedChoiceOptions = new List<string>
        {
            ChoiceNodeIds.FieldMedicTraumaSurgeon,
        };

        data.intendedFeatProgression = new List<string>
        {
            // Universal cadence: 1, 4, 7, 10, 13, 16, 19
            FeatIds.MedicalExpert,               // Level 1  — already in equippedFeatIds
            FeatIds.ArmorProficiencyLight,        // Level 4
            FeatIds.Cautious,                     // Level 7
            FeatIds.PersonalFirearmsProficiency,  // Level 10
            FeatIds.Toughness,                    // Level 13
            FeatIds.AdvancedFirearmsProficiency,  // Level 16
            FeatIds.Alertness,                    // Level 19

            // Class cadence: 3, 6, 9, 13, 16, 19
            FeatIds.ImprovedInitiative,           // Level 3
            FeatIds.DefensiveMartialArts,         // Level 6
            FeatIds.Dodge,                        // Level 9
            FeatIds.Educated,                     // Level 13
            FeatIds.ArmorProficiencyMedium,       // Level 16
            FeatIds.Toughness,                    // Level 19
        };

        return data;
    }
}
