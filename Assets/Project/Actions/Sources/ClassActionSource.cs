using System.Collections.Generic;
using UnityEngine;

public class ClassActionSource : IActionSource
{
    private ClassDefinition classDef;
    private CharacterData characterData;

    public ClassActionSource(ClassDefinition classDef, CharacterData characterData) 
    { 
        this.classDef = classDef; 
        this.characterData = characterData; 
    }

    public List<ActionDefinition> GetActions(Unit unit)
    {
        var result = new List<ActionDefinition>();

        for (int lvl = 1; lvl <= characterData.level; lvl++)
        {
            if (!classDef.levelTable.TryGetValue(lvl, out var entries)) continue;

            foreach (var entry in entries)
            {
                // Skip path-gated entries for unchosen paths
                if (entry.prerequisiteOptionId != null &&
                    !characterData.takenChoiceOptionIds.Contains(entry.prerequisiteOptionId))
                    continue;

                foreach (var id in entry.actionUnlockIds)
                {
                    var def = ActionDatabase.Get(id);
                    if (def != null) result.Add(def);
                }
            }
        }

        // Collect action unlocks from taken choice options
        foreach (var optionId in characterData.takenChoiceOptionIds)
        {
            foreach (var nodeDef in ChoiceNodeDatabase.Definitions.Values)
            {
                foreach (var option in nodeDef.options)
                {
                    if (option.optionId != optionId) continue;

                    foreach (var id in option.actionUnlockIds)
                    {
                        var def = ActionDatabase.Get(id);
                        if (def != null) result.Add(def);
                    }
                }
            }
        }

        // Actions granted by unlocked passives (active Helix mutations: Adrenaline Surge, Adrenal Frenzy)
        foreach (var passiveId in characterData.unlockedPassiveIds)
        {
            var passive = PassiveDatabase.Get(passiveId);
            if (passive == null) continue;

            foreach (var id in passive.grantedActionIds)
            {
                var def = ActionDatabase.Get(id);
                if (def != null && !result.Contains(def)) result.Add(def);
            }
        }

        return result;
    }
}
