using UnityEngine;
using System.Collections.Generic;

public static class AnyhaMcRae
{
    public static CharacterData Build()
    {
        var data = new CharacterData(
            id: PregenIds.AnyhaMcRae,
            name: "Anyha McRae",
            hp: 5,    // d6 (6) + END mod (-1)
            initiative: 2     // AGI mod +2; Improved Initiative adds +4
        );

        // Identity
        data.speciesId = SpeciesIds.Phytonian;
        data.classId = ClassIds.Telepath;
        data.backgroundId = BackgroundIds.Psychic;
        data.isEnemy = false;
        data.level = 1;

        // Ability Scores
        // 25-point buy, before species modifiers (Phytonian +2 AGI, -2 END, applied by SpeciesDefinition)
        data.power = 9;
        data.agility = 12;
        data.endurance = 10;
        data.cognition = 14;
        data.insight = 10;
        data.presence = 16;

        // Background selections
        // Psychic (choose 2): Diplomacy and SenseMotive are already class skills → +1 competence each.
        data.backgroundFeatChoice = FeatIds.IronWill;
        data.backgroundSkillChoices = new List<SkillType>
        {
            SkillType.Diplomacy,
            SkillType.SenseMotive,
        };

        CharacterBuilder.BuildToLevel(data, 1);

        return data;
    }

    // Pre-written level-up route (debug bypass, see PregenProgression)
    public static Dictionary<int, PregenLevelPlan> BuildRoute()
    {
        var route = PregenLevelPlan.CreateRoute();

        // Feats: universal 1, 4, 7, 10, 13, 16, 19 | Telepath 3, 6, 9, 13, 16, 19 (Telepath bonus feat list)
        route[1].feats.Add(FeatIds.ImprovedInitiative);
        route[1].feats.Add(FeatIds.SimpleWeaponsProficiency);     // species grant
        route[1].feats.Add(FeatIds.IronWill);                     // background grant
        route[3].feats.Add(FeatIds.Studious);                     // class
        route[4].feats.Add(FeatIds.Dodge);
        route[6].feats.Add(FeatIds.Deceptive);                    // class
        route[7].feats.Add(FeatIds.Renown);
        route[9].feats.Add(FeatIds.Attentive);                    // class
        route[10].feats.Add(FeatIds.PersonalFirearmsProficiency);
        route[13].feats.Add(FeatIds.Alertness);
        route[13].feats.Add(FeatIds.Focused);                     // class
        route[16].feats.Add(FeatIds.Educated);                    // requires Studious (L3)
        route[16].feats.Add(FeatIds.Stealthy);                    // class
        route[19].feats.Add(FeatIds.AdvancedFirearmsProficiency); // requires PFP (L10)
        route[19].feats.Add(FeatIds.Confident);                   // class

        // Ability score increases: all Presence (16 → 21)
        route[4].asi = StatType.Presence;
        route[8].asi = StatType.Presence;
        route[12].asi = StatType.Presence;
        route[16].asi = StatType.Presence;
        route[20].asi = StatType.Presence;

        // Trigger Power picks (power must already be known) and path
        route[2].choiceOptions.Add(TriggerPower.OptionIdFor(PowerIds.TelempathicProjection));
        route[5].choiceOptions.Add(TriggerPower.OptionIdFor(PowerIds.CharmPerson));
        route[8].choiceOptions.Add(TriggerPower.OptionIdFor(PowerIds.Suggestion));
        route[10].choiceOptions.Add(ChoiceNodeIds.TelepathEmpath);
        route[14].choiceOptions.Add(TriggerPower.OptionIdFor(PowerIds.LesserDomination)); // Empath Trigger Power 4

        // Powers. A higher tier frees the slot of the tier it replaces: list it BEFORE the replacement pick
        route[1].powers.AddRange(new[] { PowerIds.TelempathicProjection, PowerIds.Missive, PowerIds.Daze,
                                         PowerIds.CharmPerson });
        route[2].powers.Add(PowerIds.LesserMindlink);
        route[3].powers.Add(PowerIds.ConcealThoughts);
        route[4].powers.AddRange(new[] { PowerIds.DetectPsionics, PowerIds.Suggestion });
        route[5].powers.Add(PowerIds.DetectThoughts);
        route[6].powers.Add(PowerIds.LesserDomination);
        route[7].powers.AddRange(new[] { PowerIds.Distract, PowerIds.ObjectReading, PowerIds.BrainLock,
                                         PowerIds.MentalBlast });
        route[8].powers.Add(PowerIds.Domination);
        route[9].powers.Add(PowerIds.ForcedMindlink);
        route[10].powers.AddRange(new[] { PowerIds.Burst, PowerIds.FalseSensoryInput, PowerIds.Mindprobe });
        route[11].powers.AddRange(new[] { PowerIds.TailorMemory, PowerIds.Catapsi });
        route[12].powers.AddRange(new[] { PowerIds.Attraction, PowerIds.InflictPain, PowerIds.PsychicCrushII });
        route[13].powers.AddRange(new[] { PowerIds.CharmCreature, PowerIds.IntellectFortressIII });
        route[14].powers.AddRange(new[] { PowerIds.Mindwipe, PowerIds.SignalFeed,
                                          PowerIds.PsychicCrushIII,     // frees the 6th slot held by Crush II
                                          PowerIds.MassInflictPain,
                                          PowerIds.ThoughtShieldV });   // Empath bonus 6th
        route[15].powers.AddRange(new[] { PowerIds.Verve, PowerIds.Insanity });
        route[16].powers.AddRange(new[] { PowerIds.ClairaudienceClairvoyance, PowerIds.WithdrawAffliction,
                                          PowerIds.PsychicCrushIV,      // frees the 7th slot held by Crush III
                                          PowerIds.MindBlank });
        route[17].powers.AddRange(new[] { PowerIds.WhiteNoise, PowerIds.Mindlink, PowerIds.PowerResistance,
                                          PowerIds.TowerOfIronWillIV });
        route[18].powers.AddRange(new[] { PowerIds.InertialBarrier, PowerIds.MassSynesthesia,
                                          PowerIds.PsychicCrushV,       // frees the 8th slot held by Crush IV
                                          PowerIds.RecallDeath,
                                          PowerIds.PrimalRegression,    // Empath bonus 6th
                                          PowerIds.UnshakableZeal });   // Empath bonus 7th
        route[19].powers.AddRange(new[] { PowerIds.MassCloudMind, PowerIds.Microcosm });
        route[20].powers.AddRange(new[] { PowerIds.FarHand, PowerIds.SensitivityToPsychicImpressions });

        // Skill ranks: 5 per level (3 + COG +2), 100 total
        // Diplomacy, SenseMotive, Bluff at cap (20); Knowledge 1–16 (16); Perception 1–12 (12)
        PregenLevelPlan.FillSkillsAtCap(route, 1, 20, SkillType.Diplomacy, SkillType.SenseMotive, SkillType.Bluff);
        PregenLevelPlan.FillSkillsAtCap(route, 1, 16, SkillType.Knowledge);
        PregenLevelPlan.FillSkillsAtCap(route, 1, 12, SkillType.Perception);

        // Intimidate 13–20 (8) and Autohypnosis 17–20 (4): one rank per level, not caught up to the cap
        for (int lvl = 13; lvl <= 20; lvl++) route[lvl].skillRanks[SkillType.Intimidate] = 1;
        for (int lvl = 17; lvl <= 20; lvl++) route[lvl].skillRanks[SkillType.Autohypnosis] = 1;

        return route;
    }
}