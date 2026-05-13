using System.Collections.Generic;
using UnityEngine;

public class ActData
{
    public int actIndex;

    public string actName;

    public List<string> planetIds;

    //Null means no flag required - act is always accessible
    public string unlockFlagId;

    public ActData(int index, string name, string unlockFlagId = null)
    {
        actIndex = index;
        actName = name;
        this.unlockFlagId = unlockFlagId;
        planetIds = new List<string>();
    }

    public bool IsUnlocked()
    {
        if (unlockFlagId == null) return true;
        return NarrativeFlagManager.HasFlag(unlockFlagId);
    }
}
