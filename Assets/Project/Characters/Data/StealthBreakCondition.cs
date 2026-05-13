using UnityEngine;
using System.Collections.Generic;

public enum StealthBreakCondition
{
    MeleeAttack,
    RangedAttack,
    ManifestPower,
    TakeDamage
}

public class StealthBreakOverride
{
    public StealthBreakCondition suppressedCondition;
    public int usesPerRound; //0 = unlimited

    public StealthBreakOverride(StealthBreakCondition condition, int uses = 0)
    {
        suppressedCondition = condition;
        usesPerRound = uses;
    }
}

public class StealthBreakProfile
{
    public List<StealthBreakOverride> overrides = new List<StealthBreakOverride>();

    public bool Suppresses(StealthBreakCondition condition)
    {
        foreach (var entry in overrides)
        {
            if (entry.suppressedCondition == condition)
                return true;
        }
        return false;
    }
}