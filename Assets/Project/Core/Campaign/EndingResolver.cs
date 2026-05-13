using UnityEngine;
using System.Collections.Generic;

public static class EndingResolver
{
    //Evaluates endings in order and returns the first match
    //Falls back to the last entry in the list if nothing matches:
    //Always put a catch-all ending last with no required flags
    public static EndingData Resolve(List<EndingData> endings)
    {
        foreach (var ending in endings)
        {
            if (ending.IsMatch()) return ending;
        }

        Debug.LogWarning("[EndingResolver] No ending matched.  Check that a fallback ending exists");
        return null;
    }
}
