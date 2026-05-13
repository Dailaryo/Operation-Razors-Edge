using UnityEngine;
using System.Collections.Generic;

public static class DialogueNodeDatabase
{
    public static Dictionary<string, DialogueNodeDefinition> Definitions = 
        new Dictionary<string, DialogueNodeDefinition>();

    public static DialogueNodeDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out DialogueNodeDefinition node)) return node;

        Debug.LogWarning($"[DialogueNodeDatabase] Node not found: {id}");
        return null;
    }
}
