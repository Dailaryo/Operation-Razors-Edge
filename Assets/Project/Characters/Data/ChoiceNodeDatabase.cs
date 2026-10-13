using UnityEngine;
using System.Collections.Generic;
using static ChoiceNodeDefinition;

public static class ChoiceNodeDatabase
{
    public static Dictionary<string, ChoiceNodeDefinition> Definitions =
        new Dictionary<string, ChoiceNodeDefinition>()
        {
            // --- Technosavant Path Choice ---
            { ChoiceNodeIds.TechnosavantPath, new ChoiceNodeDefinition(
                nodeId:      ChoiceNodeIds.TechnosavantPath,
                displayName: "Technosavant Specialization",
                nodeType:    NodeType.TreeBranch,
                treeId:      null,
                options: new List<ChoiceNodeDefinition.ChoiceOption>
                {
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.TechnosavantIronclad,
                        displayName: "Ironclad",
                        description: "Maximize personal firepower. Heavy implants, integrated weapon systems, and overwhelming offensive output.",
                        prerequisiteOptionId: null
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.TechnosavantGhostProtocol,
                        displayName: "Ghost Protocol",
                        description: "Electronic warfare and battlefield control. Disrupt enemy systems, suppress threats, and strike from the shadows.",
                        prerequisiteOptionId: null
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.TechnosavantVanguard,
                        displayName: "Vanguard",
                        description: "Team coordination and force multiplication. Every ally fights better when you're on the field.",
                        prerequisiteOptionId: null
                    ),
                }
            )},
            // --- Helix Warrior Path Choice ---
            { ChoiceNodeIds.HelixWarriorPath, new ChoiceNodeDefinition(
                nodeId:      ChoiceNodeIds.HelixWarriorPath,
                displayName: "Helix Warrior Path",
                nodeType:    NodeType.TreeBranch,
                treeId:      ChoiceNodeIds.HelixWarriorPath,
                options: new List<ChoiceNodeDefinition.ChoiceOption>
                {
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.HelixWarriorBerserker,
                        displayName: "Berserker",
                        description: "Kill-chaining melee escalation. Rampage, Bloodlust, and Killing Blow reward aggressive close-range fighting."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.HelixWarriorApexPredator,
                        displayName: "Apex Predator",
                        description: "First-strike and mobility. Hunting Instinct and Predatory Leap reward isolated targets and aggressive positioning."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.HelixWarriorIronFlesh,
                        displayName: "Iron Flesh",
                        description: "DR escalation and retaliation. Hardened Hide and Reactive Plating turn you into an attrition engine."
                    ),
                }
            )},
            // --- Helix Warrior Enhanced Mutation (level 3) ---
            // Player picks one purchased mutation to upgrade to its Enhanced tier.
            { ChoiceNodeIds.HelixEnhancedMutation, new ChoiceNodeDefinition(
                nodeId:      ChoiceNodeIds.HelixEnhancedMutation,
                displayName: "Enhanced Mutation",
                nodeType:    NodeType.FreeChoice,
                options: new List<ChoiceNodeDefinition.ChoiceOption>
                {
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.HelixEnhancedMutationRapidHealing,
                        displayName: "Enhanced Rapid Healing",
                        description: "Rapid Healing upgrades to fast healing 2.",
                        passiveUnlockIds: new List<string> { PassiveIds.HelixRapidHealingEnhanced },
                        passiveRemoveIds: new List<string> { PassiveIds.HelixRapidHealing }
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.HelixEnhancedMutationMusculature,
                        displayName: "Enhanced Musculature",
                        description: "Enhanced Musculature bonus increases to +3 melee damage.",
                        passiveUnlockIds: new List<string> { PassiveIds.HelixEnhancedMusculatureImproved },
                        passiveRemoveIds: new List<string> { PassiveIds.HelixEnhancedMusculature }
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.HelixEnhancedMutationAdrenaline,
                        displayName: "Enhanced Adrenaline Surge",
                        description: "Adrenaline Surge duration increases to 4 rounds.",
                        passiveUnlockIds: new List<string> { PassiveIds.HelixAdrenalineSurge },
                        passiveRemoveIds: new List<string>()
                    ),
                }
            )},
            // --- Helix Warrior Improved Mutation (level 5) ---
            // Player picks one purchased mutation to upgrade to its Improved tier.
            { ChoiceNodeIds.HelixImprovedMutation, new ChoiceNodeDefinition(
                nodeId:      ChoiceNodeIds.HelixImprovedMutation,
                displayName: "Improved Mutation",
                nodeType:    NodeType.FreeChoice,
                options: new List<ChoiceNodeDefinition.ChoiceOption>
                {
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.HelixImprovedMutationRapidHealing,
                        displayName: "Improved Rapid Healing",
                        description: "Rapid Healing upgrades to fast healing 3 and stabilizes at 0 HP.",
                        passiveUnlockIds: new List<string> { PassiveIds.HelixRapidHealingGreater },
                        passiveRemoveIds: new List<string>
                        {
                            PassiveIds.HelixRapidHealing,
                            PassiveIds.HelixRapidHealingEnhanced
                        }
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.HelixImprovedMutationMusculature,
                        displayName: "Improved Musculature",
                        description: "Enhanced Musculature bonus increases to +4 melee damage.",
                        passiveUnlockIds: new List<string> { PassiveIds.HelixEnhancedMusculatureImproved },
                        passiveRemoveIds: new List<string>
                        {
                            PassiveIds.HelixEnhancedMusculature
                        }
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.HelixImprovedMutationAdrenaline,
                        displayName: "Improved Adrenaline Surge",
                        description: "Adrenaline Surge upgrades to Adrenal Frenzy: +6 Power and +1 attack for 4 rounds.",
                        passiveUnlockIds: new List<string> { PassiveIds.HelixAdrenalFrenzy },
                        passiveRemoveIds: new List<string> { PassiveIds.HelixAdrenalineSurge }
                    ),
                }
            )},
            // --- Helix Warrior Greater Mutation (level 8) ---
            // Player picks one purchased mutation to upgrade to its Greater tier.
            { ChoiceNodeIds.HelixGreaterMutation, new ChoiceNodeDefinition(
                nodeId:      ChoiceNodeIds.HelixGreaterMutation,
                displayName: "Greater Mutation",
                nodeType:    NodeType.FreeChoice,
                options: new List<ChoiceNodeDefinition.ChoiceOption>
                {
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.HelixGreaterMutationRapidHealing,
                        displayName: "Greater Rapid Healing",
                        description: "Rapid Healing upgrades to fast healing 3 and stabilizes at 0 HP.",
                        passiveUnlockIds: new List<string> { PassiveIds.HelixRapidHealingGreater },
                        passiveRemoveIds: new List<string>
                        {
                            PassiveIds.HelixRapidHealing,
                            PassiveIds.HelixRapidHealingEnhanced
                        }
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.HelixGreaterMutationMusculature,
                        displayName: "Greater Musculature",
                        description: "Enhanced Musculature bonus increases to +4 melee damage.",
                        passiveUnlockIds: new List<string> { PassiveIds.HelixEnhancedMusculatureImproved },
                        passiveRemoveIds: new List<string>
                        {
                            PassiveIds.HelixEnhancedMusculature
                        }
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.HelixGreaterMutationAdrenaline,
                        displayName: "Greater Adrenal Frenzy",
                        description: "Adrenal Frenzy upgrades to +8 Power and +2 attack for 5 rounds.",
                        passiveUnlockIds: new List<string> { PassiveIds.HelixAdrenalFrenzy },
                        passiveRemoveIds: new List<string> { PassiveIds.HelixAdrenalineSurge }
                    ),
                }
            )},

            // --- Infiltrator Skill Mastery (level 5) ---
            { ChoiceNodeIds.InfiltratorSkillMasteryNode, new ChoiceNodeDefinition(
                nodeId:      ChoiceNodeIds.InfiltratorSkillMasteryNode,
                displayName: "Skill Mastery",
                nodeType:    NodeType.FreeChoice,
                pickCount:   data => 3 + CharacterProgression.GetAbilityModifier(data.GetAttributeScore(StatType.Cognition)),
                options: new List<ChoiceNodeDefinition.ChoiceOption>
                {
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.InfiltratorSkillMasteryStealth,
                        displayName: "Stealth",
                        description: "You may always Take 10 on Stealth checks, even under pressure."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.InfiltratorSkillMasteryAcrobatics,
                        displayName: "Acrobatics",
                        description: "You may always Take 10 on Acrobatics checks, even under pressure."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.InfiltratorSkillMasteryDisableDevice,
                        displayName: "Disable Device",
                        description: "You may always Take 10 on DisableDevice checks, even under pressure."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.InfiltratorSkillMasteryPerception,
                        displayName: "Perception",
                        description: "You may always Take 10 on Perception checks, even under pressure."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.InfiltratorSkillMasteryInvestigate,
                        displayName: "Investigate",
                        description: "You may always Take 10 on Investigate checks, even under pressure."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.InfiltratorSkillMasterySenseMotive,
                        displayName: "Sense Motive",
                        description: "You may always Take 10 on SenseMotive checks, even under pressure."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.InfiltratorSkillMasteryBluff,
                        displayName: "Bluff",
                        description: "You may always Take 10 on Bluff checks, even under pressure."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.InfiltratorSkillMasteryKnowledge,
                        displayName: "Knowledge",
                        description: "You may always Take 10 on Knowledge checks, even under pressure."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.InfiltratorSkillMasteryAthletics,
                        displayName: "Athletics",
                        description: "You may always Take 10 on Athletics checks, even under pressure."
                    ),
                }
            )},

            // --- Infiltrator Path Choice (level 10) ---
            { ChoiceNodeIds.InfiltratorPath, new ChoiceNodeDefinition(
                nodeId:      ChoiceNodeIds.InfiltratorPath,
                displayName: "Infiltrator Path",
                nodeType:    NodeType.TreeBranch,
                treeId:      ChoiceNodeIds.InfiltratorPath,
                options: new List<ChoiceNodeDefinition.ChoiceOption>
                {
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.InfiltratorGhost,
                        displayName: "Ghost",
                        description: "Stealth mastery and assassination. Shadow Step, Death Blow, and Ghost Apotheosis " +
                                     "make you a threat enemies cannot locate, engage, or survive."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.InfiltratorOperative,
                        displayName: "Operative",
                        description: "Battlefield control through debuffs and action economy manipulation. " +
                                     "Expose Weakness, Disabling Strike, and Calculated Takedown dismantle enemy effectiveness."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.InfiltratorPhantom,
                        displayName: "Phantom",
                        description: "Psionic supplementation layered on the Infiltrator toolkit. Requires Wild Talent feat. " +
                                     "Psionic Feint, Psychic Displacement, and Phase Out add a second layer of misdirection.",
                        prerequisiteOptionId: null
                        // Note: Wild Talent prerequisite is enforced by description and future feat gate;
                        // not currently wired to requiredFeatIds since ChoiceOption doesn't support that field.
                    ),
                }
            )},

            // --- Dreadnought Path Choice (level 10) ---
            { ChoiceNodeIds.DreadnoughtPath, new ChoiceNodeDefinition(
                nodeId:      ChoiceNodeIds.DreadnoughtPath,
                displayName: "Dreadnought Path",
                nodeType:    NodeType.TreeBranch,
                treeId:      ChoiceNodeIds.DreadnoughtPath,
                options: new List<ChoiceNodeDefinition.ChoiceOption>
                {
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.DreadnoughtBastion,
                        displayName: "Bastion",
                        description: "Pure defensive escalation and ally protection. Damage Reduction, forced movement " +
                                     "immunity, and Protective Aura make you the squad's immovable anchor. " +
                                     "Bulwark lets you intercept attacks aimed at allies."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.DreadnoughtJuggernaut,
                        displayName: "Juggernaut",
                        description: "Converts defensive mass into offensive force. Unstoppable Charge displaces enemies " +
                                     "along the path, Shockwave turns Knockdown into an area effect, and Momentum " +
                                     "rewards relentless pressure on a single target."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.DreadnoughtArtillerist,
                        displayName: "Artillerist",
                        description: "Transforms physical resilience into a weapons platform. Suppression Fire, " +
                                     "Indirect Fire, and Volley Fire add ranged area control. Fortified Position " +
                                     "rewards staying stationary with DR and Defense bonuses."
                    ),
                }
            )},

            // --- Field Medic Path Choice (level 10) ---
            { ChoiceNodeIds.FieldMedicPath, new ChoiceNodeDefinition(
                nodeId:      ChoiceNodeIds.FieldMedicPath,
                displayName: "Field Medic Specialization",
                nodeType:    NodeType.TreeBranch,
                treeId:      ChoiceNodeIds.FieldMedicPath,
                options: new List<ChoiceNodeDefinition.ChoiceOption>
                {
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.FieldMedicTraumaSurgeon,
                        displayName: "Trauma Surgeon",
                        description: "Deep healing and revival mastery. Extended miracle windows, Mass Triage, " +
                                     "and Surgeon Apotheosis make you the last line between the squad and permanent loss."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.FieldMedicCombatMedic,
                        displayName: "Combat Medic",
                        description: "Healing integrated into active combat. Move-action treatment, free-action patches, " +
                                     "and Priority Patient let you keep the squad fighting without leaving the fight yourself."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.FieldMedicPharmacist,
                        displayName: "Field Pharmacist",
                        description: "Pharmaceutical expertise and chemical enhancement. Stim Injection, Field Synthesis, " +
                                     "and Pharmacological Mastery add a second dimension to your support toolkit."
                    ),
                }
            )},

            // --- Telepath Trigger Power (levels 2, 5, 8 and the path level) ---
            //One known 0–3rd level power per pick. Manifest it for 0 PP on a successful ability check
            //(power's key ability, DC 11/13/15/17 by power level). See TriggerPower
            { ChoiceNodeIds.TriggerPower, new ChoiceNodeDefinition(
                nodeId:      ChoiceNodeIds.TriggerPower,
                displayName: "Trigger Power",
                nodeType:    NodeType.FreeChoice,
                options: new List<ChoiceNodeDefinition.ChoiceOption>
                {
                    // 0-level (DC 11)
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.Burst,
                        "Burst", "Trigger Power: Burst (DC 11).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.Burst)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.Daze,
                        "Daze", "Trigger Power: Daze (DC 11).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.Daze)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.DetectPsionics,
                        "Detect Psionics", "Trigger Power: Detect Psionics (DC 11).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.DetectPsionics)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.Distract,
                        "Distract", "Trigger Power: Distract (DC 11).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.Distract)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.FarHand,
                        "Far Hand", "Trigger Power: Far Hand (DC 11).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.FarHand)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.FingerOfFire,
                        "Finger of Fire", "Trigger Power: Finger of Fire (DC 11).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.FingerOfFire)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.Missive,
                        "Missive", "Trigger Power: Missive (DC 11).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.Missive)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.TelempathicProjection,
                        "Telempathic Projection", "Trigger Power: Telempathic Projection (DC 11).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.TelempathicProjection)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.Verve,
                        "Verve", "Trigger Power: Verve (DC 11).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.Verve)),

                    // 1st-level (DC 13)
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.Attraction,
                        "Attraction", "Trigger Power: Attraction (DC 13).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.Attraction)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.CharmPerson,
                        "Charm Person", "Trigger Power: Charm Person (DC 13).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.CharmPerson)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.ConcealThoughts,
                        "Conceal Thoughts", "Trigger Power: Conceal Thoughts (DC 13).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.ConcealThoughts)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.ControlObject,
                        "Control Object", "Trigger Power: Control Object (DC 13).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.ControlObject)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.LesserBodyAdjustment,
                        "Lesser Body Adjustment", "Trigger Power: Lesser Body Adjustment (DC 13).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.LesserBodyAdjustment)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.LesserMindlink,
                        "Lesser Mindlink", "Trigger Power: Lesser Mindlink (DC 13).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.LesserMindlink)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.ObjectReading,
                        "Object Reading", "Trigger Power: Object Reading (DC 13).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.ObjectReading)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.WhiteNoise,
                        "White Noise", "Trigger Power: White Noise (DC 13).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.WhiteNoise)),

                    // 2nd-level (DC 15)
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.BrainLock,
                        "Brain Lock", "Trigger Power: Brain Lock (DC 15).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.BrainLock)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.ClairaudienceClairvoyance,
                        "Clairaudience/Clairvoyance", "Trigger Power: Clairaudience/Clairvoyance (DC 15).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.ClairaudienceClairvoyance)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.DetectThoughts,
                        "Detect Thoughts", "Trigger Power: Detect Thoughts (DC 15).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.DetectThoughts)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.ElectronicFog,
                        "Electronic Fog", "Trigger Power: Electronic Fog (DC 15).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.ElectronicFog)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.InflictPain,
                        "Inflict Pain", "Trigger Power: Inflict Pain (DC 15).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.InflictPain)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.Levitate,
                        "Levitate", "Trigger Power: Levitate (DC 15).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.Levitate)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.SensitivityToPsychicImpressions,
                        "Sensitivity to Psychic Impressions", "Trigger Power: Sensitivity to Psychic Impressions (DC 15).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.SensitivityToPsychicImpressions)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.Suggestion,
                        "Suggestion", "Trigger Power: Suggestion (DC 15).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.Suggestion)),

                    // 3rd-level (DC 17)
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.CharmCreature,
                        "Charm Creature", "Trigger Power: Charm Creature (DC 17).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.CharmCreature)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.FalseSensoryInput,
                        "False Sensory Input", "Trigger Power: False Sensory Input (DC 17).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.FalseSensoryInput)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.LesserDomination,
                        "Lesser Domination", "Trigger Power: Lesser Domination (DC 17).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.LesserDomination)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.MentalBlast,
                        "Mental Blast", "Trigger Power: Mental Blast (DC 17).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.MentalBlast)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.Mindlink,
                        "Mindlink", "Trigger Power: Mindlink (DC 17).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.Mindlink)),
                    new ChoiceNodeDefinition.ChoiceOption(ChoiceNodeIds.TriggerPowerOptionPrefix + PowerIds.NegatePsionics,
                        "Negate Psionics", "Trigger Power: Negate Psionics (DC 17).",
                        requirement: data => data.knownPowerIds.Contains(PowerIds.NegatePsionics)),
                }
            )},

            // --- Telepath Path Choice (level 10) ---
            { ChoiceNodeIds.TelepathPath, new ChoiceNodeDefinition(
                nodeId:      ChoiceNodeIds.TelepathPath,
                displayName: "Telepath Specialization",
                nodeType:    NodeType.TreeBranch,
                treeId:      ChoiceNodeIds.TelepathPath,
                options: new List<ChoiceNodeDefinition.ChoiceOption>
                {
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.TelepathEmpath,
                        displayName: "Empath",
                        description: "Battlefield emotional influence. Crowd Read, Sympathetic Link and Aura of Calm read " +
                                     "the room, turn your allies' afflictions on the enemy, and steady the squad's nerve."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.TelepathInquisitor,
                        displayName: "Inquisitor",
                        description: "Intelligence extraction and remote surveillance. Deep Scan, Trace Memory and " +
                                     "Complete Probe pull secrets from any mind, near or far."
                    ),
                    new ChoiceNodeDefinition.ChoiceOption(
                        optionId:    ChoiceNodeIds.TelepathDominator,
                        displayName: "Dominator",
                        description: "Direct mental control at scale. Mass Brain Lock, Chain Domination and Psionic " +
                                     "Puppeteer turn enemies into instruments."
                    ),
                }
            )},
        };

    public static ChoiceNodeDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out ChoiceNodeDefinition node)) return node;

        Debug.LogWarning($"[ChoiceNodeDatabase] Node not found: {id}");
        return null;
    }


}
