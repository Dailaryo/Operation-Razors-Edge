using UnityEngine;

//Temporary hit points are lost before real HP. The same source doesn't stack (a refresh keeps the higher
//amount); different sources (different effect names) stack.
public class TemporaryHitPointsEffect : StatusEffect
{
    public int amount;

    public TemporaryHitPointsEffect(string source, int amount, int duration)
        : base($"{source}_tempHP", duration)
    {
        this.amount = amount;
    }

    public override void OnRefresh(StatusEffect incoming)
    {
        if (incoming is TemporaryHitPointsEffect other)
            amount = Mathf.Max(amount, other.amount);
    }

    //Returns the damage left after this buffer absorbs what it can
    public int Absorb(int damage)
    {
        int absorbed = Mathf.Min(amount, damage);
        amount -= absorbed;
        return damage - absorbed;
    }
}
