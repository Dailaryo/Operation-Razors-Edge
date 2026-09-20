using UnityEngine;
using System.Collections.Generic;

public static class PassiveDatabase
{
    public static Dictionary<string, PassiveDefinition> Definitions =
        new Dictionary<string, PassiveDefinition>()
        {
            { PassiveIds.BlindSight, new PassiveDefinition(
                passive:    PassiveIds.BlindSight,
                name:  "Blindsight",
                desc:  "Within 60 feet, this unit requires no line of sight to locate or target creatures and objects. Range is unaffected by concealment or cover.",
                effect: new SimpleEffectSource(new List<StatusEffect>
                {
                    new BlindSightEffect(60)
                })
            )},
            { PassiveIds.NeuralAcuity1, new PassiveDefinition(
                passive: PassiveIds.NeuralAcuity1,
                name: "Neural Acuity 1",
                desc: "Cybernetic neural enhancements sharpen cognitive processing. Gain +1 competence bonus to ComputerUse, Knowledge, Investigate, Xenology, and DisableDevice checks.",
                effect: new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.SkillComputerUse, 1, PassiveIds.NeuralAcuity1, StatModifier.BonusType.Competence),
                    new StatBonusEffect(StatType.SkillKnowledge, 1, PassiveIds.NeuralAcuity1, StatModifier.BonusType.Competence),
                    new StatBonusEffect(StatType.SkillInvestigate, 1, PassiveIds.NeuralAcuity1, StatModifier.BonusType.Competence),
                    new StatBonusEffect(StatType.SkillXenology, 1, PassiveIds.NeuralAcuity1, StatModifier.BonusType.Competence),
                    new StatBonusEffect(StatType.SkillDisableDevice, 1, PassiveIds.NeuralAcuity1, StatModifier.BonusType.Competence),
                })
            )},
            { PassiveIds.IntegratedTargeting, new PassiveDefinition(
                passive: PassiveIds.IntegratedTargeting,
                name: "Integrated Targeting",
                desc: "Cybernetic targeting systems perform optimally at range. Gain +1 equipment bonus to Accuracy when no enemy is within melee range.",
                effect: new SimpleEffectSource(new List<StatusEffect>
                {
                    new IntegratedTargetingEffect()
                })
            )},
            { PassiveIds.SubdermalWeave1, new PassiveDefinition(
                passive: PassiveIds.SubdermalWeave1,
                name: "Subdermal Weave 1",
                desc: "Light subdermal plating reduces incoming physical damage. Gain DR 1/—.",
                effect: new SimpleEffectSource(new List<StatusEffect>
                {
                    new DamageReductionEffect(1)
                })
            )},
            { PassiveIds.AdaptiveInterface, new PassiveDefinition(
                passive: PassiveIds.AdaptiveInterface,
                name: "Adaptive Interface",
                desc: "The Technosavant's implants adapt more efficiently to her biology over time. Gain +1 cybernetic implant capacity.",
                effect: new SimpleEffectSource(new List<StatusEffect>
                {
                    new AdaptiveInterfaceEffect()
                })
            )},
            { PassiveIds.ThreatAnalysis, new PassiveDefinition(
                passive: PassiveIds.ThreatAnalysis,
                name: "Threat Analysis",
                desc: "A passive threat-recognition implant continuously feeds situational data, improving reaction time. Gain +1 insight bonus to Response saves.",
                effect: new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.Evasion, 1, PassiveIds.ThreatAnalysis, StatModifier.BonusType.Insight)
                })
            )},
            { PassiveIds.NeuralAcuity2, new PassiveDefinition(
                passive: PassiveIds.NeuralAcuity2,
                name: "Neural Acuity 2",
                desc: "Further cybernetic refinement of cognitive processing. Gain +2 competence bonus to ComputerUse, Knowledge, Investigate, Xenology, and DisableDevice checks.",
                effect: new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.SkillComputerUse,   2, PassiveIds.NeuralAcuity2, StatModifier.BonusType.Competence),
                    new StatBonusEffect(StatType.SkillKnowledge,     2, PassiveIds.NeuralAcuity2, StatModifier.BonusType.Competence),
                    new StatBonusEffect(StatType.SkillInvestigate,   2, PassiveIds.NeuralAcuity2, StatModifier.BonusType.Competence),
                    new StatBonusEffect(StatType.SkillXenology,      2, PassiveIds.NeuralAcuity2, StatModifier.BonusType.Competence),
                    new StatBonusEffect(StatType.SkillDisableDevice, 2, PassiveIds.NeuralAcuity2, StatModifier.BonusType.Competence),
                })
            )},
            { PassiveIds.SubdermalWeave2, new PassiveDefinition(
                passive: PassiveIds.SubdermalWeave2,
                name: "Subdermal Weave 2",
                desc: "Reinforced subdermal plating provides greater protection against incoming damage. Gain DR 2/—.",
                effect: new SimpleEffectSource(new List<StatusEffect>
                {
                    new DamageReductionEffect(2)
                })
            )},
            { PassiveIds.OverclockProtocol, new PassiveDefinition(
                passive: PassiveIds.OverclockProtocol,
                name: "Overclock Protocol",
                desc: "Combat systems pre-load targeting solutions before engagement. Gain +1 equipment bonus to Initiative.",
                effect: new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.Initiative, 1, PassiveIds.OverclockProtocol, StatModifier.BonusType.Equipment)
                })
            )},
            { PassiveIds.NeuralAcuity3, new PassiveDefinition(
                passive: PassiveIds.NeuralAcuity3,
                name:    "Neural Acuity III",
                desc:    "Maximum cybernetic cognitive enhancement. +3 competence to all Cognition-linked skill checks.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.SkillComputerUse,   3, PassiveIds.NeuralAcuity3, StatModifier.BonusType.Competence),
                    new StatBonusEffect(StatType.SkillKnowledge,     3, PassiveIds.NeuralAcuity3, StatModifier.BonusType.Competence),
                    new StatBonusEffect(StatType.SkillInvestigate,   3, PassiveIds.NeuralAcuity3, StatModifier.BonusType.Competence),
                    new StatBonusEffect(StatType.SkillXenology,      3, PassiveIds.NeuralAcuity3, StatModifier.BonusType.Competence),
                    new StatBonusEffect(StatType.SkillDisableDevice, 3, PassiveIds.NeuralAcuity3, StatModifier.BonusType.Competence),
                })
            )},
            { PassiveIds.HardenedChassis1, new PassiveDefinition(
                passive: PassiveIds.HardenedChassis1,
                name:    "Hardened Chassis I",
                desc:    "Reinforced plating provides greater structural integrity. +2 to Focus saves, +5 max HP.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new HardenedChassisEffect(resolveBonus: 2, hpBonus: 5)
                })
            )},
            { PassiveIds.BallisticIntegration, new PassiveDefinition(
                passive: PassiveIds.BallisticIntegration,
                name:    "Ballistic Integration",
                desc:    "Ranged weapon systems integrate directly with targeting implants. Ranged attacks gain bonus damage equal to half COG modifier.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new BallisticIntegrationEffect()
                })
            )},
            { PassiveIds.HardenedChassis2, new PassiveDefinition(
                passive: PassiveIds.HardenedChassis2,
                name:    "Hardened Chassis II",
                desc:    "Heavy-grade subdermal reinforcement. DR increases to 4/—, stacking with Subdermal Weave for total DR 6/—.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new HardenedChassisEffect(resolveBonus: 2, hpBonus: 0),
                    new DamageReductionEffect(4)
                })
            )},
            { PassiveIds.TargetLock, new PassiveDefinition(
                passive: PassiveIds.TargetLock,
                name:    "Target Lock",
                desc:    "After a successful DiagnoseWeakness, your first attack against that target deals +2d6 damage.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new TargetLockEffect()
                })
            )},
            { PassiveIds.KineticAbsorption, new PassiveDefinition(
                passive: PassiveIds.KineticAbsorption,
                name:    "Kinetic Absorption",
                desc:    "Once per combat, automatically reduce incoming physical damage by COG modifier.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new KineticAbsorptionEffect()
                })
            )},
            { PassiveIds.WeaponsIntegration, new PassiveDefinition(
                passive: PassiveIds.WeaponsIntegration,
                name:    "Weapons Integration",
                desc:    "Full cybernetic weapon integration. Add COG modifier to all ranged attack rolls.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new WeaponsIntegrationEffect()
                })
            )},
            { PassiveIds.IronApotheosis, new PassiveDefinition(
                passive: PassiveIds.IronApotheosis,
                name:    "Iron Apotheosis",
                desc:    "Capstone: all Ironclad passives active at full value simultaneously. Cybernetic Power max +2.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new ApotheosisEffect("IronApotheosis")
                })
            )},
            { PassiveIds.SignalScramble1, new PassiveDefinition(
                passive: PassiveIds.SignalScramble1,
                name:    "Signal Scramble I",
                desc:    "DiagnoseWeakness also reduces the target's Accuracy by 1 for its duration.",
                effect:  null // Applied conditionally via DiagnoseWeakness — see SignalScrambleEffect
            )},
            { PassiveIds.GhostStep, new PassiveDefinition(
                passive: PassiveIds.GhostStep,
                name:    "Ghost Step",
                desc:    "Stealth becomes a class skill. +2 competence bonus to Stealth checks.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new GhostStepEffect()
                })
            )},
            { PassiveIds.SignalScramble2, new PassiveDefinition(
                passive: PassiveIds.SignalScramble2,
                name:    "Signal Scramble II",
                desc:    "SystemOverride range increases by 2 tiles. Signal Scramble Accuracy penalty increases to 2.",
                effect:  null // Applied conditionally — see DiagnoseWeaknessEffect and ActionDatabase
            )},
            { PassiveIds.FeedbackLoop, new PassiveDefinition(
                passive: PassiveIds.FeedbackLoop,
                name:    "Feedback Loop",
                desc:    "When SystemOverride succeeds, the target takes COG modifier damage.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new FeedbackLoopEffect()
                })
            )},
            { PassiveIds.PhantomProtocol, new PassiveDefinition(
                passive: PassiveIds.PhantomProtocol,
                name:    "Phantom Protocol",
                desc:    "Stealth bonus increases to +4. Move at full speed while stealthed.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new PhantomProtocolEffect()
                })
            )},
            { PassiveIds.GhostApotheosis, new PassiveDefinition(
                passive: PassiveIds.GhostApotheosis,
                name:    "Ghost Apotheosis",
                desc:    "Capstone: all Ghost Protocol passives active at full value simultaneously. Cybernetic Power max +2.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new ApotheosisEffect("GhostApotheosis")
                })
            )},
            { PassiveIds.TacticalUplink1, new PassiveDefinition(
                passive: PassiveIds.TacticalUplink1,
                name:    "Tactical Uplink I",
                desc:    "Allies within 3 tiles gain +1 equipment bonus to Accuracy.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new TacticalUplinkEffect(1)
                })
            )},
            { PassiveIds.CommandPresence, new PassiveDefinition(
                passive: PassiveIds.CommandPresence,
                name:    "Command Presence",
                desc:    "Your Presence modifier (minimum 0) is added to team initiative rolls.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new CommandPresenceEffect()
                })
            )},
            { PassiveIds.TacticalUplink2, new PassiveDefinition(
                passive: PassiveIds.TacticalUplink2,
                name:    "Tactical Uplink II",
                desc:    "Tactical Uplink bonus increases to +2, range increases to 5 tiles.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new TacticalUplinkEffect(2)
                })
            )},
            { PassiveIds.RelayNetwork, new PassiveDefinition(
                passive: PassiveIds.RelayNetwork,
                name:    "Relay Network",
                desc:    "DiagnoseWeakness now affects all enemies within a 3-tile radius of the primary target.",
                effect:  null // Applied via DiagnoseWeakness — see DiagnoseWeaknessEffect
            )},
            { PassiveIds.DistributedProcessing, new PassiveDefinition(
                passive: PassiveIds.DistributedProcessing,
                name:    "Distributed Processing",
                desc:    "While NeuralSync is active, linked allies gain +1 to all saves.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new DistributedProcessingEffect()
                })
            )},
            { PassiveIds.CommandOverride, new PassiveDefinition(
                passive: PassiveIds.CommandOverride,
                name:    "Command Override",
                desc:    "Once per combat, grant one ally within 6 tiles a free action outside their normal turn.",
                effect:  null // Delivered as an action — see ActionIds.CommandOverrideAction
            )},
            { PassiveIds.VanguardApotheosis, new PassiveDefinition(
                passive: PassiveIds.VanguardApotheosis,
                name:    "Vanguard Apotheosis",
                desc:    "Capstone: all Vanguard passives active at full value simultaneously. Cybernetic Power max +2.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new ApotheosisEffect("VanguardApotheosis")
                })
            )},
            // --- Helix Warrior Core Mutations ---
            { PassiveIds.HelixAdaptivePhysiology, new PassiveDefinition(
                passive: PassiveIds.HelixAdaptivePhysiology,
                name:    "Adaptive Physiology",
                desc:    "The Helix Warrior's altered biology repairs damage continuously. Fast healing 1 while above 0 HP.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.FastHealing, 1, PassiveIds.HelixAdaptivePhysiology, StatModifier.BonusType.Untyped)
                })
            )},
            { PassiveIds.HelixRapidHealing, new PassiveDefinition(
                passive: PassiveIds.HelixRapidHealing,
                name:    "Rapid Healing",
                desc:    "Mutation: Regenerate 1 HP per round while above 0 HP.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.FastHealing, 1, PassiveIds.HelixRapidHealing, StatModifier.BonusType.Untyped)
                })
            )},
            { PassiveIds.HelixRapidHealingEnhanced, new PassiveDefinition(
                passive: PassiveIds.HelixRapidHealingEnhanced,
                name:    "Rapid Healing (Enhanced)",
                desc:    "Mutation upgraded: Fast healing increases to 2.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.FastHealing, 2, PassiveIds.HelixRapidHealingEnhanced, StatModifier.BonusType.Untyped)
                })
            )},
            { PassiveIds.HelixRapidHealingGreater, new PassiveDefinition(
                passive: PassiveIds.HelixRapidHealingGreater,
                name:    "Rapid Healing (Greater)",
                desc:    "Mutation upgraded: Fast healing 3. Automatically stabilize at 0 HP.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.FastHealing, 3, PassiveIds.HelixRapidHealingGreater, StatModifier.BonusType.Untyped)
                })
            )},
            { PassiveIds.HelixEnhancedMusculature, new PassiveDefinition(
                passive: PassiveIds.HelixEnhancedMusculature,
                name:    "Enhanced Musculature",
                desc:    "Mutation: +2 species bonus on melee damage rolls and Power checks.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.MeleePower, 2, PassiveIds.HelixEnhancedMusculature, StatModifier.BonusType.Racial)
                })
            )},
            { PassiveIds.HelixEnhancedMusculatureImproved, new PassiveDefinition(
                passive: PassiveIds.HelixEnhancedMusculatureImproved,
                name:    "Enhanced Musculature (Improved)",
                desc:    "Mutation upgraded: +4 bonus on melee damage rolls and Power checks.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.MeleePower, 4, PassiveIds.HelixEnhancedMusculatureImproved, StatModifier.BonusType.Racial)
                })
            )},
            { PassiveIds.HelixAdrenalineSurge, new PassiveDefinition(
                passive: PassiveIds.HelixAdrenalineSurge,
                name:    "Adrenaline Surge",
                desc:    "Mutation (active): Once per combat, gain +4 Power for 3 rounds as a free action.",
                effect:  null // Delivered as an action — see ActionIds.HelixAdrenalineSurge
            )},
            { PassiveIds.HelixPredatoryReflexes, new PassiveDefinition(
                passive: PassiveIds.HelixPredatoryReflexes,
                name:    "Predatory Reflexes",
                desc:    "Mutation: +2 species bonus to Initiative.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.Initiative, 2, PassiveIds.HelixPredatoryReflexes, StatModifier.BonusType.Racial)
                })
            )},
            { PassiveIds.HelixBoneDensity, new PassiveDefinition(
                passive: PassiveIds.HelixBoneDensity,
                name:    "Bone Density",
                desc:    "Mutation: +1 natural armor bonus to Defense.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.NaturalArmor, 1, PassiveIds.HelixBoneDensity, StatModifier.BonusType.Racial)
                })
            )},
            { PassiveIds.HelixAdrenalFrenzy, new PassiveDefinition(
                passive: PassiveIds.HelixAdrenalFrenzy,
                name:    "Adrenal Frenzy",
                desc:    "Mutation upgraded (active): +6 Power and +1 attack roll for 4 rounds, 1/combat. Replaces Adrenaline Surge.",
                effect:  null // Delivered as an action — see ActionIds.HelixAdrenalFrenzy
            )},
            // --- Helix Warrior Core Class Features ---
            { PassiveIds.HelixPowerfulBuild, new PassiveDefinition(
                passive: PassiveIds.HelixPowerfulBuild,
                name:    "Powerful Build",
                desc:    "Counts as one size category larger for grapple, bull rush, overrun, and weapon sizing. Does not change actual size.",
                effect:  null // Flag checked by grapple and forced movement resolvers — no stat effect
            )},
            { PassiveIds.HelixBrutalStrike, new PassiveDefinition(
                passive: PassiveIds.HelixBrutalStrike,
                name:    "Brutal Strike",
                desc:    "Once per round, one melee attack deals +1d6 bonus damage if you moved at least 10 feet before attacking.",
                effect:  null // Hooks into pre-damage phase — scoped during Berserker path authoring
            )},
            // --- Helix Warrior Berserker Path ---
            { PassiveIds.HelixRampage, new PassiveDefinition(
                passive: PassiveIds.HelixRampage,
                name:    "Rampage",
                desc:    "On any melee kill, gain a bonus melee attack against an adjacent enemy.",
                effect:  null // Hooks into post-kill resolution — scoped during Berserker action authoring
            )},
            { PassiveIds.HelixBloodlust, new PassiveDefinition(
                passive: PassiveIds.HelixBloodlust,
                name:    "Bloodlust",
                desc:    "While below 50% HP, gain bonus melee damage.",
                effect:  null // Conditional — scoped during Berserker action authoring
            )},
            { PassiveIds.HelixSavageMomentum, new PassiveDefinition(
                passive: PassiveIds.HelixSavageMomentum,
                name:    "Savage Momentum",
                desc:    "Each consecutive melee kill this combat increases your attack bonus by 1, stacking up to 3.",
                effect:  null // State-tracking — scoped during Berserker action authoring
            )},
            { PassiveIds.HelixKillingBlow, new PassiveDefinition(
                passive: PassiveIds.HelixKillingBlow,
                name:    "Killing Blow",
                desc:    "Melee attacks against targets at 25% HP or below automatically deal maximum damage.",
                effect:  null // Hooks into pre-damage phase — scoped during Berserker action authoring
            )},
            { PassiveIds.HelixDevastatingCleave, new PassiveDefinition(
                passive: PassiveIds.HelixDevastatingCleave,
                name:    "Devastating Cleave",
                desc:    "Cleave attacks deal an additional die of damage.",
                effect:  null // Hooks into Cleave resolution — scoped during Berserker action authoring
            )},
            // --- Helix Warrior Apex Predator Path ---
            { PassiveIds.HelixHuntingInstinct, new PassiveDefinition(
                passive: PassiveIds.HelixHuntingInstinct,
                name:    "Hunting Instinct",
                desc:    "Bonus melee damage against targets not currently in melee with any ally.",
                effect:  null // Conditional — scoped during Apex Predator action authoring
            )},
            { PassiveIds.HelixLethalTempo, new PassiveDefinition(
                passive: PassiveIds.HelixLethalTempo,
                name:    "Lethal Tempo",
                desc:    "Each consecutive attack against the same target this turn gains +1 to attack and damage, stacking up to 3.",
                effect:  null // State-tracking — scoped during Apex Predator action authoring
            )},
            { PassiveIds.HelixExposedWeakness, new PassiveDefinition(
                passive: PassiveIds.HelixExposedWeakness,
                name:    "Exposed Weakness",
                desc:    "Marked Prey targets take increased damage from all sources.",
                effect:  null // Applied via Mark Prey action — scoped during Apex Predator action authoring
            )},
            { PassiveIds.HelixAmbushStriker, new PassiveDefinition(
                passive: PassiveIds.HelixAmbushStriker,
                name:    "Ambush Striker",
                desc:    "Deal bonus damage on the opening strike of each combat.",
                effect:  null // Conditional on combat-start state — scoped during Apex Predator action authoring
            )},
            { PassiveIds.HelixApexSpeed, new PassiveDefinition(
                passive: PassiveIds.HelixApexSpeed,
                name:    "Apex Speed",
                desc:    "Move speed increases by 10 feet.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.MoveSpeed, 2, PassiveIds.HelixApexSpeed, StatModifier.BonusType.Racial)
                })
            )},
            // --- Helix Warrior Iron Flesh Path ---
            { PassiveIds.HelixHardenedHide, new PassiveDefinition(
                passive: PassiveIds.HelixHardenedHide,
                name:    "Hardened Hide",
                desc:    "Your skin hardens into a natural armored surface. DR 2/—.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new DamageReductionEffect(2)
                })
            )},
            { PassiveIds.HelixReactivePlating, new PassiveDefinition(
                passive: PassiveIds.HelixReactivePlating,
                name:    "Reactive Plating",
                desc:    "Enemies that hit you with a melee attack take minor damage.",
                effect:  null // Hooks into post-hit resolution — scoped during Iron Flesh action authoring
            )},
            { PassiveIds.HelixWoundSuppression, new PassiveDefinition(
                passive: PassiveIds.HelixWoundSuppression,
                name:    "Wound Suppression",
                desc:    "Ignore the first penalty die from wound effects each round.",
                effect:  null // Hooks into wound/debuff resolution — scoped during Iron Flesh action authoring
            )},
            { PassiveIds.HelixFortressBody, new PassiveDefinition(
                passive: PassiveIds.HelixFortressBody,
                name:    "Fortress Body",
                desc:    "Natural armor increases by 2 and critical threat range against you is reduced by 1.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.NaturalArmor, 2, PassiveIds.HelixFortressBody, StatModifier.BonusType.Racial)
                })
            )},
            { PassiveIds.HelixUnbreakable, new PassiveDefinition(
                passive: PassiveIds.HelixUnbreakable,
                name:    "Unbreakable",
                desc:    "Once per encounter, cannot be reduced below 1 HP.",
                effect:  null // Same pattern as ResilienceEffect — scoped during Iron Flesh action authoring
            )},
            { PassiveIds.HelixLivingWall, new PassiveDefinition(
                passive: PassiveIds.HelixLivingWall,
                name:    "Living Wall",
                desc:    "Adjacent allies gain DR 1/— and +1 to Fortitude saves while you are above 0 HP.",
                effect:  null // Aura — scoped during Iron Flesh action authoring
            )},
            { PassiveIds.JuggernautPassive, new PassiveDefinition(
                passive: PassiveIds.JuggernautPassive,
                name:    "Juggernaut",
                desc:    "Cannot be moved involuntarily by bull rush, knockback, or forced displacement unless the source is Large or larger. Ignores the first 5 feet of any forced movement that does bypass the immunity.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new JuggernautEffect()
                })
            )},
            { PassiveIds.LowLightVision, new PassiveDefinition(
                passive: PassiveIds.LowLightVision,
                name:    "Low-Light Vision",
                desc:    "Sees twice as far as baseline human in dim light. Retains color and detail.",
                effect:  null // Visual/detection flag — no stat effect currently implemented
            )},

            // --- Cryptian Species Traits ---
            { PassiveIds.CryptianEmpathicSense, new PassiveDefinition(
                passive: PassiveIds.CryptianEmpathicSense,
                name:    "Empathic Sense",
                desc:    "+2 species bonus on Perception and SenseMotive checks. Cryptian biological sensitivity " +
                         "to emotional and behavioral cues operates independently of skill rank investment.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.SkillPerception,   2, PassiveIds.CryptianEmpathicSense, StatModifier.BonusType.Racial),
                    new StatBonusEffect(StatType.SkillSenseMotive,  2, PassiveIds.CryptianEmpathicSense, StatModifier.BonusType.Racial),
                })
            )},

            { PassiveIds.CryptianSleepImmunity, new PassiveDefinition(
                passive: PassiveIds.CryptianSleepImmunity,
                name:    "Sleep Immunity",
                desc:    "Immune to sleep effects and forced unconsciousness powers.",
                effect:  null // Flag checked by sleep/unconsciousness resolvers — no stat effect
            )},

            { PassiveIds.CryptianFocusSaveBonus, new PassiveDefinition(
                passive: PassiveIds.CryptianFocusSaveBonus,
                name:    "Cryptian Will",
                desc:    "+1 species bonus on Focus saves against mind-affecting psionic powers.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.Willpower, 1, PassiveIds.CryptianFocusSaveBonus, StatModifier.BonusType.Racial),
                })
            )},

            { PassiveIds.CryptianConcealedDetection, new PassiveDefinition(
                passive: PassiveIds.CryptianConcealedDetection,
                name:    "Concealed Detection",
                desc:    "Automatically makes a free Perception or Investigate check when passing within 5 ft of a " +
                         "concealed door, panel, or hidden passage, as if actively searching.",
                effect:  null // Triggered by movement resolver — no stat effect
            )},

            { PassiveIds.CryptianLightSensitivity, new PassiveDefinition(
                passive: PassiveIds.CryptianLightSensitivity,
                name:    "Light Sensitivity",
                desc:    "-1 penalty to all attack rolls and skill checks in bright light conditions.",
                effect:  null // Applied conditionally by lighting resolver — no persistent stat effect
            )},

            // --- Infiltrator Base Class Features ---
            { PassiveIds.InfiltratorSweep, new PassiveDefinition(
                passive: PassiveIds.InfiltratorSweep,
                name:    "Sweep",
                desc:    "At the start of each encounter, make a free Perception check with +4 bonus covering 30 ft. " +
                         "DC 10 reveals unconcealed threats, exits, and surveillance. Concealed elements are revealed " +
                         "if the check meets their Stealth result.",
                effect:  null // Hooks into encounter-start logic in CombatManager — no persistent stat effect
            )},

            { PassiveIds.InfiltratorImprovedEvasion, new PassiveDefinition(
                passive: PassiveIds.InfiltratorImprovedEvasion,
                name:    "Improved Evasion",
                desc:    "On a successful Response save against an area effect, take no damage. " +
                         "On a failed save, take half damage. Requires light or no armor.",
                effect:  null // Checked during damage resolution — no persistent stat effect
            )},

            { PassiveIds.InfiltratorSkillMastery, new PassiveDefinition(
                passive: PassiveIds.InfiltratorSkillMastery,
                name:    "Skill Mastery",
                desc:    "Select 3 + COG modifier skills. You may always Take 10 on those skills even under pressure.",
                effect:  null // Tag checked by SkillResolver — selected skills stored via ChoiceNode
            )},

            { PassiveIds.InfiltratorWithoutATrace, new PassiveDefinition(
                passive: PassiveIds.InfiltratorWithoutATrace,
                name:    "Without a Trace",
                desc:    "Attempts to detect or track your use of Stealth, Acrobatics, DisableDevice, Athletics, " +
                         "or Escape Artist suffer a −4 penalty.",
                effect:  null // Applies penalty to opposing checks — scoped during stealth/detection authoring
            )},

            { PassiveIds.InfiltratorImprovised, new PassiveDefinition(
                passive: PassiveIds.InfiltratorImprovised,
                name:    "Improvised Implements",
                desc:    "No penalty on improvised weapons, Athletics (Climb), or DisableDevice checks without proper tools.",
                effect:  null // Removes penalty flags — checked by SkillResolver and CombatResolver
            )},

            { PassiveIds.InfiltratorImprovisedDmg, new PassiveDefinition(
                passive: PassiveIds.InfiltratorImprovisedDmg,
                name:    "Improvised Weapon Damage",
                desc:    "Improvised weapons deal damage as one size category larger.",
                effect:  null // Hooks into damage calculation — scoped during weapon damage authoring
            )},

            { PassiveIds.InfiltratorImprovedSweep, new PassiveDefinition(
                passive: PassiveIds.InfiltratorImprovedSweep,
                name:    "Improved Sweep",
                desc:    "Sweep now also reveals the relative threat level of each detected enemy — " +
                         "stronger, on par, or weaker by HD comparison.",
                effect:  null // Extends Sweep output — hooks into CombatManager encounter-start logic
            )},

            // --- Infiltrator Ghost Path ---
            { PassiveIds.InfiltratorGhostShadowStep, new PassiveDefinition(
                passive: PassiveIds.InfiltratorGhostShadowStep,
                name:    "Shadow Step",
                desc:    "Moving through dim or no-light tiles does not trigger opportunity attacks.",
                effect:  null // Checked by OA resolution — scoped during M22 opportunity attack authoring
            )},

            { PassiveIds.InfiltratorGhostDeathBlow, new PassiveDefinition(
                passive: PassiveIds.InfiltratorGhostDeathBlow,
                name:    "Death Blow",
                desc:    "+2d6 damage on any attack against a target unaware of your position.",
                effect:  null // Conditional — checked during attack resolution against unaware targets
            )},

            { PassiveIds.InfiltratorGhostShadowStepII, new PassiveDefinition(
                passive: PassiveIds.InfiltratorGhostShadowStepII,
                name:    "Shadow Step II",
                desc:    "Shadow Step benefit extends to movement through partial cover tiles.",
                effect:  null // Extends Shadow Step flag — checked by OA resolution
            )},

            { PassiveIds.InfiltratorGhostPerfectStrike, new PassiveDefinition(
                passive: PassiveIds.InfiltratorGhostPerfectStrike,
                name:    "Perfect Strike",
                desc:    "Attacks made from stealth ignore all Defense bonuses granted by cover.",
                effect:  null // Checked during Defense calculation when attacker is hidden
            )},

            { PassiveIds.InfiltratorGhostAssassination, new PassiveDefinition(
                passive: PassiveIds.InfiltratorGhostAssassination,
                name:    "Assassination",
                desc:    "Kills made from stealth grant an immediate free Hide action.",
                effect:  null // Hooks into post-kill resolution — scoped during stealth authoring (M23)
            )},

            { PassiveIds.InfiltratorGhostShadowWalk, new PassiveDefinition(
                passive: PassiveIds.InfiltratorGhostShadowWalk,
                name:    "Shadow Walk",
                desc:    "Movement while hidden never triggers opportunity attacks.",
                effect:  null // Extends OA immunity to all movement while hidden — scoped during M22/M23
            )},

            { PassiveIds.InfiltratorGhostApotheosis, new PassiveDefinition(
                passive: PassiveIds.InfiltratorGhostApotheosis,
                name:    "Ghost Apotheosis",
                desc:    "Permanently hidden while not attacking or otherwise revealing position.",
                effect:  null // Persistent stealth state — scoped during M23 stealth authoring
            )},

            // --- Infiltrator Operative Path ---
            { PassiveIds.InfiltratorOperativeReadTheRoom, new PassiveDefinition(
                passive: PassiveIds.InfiltratorOperativeReadTheRoom,
                name:    "Read the Room",
                desc:    "Sweep triggers automatically at the start of every round, not just encounter start.",
                effect:  null // Extends Sweep cadence — hooks into TurnManager round-start logic
            )},

            { PassiveIds.InfiltratorOperativeExploitOpening, new PassiveDefinition(
                passive: PassiveIds.InfiltratorOperativeExploitOpening,
                name:    "Exploit Opening",
                desc:    "+1d6 damage against targets that currently have an active debuff.",
                effect:  null // Conditional — checked during damage resolution
            )},

            { PassiveIds.InfiltratorOperativeTacticalBlind, new PassiveDefinition(
                passive: PassiveIds.InfiltratorOperativeTacticalBlind,
                name:    "Tactical Blind",
                desc:    "Sweep reveals the stealth positions of hidden enemies.",
                effect:  null // Extends Sweep output — hooks into CombatManager Sweep logic
            )},

            { PassiveIds.InfiltratorOperativeSurgicalPrecision, new PassiveDefinition(
                passive: PassiveIds.InfiltratorOperativeSurgicalPrecision,
                name:    "Surgical Precision",
                desc:    "Ignore the first 5 points of damage reduction on all attacks.",
                effect:  null // Checked during DR calculation — scoped during damage authoring
            )},

            { PassiveIds.InfiltratorOperativeControlTempo, new PassiveDefinition(
                passive: PassiveIds.InfiltratorOperativeControlTempo,
                name:    "Control the Tempo",
                desc:    "Applying a debuff to an enemy grants a free 5 ft step.",
                effect:  null // Hooks into debuff application — scoped during status effect authoring
            )},

            { PassiveIds.InfiltratorOperativeApotheosis, new PassiveDefinition(
                passive: PassiveIds.InfiltratorOperativeApotheosis,
                name:    "Operative Apotheosis",
                desc:    "All Operative debuffs may stack once per target.",
                effect:  null // Modifies debuff stacking rules — scoped during status effect authoring
            )},

            // --- Infiltrator Phantom Path ---
            { PassiveIds.InfiltratorPhantomBlurredPresence, new PassiveDefinition(
                passive: PassiveIds.InfiltratorPhantomBlurredPresence,
                name:    "Blurred Presence",
                desc:    "20% miss chance on all attacks against you.",
                effect:  null // Miss chance flag — checked during attack resolution
            )},

            { PassiveIds.InfiltratorPhantomEchoStep, new PassiveDefinition(
                passive: PassiveIds.InfiltratorPhantomEchoStep,
                name:    "Echo Step",
                desc:    "Leave a false position signature after each move action.",
                effect:  null // Detection misdirection — scoped during M23 stealth/detection authoring
            )},

            { PassiveIds.InfiltratorPhantomMindFog, new PassiveDefinition(
                passive: PassiveIds.InfiltratorPhantomMindFog,
                name:    "Mind Fog",
                desc:    "Enemies that fail a Focus save against your Phantom actions are Slowed for 1 round.",
                effect:  null // Applied on failed save — hooks into Phantom action resolution
            )},

            { PassiveIds.InfiltratorPhantomGhostwalk, new PassiveDefinition(
                passive: PassiveIds.InfiltratorPhantomGhostwalk,
                name:    "Ghostwalk",
                desc:    "Move through occupied tiles freely.",
                effect:  null // Overrides movement collision — scoped during MovementResolver authoring
            )},

            { PassiveIds.InfiltratorPhantomStrike, new PassiveDefinition(
                passive: PassiveIds.InfiltratorPhantomStrike,
                name:    "Phantom Strike",
                desc:    "Your attacks are treated as psionic for the purpose of bypassing physical damage reduction.",
                effect:  null // Attack type flag — checked during DR calculation
            )},

            { PassiveIds.InfiltratorPhantomApotheosis, new PassiveDefinition(
                passive: PassiveIds.InfiltratorPhantomApotheosis,
                name:    "Phantom Apotheosis",
                desc:    "Once per round, any one Phantom action may be used as a free action.",
                effect:  null // Action economy modifier — scoped during Phantom action authoring
            )},

            // --- Osteo Species Traits ---
            { PassiveIds.OsteoCombatConditioning, new PassiveDefinition(
                passive: PassiveIds.OsteoCombatConditioning,
                name:    "Combat Conditioning",
                desc:    "+1 species bonus on all attack rolls. Instinctual aggression encoded in Osteo genetics.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.BaseAttackBonus, 1, PassiveIds.OsteoCombatConditioning, StatModifier.BonusType.Racial)
                })
            )},

            { PassiveIds.OsteoFortitudeHardening, new PassiveDefinition(
                passive: PassiveIds.OsteoFortitudeHardening,
                name:    "Fortitude Hardening",
                desc:    "+1 species bonus on Fortitude/Resolve saves. Enhanced skeletal and metabolic resilience.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.Fortitude, 1, PassiveIds.OsteoFortitudeHardening, StatModifier.BonusType.Racial)
                })
            )},

            { PassiveIds.OsteoAggressionAura, new PassiveDefinition(
                passive: PassiveIds.OsteoAggressionAura,
                name:    "Aggression Aura",
                desc:    "Osteos do not register fear from lesser threats. Fear effects with a sourceLevel greater than 0 " +
                         "but lower than this unit's level are blocked entirely.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new AggressionAuraEffect()
                })
            )},

            // --- Dreadnought Base Class Features ---
            { PassiveIds.DreadnoughtFearless, new PassiveDefinition(
                passive: PassiveIds.DreadnoughtFearless,
                name:    "Fearless",
                desc:    "Immune to all fear effects. Shaken, Frightened, and Panicked cannot be applied by any source.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new DreadnoughtFearlessEffect()
                })
            )},

            { PassiveIds.DreadnoughtStability, new PassiveDefinition(
                passive: PassiveIds.DreadnoughtStability,
                name:    "Stability",
                desc:    "+4 bonus on all checks to resist being tripped, overrun, knocked prone, or pushed back by bull rush. " +
                         "Bastion path level 12 upgrades this to full forced movement immunity.",
                effect:  null // Flag checked by forced movement resolver — no persistent stat effect
            )},

            { PassiveIds.DreadnoughtUnhindered, new PassiveDefinition(
                passive: PassiveIds.DreadnoughtUnhindered,
                name:    "Unhindered",
                desc:    "Armor worn by this unit has its armor check penalty reduced by 2 (minimum 0).",
                effect:  null // Applied during armor equip evaluation — no persistent stat effect
            )},

            { PassiveIds.DreadnoughtSteamroller, new PassiveDefinition(
                passive: PassiveIds.DreadnoughtSteamroller,
                name:    "Steamroller",
                desc:    "May attempt overrun without moving first. Gain +2 on trip checks when the target braces against the overrun.",
                effect:  null // Checked by forced movement resolver — no persistent stat effect
            )},

            { PassiveIds.DreadnoughtKnockdown, new PassiveDefinition(
                passive: PassiveIds.DreadnoughtKnockdown,
                name:    "Knockdown",
                desc:    "On any melee hit where Power modifier applies to damage, the target must make a Fortitude save " +
                         "(DC equal to damage dealt) or fall prone.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new DreadnoughtKnockdownEffect()
                })
            )},

            { PassiveIds.DreadnoughtHeavyArtillery, new PassiveDefinition(
                passive: PassiveIds.DreadnoughtHeavyArtillery,
                name:    "Heavy Artillery",
                desc:    "All weapons are treated as one size category smaller for the purpose of one-hand wield requirements. " +
                         "Two-handed weapons may be wielded in one hand.",
                effect:  null // Checked by weapon equip and attack resolution — no persistent stat effect
            )},

            { PassiveIds.DreadnoughtMasterDefender, new PassiveDefinition(
                passive: PassiveIds.DreadnoughtMasterDefender,
                name:    "Master Defender",
                desc:    "Gain a Defense bonus when fighting defensively (+2) or taking total defense (+2/+4/+6 scaling with " +
                         "class level at levels 4/7/10). Requires medium, heavy, or powered armor. " +
                         "Bastion path level 13 removes the stance requirement.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new DreadnoughtMasterDefenderEffect()
                })
            )},

            // --- Dreadnought Bastion Path ---
            { PassiveIds.BastionDR2, new PassiveDefinition(
                passive: PassiveIds.BastionDR2,
                name:    "Damage Reduction 2/—",
                desc:    "Your physical mass and conditioning absorb incoming damage. DR 2/—.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new DamageReductionEffect(2)
                })
            )},

            { PassiveIds.BastionDR4, new PassiveDefinition(
                passive: PassiveIds.BastionDR4,
                name:    "Damage Reduction 4/—",
                desc:    "Reinforced physique. DR 4/—.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new DamageReductionEffect(4)
                })
            )},

            { PassiveIds.BastionDR6, new PassiveDefinition(
                passive: PassiveIds.BastionDR6,
                name:    "Damage Reduction 6/—",
                desc:    "Exceptional physical resilience. DR 6/—.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new DamageReductionEffect(6)
                })
            )},

            { PassiveIds.BastionDR8, new PassiveDefinition(
                passive: PassiveIds.BastionDR8,
                name:    "Damage Reduction 8/—",
                desc:    "Apotheosis-level physical endurance. DR 8/—.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new DamageReductionEffect(8)
                })
            )},

            { PassiveIds.BastionAnchored, new PassiveDefinition(
                passive: PassiveIds.BastionAnchored,
                name:    "Anchored",
                desc:    "Immune to all forced movement. Stability bonus increases to +8. " +
                         "No bull rush, knockback, or displacement can move this unit.",
                effect:  null // Flag checked by forced movement resolver — supersedes DreadnoughtStability
            )},

            { PassiveIds.BastionMasterDefenderAlways, new PassiveDefinition(
                passive: PassiveIds.BastionMasterDefenderAlways,
                name:    "Master Defender (Always On)",
                desc:    "Master Defender bonus applies regardless of stance. " +
                         "No longer requires fighting defensively or total defense.",
                effect:  null // Flag read by DreadnoughtMasterDefenderEffect.GetStatModifiers
            )},

            { PassiveIds.BastionProtectiveAura, new PassiveDefinition(
                passive: PassiveIds.BastionProtectiveAura,
                name:    "Protective Aura",
                desc:    "Adjacent allies gain +2 Defense bonus while you are above 0 HP. " +
                         "Bastion Apotheosis extends this to all allies within 6 tiles.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new BastionProtectiveAuraEffect()
                })
            )},

            { PassiveIds.BastionFortress, new PassiveDefinition(
                passive: PassiveIds.BastionFortress,
                name:    "Fortress",
                desc:    "Master Defender bonus doubles when fighting defensively.",
                effect:  null // Read by DreadnoughtMasterDefenderEffect — doubles bonus when stance active
            )},

            { PassiveIds.BastionLastStand, new PassiveDefinition(
                passive: PassiveIds.BastionLastStand,
                name:    "Last Stand",
                desc:    "Once per combat, when you would be reduced to 0 HP or below, you instead remain at 1 HP for 1 round.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new BastionLastStandEffect()
                })
            )},

            { PassiveIds.BastionApotheosis, new PassiveDefinition(
                passive: PassiveIds.BastionApotheosis,
                name:    "Bastion Apotheosis",
                desc:    "DR increases to 8/—. Protective Aura extends to all allies within 6 tiles.",
                effect:  null // DR delivered by BastionDR8; aura range handled by BastionProtectiveAuraEffect
            )},

            // --- Dreadnought Juggernaut Path ---
            { PassiveIds.JuggernautShockwave, new PassiveDefinition(
                passive: PassiveIds.JuggernautShockwave,
                name:    "Shockwave",
                desc:    "Knockdown now applies to all enemies adjacent to the primary target on proc. " +
                         "All adjacent enemies must make the same Fortitude save.",
                effect:  null // Extends DreadnoughtKnockdownEffect — checked by resolver when passive is present
            )},

            { PassiveIds.JuggernautMomentum, new PassiveDefinition(
                passive: PassiveIds.JuggernautMomentum,
                name:    "Momentum",
                desc:    "Each consecutive melee attack against the same target this combat deals +1 cumulative bonus damage, " +
                         "stacking without limit.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new JuggernautMomentumEffect()
                })
            )},

            { PassiveIds.JuggernautCriticalMass, new PassiveDefinition(
                passive: PassiveIds.JuggernautCriticalMass,
                name:    "Critical Mass",
                desc:    "Melee critical hits force a Knockdown save at +5 DC bonus.",
                effect:  null // Checked by crit resolution — hooks into attack resolution when crits are implemented
            )},

            { PassiveIds.JuggernautRampage, new PassiveDefinition(
                passive: PassiveIds.JuggernautRampage,
                name:    "Rampage",
                desc:    "A killing melee blow grants one immediate free melee attack against an adjacent enemy.",
                effect:  null // Hooks into HandleUnitDeath — same pattern as FrightfulPresenceEffect
            )},

            { PassiveIds.JuggernautAbilitySurgeUpgrade, new PassiveDefinition(
                passive: PassiveIds.JuggernautAbilitySurgeUpgrade,
                name:    "Ability Surge Upgrade",
                desc:    "Ability Surge Power bonus increases to +10.",
                effect:  null // Flag read by AbilitySurge action factory — no persistent stat effect
            )},

            { PassiveIds.JuggernautApotheosis, new PassiveDefinition(
                passive: PassiveIds.JuggernautApotheosis,
                name:    "Juggernaut Apotheosis",
                desc:    "Ability Surge becomes permanent and always active. Cannot be knocked prone by any source.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new JuggernautApotheosisEffect()
                })
            )},

            // --- Dreadnought Artillerist Path ---
            { PassiveIds.ArtilleristSiegeTraining, new PassiveDefinition(
                passive: PassiveIds.ArtilleristSiegeTraining,
                name:    "Siege Training",
                desc:    "Heavy Artillery extends to all weapon categories. " +
                         "Two-handed heavy weapons are treated as one-handed.",
                effect:  null // Extends DreadnoughtHeavyArtillery flag — checked by weapon resolver
            )},

            { PassiveIds.ArtilleristBrace, new PassiveDefinition(
                passive: PassiveIds.ArtilleristBrace,
                name:    "Brace",
                desc:    "Attacks made from a stationary position (no movement this turn) gain +1 to attack rolls " +
                         "and +2 to damage rolls.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new ArtilleristBraceEffect()
                })
            )},

            { PassiveIds.ArtilleristHeavyArtilleristCaliber, new PassiveDefinition(
                passive: PassiveIds.ArtilleristHeavyArtilleristCaliber,
                name:    "Heavy Caliber",
                desc:    "Ranged weapon damage dice increase by one step (d6→d8, d8→d10, etc.).",
                effect:  null // Applied during damage initialization — hooks into RollBaseDamage
            )},

            { PassiveIds.ArtilleristFortifiedPosition, new PassiveDefinition(
                passive: PassiveIds.ArtilleristFortifiedPosition,
                name:    "Fortified Position",
                desc:    "When stationary for a full round, gain DR 2/— and +4 Defense until your next movement.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new ArtilleristFortifiedPositionEffect()
                })
            )},

            // --- Radix Species Traits ---
            { PassiveIds.RadixStability, new PassiveDefinition(
                passive: PassiveIds.RadixStability,
                name:    "Stability",
                desc:    "+4 species bonus on checks to resist bull rush and trip attempts while standing on the ground.",
                effect:  null // No StatType for bull rush/trip resistance yet — stub until M22
            )},

            { PassiveIds.RadixToxinResilience, new PassiveDefinition(
                passive: PassiveIds.RadixToxinResilience,
                name:    "Toxin Resilience",
                desc:    "+2 species bonus on Fortitude saves against poisons, drugs, and toxins.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.Fortitude, 2, PassiveIds.RadixToxinResilience, StatModifier.BonusType.Racial)
                })
            )},

            { PassiveIds.RadixPsionicResilience, new PassiveDefinition(
                passive: PassiveIds.RadixPsionicResilience,
                name:    "Psionic Resilience",
                desc:    "+2 species bonus on Focus saves against psionic powers and effects.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.Willpower, 2, PassiveIds.RadixPsionicResilience, StatModifier.BonusType.Racial)
                })
            )},

            { PassiveIds.RadixStructuralIntuition, new PassiveDefinition(
                passive: PassiveIds.RadixStructuralIntuition,
                name:    "Structural Intuition",
                desc:    "Free Investigate check (DC 15) when passing within 10 feet of structural weaknesses, hidden engineering features, unusual construction, or concealed passages in built environments.",
                effect:  null // Tile-adjacency hook not yet implemented — stub until detection system is in place
            )},

            { PassiveIds.RadixRepairAptitude, new PassiveDefinition(
                passive: PassiveIds.RadixRepairAptitude,
                name:    "Repair Aptitude",
                desc:    "+2 species bonus on Repair checks.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.SkillRepair, 2, PassiveIds.RadixRepairAptitude, StatModifier.BonusType.Racial)
                })
            )},

            { PassiveIds.RadixDarkvision, new PassiveDefinition(
                passive: PassiveIds.RadixDarkvision,
                name:    "Darkvision",
                desc:    "Can see in total darkness up to 60 feet. Vision is black and white only.",
                effect:  null // Stub — darkvision is mechanically distinct from low-light vision; hook pending lighting system
            )},

            // --- Field Medic Base Class Features ---
            { PassiveIds.FieldMedicMedicalSpecialist1, new PassiveDefinition(
                passive: PassiveIds.FieldMedicMedicalSpecialist1,
                name:    "Medical Specialist",
                desc:    "+1 competence bonus on all TreatInjury checks.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.SkillTreatInjury, 1,
                        PassiveIds.FieldMedicMedicalSpecialist1, StatModifier.BonusType.Competence)
                })
            )},

            { PassiveIds.FieldMedicMedicalSpecialist2, new PassiveDefinition(
                passive: PassiveIds.FieldMedicMedicalSpecialist2,
                name:    "Medical Specialist",
                desc:    "+2 competence bonus on all TreatInjury checks.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.SkillTreatInjury, 2,
                        PassiveIds.FieldMedicMedicalSpecialist2, StatModifier.BonusType.Competence)
                })
            )},

            { PassiveIds.FieldMedicMedicalSpecialist3, new PassiveDefinition(
                passive: PassiveIds.FieldMedicMedicalSpecialist3,
                name:    "Medical Specialist",
                desc:    "+3 competence bonus on all TreatInjury checks.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.SkillTreatInjury, 3,
                        PassiveIds.FieldMedicMedicalSpecialist3, StatModifier.BonusType.Competence)
                })
            )},

            { PassiveIds.FieldMedicExpertHealer, new PassiveDefinition(
                passive: PassiveIds.FieldMedicExpertHealer,
                name:    "Expert Healer",
                desc:    "Successful TreatInjury heals restore additional HP equal to the healer's Field Medic class level.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new FieldMedicExpertHealerEffect()
                })
            )},

            { PassiveIds.FieldMedicMedicalMastery, new PassiveDefinition(
                passive: PassiveIds.FieldMedicMedicalMastery,
                name:    "Medical Mastery",
                desc:    "May always Take 10 on TreatInjury checks regardless of stress or distractions.",
                effect:  null // CanTakeTen hook not yet implemented in SkillCheckExecutor — stub consistent with InfiltratorSkillMastery
            )},

            // --- Field Medic Trauma Surgeon Path ---
            { PassiveIds.TSExtendedWindow, new PassiveDefinition(
                passive: PassiveIds.TSExtendedWindow,
                name:    "Extended Window",
                desc:    "Minor Medical Miracle window extends to 1 minute (6 rounds).",
                effect:  null // Flag read by MinorMedicalMiracleEffect — stub until effect reads passive list
            )},

            { PassiveIds.TSTriageProtocol, new PassiveDefinition(
                passive: PassiveIds.TSTriageProtocol,
                name:    "Triage Protocol",
                desc:    "At encounter start, automatically identifies all wounded allies in priority order by HP percentage.",
                effect:  null // Sweep-style passive — hooks into combat start initialization; deferred to UI pass (M27)
            )},

            { PassiveIds.TSExpertHealerUpgrade, new PassiveDefinition(
                passive: PassiveIds.TSExpertHealerUpgrade,
                name:    "Expert Healer (Upgraded)",
                desc:    "Expert Healer bonus increases to class level + INS modifier (was class level alone).",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new TSExpertHealerUpgradeEffect()
                })
            )},

            { PassiveIds.TSSteadyHands, new PassiveDefinition(
                passive: PassiveIds.TSSteadyHands,
                name:    "Steady Hands",
                desc:    "TreatInjury checks are never subject to circumstance penalties from combat, movement, or environmental conditions.",
                effect:  null // Circumstance penalty system not yet implemented — stub consistent with MedicalMastery
            )},

            { PassiveIds.TSMedicalSpecialist4, new PassiveDefinition(
                passive: PassiveIds.TSMedicalSpecialist4,
                name:    "Medical Specialist",
                desc:    "+4 competence bonus on all TreatInjury checks.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.SkillTreatInjury, 4,
                        PassiveIds.TSMedicalSpecialist4, StatModifier.BonusType.Competence)
                })
            )},

            { PassiveIds.TSMedicalSpecialist5, new PassiveDefinition(
                passive: PassiveIds.TSMedicalSpecialist5,
                name:    "Medical Specialist",
                desc:    "+5 competence bonus on all TreatInjury checks.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.SkillTreatInjury, 5,
                        PassiveIds.TSMedicalSpecialist5, StatModifier.BonusType.Competence)
                })
            )},

            { PassiveIds.TSExtendedMiracle, new PassiveDefinition(
                passive: PassiveIds.TSExtendedMiracle,
                name:    "Extended Miracle",
                desc:    "Medical Miracle window extends to 10 minutes (60 rounds). DC reduced to 35.",
                effect:  null // Flag read by MedicalMiracleEffect — stub until effect reads passive list
            )},

            { PassiveIds.TSSurgeonApotheosis, new PassiveDefinition(
                passive: PassiveIds.TSSurgeonApotheosis,
                name:    "Surgeon Apotheosis",
                desc:    "Medical Miracle DC reduced to 25. On success, restored HP is maximized (no roll — full max HP).",
                effect:  null // Flag read by MedicalMiracleEffect — stub until effect reads passive list
            )},

            // --- Field Medic Combat Medic Path ---
            { PassiveIds.CMCombatTriage, new PassiveDefinition(
                passive: PassiveIds.CMCombatTriage,
                name:    "Combat Triage",
                desc:    "TreatInjury and HealAlly actions cost a Move action instead of a Standard action.",
                effect:  null // Action category override — requires ActionDefinition to be mutable per-unit or
                              // a resolver-side check. Deferred to action economy pass; stub for now.
            )},

            { PassiveIds.CMTacticalPositioning, new PassiveDefinition(
                passive: PassiveIds.CMTacticalPositioning,
                name:    "Tactical Positioning",
                desc:    "Does not provoke opportunity attacks when moving to or treating a wounded ally.",
                effect:  null // AoO system not yet implemented — stub consistent with M22 scope
            )},

            { PassiveIds.CMUnderFire, new PassiveDefinition(
                passive: PassiveIds.CMUnderFire,
                name:    "Under Fire",
                desc:    "Can Take 10 on TreatInjury even while taking fire. Extends Medical Mastery.",
                effect:  null // CanTakeTen hook not yet implemented — stub consistent with MedicalMastery
            )},

            { PassiveIds.CMSuppressiveCare, new PassiveDefinition(
                passive: PassiveIds.CMSuppressiveCare,
                name:    "Suppressive Care",
                desc:    "When Harvey treats an ally, that ally gains +2 Defense until the start of Harvey's next turn.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new CMSuppressiveCareEffect()
                })
            )},

            { PassiveIds.CMMedicalSpecialist4, new PassiveDefinition(
                passive: PassiveIds.CMMedicalSpecialist4,
                name:    "Medical Specialist",
                desc:    "+4 competence bonus on all TreatInjury checks.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.SkillTreatInjury, 4,
                        PassiveIds.CMMedicalSpecialist4, StatModifier.BonusType.Competence)
                })
            )},

            { PassiveIds.CMMedicalSpecialist5, new PassiveDefinition(
                passive: PassiveIds.CMMedicalSpecialist5,
                name:    "Medical Specialist",
                desc:    "+5 competence bonus on all TreatInjury checks.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.SkillTreatInjury, 5,
                        PassiveIds.CMMedicalSpecialist5, StatModifier.BonusType.Competence)
                })
            )},

            // --- Field Medic Field Pharmacist Path ---
            { PassiveIds.FPCompoundExpertise, new PassiveDefinition(
                passive: PassiveIds.FPCompoundExpertise,
                name:    "Compound Expertise",
                desc:    "Pharmaceutical items Harvey applies have their effect duration doubled.",
                effect:  null // Duration doubling requires buff system to expose duration as a modifiable value.
                              // Stub until buff duration architecture is formalized.
            )},

            { PassiveIds.FPAntitoxinProtocol, new PassiveDefinition(
                passive: PassiveIds.FPAntitoxinProtocol,
                name:    "Antitoxin Protocol",
                desc:    "Successful TreatInjury checks also remove one active poison or toxin status from the target.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new FPAntitoxinProtocolEffect()
                })
            )},

            { PassiveIds.FPCompoundStim, new PassiveDefinition(
                passive: PassiveIds.FPCompoundStim,
                name:    "Compound Stim",
                desc:    "Stim Injection now grants +2 to two stats simultaneously.",
                effect:  null // Flag read by FPStimInjectionEffect — stub until stim action is implemented
            )},

            { PassiveIds.FPOverdose, new PassiveDefinition(
                passive: PassiveIds.FPOverdose,
                name:    "Overdose",
                desc:    "When a stim is applied to an already-stimmed ally, the second stim grants +1 bonus instead of overwriting.",
                effect:  null // Stacking logic for stim effects — deferred to stim action implementation
            )},

            { PassiveIds.FPPharmacologicalMastery, new PassiveDefinition(
                passive: PassiveIds.FPPharmacologicalMastery,
                name:    "Pharmacological Mastery",
                desc:    "Immune to all poison, drug, and toxin effects.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new FPPharmacologicalMasteryEffect()
                })
            )},

            { PassiveIds.FPMedicalSpecialist4, new PassiveDefinition(
                passive: PassiveIds.FPMedicalSpecialist4,
                name:    "Medical Specialist",
                desc:    "+4 competence bonus on all TreatInjury checks.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.SkillTreatInjury, 4,
                        PassiveIds.FPMedicalSpecialist4, StatModifier.BonusType.Competence)
                })
            )},

            { PassiveIds.FPMedicalSpecialist5, new PassiveDefinition(
                passive: PassiveIds.FPMedicalSpecialist5,
                name:    "Medical Specialist",
                desc:    "+5 competence bonus on all TreatInjury checks.",
                effect:  new SimpleEffectSource(new List<StatusEffect>
                {
                    new StatBonusEffect(StatType.SkillTreatInjury, 5,
                        PassiveIds.FPMedicalSpecialist5, StatModifier.BonusType.Competence)
                })
            )},
        };

    public static PassiveDefinition Get(string passiveId)
    {
        if (Definitions.TryGetValue(passiveId, out var def))
            return def;

        UnityEngine.Debug.LogWarning($"PassiveDatabase: '{passiveId}' not found.");
        return null;
    }
}
