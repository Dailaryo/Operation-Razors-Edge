using UnityEngine;
using System.Collections.Generic;

public static class AlexandraIvanova
{
    public static CharacterData Build()
    {
        var data = new CharacterData(
            id: PregenIds.AlexandraIvanova,
            name: "Alexandra Ivanova",
            hp: 7,
            attack: 0,
            initiative: -1
        );

        //Identity
        data.speciesId = SpeciesIds.Delphin;
        data.classId = ClassIds.Technosavant;
        data.backgroundId = BackgroundIds.Academic;
        data.isEnemy = false;
        data.level = 1;

        //Ability Scores
        // 25-point buy, post-genotype modifiers applied
        // Power 10 (+2 racial) = 12  |  Agility 10 (-2 racial) = 8
        // Endurance 12 = 12          |  Cognition 16 (+4 racial) = 20
        // Insight 10 (-4 racial) = 6 |  Presence 10 = 10
        data.power = 12;
        data.agility = 8;
        data.endurance = 12;
        data.cognition = 20;
        data.insight = 6;
        data.presence = 10;

        //Class Skills
        // Technosavant base list; Academic background adds Knowledge and Investigate.
        // ComputerUse was already a class skill so background grants +1 competence instead.
        data.classSkills = new List<SkillType>
        {
            SkillType.ComputerUse,
            SkillType.Repair,
            SkillType.DisableDevice,
            SkillType.Demolitions,
            SkillType.Knowledge,
            SkillType.Investigate,
            SkillType.Perception,
            SkillType.SenseMotive,
            SkillType.TreatInjury,
            SkillType.Xenology,
        };

        // Academic background overlap: ComputerUse already a class skill → +1 competence
        data.backgroundSkillBonuses = new List<StatModifier>
        {
            new StatModifier(
                StatType.SkillComputerUse,
                1,
                1f,
                BackgroundIds.Academic + "_ComputerUse",
                StatModifier.BonusType.Competence)
        };

        //Skill Ranks
        // Level 1: 7 (class) + 5 (COG mod) = 12 ranks
        data.skillRanks = new Dictionary<SkillType, int>
        {
            { SkillType.ComputerUse,   4 },
            { SkillType.Knowledge,     2 },
            { SkillType.Investigate,   2 },
            { SkillType.Perception,    2 },
            { SkillType.DisableDevice, 2 },
        };

        //Level 1 Unlocks
        // Set directly — bypasses LevelUp() to avoid triggering feat cadence
        data.unlockedActions = new List<string>
        {
            ActionIds.RangedAttack,
            ActionIds.Overcharge,
        };

        data.unlockedPassiveIds = new List<string>
        {
            PassiveIds.NeuralAcuity1,
        };

        //Feats
        // Level 1 universal cadence: Cybertaker
        data.equippedFeatIds = new List<string>
        {
            FeatIds.Cybertaker,
        };

        //Intended Progression
        // Records the full intended path for this pregen so LevelUp() makes
        // the right choices automatically as she advances through play.
        // Path selection at level 10: Vanguard
        // Feat selections follow the design doc cadence.
        data.intendedChoiceOptions = new List<string>
        {
            ChoiceNodeIds.TechnosavantVanguard,
        };

        data.intendedFeatProgression = new List<string>
        {
            // Universal cadence: 1, 4, 7, 10, 13, 16, 19
            FeatIds.Cybertaker,           // Level 1  — already in equippedFeatIds
            FeatIds.PreciseShot,          // Level 4
            FeatIds.IronWill,             // Level 7
            FeatIds.OverwatchSpecialist,  // Level 10
            FeatIds.TacticalRelay,        // Level 13
            FeatIds.Resilience,           // Level 16
            // Level 19 TBD — deferred until higher level content is authored

            // Class cadence: 5, 10, 15, 20
            FeatIds.SensorLink,           // Level 5
            FeatIds.CyberneticMastery,    // Level 10
            // Level 15 and 20 delivered as Vanguard path passives
        };

        return data;
    }
}