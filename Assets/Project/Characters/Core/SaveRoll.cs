using UnityEngine;

//Result of a saving throw: the total plus the natural d20 roll. Comparing it against a DC applies the d20 rule:
//natural 20 always succeeds, natural 1 always fails. Prints as the total (with the natural roll on a 1 or 20).
//No implicit conversion to int on purpose: use .total where a plain number is needed
public class SaveRoll
{
    public readonly int total;
    public readonly int natural;

    public SaveRoll(int total, int natural)
    {
        this.total = total;
        this.natural = natural;
    }

    public bool AutoSuccess => natural == 20;
    public bool AutoFail => natural == 1;

    public bool Beats(int dc) => AutoSuccess || (!AutoFail && total >= dc);

    public static bool operator >=(SaveRoll save, int dc) => save.Beats(dc);
    public static bool operator <=(SaveRoll save, int dc) => !save.Beats(dc + 1);
    public static bool operator >(SaveRoll save, int dc) => save.Beats(dc + 1);
    public static bool operator <(SaveRoll save, int dc) => !save.Beats(dc);

    //Situational bonuses added after the roll (Psychic Crush's Resolve bonus) keep the natural roll
    public static SaveRoll operator +(SaveRoll save, int bonus) => new SaveRoll(save.total + bonus, save.natural);

    public override string ToString() =>
        AutoSuccess || AutoFail ? $"{total} (natural {natural})" : total.ToString();
}
