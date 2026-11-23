using UnityEngine;

//Command Override (Vanguard L19): one bonus Standard action in the holder's next planning phase.
//Placed during round N's execution; ActionEconomyResolver reads it while round N+1 is planned; it ends at N+1's end phase.
//Counts its own end phases, so the end-of-round extension rule doesn't stretch it
public class CommandOverrideBonusEffect : StatusEffect
{
    private int endPhasesLeft = 2;

    public CommandOverrideBonusEffect() : base("Command Override", 0)
    {
        isPersistent = true;
    }

    public override void OnRefresh(StatusEffect incoming)
    {
        endPhasesLeft = 2;
        ended = false;
    }

    public override void OnTurnEnd(Unit unit)
    {
        if (--endPhasesLeft <= 0) ended = true;
    }
}
