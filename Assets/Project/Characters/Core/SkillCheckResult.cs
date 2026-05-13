using UnityEngine;

public class SkillCheckResult
{
    public SkillType skill;

    public int roll; //d20 result (1 if autosuccess)
    public int modifier; //total modifier
    public int total; //roll + modifier
    public int dc; //target DC

    public bool autoSuccess; //true if modifier alone meets or exceeds DC
    public bool success; //final pass/fail
    public int degreeOfSuccess; //total - DC
    public int tier; //0 = failure, 1+ = success tiers assigned by SkillCheckExecutor
}
