using UnityEngine;
using System.Collections.Generic;

public class EndingData
{
    public string endingId;
    public string title;
    public string summary;

    //All flags in this list must be set for this ending to trigger
    //Evaluated in order - first match wins
    public List<string> requiredFlags;

    public EndingData(string id, string title, string summary, List<string> requiredFlags)
    {
        endingId = id;
        this.title = title;
        this.summary = summary;
        this.requiredFlags = requiredFlags ?? new List<string>();
    }

    public bool IsMatch()
    {
        foreach (string flag  in requiredFlags)
        {
            if (!NarrativeFlagManager.HasFlag(flag)) return false;
        }
        return true;
    }
}
