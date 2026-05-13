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
        };

    public static PassiveDefinition Get(string passiveId)
    {
        if (Definitions.TryGetValue(passiveId, out var def))
            return def;

        UnityEngine.Debug.LogWarning($"PassiveDatabase: '{passiveId}' not found.");
        return null;
    }
}
