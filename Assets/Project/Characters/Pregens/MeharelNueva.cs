using UnityEngine;
using System.Collections.Generic;

public static class MeharelNueva
{
    public static CharacterData Build()
    {
        var data = new CharacterData(
            id:         PregenIds.MeharelNueva,
            name:       "Meharel Nueva",
            hp:         7,
            attack:     0,
            initiative: 4
        );

        // Identity
        data.speciesId    = SpeciesIds.Cryptian;
        data.classId      = ClassIds.Infiltrator;
        data.backgroundId = BackgroundIds.Criminal;
        data.isEnemy      = false;
        data.level        = 1;

        // Ability Scores
        // 25-point buy, post-Cryptian modifiers applied:
        // Power  9                   =  9  |  Agility  16 (+2 racial) = 18
        // Endurance 10 (-2 racial)   =  8  |  Cognition 14 (+2 racial) = 16
        // Insight 10                 = 10  |  Presence  12 (+2 racial) = 14
        data.power     =  9;
        data.agility   = 18;
        data.endurance =  8;
        data.cognition = 16;
        data.insight   = 10;
        data.presence  = 14;

        // Class Skills
        // Infiltrator base list: Acrobatics, Athletics, Bluff, DisableDevice,
        // Investigate, Knowledge, Perception, SenseMotive, Stealth.
        // Criminal background: Stealth and DisableDevice already class skills → +1 competence each.
        // Knowledge already a class skill → +1 competence.
        // Bluff already on class list — no new skill added, +1 competence only.
        data.classSkills = new List<SkillType>
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
        };

        // Background skill bonuses
        // Stealth, DisableDevice, Knowledge, Bluff all already on class list → +1 competence each
        data.backgroundSkillBonuses = new List<StatModifier>
        {
            new StatModifier(
                StatType.SkillStealth, 1, 1f,
                BackgroundIds.Criminal + "_Stealth",
                StatModifier.BonusType.Competence),
            new StatModifier(
                StatType.SkillDisableDevice, 1, 1f,
                BackgroundIds.Criminal + "_DisableDevice",
                StatModifier.BonusType.Competence),
            new StatModifier(
                StatType.SkillBluff, 1, 1f,
                BackgroundIds.Criminal + "_Bluff",
                StatModifier.BonusType.Competence),
            new StatModifier(
                StatType.SkillKnowledge, 1, 1f,
                BackgroundIds.Criminal + "_Knowledge",
                StatModifier.BonusType.Competence),
        };

        // Skill Ranks
        // Level 1: 7 (class) + 3 (COG mod) = 10 ranks
        data.skillRanks = new Dictionary<SkillType, int>
        {
            { SkillType.Stealth,       4 },
            { SkillType.Perception,    2 },
            { SkillType.DisableDevice, 2 },
            { SkillType.Acrobatics,    2 },
        };

        // Level 1 Unlocks
        // Set directly — bypasses LevelUp() to avoid triggering feat cadence.
        // RangedAttack not in Universal list; must be set explicitly.
        // Sweep is a passive delivered by the class at level 1.
        data.unlockedActions = new List<string>
        {
            ActionIds.RangedAttack,
        };

        data.unlockedPassiveIds = new List<string>
        {
            PassiveIds.InfiltratorSweep,
            PassiveIds.CryptianEmpathicSense,
            PassiveIds.CryptianSleepImmunity,
            PassiveIds.CryptianFocusSaveBonus,
            PassiveIds.CryptianConcealedDetection,
            PassiveIds.CryptianLightSensitivity,
        };

        // Feats
        // Level 1 universal cadence: Stealthy
        // Level 1 background bonus: PersonalFirearmsProficiency
        data.equippedFeatIds = new List<string>
        {
            FeatIds.Stealthy,
            FeatIds.PersonalFirearmsProficiency,
        };

        // Intended Progression
        // Full planned path so LevelUp() makes correct choices automatically.
        // Skill Mastery selections at level 5: Stealth, DisableDevice, Acrobatics.
        // Path selection at level 10: Ghost.
        data.intendedChoiceOptions = new List<string>
        {
            ChoiceNodeIds.InfiltratorSkillMasteryStealth,
            ChoiceNodeIds.InfiltratorSkillMasteryDisableDevice,
            ChoiceNodeIds.InfiltratorSkillMasteryAcrobatics,
            ChoiceNodeIds.InfiltratorGhost,
        };

        data.intendedFeatProgression = new List<string>
        {
            // Universal cadence: 1, 4, 7, 10, 13, 16, 19
            FeatIds.Stealthy,                   // Level 1  — already in equippedFeatIds
            FeatIds.PointBlankShot,             // Level 4
            FeatIds.DeadAim,                    // Level 7
            FeatIds.ImprovedInitiative,         // Level 10
            FeatIds.Dodge,                      // Level 13
            FeatIds.DoubleTap,                  // Level 16
            FeatIds.ImprovedInitiative,         // Level 19 — placeholder; Dead Aim (Improved) deferred

            // Class cadence: 3, 6, 9, 13, 16, 19
            FeatIds.AdvancedFirearmsProficiency, // Level 3
            FeatIds.FarShot,                    // Level 6
            FeatIds.PreciseShot,                // Level 9
            FeatIds.Alertness,                  // Level 13 (Ghost path bonus)
            FeatIds.Stealthy,                   // Level 16 (Ghost path bonus — second take)
            FeatIds.Meticulous,                 // Level 19 (Ghost path bonus)

            // Background bonus
            FeatIds.PersonalFirearmsProficiency, // Level 1 — already in equippedFeatIds
        };

        return data;
    }
}
