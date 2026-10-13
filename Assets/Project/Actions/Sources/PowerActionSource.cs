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

        return result;
    }
}