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
        };

    public static ChoiceNodeDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out ChoiceNodeDefinition node)) return node;

        Debug.LogWarning($"[ChoiceNodeDatabase] Node not found: {id}");
        return null;
    }


}
