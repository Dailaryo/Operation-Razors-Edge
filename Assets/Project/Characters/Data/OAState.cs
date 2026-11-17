using UnityEngine;

public class OAState
{
    public int maxPerRound;
    public int usedThisRound;

    public bool reactWhileHidden = false;

    public bool isFlatFooted = false;          //Mirrors Unit.IsFlatFooted (set through Unit.SetFlatFooted)
    public bool actsWhileFlatFooted = false;   //Combat Reflexes: attacks of opportunity while flat-footed (Pathfinder)

    public OAState(int max = 1)
    {
        maxPerRound = max;
        usedThisRound = 0;
    }

    public bool CanTakeOA()
    {
        if (isFlatFooted && !actsWhileFlatFooted) return false;
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
