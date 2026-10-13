using UnityEngine;
using System.Collections.Generic;

//One level of a pregen's pre-written progression route
public class PregenLevelPlan
{
    public List<string> feats = new List<string>();          //granted (unlockedFeatIds) are free; others cost a feat point
    public List<string> choiceOptions = new List<string>();

    //A higher tier frees the slots of the lower tiers it replaces:
    //list it BEFORE the replacement picks that fill those slots
    public List<string> powers = new List<string>();
    public StatType? asi = null;                               //ASI levels only
    public Dictionary<SkillType, int> skillRanks = new Dictionary<SkillType, int>();

    //Creates a route with an empty entry for every level from1 to maxLevel
    public static Dictionary<int, PregenLevelPlan> CreateRoute(int maxLevel = 20)
    {
        var route = new Dictionary<int, PregenLevelPlan>();
        for (int lvl = 1;  lvl <= maxLevel; lvl++)
            route[lvl] = new PregenLevelPlan();
        return route;
    }

    //Keeps each skill at the rank cap (rank = level) from fromLevel through toLevel,
    //A skill joining after level 1 catches up to the cap on its first level (retroactive COG ranks).
    public static void FillSkillsAtCap(Dictionary<int, PregenLevelPlan> route, int fromLevel, int toLevel,
        params SkillType[] skills)
    {
        for (int lvl = fromLevel; lvl <= toLevel; lvl++)
        {
            int ranks = lvl == fromLevel ? fromLevel : 1;

            foreach (var skill in skills)
            {
                route[lvl].skillRanks.TryGetValue(skill, out int existing);
                route[lvl].skillRanks[skill] = existing + ranks;
            }
        }
    }
}