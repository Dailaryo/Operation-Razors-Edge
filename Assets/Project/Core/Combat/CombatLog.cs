using System.Collections.Generic;
using UnityEngine;

public class CombatLog
{
    static List<string> logEntries = new List<string>();

    //Event fired whenever a new log entry is added (used by UI)
    public static System.Action<string> OnLogAdded;

    public static void Add(string message)
    {
        logEntries.Add(message);
        Debug.Log(message);
        OnLogAdded?.Invoke(message);
    }

    public static List<string> GetLog()
    {
        return logEntries;
    }

    public static void Clear()
    {
        logEntries.Clear();
    }
}
