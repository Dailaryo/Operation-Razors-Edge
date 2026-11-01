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
            initiative: -1
        );

        //Identity
        data.speciesId = SpeciesIds.Delphin;
        data.classId = ClassIds.Technosavant;
        data.backgroundId = BackgroundIds.Academic;
        data.isEnemy = false;

        //Ability Scores
        // 25-point buy, before species modifiers (applied by SpeciesDefinition)
        data.power = 10;
        data.agility = 10;
        data.endurance = 12;
        data.cognition = 16;
        data.insight = 10;
        data.presence = 10;

        // Background selections
        // Academic grants all 3; ComputerUse, Investigate and Knowledge are already
        // Technosavant class skills, so each becomes +1 competence.
        data.backgroundSkillChoices = new List<SkillType>
        {
            SkillType.ComputerUse,
            SkillType.Investigate,
            SkillType.Knowledge,
        };

        // Class skills, level-1 unlocks, feats and skill ranks come from
        // CharacterBuilder + BuildRoute(), the same path respec uses.
        CharacterBuilder.BuildToLevel(data, 1);

        return data;
    }

    // Pre-written level-up route (debug bypass, see PregenProgression)
    public static Dictionary<int, PregenLevelPlan> BuildRoute()
    {
        var route = PregenLevelPlan.CreateRoute();

        // Feats: universal 1, 4, 7, 10, 13, 16, 19 | Technosavant 5, 10, 15, 20
        route[1].feats.Add(FeatIds.Cybertaker);
        route[4].feats.Add(FeatIds.PointBlankShot);       // prerequisite for Precise Shot
        route[5].feats.Add(FeatIds.SensorLink);           // class
        route[7].feats.Add(FeatIds.IronWill);
        route[10].feats.Add(FeatIds.OverwatchSpecialist);
        route[10].feats.Add(FeatIds.CyberneticMastery);   // class
        route[13].feats.Add(FeatIds.Studious);
        route[15].feats.Add(FeatIds.Educated);            // class; requires Studious
        route[16].feats.Add(FeatIds.Resilience);
        route[19].feats.Add(FeatIds.PreciseShot);
        route[20].feats.Add(FeatIds.TacticalRelay);       // class

        // Ability score increases: all Cognition (retroactive skill ranks at 8 and 16)
        route[4].asi = StatType.Cognition;
        route[8].asi = StatType.Cognition;
        route[12].asi = StatType.Cognition;
        route[16].asi = StatType.Cognition;
        route[20].asi = StatType.Cognition;

        // Path
        route[10].choiceOptions.Add(ChoiceNodeIds.TechnosavantVanguard);

        // Skill ranks: every chosen skill kept at the cap
        PregenLevelPlan.FillSkillsAtCap(route, 1, 20,
            SkillType.ComputerUse, SkillType.Repair, SkillType.DisableDevice, SkillType.Demolitions,
            SkillType.Knowledge, SkillType.Investigate, SkillType.Perception, SkillType.SenseMotive,
            SkillType.TreatInjury, SkillType.Xenology, SkillType.ZeroGOperations, SkillType.Stealth);
        PregenLevelPlan.FillSkillsAtCap(route, 8, 20, SkillType.Athletics);    // COG 22 at L8
        PregenLevelPlan.FillSkillsAtCap(route, 16, 20, SkillType.Acrobatics);  // COG 24 at L16

        return route;
    }
}