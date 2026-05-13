using System.Collections.Generic;
using UnityEngine;

public static class NarrativeFlagManager
{
    private static Dictionary<string, int> Flags = new Dictionary<string, int>();

    public static void SetFlag(string key, int value)
    {
        Flags[key] = value;
        PlayerPrefs.SetInt(key, value);
        PlayerPrefs.Save();
    }

    public static int GetFlag(string key)
    {
        if (Flags.TryGetValue(key, out int cached)) return cached;

        //Lazy load from PlayerPrefs if not in memory
        if (PlayerPrefs.HasKey(key))
        {
            int stored = PlayerPrefs.GetInt(key);
            Flags[key] = stored;
            return stored;
        }

        return 0;
    }

    public static bool HasFlag(string key)
    {
        return GetFlag(key) != 0;
    }

    public static void ClearAll()
    {
        Flags.Clear();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public static List<string> GetAllKeys()
    {
        return new List<string>(Flags.Keys);
    }
}
