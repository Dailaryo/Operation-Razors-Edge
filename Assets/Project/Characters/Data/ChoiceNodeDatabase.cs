using UnityEngine;
using System.Collections.Generic;
using static ChoiceNodeDefinition;

public static class ChoiceNodeDatabase
{
    public static Dictionary<string, ChoiceNodeDefinition> Definitions =
        new Dictionary<string, ChoiceNodeDefinition>()
        {
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
        };

    public static ChoiceNodeDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out ChoiceNodeDefinition node)) return node;

        Debug.LogWarning($"[ChoiceNodeDatabase] Node not found: {id}");
        return null;
    }


}
