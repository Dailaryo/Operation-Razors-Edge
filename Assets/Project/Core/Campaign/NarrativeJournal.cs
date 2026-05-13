using UnityEngine;
using System.Collections.Generic;

public static class NarrativeJournal
{
    //Each entry is a human-readable string tied to when a flag was set
    private static List<string> entries = new List<string>();

    //Call this when setting a narrative flag that has player-visible meaning
    //The entry string is what the player will see in the journal UI
    public static void AddEntry(string flagKey, string displayText)
    {
        NarrativeFlagManager.SetFlag(flagKey, 1);

        string entry = displayText;
        entries.Add(entry);

        Debug.Log($"[Journal] Entry added: {displayText}");
    }

    //Returns a copy of all journal entries in order
    public static List<string> GetEntries()
    {
        return new List<string>(entries);
    }

    public static void ClearAll()
    {
        entries.Clear();
    }

    //Called by SaveManager - returns raw entries for serialization
    public static List<string> GetRawEntries()
    {
        return entries;
    }

    //Called by SaveManager in load - restores entries without re-firing flags
    public static void LoadEntries(List<string> loaded)
    {
        entries = new List<string>(loaded);
    }
}
