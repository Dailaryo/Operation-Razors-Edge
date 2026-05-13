using UnityEngine;
using System.Collections.Generic;

public class SkillCheckDefinition
{
    public enum CheckMode { FixedDC, Opposed, Passive }

    public string checkId;
    public SkillType skill;
    public CheckMode checkMode;
    public int fixedDC;
    public SkillType opposedSkill;
    public List<SkillCheckTier> tiers = new List<SkillCheckTier>();
    public string narrativeFlagId;
    public int narrativeFlagTier = 1;
    public string description;
}
