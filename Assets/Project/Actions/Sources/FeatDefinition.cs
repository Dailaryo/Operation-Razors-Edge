using UnityEngine;
using System.Collections.Generic;

public class FeatDefinition
{
    //Identity
    public string featId;
    public string displayName;
    public string description;

    //Taxonomy
    public enum FeatCategory
    {
        Combat,
        Tactical,
        Passive,
        Utility,
        Psionic
    }

    public enum FeatTag
    {
        Melee,
        Ranged,
        Tech,
        Psionic
    }

    public FeatCategory category;
    public List<FeatTag> tags;

    //Prerequisites
    public int requiredLevel;
    public List<string> requiredFeatIds;
    public Dictionary<StatType, int> requiredAttributes;

    //How many times this feat can be taken. Default 1: all feats are once-only
    //unless the feat itself says otherwise (case-by-case exceptions).
    public int maxTakes;

    //Payload
    //A feat may grant actions, passive effects, or both
    //Either field may be null if the feat only does one thing
    public IActionSource actionSource; //grants new actions to the unit
    public IEffectSource effectSource; //applies passiveStatusEffects to the unit

    public StealthBreakProfile stealthBreakProfile;

    //Constructor
    public FeatDefinition(
        string featId,
        string displayName,
        string description,
        FeatCategory category,
        List<FeatTag> tags = null,
        int requiredLevel = 1,
        List<string> requiredFeatIds = null,
        Dictionary<StatType, int> requiredAttributes = null,
        IActionSource actionSource = null,
        IEffectSource effectSource = null,
        StealthBreakProfile stealthBreakProfile = null,
        int maxTakes = 1
    )
    {
        this.featId = featId;
        this.displayName = displayName;
        this.description = description;
        this.category = category;
        this.tags = tags ?? new List<FeatTag>();
        this.requiredLevel = requiredLevel;
        this.requiredFeatIds = requiredFeatIds ?? new List<string>();
        this.requiredAttributes = requiredAttributes ?? new Dictionary<StatType, int>();
        this.actionSource = actionSource;
        this.effectSource = effectSource;
        this.stealthBreakProfile = stealthBreakProfile;
        this.maxTakes = maxTakes < 1 ? 1 : maxTakes;
    }

    public bool MeetsPrerequisites(CharacterData character)
    {
        //Level check
        if (character.level < requiredLevel) return false;

        //Prerequisite feat check
        foreach (string prereqId in requiredFeatIds)
        {
            if (!character.equippedFeatIds.Contains(prereqId)) return false;
        }

        //Attribute threshold check
        foreach (var kvp in requiredAttributes)
        {
            int characterValue = character.GetAttributeScore(kvp.Key);

            if (characterValue < kvp.Value) return false;
        }

        return true;
    }

    public int GetTimesTaken(CharacterData character)
    {
        int count = 0;
        foreach (var id in character.equippedFeatIds)
            if (id == featId) count++;
        return count;
    }

    //Single availability rule shared by the level-up flow and the pregen plan applier:
    //under the take limit, and prerequisites met
    public bool CanTake(CharacterData character, out string reason)
    {
        if (GetTimesTaken(character) >= maxTakes)
        {
            reason = maxTakes == 1 ? "already taken" : $"already taken {maxTakes} times";
            return false;
        }

        if (!MeetsPrerequisites(character))
        {
            reason = "prerequisites not met";
            return false;
        }

        reason = null;
        return true;
    }
}
