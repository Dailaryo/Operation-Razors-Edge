using UnityEngine;

public static class ConcentrationManager
{
    public const string ConcentrationCheckId = "concentration_psionic";

    //DC constants matching d20 framework
    public const int DC_DefensiveManifesting = 15;
    public const int DC_DamagedWhileManifesting = 10; //+damage dealt added by caller

    public static void RequestConcentrationCheck(
        Unit manifester,
        int dc,
        System.Action<bool> onResult)
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
                    CombatLog.Add($"{manifester.unitName}: Concentration broken — " +
                                  $"power points consumed, no effect.");
                }
                onResult?.Invoke(result.success);
            }
        };

        SkillCheckExecutor.Execute(context);
    }

    //Convenience method: damaged while manifesting
    //DC = 10 + damage dealt (d20 standard)
    public static void RequestDamagedCheck(
        Unit manifester,
        int damageTaken,
        System.Action<bool> onResult)
    {
        int dc = DC_DamagedWhileManifesting + damageTaken;
        RequestConcentrationCheck(manifester, dc, onResult);
    }

    //Convenience method: defensive manifesting to avoid opportunity attacks
    public static void RequestDefensiveCheck(
        Unit manifester,
        int powerLevel,
        System.Action<bool> onResult)
    {
        int dc = DC_DefensiveManifesting + (powerLevel * 2);
        RequestConcentrationCheck(manifester, dc, onResult);
    }
}
