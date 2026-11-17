using UnityEngine;
using System.Collections.Generic;

public class SquadRuntime
{
    public CharacterData source;

    public int hpDamage;

    public int powerPointsSpent;

    public int zeroLevelUsesSpent;

    //Reserved for exploration-phase time pressure: effects that outlast an encounter
    //(minute/hour durations) will carry between encounters here. Unused until the
    //objective-implementation milestone; until then long-duration powers end at encounter end.
    //HealToFull already clears it on Hub return.
    public List<StatusEffect> persistentEffects = new List<StatusEffect>();

    public bool isIncapacitated = false;

    //Ended an encounter Disabled, Dying or Stabilized: alive, so a full rest (Restore Squad) brings them back.
    //Dead (rollback) and Microcosm-removed members stay out until the Hub
    public bool downAtEncounterEnd = false;

    public CharacterSnapshot snapshot;

    public SquadRuntime(CharacterData source)
    {
        this.source = source;
        this.hpDamage = source.hpDamage;
        this.powerPointsSpent = source.powerPointsSpent;
        this.isIncapacitated=source.isIncapacitated;
        this.zeroLevelUsesSpent = source.zeroLevelUsesSpent;
    }

    public void ApplyDamage(int amount)
    {
        hpDamage += amount;
    }

    public void HealToFull()
    {
        hpDamage = 0;
        persistentEffects.Clear();
        powerPointsSpent = 0;
        zeroLevelUsesSpent=0;
    }
}
