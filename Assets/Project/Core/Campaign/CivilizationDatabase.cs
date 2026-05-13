using UnityEngine;
using System.Collections.Generic;

public static class CivilizationDatabase
{
    public static readonly Dictionary<string, CivilizationData> Definitions =
        new Dictionary<string, CivilizationData>()
        {
            {
                CivilizationIds.HumanEmpire,
                new CivilizationData(
                    CivilizationIds.HumanEmpire,
                    "Human Empire",
                    new TechTierProfile(defaultTier: 7)
                )
            },
            {
                CivilizationIds.TaleiranRepublic,
                new CivilizationData(
                    CivilizationIds.TaleiranRepublic,
                    "Taleiran",
                    new TechTierProfile(defaultTier: 8)
                )
            },
            {
                CivilizationIds.QuestranCommune,
                new CivilizationData(
                    CivilizationIds.QuestranCommune,
                    "Questran",
                    new TechTierProfile(defaultTier: 7)
                )
            },
            {
                CivilizationIds.VaaraAlliance,
                new CivilizationData(
                    CivilizationIds.VaaraAlliance,
                    "Vaara",
                    new TechTierProfile(defaultTier: 7)
                )
            },
            {
                CivilizationIds.KhreKaHive,
                new CivilizationData(
                    CivilizationIds.KhreKaHive,
                    "Khre'Ka",
                    new TechTierProfile(defaultTier: 7)
                )
            },
        };

    public static CivilizationData Get(string id)
    {
        if (Definitions.TryGetValue(id, out CivilizationData civ)) return civ;

        Debug.LogWarning($"[CivilizationDatabase] Civilization not found: {id}");
        return null;
    }
}
