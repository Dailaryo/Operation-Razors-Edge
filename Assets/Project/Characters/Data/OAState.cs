using UnityEngine;

public class OAState
{
    public int maxPerRound;
    public int usedThisRound;

    public bool reactWhileHidden = false;

    public OAState(int max = 1)
    {
        maxPerRound = max;
        usedThisRound = 0;
    }

    public bool CanTakeOA()
    {
        return usedThisRound < maxPerRound;
    }

    public void RecordOA()
    {
        usedThisRound++;
    }
    
    public void ResetRound()
    {
        usedThisRound = 0;
    }
}
