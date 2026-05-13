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
}
