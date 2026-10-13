using UnityEngine;
using System.Collections.Generic;
using static ClassDefinition;

public static class TechnosavantClass
{
    public static ClassDefinition Create()
    {
        var def = new ClassDefinition(
            classId: ClassIds.Technosavant,
            diplayName: "Technosavant",
            statBias: StatType.Cognition,
            babProgression: BABProgression.ThreeQuarters,
            hpPerLevel: 6,
            skillRanksPerLevel: 7,
            resolveSave: SaveProgression.Good,
            responseSave: SaveProgression.Poor,
            focusSave: SaveProgression.Poor,
            featCadence: new List<int> { 5, 10, 15, 20 },
            classSkills: new List<SkillType>
            {
                SkillType.ComputerUse,
                SkillType.Repair,
                SkillType.DisableDevice,
                SkillType.Demolitions,
                SkillType.Knowledge,
                SkillType.Investigate,
                SkillType.Perception,
                SkillType.SenseMotive,
                SkillType.TreatInjury,
                SkillType.Xenology,
            },
            levelTable: new Dictionary<int, List<ClassLevelEntry>>
            {
                // ── TIER 1 ──────────────────────────────────────────────
                { 1, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.RangedAttack,
                            ActionIds.Overcharge
                        },
                        passiveIds: new List<string>
                        {
                            PassiveIds.NeuralAcuity1
                        })
                }},

                { 2, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.DiagnoseWeakness
                        },
                        passiveIds: new List<string>
                        {
                            PassiveIds.IntegratedTargeting
                        })
                }},

                { 3, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string>
                        {
                            PassiveIds.SubdermalWeave1
                        })
                }},

                { 4, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.DeployField
                        })
                }},

                { 5, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string>
                        {
                            PassiveIds.AdaptiveInterface
                        })
                }},

                { 6, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.SystemOverride
                        },
                        passiveIds: new List<string>
                        {
                            PassiveIds.ThreatAnalysis
                        })
                }},

                { 7, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        passiveIds: new List<string>
                        {
                            PassiveIds.NeuralAcuity2
                        })
                }},

                { 8, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.EmergencyPatch
                        },
                        passiveIds: new List<string>
                        {
                            PassiveIds.SubdermalWeave2
                        })
                }},

                { 9, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.PowerSurge
                        })
                }},

                { 10, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.FullSpectrumInterface
                        },
                        passiveIds: new List<string>
                        {
                            PassiveIds.OverclockProtocol
                        },
                        choicNodeId: ChoiceNodeIds.TechnosavantPath)
                }},

                // ── TIER 2 (LEVELS 11–20) ────────────────────────────────
                { 11, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ironclad
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.WeaponMount
                        },
                        passiveIds: new List<string>
                        {
                            PassiveIds.HardenedChassis1
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantIronclad),

                    new ClassLevelEntry(                          // Ghost Protocol
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.RemoteInterface
                        },
                        passiveIds: new List<string>
                        {
                            PassiveIds.SignalScramble1
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantGhostProtocol),

                    new ClassLevelEntry(                          // Vanguard
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.NeuralSync
                        },
                        passiveIds: new List<string>
                        {
                            PassiveIds.TacticalUplink1
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantVanguard),
                }},

                { 12, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ironclad
                        passiveIds: new List<string>
                        {
                            PassiveIds.BallisticIntegration
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantIronclad),

                    new ClassLevelEntry(                          // Ghost Protocol
                        passiveIds: new List<string>
                        {
                            PassiveIds.GhostStep
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantGhostProtocol),

                    new ClassLevelEntry(                          // Vanguard
                        passiveIds: new List<string>
                        {
                            PassiveIds.CommandPresence
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantVanguard),
                }},

                { 13, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ironclad
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.OverdriveProtocol
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantIronclad),

                    new ClassLevelEntry(                          // Ghost Protocol
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.NeuralIntrusion
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantGhostProtocol),

                    new ClassLevelEntry(                          // Vanguard
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.CoordinatedFire
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantVanguard),
                }},

                { 14, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ironclad
                        passiveIds: new List<string>
                        {
                            PassiveIds.HardenedChassis2
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantIronclad),

                    new ClassLevelEntry(                          // Ghost Protocol
                        passiveIds: new List<string>
                        {
                            PassiveIds.SignalScramble2
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantGhostProtocol),

                    new ClassLevelEntry(                          // Vanguard
                        passiveIds: new List<string>
                        {
                            PassiveIds.TacticalUplink2
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantVanguard),
                }},

                { 15, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ironclad
                        passiveIds: new List<string>
                        {
                            PassiveIds.TargetLock
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantIronclad),

                    new ClassLevelEntry(                          // Ghost Protocol
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.ActivateElectronicShroud
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantGhostProtocol),

                    new ClassLevelEntry(                          // Vanguard
                        passiveIds: new List<string>
                        {
                            PassiveIds.RelayNetwork
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantVanguard),
                }},

                { 16, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ironclad
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.ArmamentBurst
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantIronclad),

                    new ClassLevelEntry(                          // Ghost Protocol
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.CascadeBurst
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantGhostProtocol),

                    new ClassLevelEntry(                          // Vanguard
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.ShieldProtocol
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantVanguard),
                }},

                { 17, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ironclad
                        passiveIds: new List<string>
                        {
                            PassiveIds.NeuralAcuity3
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantIronclad),

                    new ClassLevelEntry(                          // Ghost Protocol
                        passiveIds: new List<string>
                        {
                            PassiveIds.NeuralAcuity3
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantGhostProtocol),

                    new ClassLevelEntry(                          // Vanguard
                        passiveIds: new List<string>
                        {
                            PassiveIds.NeuralAcuity3
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantVanguard),
                }},

                { 18, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ironclad
                        passiveIds: new List<string>
                        {
                            PassiveIds.KineticAbsorption
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantIronclad),

                    new ClassLevelEntry(                          // Ghost Protocol
                        passiveIds: new List<string>
                        {
                            PassiveIds.FeedbackLoop
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantGhostProtocol),

                    new ClassLevelEntry(                          // Vanguard
                        passiveIds: new List<string>
                        {
                            PassiveIds.DistributedProcessing
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantVanguard),
                }},

                { 19, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ironclad
                        passiveIds: new List<string>
                        {
                            PassiveIds.WeaponsIntegration
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantIronclad),

                    new ClassLevelEntry(                          // Ghost Protocol
                        passiveIds: new List<string>
                        {
                            PassiveIds.PhantomProtocol
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantGhostProtocol),

                    new ClassLevelEntry(                          // Vanguard
                        passiveIds: new List<string>
                        {
                            PassiveIds.CommandOverride
                        },
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.CommandOverrideAction
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantVanguard),
                }},

                { 20, new List<ClassLevelEntry>
                {
                    new ClassLevelEntry(                          // Ironclad
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.TerminalForce
                        },
                        passiveIds: new List<string>
                        {
                            PassiveIds.IronApotheosis
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantIronclad),

                    new ClassLevelEntry(                          // Ghost Protocol
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.GridCollapse
                        },
                        passiveIds: new List<string>
                        {
                            PassiveIds.GhostApotheosis
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantGhostProtocol),

                    new ClassLevelEntry(                          // Vanguard
                        actionUnlockIds: new List<string>
                        {
                            ActionIds.HighCommand
                        },
                        passiveIds: new List<string>
                        {
                            PassiveIds.VanguardApotheosis
                        },
                        prerequisiteOptionId: ChoiceNodeIds.TechnosavantVanguard),
                }},
            }
        );

        def.resource = new ResourceDefinition(
            resourceId: "technosavant_power",
            displayName: "Cybernetic Power",
            regenMode: ResourceDefinition.RegenMode.FullOnCombatStart,
            baseMax: 4);

        // Bonus feats (d20 Future Technosavant list; alien weapon, crafting, surgery and vehicle feats dropped)
        // Adapted: project tech feats and the Smart-hero Studious/Educated line added
        def.bonusFeatIds = new List<string>
        {
            FeatIds.ArmorProficiencyLight, FeatIds.ArmorProficiencyMedium, FeatIds.ArmorProficiencyPowered,
            FeatIds.Cybertaker, FeatIds.ExoticFirearmsProficiency, FeatIds.ExoticMeleeWeaponProficiency,
            // Adaptations
            FeatIds.TechLiterate, FeatIds.SensorLink, FeatIds.CyberneticMastery, FeatIds.TacticalRelay,
            FeatIds.Studious, FeatIds.Educated,
        };
        def.pathBonusFeatIds = new Dictionary<string, List<string>>
        {
            { ChoiceNodeIds.TechnosavantVanguard, new List<string> { FeatIds.OverwatchSpecialist } },
        };

        return def;
    }
}