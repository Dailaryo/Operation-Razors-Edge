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
        };

    public static ChoiceNodeDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out ChoiceNodeDefinition node)) return node;

        Debug.LogWarning($"[ChoiceNodeDatabase] Node not found: {id}");
        return null;
    }


}
