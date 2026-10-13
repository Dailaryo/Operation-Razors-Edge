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
                featId:      FeatIds.PreciseShot,
                displayName: "Precise Shot",
                description: "You have learned to shoot into melee without risk of hitting allies. " +
                             "Removes the −4 penalty on ranged attacks when a target is engaged in melee.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Ranged },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.PointBlankShot }
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
                        new StatBonusEffect(StatType.MaxHP, 10, FeatIds.Toughness, StatModifier.BonusType.Untyped)
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
                        new StatBonusEffect(StatType.Willpower,  2, FeatIds.IronWill, StatModifier.BonusType.Untyped)
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
                        new StatBonusEffect(StatType.SkillComputerUse, 2, FeatIds.TechLiterate, StatModifier.BonusType.Untyped),
                        new StatBonusEffect(StatType.SkillDisableDevice, 2, FeatIds.TechLiterate, StatModifier.BonusType.Untyped)
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
                        new StatBonusEffect(StatType.Accuracy, 1, FeatIds.ImprovedCombatMartialArts, StatModifier.BonusType.Untyped)
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
                        new StatBonusEffect(StatType.Accuracy, 1, FeatIds.AdvancedCombatMartialArts, StatModifier.BonusType.Untyped)
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
                        new StatBonusEffect(StatType.MeleePower, 4, FeatIds.ImprovedGrapple, StatModifier.BonusType.Untyped)
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

            // --- Ranged Combat Chain ---

            { FeatIds.PersonalFirearmsProficiency, new FeatDefinition(
                featId:      FeatIds.PersonalFirearmsProficiency,
                displayName: "Personal Firearms Proficiency",
                description: "You are proficient with all personal firearms. No non-proficiency penalty on attack rolls with pistols, rifles, and similar personal ranged weapons.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Ranged },
                requiredLevel: 1
            )},

            { FeatIds.AdvancedFirearmsProficiency, new FeatDefinition(
                featId:      FeatIds.AdvancedFirearmsProficiency,
                displayName: "Advanced Firearms Proficiency",
                description: "Extended training with high-powered ranged weapons. Unlocks precision rifles and scoped weapon categories. No non-proficiency penalty on advanced ranged weapons.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Ranged },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.PersonalFirearmsProficiency }
            )},

            { FeatIds.PointBlankShot, new FeatDefinition(
                featId:      FeatIds.PointBlankShot,
                displayName: "Point Blank Shot",
                description: "You are accurate at close range. Gain +1 to attack and damage rolls against targets within 30 feet.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Ranged },
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.PointBlankShot,
                    new List<StatusEffect>
                    {
                        // Conditional — applied by CombatResolver when target is within 30 ft (6 tiles)
                        // Placeholder: no persistent stat effect; checked at attack resolution
                    }
                )
            )},

            { FeatIds.FarShot, new FeatDefinition(
                featId:      FeatIds.FarShot,
                displayName: "Far Shot",
                description: "You can make accurate shots at extreme range. Ranged weapon range increments increase by 50%.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Ranged },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.PointBlankShot }
            )},

            { FeatIds.DeadAim, new FeatDefinition(
                featId:      FeatIds.DeadAim,
                displayName: "Dead Aim",
                description: "By taking a moment to line up your shot precisely, you make a devastating ranged attack. " +
                             "Spend your Standard and Move actions to make one ranged attack with a significant bonus to hit. " +
                             "Cannot be used if you moved this turn.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Ranged },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.FarShot },
                actionSource: new FeatActionSource(FeatIds.DeadAim, new List<string> { ActionIds.DeadAim })
            )},

            { FeatIds.DoubleTap, new FeatDefinition(
                featId:      FeatIds.DoubleTap,
                displayName: "Double Tap",
                description: "A rapid follow-up shot at the same target. Make a second ranged attack at −2 to hit immediately after your first.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Ranged },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.PersonalFirearmsProficiency }
            )},

            // --- Skill Feats ---

            { FeatIds.Stealthy, new FeatDefinition(
                featId:      FeatIds.Stealthy,
                displayName: "Stealthy",
                description: "Training in staying out of sight. Gain +2 to Stealth checks. May be taken twice.",
                category:    FeatDefinition.FeatCategory.Utility,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.Stealthy,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.SkillStealth, 2, FeatIds.Stealthy, StatModifier.BonusType.Untyped)
                    }
                )
            )},

            { FeatIds.Alertness, new FeatDefinition(
                featId:      FeatIds.Alertness,
                displayName: "Alertness",
                description: "Heightened situational awareness. Gain +2 to Perception and SenseMotive checks.",
                category:    FeatDefinition.FeatCategory.Utility,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.Alertness,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.SkillPerception,  2, FeatIds.Alertness, StatModifier.BonusType.Untyped),
                        new StatBonusEffect(StatType.SkillSenseMotive, 2, FeatIds.Alertness, StatModifier.BonusType.Untyped)
                    }
                )
            )},

            { FeatIds.Meticulous, new FeatDefinition(
                featId:      FeatIds.Meticulous,
                displayName: "Meticulous",
                description: "Careful, methodical attention to detail. Gain +2 to Investigate checks.",
                category:    FeatDefinition.FeatCategory.Utility,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.Meticulous,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.SkillInvestigate, 2, FeatIds.Meticulous, StatModifier.BonusType.Untyped)
                    }
                )
            )},

            { FeatIds.ImprovedInitiative, new FeatDefinition(
                featId:      FeatIds.ImprovedInitiative,
                displayName: "Improved Initiative",
                description: "Hair-trigger reflexes let you act before most opponents. Gain +4 to Initiative.",
                category:    FeatDefinition.FeatCategory.Combat,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.ImprovedInitiative,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.Initiative, 4, FeatIds.ImprovedInitiative, StatModifier.BonusType.Untyped)
                    }
                )
            )},

            { FeatIds.Dodge, new FeatDefinition(
                featId:      FeatIds.Dodge,
                displayName: "Dodge",
                description: "Constant movement and awareness keep you harder to hit. Gain +1 to Defense.",
                category:    FeatDefinition.FeatCategory.Passive,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.Dodge,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.Defense, 1, FeatIds.Dodge, StatModifier.BonusType.Dodge)
                    }
                )
            )},

            // --- Armor Proficiency Chain ---

            { FeatIds.ArmorProficiencyLight, new FeatDefinition(
                featId:      FeatIds.ArmorProficiencyLight,
                displayName: "Armor Proficiency (Light)",
                description: "Proficient with all light armor. No armor check penalty on attack rolls or skills.",
                category:    FeatDefinition.FeatCategory.Combat,
                requiredLevel: 1
            )},

            { FeatIds.ArmorProficiencyMedium, new FeatDefinition(
                featId:      FeatIds.ArmorProficiencyMedium,
                displayName: "Armor Proficiency (Medium)",
                description: "Proficient with all medium armor. No armor check penalty on attack rolls or skills.",
                category:    FeatDefinition.FeatCategory.Combat,
                requiredLevel: 1
            )},

            { FeatIds.ArmorProficiencyHeavy, new FeatDefinition(
                featId:      FeatIds.ArmorProficiencyHeavy,
                displayName: "Armor Proficiency (Heavy)",
                description: "Proficient with all heavy armor. No armor check penalty on attack rolls or skills.",
                category:    FeatDefinition.FeatCategory.Combat,
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.ArmorProficiencyMedium }
            )},

            // --- Power Attack Derivatives ---

            { FeatIds.ImprovedBullRush, new FeatDefinition(
                featId:      FeatIds.ImprovedBullRush,
                displayName: "Improved Bull Rush",
                description: "You can bull rush opponents without provoking an attack of opportunity. " +
                             "Gain +4 on opposed Power checks when attempting a bull rush.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Melee },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.PowerAttack }
            )},

            { FeatIds.Sunder, new FeatDefinition(
                featId:      FeatIds.Sunder,
                displayName: "Sunder",
                description: "You can attempt to damage or destroy an opponent's weapon or equipment. " +
                             "Make a melee attack against the item rather than the target; success deals damage to the item.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Melee },
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.PowerAttack }
            )},

            // --- Massive Damage Threshold / Fear / Endurance ---

            { FeatIds.ImprovedDamageThreshold, new FeatDefinition(
                featId:      FeatIds.ImprovedDamageThreshold,
                displayName: "Improved Damage Threshold",
                description: "Your body shrugs off blows that would incapacitate others. " +
                             "+3 to your massive damage threshold. May be taken multiple times; effects stack.",
                category:    FeatDefinition.FeatCategory.Passive,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.ImprovedDamageThreshold,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.MassiveDamageThreshold, 3,
                            FeatIds.ImprovedDamageThreshold, StatModifier.BonusType.Untyped)
                    }
                )
            )},

            { FeatIds.FrightfulPresence, new FeatDefinition(
                featId:      FeatIds.FrightfulPresence,
                displayName: "Frightful Presence",
                description: "Your combat ferocity unnerves weaker opponents. When you reduce an enemy to 0 HP, " +
                             "all enemies within 10 feet must make a Focus save (DC 10 + half level + PRE modifier) " +
                             "or become Shaken for 2 rounds.",
                category:    FeatDefinition.FeatCategory.Combat,
                tags:        new List<FeatDefinition.FeatTag> { FeatDefinition.FeatTag.Melee },
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.FrightfulPresence,
                    new List<StatusEffect>
                    {
                        new FrightfulPresenceEffect()
                    }
                )
            )},

            { FeatIds.Endurance, new FeatDefinition(
                featId:      FeatIds.Endurance,
                displayName: "Endurance",
                description: "Prolonged physical exertion is less taxing for you. Sleep in light armor without " +
                             "penalties. Extended marches and forced activity impose reduced fatigue.",
                category:    FeatDefinition.FeatCategory.Passive,
                requiredLevel: 1
            )},

            // --- Field Medic Feats ---

            { FeatIds.MedicalExpert, new FeatDefinition(
                featId:      FeatIds.MedicalExpert,
                displayName: "Medical Expert",
                description: "Advanced field medicine training. A medkit can be used to treat any number of " +
                             "patients per day, removing the standard single-use restriction.",
                category:    FeatDefinition.FeatCategory.Utility,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.MedicalExpert,
                    new List<StatusEffect>
                    {
                        new MedicalExpertEffect()
                    }
                )
            )},

            { FeatIds.DefensiveMartialArts, new FeatDefinition(
                featId:      FeatIds.DefensiveMartialArts,
                displayName: "Defensive Martial Arts",
                description: "Training in defensive techniques against melee attackers. Gain +1 Defense.",
                category:    FeatDefinition.FeatCategory.Passive,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.DefensiveMartialArts,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.Defense, 1, FeatIds.DefensiveMartialArts, StatModifier.BonusType.Dodge)
                    }
                )
            )},

            { FeatIds.Cautious, new FeatDefinition(
                featId:      FeatIds.Cautious,
                displayName: "Cautious",
                description: "Careful, methodical attention to your environment. " +
                             "Gain +2 to Investigate and DisableDevice checks.",
                category:    FeatDefinition.FeatCategory.Utility,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.Cautious,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.SkillInvestigate,   2, FeatIds.Cautious, StatModifier.BonusType.Untyped),
                        new StatBonusEffect(StatType.SkillDisableDevice, 2, FeatIds.Cautious, StatModifier.BonusType.Untyped),
                    }
                )
            )},

            { FeatIds.Studious, new FeatDefinition(
                featId:      FeatIds.Studious,
                displayName: "Studious",
                description: "A disciplined habit of study and research. Gain +2 to Knowledge checks.",
                category:    FeatDefinition.FeatCategory.Utility,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.Studious,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.SkillKnowledge, 2, FeatIds.Studious, StatModifier.BonusType.Untyped)
                    }
                )
            )},

            { FeatIds.Educated, new FeatDefinition(
                featId:      FeatIds.Educated,
                displayName: "Educated",
                description: "Deep academic training or implanted expertise across multiple knowledge domains. " +
                             "Gain +2 to Knowledge checks.",
                category:    FeatDefinition.FeatCategory.Utility,
                requiredLevel: 1,
                requiredFeatIds: new List<string> { FeatIds.Studious },
                effectSource: new FeatEffectSource(
                    FeatIds.Educated,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.SkillKnowledge, 2, FeatIds.Educated, StatModifier.BonusType.Untyped)
                    }
                )
            )},

            { FeatIds.Attentive, new FeatDefinition(
                featId:      FeatIds.Attentive,
                displayName: "Attentive",
                description: "You notice what others miss. Gain +2 to Investigate and Sense Motive checks.",
                category:    FeatDefinition.FeatCategory.Utility,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.Attentive,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.SkillInvestigate,  2, FeatIds.Attentive, StatModifier.BonusType.Untyped),
                        new StatBonusEffect(StatType.SkillSenseMotive, 2, FeatIds.Attentive, StatModifier.BonusType.Untyped),
                    }
                )
            )},
            { FeatIds.Deceptive, new FeatDefinition(
                featId:      FeatIds.Deceptive,
                displayName: "Deceptive",
                description: "You are adept at misdirection. Gain +2 to Bluff checks.",
                category:    FeatDefinition.FeatCategory.Utility,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.Deceptive,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.SkillBluff, 2, FeatIds.Deceptive, StatModifier.BonusType.Untyped),
                    }
                )
            )},

            { FeatIds.Renown, new FeatDefinition(
                featId:      FeatIds.Renown,
                displayName: "Renown",
                description: "Your name carries weight. Gain +2 to Diplomacy and Intimidate checks.",
                category:    FeatDefinition.FeatCategory.Utility,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.Renown,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.SkillDiplomacy,  2, FeatIds.Renown, StatModifier.BonusType.Untyped),
                        new StatBonusEffect(StatType.SkillIntimidate, 2, FeatIds.Renown, StatModifier.BonusType.Untyped),
                    }
                )
            )},

            { FeatIds.Focused, new FeatDefinition(
                featId:      FeatIds.Focused,
                displayName: "Focused",
                description: "Your balance and concentration hold under pressure. Gain +2 to Acrobatics and Autohypnosis checks.",
                category:    FeatDefinition.FeatCategory.Utility,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.Focused,
                    new List<StatusEffect>
                    {
                        new StatBonusEffect(StatType.SkillAcrobatics,   2, FeatIds.Focused, StatModifier.BonusType.Untyped),
                        new StatBonusEffect(StatType.SkillAutohypnosis, 2, FeatIds.Focused, StatModifier.BonusType.Untyped),
                    }
                )
            )},

            { FeatIds.Confident, new FeatDefinition(
                featId:      FeatIds.Confident,
                displayName: "Confident",
                description: "Your sense of self is hard to shake. Gain +2 on saves against mind-affecting effects.",
                category:    FeatDefinition.FeatCategory.Passive,
                requiredLevel: 1,
                effectSource: new FeatEffectSource(
                    FeatIds.Confident,
                    new List<StatusEffect>
                    {
                        new ConditionalSaveBonusEffect(StatType.Willpower, 2, EffectDescriptor.MindAffecting, FeatIds.Confident),
                        new ConditionalSaveBonusEffect(StatType.Fortitude, 2, EffectDescriptor.MindAffecting, FeatIds.Confident),
                        new ConditionalSaveBonusEffect(StatType.Evasion,   2, EffectDescriptor.MindAffecting, FeatIds.Confident),
                    }
                )
            )},

            // --- Class bonus-list feats: STUBS ---
            // No effect or prerequisites yet; authored when a pregen route or class feature needs them.
            { FeatIds.ActionBoost, new FeatDefinition(featId: FeatIds.ActionBoost, displayName: "Action Boost",
                description: "Stub.", category: FeatDefinition.FeatCategory.Utility) },
            { FeatIds.Acrobatic, new FeatDefinition(featId: FeatIds.Acrobatic, displayName: "Acrobatic",
                description: "Stub. +2 to Acrobatics checks.", category: FeatDefinition.FeatCategory.Utility) },
            { FeatIds.AdvancedTwoWeaponFighting, new FeatDefinition(featId: FeatIds.AdvancedTwoWeaponFighting, displayName: "Advanced Two-Weapon Fighting",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.ArmorProficiencyPowered, new FeatDefinition(featId: FeatIds.ArmorProficiencyPowered, displayName: "Armor Proficiency (powered)",
                description: "Stub. Proficient with powered armor.", category: FeatDefinition.FeatCategory.Passive) },
            { FeatIds.Athletic, new FeatDefinition(featId: FeatIds.Athletic, displayName: "Athletic",
                description: "Stub. +2 to Athletics checks.", category: FeatDefinition.FeatCategory.Utility) },
            { FeatIds.BurstFire, new FeatDefinition(featId: FeatIds.BurstFire, displayName: "Burst Fire",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.CombatExpertise, new FeatDefinition(featId: FeatIds.CombatExpertise, displayName: "Combat Expertise",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.CombatThrow, new FeatDefinition(featId: FeatIds.CombatThrow, displayName: "Combat Throw",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.ElusiveTarget, new FeatDefinition(featId: FeatIds.ElusiveTarget, displayName: "Elusive Target",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.ExoticFirearmsProficiency, new FeatDefinition(featId: FeatIds.ExoticFirearmsProficiency, displayName: "Exotic Firearms Proficiency",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.ExoticMeleeWeaponProficiency, new FeatDefinition(featId: FeatIds.ExoticMeleeWeaponProficiency, displayName: "Exotic Melee Weapon Proficiency",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.GreatFortitude, new FeatDefinition(featId: FeatIds.GreatFortitude, displayName: "Great Fortitude",
                description: "Stub. +2 on Resolve saves.", category: FeatDefinition.FeatCategory.Passive) },
            { FeatIds.ImprovedCombatThrow, new FeatDefinition(featId: FeatIds.ImprovedCombatThrow, displayName: "Improved Combat Throw",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.ImprovedDisarm, new FeatDefinition(featId: FeatIds.ImprovedDisarm, displayName: "Improved Disarm",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.ImprovedTrip, new FeatDefinition(featId: FeatIds.ImprovedTrip, displayName: "Improved Trip",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.ImprovedTwoWeaponFighting, new FeatDefinition(featId: FeatIds.ImprovedTwoWeaponFighting, displayName: "Improved Two-Weapon Fighting",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.LightningReflexes, new FeatDefinition(featId: FeatIds.LightningReflexes, displayName: "Lightning Reflexes",
                description: "Stub. +2 on Response saves.", category: FeatDefinition.FeatCategory.Passive) },
            { FeatIds.Mobility, new FeatDefinition(featId: FeatIds.Mobility, displayName: "Mobility",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.NervePinch, new FeatDefinition(featId: FeatIds.NervePinch, displayName: "Nerve Pinch",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.Nimble, new FeatDefinition(featId: FeatIds.Nimble, displayName: "Nimble",
                description: "Stub. +2 to Acrobatics and Disable Device checks.", category: FeatDefinition.FeatCategory.Utility) },
            { FeatIds.Run, new FeatDefinition(featId: FeatIds.Run, displayName: "Run",
                description: "Stub.", category: FeatDefinition.FeatCategory.Passive) },
            { FeatIds.SpringAttack, new FeatDefinition(featId: FeatIds.SpringAttack, displayName: "Spring Attack",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.Strafe, new FeatDefinition(featId: FeatIds.Strafe, displayName: "Strafe",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.TwoWeaponFighting, new FeatDefinition(featId: FeatIds.TwoWeaponFighting, displayName: "Two-Weapon Fighting",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.UltraImmuneSystem, new FeatDefinition(featId: FeatIds.UltraImmuneSystem, displayName: "Ultra Immune System",
                description: "Stub.", category: FeatDefinition.FeatCategory.Passive) },
            { FeatIds.WeaponFocus, new FeatDefinition(featId: FeatIds.WeaponFocus, displayName: "Weapon Focus",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.WhirlwindAttack, new FeatDefinition(featId: FeatIds.WhirlwindAttack, displayName: "Whirlwind Attack",
                description: "Stub.", category: FeatDefinition.FeatCategory.Combat) },
            { FeatIds.ZeroGTraining, new FeatDefinition(featId: FeatIds.ZeroGTraining, displayName: "Zero-G Training",
                description: "Stub.", category: FeatDefinition.FeatCategory.Utility) },
        };

    public static FeatDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out FeatDefinition feat)) return feat;

        Debug.LogWarning($"[FeatDatabase] Feat not found: {id}");
        return null;
    }
}
