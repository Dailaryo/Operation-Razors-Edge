using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public static class SpeciesDatabase
{
    public static Dictionary<string, SpeciesDefinition> Definitions =
        new Dictionary<string, SpeciesDefinition>()
        {
            { SpeciesIds.Delphin, new SpeciesDefinition(
                speciesId: SpeciesIds.Delphin,
                displayName: "Delphin",
                description: "Homo sapiens delphus. Engineered from dolphin genetic material, Delphins are defined by exceptional cognitive capacity at the cost of physical agility and intuition. Their echolocation renders conventional concealment largely irrelevant within close range.",
                statBonuses: new List<StatModifier>
                {
                    new StatModifier(StatType.Power, 2, 1f, SpeciesIds.Delphin, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Agility, -2, 1f, SpeciesIds.Delphin, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Cognition, 4, 1f, SpeciesIds.Delphin, StatModifier.BonusType.Racial),
                    new StatModifier(StatType.Insight, -4, 1f, SpeciesIds.Delphin, StatModifier.BonusType.Racial)
                },
                passiveIds: new List<string>
                {
                    PassiveIds.BlindSight
                }
            )},
        };

    public static SpeciesDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out SpeciesDefinition species)) return species;

        Debug.LogWarning($"[SpeciesDatabase] Species not found: {id}");
        return null;
    }
}
