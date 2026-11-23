using UnityEngine;
using System.Collections.Generic;

//Coordinated Fire mark, carried by the marked enemy. The next attack against it by one of the marker's allies
//(not the marker) gets +2 to the attack roll and, on a hit, +1d6 damage. That attack spends the mark, hit or miss.
//Unused, it fades at the end of the marker's next execution phase (the end phase after the round it was placed in)
public class CoordinatedFireMarkerEffect : StatusEffect
{
    public const int AccuracyBonus = 2;

    private Unit marker;
    private int endPhasesLeft = 2;      //placed in round N: survives N's end phase, ends at N+1's
    private bool spent = false;
    private ActionContext armedAttack;  //the attack that spent the mark, for its damage on a hit

    public CoordinatedFireMarkerEffect(Unit marker) : base("Coordinated Fire", 0)
    {
        this.marker = marker;
        isPersistent = true; //expiry is counted here, so the end-of-round extension rule doesn't stretch it
        isHarmful = true;
        Priority = 5;
    }

    public override void OnBeforeAttackRoll(ActionContext ctx)
    {
        if (spent || ctx.finalTarget != Target) return;
        if (ctx.attacker == null || ctx.attacker == marker) return;
        if (ctx.attacker.isEnemy != marker.isEnemy) return;

        spent = true;
        armedAttack = ctx;
        ctx.attackRollModifier += AccuracyBonus;
        CombatLog.Add($"{ctx.attacker.unitName} acts on {marker.unitName}'s Coordinated Fire against {Target.unitName}: " +
                      $"+{AccuracyBonus} to the attack roll.");
    }

    //Runs after the hit roll, only on a hit: the +1d6 for the attack that spent the mark
    public override void OnBeforeAction(ActionContext ctx)
    {
        if (ctx != armedAttack) return;
        armedAttack = null;

        int roll = Random.Range(1, 7);
        ctx.modifiedDamage += roll;
        CombatLog.Add($"Coordinated Fire: +1d6 damage ({roll}).");
    }

    //Marked again: fresh window and a fresh charge
    public override void OnRefresh(StatusEffect incoming)
    {
        if (incoming is CoordinatedFireMarkerEffect other) marker = other.marker;
        endPhasesLeft = 2;
        spent = false;
        armedAttack = null;
        ended = false;
    }

    public override void OnTurnEnd(Unit unit)
    {
        if (spent || --endPhasesLeft <= 0) ended = true;
    }
}