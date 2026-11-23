using UnityEngine;

public class SkillCheckContext
{
    public enum CallerContext { Combat, Hub, Exploration, Dialogue, Medical, Psionic }

    public string checkId;
    public Unit attacker;
    public Unit defender;
    public CallerContext callerContext;
    public bool showPreview;
    public System.Action<SkillCheckResult> onResult;

    //Overrides: callers set these when the definition's defaults don't apply
    public int? dcOverride; //For dynamic DCs
    public int? defenderTotalOverride; //For non-skill opposition
    public int? defenderNaturalRoll;   //Natural d20 of a defender's saving throw used as defenderTotalOverride: 20 always saves, 1 always fails
}
