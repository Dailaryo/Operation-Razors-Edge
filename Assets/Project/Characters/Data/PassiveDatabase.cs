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
        };

    public static PassiveDefinition Get(string passiveId)
    {
        if (Definitions.TryGetValue(passiveId, out var def))
            return def;

        UnityEngine.Debug.LogWarning($"PassiveDatabase: '{passiveId}' not found.");
        return null;
    }
}
