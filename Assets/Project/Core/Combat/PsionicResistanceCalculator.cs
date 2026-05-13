using UnityEngine;

public static class PsionicResistanceCalculator
{
    // Check whether a psionic power penetrates the target's resistance
    // Returns true if the power succeeds (penetrates or target has no resistance)
    public static bool CheckPenetration(Unit manifester, Unit target)
    {
        int resistance = GetPsionicResistance(target);

        if (resistance <= 0) return true; //No resistance to overcome

        int manifesterLevel = manifester.sourceCharacterData != null
            ? manifester.sourceCharacterData.manifesterLevel
            : 0;

        int roll = Random.Range(1, 21);
        int total = roll + manifesterLevel;

        bool success = total >= resistance;

        CombatLog.Add($"[Psionic Resistance] {manifester.unitName} vs {target.unitName}: " +
                      $"rolled {roll} + ML {manifesterLevel} = {total} vs PR {resistance} — " +
                      $"{(success ? "Penetrated" : "Resisted")}");

        return success;
    }

    public static int GetPsionicResistance(Unit target)
    {
        if (target.sourceCharacterData == null) return 0;
        return target.sourceCharacterData.psionicResistance;
    }
}
