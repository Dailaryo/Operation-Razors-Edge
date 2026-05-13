using UnityEngine;
using System.Collections.Generic;

public static class SkillCheckExecutor
{
    public static event System.Action<SkillCheckPreviewUI.SkillCheckPreview> OnPreviewRequested;

    public static void Execute(SkillCheckContext context)
    {
        //Step 1: Load definition
        var skillCheck = SkillCheckDatabase.Get(context.checkId);
        if (skillCheck == null) return;

        //Step 2: Validate trained-only gate
        if (!SkillResolver.CanAttempt(context.attacker, skillCheck.skill))
        {
            var failResult = new SkillCheckResult
            {
                skill = skillCheck.skill,
                success = false,
                tier = 0
            };
            context.onResult?.Invoke(failResult);
            return;
        }

        //Step 3: Preview
        if (context.showPreview)
        {
            int modifier = SkillResolver.GetSkillTotal(context.attacker, skillCheck.skill);
            int dcModifier = skillCheck.checkMode == SkillCheckDefinition.CheckMode.FixedDC
                ? DifficultySettings.GetSkillCheckDCModifier() : 0;
            int dc = context.dcOverride ?? (skillCheck.fixedDC + dcModifier);

            var preview = new SkillCheckPreviewUI.SkillCheckPreview
            {
                skill = skillCheck.skill,
                skillDisplayName = SkillDatabase.Definitions[skillCheck.skill.ToString()].displayName,
                modifier = modifier,
                dc = dc,
                isOpposed = skillCheck.checkMode == SkillCheckDefinition.CheckMode.Opposed,
                autoSuccess = skillCheck.checkMode == SkillCheckDefinition.CheckMode.FixedDC && modifier >= dc,
                minimumRoll = (skillCheck.checkMode == SkillCheckDefinition.CheckMode.FixedDC && modifier < dc)
                    ?Mathf.Clamp(dc-modifier, 1, 20 ) : 0,
                description = skillCheck.description
            };

            OnPreviewRequested?.Invoke(preview);
        }

        //Step 4: Roll
        SkillCheckResult result;
        switch (skillCheck.checkMode)
        {
            case SkillCheckDefinition.CheckMode.FixedDC:
                int dc = context.dcOverride ?? (skillCheck.fixedDC + DifficultySettings.GetSkillCheckDCModifier());
                result = SkillResolver.Resolve(context.attacker, skillCheck.skill, dc);
                break;

            case SkillCheckDefinition.CheckMode.Opposed:
                if (context.defenderTotalOverride.HasValue)
                {
                    //Attacker rolls normally; defender total is supplied by caller
                    result = SkillResolver.Resolve(context.attacker, skillCheck.skill, context.defenderTotalOverride.Value);
                    result.dc = context.defenderTotalOverride.Value;
                }
                else
                {
                    result = SkillResolver.ResolveOpposed(
                        context.attacker, skillCheck.skill,
                        context.defender, skillCheck.opposedSkill);
                }
                break;

            default:
                Debug.LogWarning($"[SkillCheckExecutor] CheckMode {skillCheck.checkMode} not handled");
                return;
        }

        //Step 5: Grade tier
        result.tier = 0;
        int highestMet = -1;

        for (int i = 0; i < skillCheck.tiers.Count; i++)
        {
            var t = skillCheck.tiers[i];
            if (t.tier == 0) continue; //Skip failure tier
            if (result.success && result.total >= t.minimumTotal && t.tier > highestMet)
            {
                highestMet = t.tier;
                result.tier = t.tier;
            }
        }

        //Step 6: Auto-consequences
        if (result.tier >= skillCheck.narrativeFlagTier && !string.IsNullOrEmpty(skillCheck.narrativeFlagId))
        {
            NarrativeFlagManager.SetFlag(skillCheck.narrativeFlagId, result.tier);

            string journalText = skillCheck.tiers.Find(t => t.tier == result.tier)?.journalText;
            if (!string.IsNullOrEmpty(journalText))
                NarrativeJournal.AddEntry(skillCheck.narrativeFlagId, journalText);
        }

        //Step 7: Log
        if (context.callerContext == SkillCheckContext.CallerContext.Combat)
        {
            string logLine;
            var tieredEntry = skillCheck.tiers.Find(t => t.tier == result.tier);

            if (tieredEntry != null && !string.IsNullOrEmpty(tieredEntry.logText))
                logLine = tieredEntry.logText;
            else
                logLine = $"{context.attacker.unitName} {skillCheck.description}: " +
                          $"{result.total} vs {result.dc} — " +
                          $"{(result.success ? "Success" : "Failure")}";

            CombatLog.Add(logLine);
        }
        else if (context.callerContext == SkillCheckContext.CallerContext.Dialogue
            && context.showPreview)
        {
            //Only log player-initiated dialogue checks, passive checks run silently
            CombatLog.Add($"[Dialogue] {context.attacker.unitName} {skillCheck.description}: " +
                  $"{result.total} vs {result.dc} — " +
                  $"{(result.success ? "Success" : "Failure")}");
        }
        else if (context.callerContext == SkillCheckContext.CallerContext.Psionic)
        {
            CombatLog.Add($"[Concentration] {context.attacker.unitName} {skillCheck.description}: " +
                          $"{result.total} vs {result.dc} — " +
                          $"{(result.success ? "Success" : "Failure")}");
        }

        //Step 8: Fire callback
        context.onResult?.Invoke(result);
    }

    public static void RequestPreview(string checkId, Unit unit, int? dcOverride = null)
    {
        var def = SkillCheckDatabase.Get(checkId);
        if (def == null) return;

        int modifier = SkillResolver.GetSkillTotal(unit, def.skill);
        bool isOpposed = def.checkMode == SkillCheckDefinition.CheckMode.Opposed;
        int dcModifier = isOpposed ? 0 : DifficultySettings.GetSkillCheckDCModifier();
        int dc = dcOverride ?? (def.fixedDC + dcModifier);
        bool autoSuccess = !isOpposed && modifier >= dc;

        var preview = new SkillCheckPreviewUI.SkillCheckPreview
        {
            skill = def.skill,
            skillDisplayName = SkillDatabase.Definitions[def.skill.ToString()].displayName,
            modifier = modifier,
            dc = dc,
            isOpposed = isOpposed,
            autoSuccess = autoSuccess,
            minimumRoll = autoSuccess ? 0 : Mathf.Clamp(dc - modifier, 1, 20),
            description = def.description
        };

        OnPreviewRequested?.Invoke(preview);
    }
}
