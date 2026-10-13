using UnityEngine;

public static class PsionicResistanceCalculator
{
    // Check whether a psionic power penetrates the target's resistance
    // Returns true if the power succeeds (penetrates or target has no resistance)
    public static bool CheckPenetration(Unit manifester, Unit target, bool log = true,
        EffectDescriptor incoming = EffectDescriptor.None)
    {
        int resistance = GetPsionicResistance(target, incoming);

        if (resistance <= 0) return true; //No resistance to overcome

        int manifesterLevel = manifester.GetEffectiveManifesterLevel();

        int roll = Random.Range(1, 21);
        int total = roll + manifesterLevel;

        bool success = total >= resistance;

        if (log)
        {
            CombatLog.Add($"[Psionic Resistance] {manifester.unitName} vs {target.unitName}: " +
                          $"rolled {roll} + ML {manifesterLevel} = {total} vs PR {resistance} — " +
                          $"{(success ? "Penetrated" : "Resisted")}");
        }
        

        return success;
    }

    public static int GetPsionicResistance(Unit target, EffectDescriptor incoming = EffectDescriptor.None)
    {
        int resistance = target.sourceCharacterData != null ? target.sourceCharacterData.psionicResistance : 0;

        foreach (var effect in target.activeEffects)
            if (effect is PowerResistanceEffect granted && granted.AppliesAgainst(incoming))
                resistance = Mathf.Max(resistance, granted.resistance);

        return resistance;
    }
}
