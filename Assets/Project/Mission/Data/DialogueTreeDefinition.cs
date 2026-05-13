using UnityEngine;
using System.Collections.Generic;

public class DialogueTreeDefinition
{
    public string treeId;
    public string displayName;
    public string startNodeId;

    // Inventory of all nodeIds in this conversation.
    // Used for validation and tooling; traversal follows nextNodeIds links.
    public List<string> nodes = new List<string>();

    public ConversationType convoType;

    // Optional journal summary written when a OneTime conversation completes.
    // Null = no journal entry generated.
    public string completionSummary;

    public enum ConversationType
    {
        OneTime,
        Repeatable,
        Persistent
    }

    public DialogueTreeDefinition(string id, string name, string startNode, ConversationType type)
    {
        treeId = id;
        displayName = name;
        startNodeId = startNode;
        convoType = type;
    }
}
