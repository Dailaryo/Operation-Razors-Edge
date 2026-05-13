using UnityEngine;
using System.Collections.Generic;

public class NpcDefinition
{
    public string npcId;
    public string displayName;

    // Always-available conversations; shown when no contextual tree qualifies.
    // Presented in order — first in list is the default greeting/fallback.
    public List<string> persistentTreeIds = new List<string>();

    // Situation-specific conversations; each entry pairs a required flag
    // with a dialogueTreeId. Evaluated top-to-bottom; first match wins.
    public List<NpcContextualDialogue> contextualDialogue = new List<NpcContextualDialogue>();

    public NpcDefinition(string id, string name)
    {
        npcId = id;
        displayName = name;
    }
}

// A single contextual conversation entry.
// The tree is available when requiredFlag is set in NarrativeFlagManager.
// Null requiredFlag means always contextually available (use sparingly —
// prefer persistentTreeIds for unconditional conversations).
public class NpcContextualDialogue
{
    public string requiredFlag;
    public string dialogueTreeId;

    public NpcContextualDialogue(string flag, string treeId)
    {
        requiredFlag = flag;
        dialogueTreeId = treeId;
    }
}