using UnityEngine;

public class CivilizationData
{
    public string civilizationId;
    public string displayName;
    public TechTierProfile baselineTiers;

    public CivilizationData(string civId, string name, TechTierProfile baseline)
    {
        civilizationId = civId;
        displayName = name;
        baselineTiers = baseline;
    }
}
