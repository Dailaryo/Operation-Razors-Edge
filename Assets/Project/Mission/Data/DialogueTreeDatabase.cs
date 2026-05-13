using UnityEngine;
using System.Collections.Generic;

public static class DialogueTreeDatabase
{
    public static Dictionary<string, DialogueTreeDefinition> Definitions =
        new Dictionary<string, DialogueTreeDefinition>();

    public static DialogueTreeDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out DialogueTreeDefinition tree)) return tree;

        Debug.LogWarning($"[DialogueTreeDatabase] Tree not found: {id}");
        return null;
    }
}
