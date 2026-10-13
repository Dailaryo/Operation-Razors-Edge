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
        // 25-point buy, before species modifiers (applied by SpeciesDefinition)
        data.power      = 9;
        data.agility    = 10;
        data.endurance  = 12;
        data.cognition  = 14;
        data.insight    = 16;
        data.presence   = 10;

        // Background selections
        // Doctor: TreatInjury and Knowledge are already class skills → +1 competence each.
        data.backgroundSkillChoices = new List<SkillType>
        {
            SkillType.TreatInjury,
            SkillType.Knowledge,
        };

        CharacterBuilder.BuildToLevel(data, 1);

        return data;
    }

    // Pre-written level-up route (debug bypass, see PregenProgression)
    public static Dictionary<int, PregenLevelPlan> BuildRoute()
    {
        var route = PregenLevelPlan.CreateRoute();

        // Feats: universal 1, 4, 7, 10, 13, 16, 19 | Field Medic 3, 6, 9, 13, 16, 19
        route[1].feats.Add(FeatIds.MedicalExpert);
        route[1].feats.Add(FeatIds.SimpleWeaponsProficiency);     // species grant
        route[3].feats.Add(FeatIds.ImprovedInitiative);           // class
        route[4].feats.Add(FeatIds.ArmorProficiencyLight);
        route[6].feats.Add(FeatIds.DefensiveMartialArts);         // class
        route[7].feats.Add(FeatIds.Cautious);
        route[9].feats.Add(FeatIds.Dodge);                        // class
        route[10].feats.Add(FeatIds.PersonalFirearmsProficiency);
        route[13].feats.Add(FeatIds.Toughness);
        route[13].feats.Add(FeatIds.Studious);                    // class (replaces Educated)
        route[16].feats.Add(FeatIds.AdvancedFirearmsProficiency);
        route[16].feats.Add(FeatIds.ArmorProficiencyMedium);      // class
        route[19].feats.Add(FeatIds.Alertness);
        route[19].feats.Add(FeatIds.Educated);            // class; requires Studious (L13)

        // Ability score increases: all Insight
        route[4].asi = StatType.Insight;
        route[8].asi = StatType.Insight;
        route[12].asi = StatType.Insight;
        route[16].asi = StatType.Insight;
        route[20].asi = StatType.Insight;

        // Path
        route[10].choiceOptions.Add(ChoiceNodeIds.FieldMedicTraumaSurgeon);

        // Skill ranks: every chosen skill kept at the cap
        PregenLevelPlan.FillSkillsAtCap(route, 1, 20,
            SkillType.TreatInjury, SkillType.Knowledge, SkillType.Perception,
            SkillType.SenseMotive, SkillType.Survival);

        return route;
    }
}