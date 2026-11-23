using UnityEngine;

//Stops the bearer's fast healing for the next N end phases (Pathfinder: a troll's regeneration stops on a round it takes
//acid or fire damage). Apply it from any damage or ability that should stop fast healing; damage types don't exist yet,
//so nothing applies it today. Counted down by CombatManager's fast-healing step instead of the normal duration tick,
//so it always covers the end phase right after it's applied
public class FastHealingSuppressedEffect : StatusEffect
{
    public int roundsLeft;

    public FastHealingSuppressedEffect(int rounds = 1) : base("Fast Healing Suppressed", 0)
    {
        roundsLeft = rounds;
        isPersistent = true;
        isHarmful = true;
    }

    //Applied again while active: keep the longer suppresion
    public override void OnRefresh(StatusEffect incoming)
    {
        if (incoming is FastHealingSuppressedEffect other)
            roundsLeft = Mathf.Max(roundsLeft, other.roundsLeft);
    }

    //Called by the fast-healing step for each end phase it blocks
    public void ConsumeRound(Unit unit)
    {
        roundsLeft--;
        if (roundsLeft <= 0) unit.RemoveEffect(this);
    }
}
