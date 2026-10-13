using UnityEngine;

public static class ConcentrationManager
{
    public const string ConcentrationCheckId = SkillCheckIds.PsionicConcentration;

    //DC constants matching d20 framework
    public const int DC_DefensiveManifesting = 15;
    public const int DC_DamagedWhileManifesting = 10; //+damage dealt +power level added by caller

    public static void RequestConcentrationCheck(
        Unit manifester,
        int dc,
        System.Action<bool> onResult,
        string failureLog = null)
    {
        var context = new SkillCheckContext
        {
            checkId = ConcentrationCheckId,
            attacker = manifester,
            callerContext = SkillCheckContext.CallerContext.Psionic,
            showPreview = false,
            dcOverride = dc,
            onResult = (result) =>
            {
                if (!result.success)
                {
                    CombatLog.Add(failureLog ?? $"{manifester.unitName}: Concentration broken — " +
                        $"power points consumed, no effect.");
                }
                onResult?.Invoke(result.success);
            }
        };

        SkillCheckExecutor.Execute(context);
    }

    //Convenience method: damaged while manifesting
    //DC = 10 + damage dealt + power level (3.5 / Pathfinder)
    public static void RequestDamagedCheck(
        Unit manifester,
        int damageTaken,
        int powerLevel,
        System.Action<bool> onResult,
        string failureLog = null)
    {
        int dc = DC_DamagedWhileManifesting + damageTaken + powerLevel;
        RequestConcentrationCheck(manifester, dc, onResult, failureLog);
    }

    public const int CombatManifestationBonus = 4;

    //Convenience method: defensive manifesting to avoid opportunity attacks
    public static void RequestDefensiveCheck(
        Unit manifester,
        int powerLevel,
        System.Action<bool> onResult)
    {
        int dc = DC_DefensiveManifesting + (powerLevel * 2);

        //Combat Manifestation (Telepath 7): +4 on this check, applied as a lower DC
        var data = manifester.sourceCharacterData;
        if (data != null && data.unlockedPassiveIds.Contains(PassiveIds.TelepathCombatManifestation))
            dc -= CombatManifestationBonus;

        RequestConcentrationCheck(manifester, dc, onResult);
    }
}
