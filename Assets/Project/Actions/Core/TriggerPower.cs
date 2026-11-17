using UnityEngine;

//Telepath Trigger Power (d20 Modern): powers chosen at Telepath 2, 5, 8 and the path level.
//An ability check with the power's own key ability against DC 11/13/15/17 (power level 0–3) manifests it
//for 0 PP, including augmentation and catapsi doubling. On a failure the normal cost is paid and the power
//still manifests. One attempt per power per round; no daily limit.
public static class TriggerPower
{
    public static string OptionIdFor(string powerId) => ChoiceNodeIds.TriggerPowerOptionPrefix + powerId;

    public static bool IsTriggerPower(Unit unit, PsionicPowerDefinition power)
    {
        var data = unit.sourceCharacterData;
        return data != null && data.takenChoiceOptionIds.Contains(OptionIdFor(power.actionID));
    }

    public static int GetDC(PsionicPowerDefinition power) => 11 + 2 * power.powerLevel;

    //Rolls the check if this is one of the unit's trigger powers and it hasn't been tried this round.
    //Call only when the manifestation would otherwise cost PP
    public static bool TryTrigger(Unit unit, PsionicPowerDefinition power)
    {
        if (!IsTriggerPower(unit, power)) return false;
        if (unit.triggerAttemptsThisRound.Contains(power.actionID)) return false;

        unit.triggerAttemptsThisRound.Add(power.actionID);

        int roll = Random.Range(1, 21);
        int modifier = unit.GetModifier(power.keyAbility);
        int total = roll + modifier;
        int dc = GetDC(power);
        bool success = total >= dc;

        CombatLog.Add($"{unit.unitName} Trigger Power ({power.DisplayName}): {roll} + {modifier} = {total} " +
                      $"vs DC {dc} — {(success ? "no power points spent" : "normal cost")}");

        return success;
    }
}
