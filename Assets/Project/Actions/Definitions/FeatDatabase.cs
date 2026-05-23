using UnityEngine;
using System.Collections.Generic;

public static class FeatDatabase
{
    public static Dictionary<string, FeatDefinition> Definitions =
        new Dictionary<string, FeatDefinition>()
        {
            //Combat Feats
            {FeatIds.PowerAttack, new FeatDefinition(
                featId: FeatIds.PowerAttack,
                displayName: "Power Attack",
                description: "Your strikes hit harder at the cost of precision.  Gain +3 damage on all attacks",
                category: FeatDefinition.FeatCategory.Combat,
                tags: new List<FeatDefinition.FeatTag> {FeatDefinition.FeatTag.Melee},
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.PowerAttack,
                    new List<StatusEffect>
                    {
                        new BonusDamageEffect(3)
                    }
                )
            )},

            {FeatIds.PreciseShot, new FeatDefinition(
                featId: FeatIds.PreciseShot,
                displayName: "Precise Shot",
                description: "Careful aim improves your ranged accuracy.  Gain +2 Accuracy",
                category: FeatDefinition.FeatCategory.Combat,
                tags: new List<FeatDefinition.FeatTag> {FeatDefinition.FeatTag.Ranged},
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.PreciseShot,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.Accuracy, 2, FeatIds.PreciseShot, StatModifier.BonusType.Competence)
                    }
                )
            )},

            //Passive Feats
            { FeatIds.Toughness, new FeatDefinition(
                featId:      FeatIds.Toughness,
                displayName: "Toughness",
                description: "Years of punishment have hardened your body. Gain +10 Max HP.",
                category:    FeatDefinition.FeatCategory.Passive,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.Toughness,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.MaxHP, 10, FeatIds.Toughness, StatModifier.BonusType.Competence)
                    }
                )
            )},

            { FeatIds.IronWill, new FeatDefinition(
                featId:      FeatIds.IronWill,
                displayName: "Iron Will",
                description: "Mental fortitude carries you through. Gain +2 to Willpower saves.",
                category:    FeatDefinition.FeatCategory.Passive,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.IronWill,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.Willpower,  2, FeatIds.IronWill, StatModifier.BonusType.Morale)
                    }
                )
            )},

            { FeatIds.CombatReflexes, new FeatDefinition(
                featId:      FeatIds.CombatReflexes,
                displayName: "Combat Reflexes",
                description: "Your battlefield awareness lets you strike at multiple openings. " +
                             "You may take a number of opportunity attacks per round equal to your Agility modifier (minimum 1).",
                category:    FeatDefinition.FeatCategory.Combat,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.CombatReflexes,
                    new List<StatusEffect>
                    {
                       new CombatReflexesEffect()
                    }
                )
            )},

            { FeatIds.TechLiterate, new FeatDefinition(
                featId: FeatIds.TechLiterate,
                displayName: "Tech Literate",
                description: "Extensive technical training sharpens your ability to interface with and disable complex systems. Gain +2 to ComputerUse and DisableDevice checks.",
                category: FeatDefinition.FeatCategory.Utility,
                tags: new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Tech },
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.TechLiterate,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.SkillComputerUse, 2, FeatIds.TechLiterate, StatModifier.BonusType.Competence),
                        new StatBonusEffect(StatType.SkillDisableDevice, 2, FeatIds.TechLiterate, StatModifier.BonusType.Competence)
                    }
                )
            )},

            { FeatIds.Cybertaker, new FeatDefinition(
                featId: FeatIds.Cybertaker,
                displayName: "Cybertaker",
                description: "Your body accepts cybernetic implants more readily than most. Gain +1 cybernetic implant capacity.",
                category: FeatDefinition.FeatCategory.Passive,
                tags: new List<FeatDefinition.FeatTag> {FeatDefinition.FeatTag.Tech },
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.Cybertaker,
                    new List<StatusEffect>
                    {
                        new CybertakerEffect()
                    }
                )
            )},

            { FeatIds.SensorLink, new FeatDefinition(
                featId:      FeatIds.SensorLink,
                displayName: "Sensor Link",
                description: "Cybernetic sensor integration enhances awareness. +1 to Perception checks. Once per turn as a free action, share your Perception result with adjacent allies.",
                category:    FeatDefinition.FeatCategory.Utility,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Tech },
                requiredLevel: 1,
                actionSource: new FeatActionSource(FeatIds.SensorLink, new List<string>{ActionIds.SensorLinkShare}),
                effectSource: new FeatEffectSource(
                    FeatIds.SensorLink,
                    new List<StatusEffect>
                    {
                        new SensorLinkFeatEffect()
                    }
                )
            )},

            { FeatIds.OverwatchSpecialist, new FeatDefinition(
                featId:      FeatIds.OverwatchSpecialist,
                displayName: "Overwatch Specialist",
                description: "You cover active field zones with deadly precision. When an enemy moves into a tile affected by your DeployField, you may make one ranged opportunity attack against them.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Tech, FeatDefinition.FeatTag.Ranged },
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.OverwatchSpecialist,
                    new List<StatusEffect>
                    {
                        new OverwatchSpecialistEffect()
                    }
                )
            )},
            { FeatIds.CyberneticMastery, new FeatDefinition(
                featId:      FeatIds.CyberneticMastery,
                displayName: "Cybernetic Mastery",
                description: "Deep integration with your cybernetic systems increases their capacity and efficiency. Cybernetic Power max +2. Overcharge costs 1 less Cybernetic Power (minimum 0).",
                category:    FeatDefinition.FeatCategory.Passive,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Tech },
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.CyberneticMastery,
                    new List<StatusEffect>
                    {
                        new CyberneticMasteryEffect()
                    }
                )
            )},

            { FeatIds.TacticalRelay, new FeatDefinition(
                featId:      FeatIds.TacticalRelay,
                displayName: "Tactical Relay",
                description: "Your neural data link carries targeting vectors as well as sensor data. Allies benefiting from NeuralSync gain +1 to damage rolls.",
                category:    FeatDefinition.FeatCategory.Tactical,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Tech },
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.TacticalRelay,
                    new List<StatusEffect>
                    {
                        new TacticalRelayEffect()
                    }
                )
            )},

            { FeatIds.Resilience, new FeatDefinition(
                featId:      FeatIds.Resilience,
                displayName: "Resilience",
                description: "Once per combat, when you would be reduced to 0 HP or lower, you instead remain at 1 HP.",
                category:    FeatDefinition.FeatCategory.Passive,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.Resilience,
                    new List<StatusEffect>
                    {
                        new ResilienceEffect()
                    }
                )
            )},

            // --- Unarmed Combat Chain ---

            { FeatIds.Brawl, new FeatDefinition(
                featId:      FeatIds.Brawl,
                displayName: "Brawl",
                description: "Training in unarmed combat upgrades your unarmed strike from 1d3 to 1d6 damage.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Melee },
                requiredLevel: 1
            )},

            { FeatIds.ImprovedBrawl, new FeatDefinition(
                featId:      FeatIds.ImprovedBrawl,
                displayName: "Improved Brawl",
                description: "Your unarmed strikes deal 1d8 damage.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Melee },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.Brawl }
            )},

            { FeatIds.CombatMartialArts, new FeatDefinition(
                featId:      FeatIds.CombatMartialArts,
                displayName: "Combat Martial Arts",
                description: "Your unarmed strikes add your Power modifier to damage.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Melee },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.Brawl }
            )},

            { FeatIds.ImprovedCombatMartialArts, new FeatDefinition(
                featId:      FeatIds.ImprovedCombatMartialArts,
                displayName: "Improved Combat Martial Arts",
                description: "Your unarmed attacks gain a bonus to attack rolls.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Melee },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.CombatMartialArts },
                effectSource: new FeatEffectSource(
                    FeatIds.ImprovedCombatMartialArts,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.Accuracy, 1, FeatIds.ImprovedCombatMartialArts, StatModifier.BonusType.Competence)
                    }
                )
            )},

            { FeatIds.AdvancedCombatMartialArts, new FeatDefinition(
                featId:      FeatIds.AdvancedCombatMartialArts,
                displayName: "Advanced Combat Martial Arts",
                description: "Capstone of the unarmed chain. Your unarmed strikes gain a further +1 to attack rolls and deal maximum damage on a critical hit.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Melee },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.ImprovedCombatMartialArts },
                effectSource: new FeatEffectSource(
                    FeatIds.AdvancedCombatMartialArts,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.Accuracy, 1, FeatIds.AdvancedCombatMartialArts, StatModifier.BonusType.Competence)
                    }
                )
            )},

            { FeatIds.KnockoutPunch, new FeatDefinition(
                featId:      FeatIds.KnockoutPunch,
                displayName: "Knockout Punch",
                description: "A precise unarmed strike can stun or knock out the target.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Melee },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.Brawl }
            )},

            { FeatIds.ImprovedKnockoutPunch, new FeatDefinition(
                featId:      FeatIds.ImprovedKnockoutPunch,
                displayName: "Improved Knockout Punch",
                description: "Improves the DC and duration of your Knockout Punch.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Melee },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.KnockoutPunch, FeatIds.ImprovedBrawl }
            )},

            { FeatIds.ImprovedGrapple, new FeatDefinition(
                featId:      FeatIds.ImprovedGrapple,
                displayName: "Improved Grapple",
                description: "Your grapple checks use your Power modifier and gain a +4 bonus.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Melee },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.ImprovedBrawl },
                effectSource: new FeatEffectSource(
                    FeatIds.ImprovedGrapple,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.MeleePower, 4, FeatIds.ImprovedGrapple, StatModifier.BonusType.Competence)
                    }
                )
            )},

            { FeatIds.Streetfighting, new FeatDefinition(
                featId:      FeatIds.Streetfighting,
                displayName: "Streetfighting",
                description: "You deal bonus damage against flanked or surprised targets.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Melee },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.Brawl }
            )},

            // --- Cleave Chain ---

            { FeatIds.Cleave, new FeatDefinition(
                featId:      FeatIds.Cleave,
                displayName: "Cleave",
                description: "When you drop a target with a melee attack, you may immediately make a bonus melee attack against an adjacent enemy.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Melee },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.PowerAttack }
            )},

            { FeatIds.GreatCleave, new FeatDefinition(
                featId:      FeatIds.GreatCleave,
                displayName: "Great Cleave",
                description: "You may Cleave any number of times per round with no limit.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Melee },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.Cleave }
            )},
            { FeatIds.SimpleWeaponsProficiency, new FeatDefinition(
                featId:      FeatIds.SimpleWeaponsProficiency,
                displayName: "Simple Weapons Proficiency",
                description: "Proficient with all simple weapons. No non-proficiency penalty on attack rolls.",
                category:    FeatDefinition.FeatCategory.Combat,
                requiredLevel: 1
            )},
            { FeatIds.BlindFight, new FeatDefinition(
                featId:      FeatIds.BlindFight,
                displayName: "Blind-Fight",
                description: "Reroll miss chances from concealment once. Invisible attackers gain no advantage on melee attacks against you.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Melee },
                requiredLevel: 1
            )},
        };

    public static FeatDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out FeatDefinition feat)) return feat;

        Debug.LogWarning($"[FeatDatabase] Feat not found: {id}");
        return null;
    }
}
