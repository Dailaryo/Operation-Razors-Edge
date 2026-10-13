using UnityEngine;

public class StatModifier
{
    public enum BonusType
    {
        Untyped, //Always stacks: used for class bonuses, base progression
        Competence, //Highest competence always wins
        Dodge, //Always stacks: dodge bonuses are one of the few exceptions
        Morale, //Highest morale always wins
        Circumstance, //Highest circumstance always wins
        Enhancement, //Highest enhancement always wins
        Insight, //Highest insight always wins
        Racial, //Highest racial always wins, species-granted modifiers
        Equipment, //Highest equipment always wins
        Resistance, //Highest resistance always wins (Mind Blank)
    }
    
    public StatType stat;

    public int flatBonus;

    public float multiplier = 1f;

    public string source;

    public BonusType bonusType;

    public StatModifier(StatType stat, int flat, float mult, string source, BonusType bonusType = BonusType.Untyped)
    {
        this.stat = stat;
        flatBonus = flat;
        multiplier = mult;
        this.source = source;
        this.bonusType = bonusType;
    }
}
