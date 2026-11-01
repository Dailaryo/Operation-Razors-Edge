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
            initiative: 4
        );

        // Identity
        data.speciesId    = SpeciesIds.Cryptian;
        data.classId      = ClassIds.Infiltrator;
        data.backgroundId = BackgroundIds.Criminal;
        data.isEnemy      = false;
        data.level        = 1;

        // Ability Scores
        // 25-point buy, before species modifiers (applied by SpeciesDefinition)
        data.power     =  9;
        data.agility   = 16;
        data.endurance = 10;
        data.cognition = 14;
        data.insight   = 10;
        data.presence  = 12;

        // Background selections
        // Criminal (choose 2): Stealth and DisableDevice are already class skills → +1 competence each.
        data.backgroundFeatChoice = FeatIds.PersonalFirearmsProficiency;
        data.backgroundSkillChoices = new List<SkillType>
        {
            SkillType.Stealth,
            SkillType.DisableDevice,
        };

        CharacterBuilder.BuildToLevel(data, 1);

        return data;
    }

    // Pre-written level-up route (debug bypass, see PregenProgression)
    public static Dictionary<int, PregenLevelPlan> BuildRoute()
    {
        var route = PregenLevelPlan.CreateRoute();

        // Feats: universal 1, 4, 7, 10, 13, 16, 19 | Infiltrator 3, 6, 9, 13, 16, 19
        // Ranged line runs through universal slots; class slots use the Infiltrator list.
        route[1].feats.Add(FeatIds.Stealthy);
        route[1].feats.Add(FeatIds.PersonalFirearmsProficiency);  // background grant
        route[3].feats.Add(FeatIds.Dodge);                        // class
        route[4].feats.Add(FeatIds.PointBlankShot);
        route[6].feats.Add(FeatIds.DefensiveMartialArts);         // class
        route[7].feats.Add(FeatIds.FarShot);
        route[9].feats.Add(FeatIds.Cautious);                     // class
        route[10].feats.Add(FeatIds.DeadAim);
        route[13].feats.Add(FeatIds.PreciseShot);
        route[13].feats.Add(FeatIds.Alertness);                   // class
        route[16].feats.Add(FeatIds.DoubleTap);
        route[16].feats.Add(FeatIds.Attentive);                   // class
        route[19].feats.Add(FeatIds.AdvancedFirearmsProficiency);
        route[19].feats.Add(FeatIds.Meticulous);                  // class

        // Ability score increases: all Agility
        route[4].asi = StatType.Agility;
        route[8].asi = StatType.Agility;
        route[12].asi = StatType.Agility;
        route[16].asi = StatType.Agility;
        route[20].asi = StatType.Agility;

        // Skill Mastery: 3 + COG mod = 6 picks at level 5
        route[5].choiceOptions.Add(ChoiceNodeIds.InfiltratorSkillMasteryStealth);
        route[5].choiceOptions.Add(ChoiceNodeIds.InfiltratorSkillMasteryDisableDevice);
        route[5].choiceOptions.Add(ChoiceNodeIds.InfiltratorSkillMasteryAcrobatics);
        route[5].choiceOptions.Add(ChoiceNodeIds.InfiltratorSkillMasteryPerception);
        route[5].choiceOptions.Add(ChoiceNodeIds.InfiltratorSkillMasterySenseMotive);
        route[5].choiceOptions.Add(ChoiceNodeIds.InfiltratorSkillMasteryAthletics);

        // Path
        route[10].choiceOptions.Add(ChoiceNodeIds.InfiltratorGhost);

        // Skill ranks: every chosen skill kept at the cap
        PregenLevelPlan.FillSkillsAtCap(route, 1, 20,
            SkillType.Acrobatics, SkillType.Athletics, SkillType.Bluff, SkillType.DisableDevice,
            SkillType.Investigate, SkillType.Knowledge, SkillType.Perception, SkillType.SenseMotive,
            SkillType.Stealth, SkillType.ZeroGOperations);

        return route;
    }
}