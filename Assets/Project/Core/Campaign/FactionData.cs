using UnityEngine;
using System.Collections.Generic;

public class FactionData
{
    public string factionId;
    public string displayName;
    public string description;

    //Narrative disposition towards player
    public enum Disposition { Neutral, Allied, Hostile }

    //Flag key used to store this faction's disposition
    public string FactionFlagKey => "faction_" + factionId + "_disposition";

    //Mission hooks; IDs of missions this faction is involved in
    public List<string> missionHooks;

    public FactionData(string id, string name, string description)
    {
        factionId = id;
        displayName = name;
        this.description = description;
        missionHooks = new List<string>();
    }

    public Disposition GetDisposition()
    {
        int value = NarrativeFlagManager.GetFlag(FactionFlagKey);

        return value switch
        {
            1 => Disposition.Allied,
            -1 => Disposition.Hostile,
            _ => Disposition.Neutral
        };
    }

    public void SetDisposition(Disposition disposition)
    {
        int value = disposition switch
        {
            Disposition.Allied => 1,
            Disposition.Hostile => -1,
            _ => 0
        };

        NarrativeFlagManager.SetFlag(FactionFlagKey, value);
        CombatLog.Add($"[Faction] {displayName} disposition set to: {disposition}");
    }
}
