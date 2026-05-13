using System.Collections.Generic;
using UnityEngine;

public class SkillActionSource : IActionSource
{
    //Pairs an action with the skill rank threshold needed to access it
    public class SkillActionEntry
    {
        public string actionId;
        public SkillType skill;
        public int requiredRanks;

        public SkillActionEntry(string actionId, SkillType skill, int requiredRanks)
        {
            this.actionId = actionId;
            this.skill = skill;
            this.requiredRanks = requiredRanks;
        }
    }

    private List<SkillActionEntry> entries;

    public SkillActionSource(List<SkillActionEntry> entries)
    {
        this.entries = entries ?? new List<SkillActionEntry>();
    }

    public List<ActionDefinition> GetActions(Unit unit)
    {
        var result = new List<ActionDefinition>();

        if (unit.sourceCharacterData == null) return result;

        foreach (var entry in entries)
        {
            //Check rank threshold
            unit.sourceCharacterData.skillRanks.TryGetValue(entry.skill, out int ranks);

            if (ranks < entry.requiredRanks) continue;

            //Check action exist in database
            var def = ActionDatabase.Get(entry.actionId);
            if (def != null) result.Add(def);
        }

        return result;
    }
}
