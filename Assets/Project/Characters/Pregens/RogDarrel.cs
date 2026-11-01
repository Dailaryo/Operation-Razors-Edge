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
            initiative: -1
        );

        // Identity
        data.speciesId = SpeciesIds.Pentian;
        data.classId = ClassIds.HelixWarrior;
        data.backgroundId = BackgroundIds.Gladiator;
        data.isEnemy = false;
        data.level = 1;

        // Ability Scores
        // 25-point buy, before species modifiers (applied by SpeciesDefinition)
        data.power = 16;
        data.agility = 10;
        data.endurance = 14;
        data.cognition = 9;
        data.insight = 13;
        data.presence = 9;

        // Background selections
        // Gladiator (choose 1): Intimidate is already a class skill → +1 competence.
        data.backgroundFeatChoice = FeatIds.Brawl;
        data.backgroundSkillChoices = new List<SkillType>
        {
            SkillType.Intimidate,
        };

        CharacterBuilder.BuildToLevel(data, 1);

        return data;
    }

    // Pre-written level-up route (debug bypass, see PregenProgression)
    public static Dictionary<int, PregenLevelPlan> BuildRoute()
    {
        var route = PregenLevelPlan.CreateRoute();

        // Feats: universal 1, 4, 7, 10, 13, 16, 19 | Helix Warrior 2, 6, 9, 13, 16, 19
        route[1].feats.Add(FeatIds.PowerAttack);
        route[1].feats.Add(FeatIds.Brawl);                        // background grant
        route[1].feats.Add(FeatIds.SimpleWeaponsProficiency);     // species grant
        route[2].feats.Add(FeatIds.ImprovedBrawl);                // class
        route[4].feats.Add(FeatIds.Cleave);
        route[6].feats.Add(FeatIds.CombatMartialArts);            // class
        route[7].feats.Add(FeatIds.GreatCleave);
        route[9].feats.Add(FeatIds.KnockoutPunch);                // class
        route[10].feats.Add(FeatIds.ImprovedCombatMartialArts);
        route[13].feats.Add(FeatIds.ImprovedKnockoutPunch);
        route[13].feats.Add(FeatIds.ImprovedGrapple);             // class
        route[16].feats.Add(FeatIds.AdvancedCombatMartialArts);
        route[16].feats.Add(FeatIds.CombatReflexes);              // class
        route[19].feats.Add(FeatIds.BlindFight);
        route[19].feats.Add(FeatIds.Streetfighting);              // class

        // Ability score increases: all Power
        route[4].asi = StatType.Power;
        route[8].asi = StatType.Power;
        route[12].asi = StatType.Power;
        route[16].asi = StatType.Power;
        route[20].asi = StatType.Power;

        // Mutations and path
        route[3].choiceOptions.Add(ChoiceNodeIds.HelixEnhancedMutationRapidHealing);
        route[5].choiceOptions.Add(ChoiceNodeIds.HelixImprovedMutationMusculature);
        route[8].choiceOptions.Add(ChoiceNodeIds.HelixGreaterMutationRapidHealing);
        route[10].choiceOptions.Add(ChoiceNodeIds.HelixWarriorBerserker);
        route[15].choiceOptions.Add(ChoiceNodeIds.HelixEnhancedMutationMusculature);   // Berserker
        route[18].choiceOptions.Add(ChoiceNodeIds.HelixImprovedMutationRapidHealing);  // Berserker

        // Skill ranks: 1 per level
        PregenLevelPlan.FillSkillsAtCap(route, 1, 20, SkillType.Intimidate);

        return route;
    }
}