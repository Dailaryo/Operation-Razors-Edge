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
        // 25-point buy, before species modifiers (applied by SpeciesDefinition)
        data.power     = 14;
        data.agility   = 10;
        data.endurance = 16;
        data.cognition = 9;
        data.insight   = 12;
        data.presence  = 10;

        // Background selections
        // Military: Athletics and Survival are already class skills → +1 competence each.
        data.backgroundFeatChoice = FeatIds.PersonalFirearmsProficiency;
        data.backgroundSkillChoices = new List<SkillType>
        {
            SkillType.Athletics,
            SkillType.Survival,
        };

        CharacterBuilder.BuildToLevel(data, 1);

        return data;
    }

    // Pre-written level-up route (debug bypass, see PregenProgression)
    public static Dictionary<int, PregenLevelPlan> BuildRoute()
    {
        var route = PregenLevelPlan.CreateRoute();

        // Feats: universal 1, 4, 7, 10, 13, 16, 19 | Dreadnought 3, 6, 9, 13, 16, 19
        route[1].feats.Add(FeatIds.PowerAttack);
        route[1].feats.Add(FeatIds.PersonalFirearmsProficiency);  // background grant
        route[1].feats.Add(FeatIds.SimpleWeaponsProficiency);     // species grant
        route[3].feats.Add(FeatIds.Cleave);                       // class
        route[4].feats.Add(FeatIds.GreatCleave);
        route[6].feats.Add(FeatIds.ImprovedBullRush);             // class
        route[7].feats.Add(FeatIds.ArmorProficiencyMedium);
        route[9].feats.Add(FeatIds.ArmorProficiencyHeavy);        // class
        route[10].feats.Add(FeatIds.CombatReflexes);
        route[13].feats.Add(FeatIds.ImprovedDamageThreshold);
        route[13].feats.Add(FeatIds.Toughness);                   // class
        route[16].feats.Add(FeatIds.FrightfulPresence);
        route[16].feats.Add(FeatIds.AdvancedFirearmsProficiency); // class
        route[19].feats.Add(FeatIds.Sunder);
        route[19].feats.Add(FeatIds.Endurance);                   // class

        // Ability score increases: all Endurance (retroactive HP at 8 and 16)
        route[4].asi = StatType.Endurance;
        route[8].asi = StatType.Endurance;
        route[12].asi = StatType.Endurance;
        route[16].asi = StatType.Endurance;
        route[20].asi = StatType.Endurance;

        // Path
        route[10].choiceOptions.Add(ChoiceNodeIds.DreadnoughtBastion);

        // Skill ranks: 1 per level
        PregenLevelPlan.FillSkillsAtCap(route, 1, 20, SkillType.Intimidate);

        return route;
    }
}