using System.Collections.Generic;
using UnityEngine;

public class PowerActionSource : IActionSource
{
    private CharacterData characterData;

    public PowerActionSource(CharacterData characterData)
    {
        this.characterData = characterData;
    }

    public List<ActionDefinition> GetActions(Unit unit)
    {
        var result = new List<ActionDefinition>();

        foreach (var id in characterData.knownPowerIds)
        {
            var power = PowerDatabase.Get(id);
            if (power == null) continue;

            result.AddRange(PowerProgression.GetManifestable(power));
        }

        //Manifesting toggles (Abilities tab), squad only: any manifester can manifest defensively,
        //Maximize Power needs the Telepath class feature
        if (!unit.isEnemy && characterData.knownPowerIds.Count > 0)
            result.Add(ActionDatabase.Get(ActionIds.ManifestDefensively));

        if (!unit.isEnemy && characterData.unlockedPassiveIds.Contains(PassiveIds.TelepathMaximizePower))
            result.Add(ActionDatabase.Get(ActionIds.MaximizePower));

        return result;
    }
}