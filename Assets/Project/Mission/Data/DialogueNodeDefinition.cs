using UnityEngine;
using System.Collections.Generic;

public class DialogueNodeDefinition
{
    public string nodeId;
    public string speakerName;
    public string text;
    public NodeType nodeType;

    //NarrativeFlag key; null = always available
    public string availabilityCondition;

    //For NodeType.SkillCheck: routes to SkillCheckExecutor
    public string skillCheckId;

    //For nodeType.SavingThrow:
    //public string savingThrowId;

    //For nodeType.AttributeCheck:
    //public string attrCheckId;

    //Flag keys set when this node is selected; each written as SetFlag(key, 1)
    public List<string> consequences = new List<string>();

    //Follow-up noeds; empty = end of conversation
    public List<string> nextNodeIds = new List<string>();

    public enum NodeType
    {
        NPCSpeech,
        PlayerChoice,
        SkillCheck,
        //SavingThrow,
        //AttributeCheck,
        ConversationEnd
    }

    public DialogueNodeDefinition(string id, string speaker, string text, NodeType type)
    {
        nodeId = id;
        speakerName = speaker;
        this.text = text;
        nodeType = type;
    }
}
