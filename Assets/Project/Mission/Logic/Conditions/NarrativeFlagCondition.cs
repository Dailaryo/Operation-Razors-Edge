using UnityEngine;

public class NarrativeFlagCondition : IObjectiveCondition
{
    public string flagId;
    public int minimumTier;

    public NarrativeFlagCondition(string flagId, int minimumTier = 1)
    {
        this.flagId = flagId;
        this.minimumTier = minimumTier;
    }

    public bool Evaluate(CombatContext context)
    {
        return NarrativeFlagManager.GetFlag(flagId) >= minimumTier;
    }

    public bool IsFailed(CombatContext context)
    {
        return false;
    }
}
