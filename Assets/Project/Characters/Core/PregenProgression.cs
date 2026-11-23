using System.Collections.Generic;
using UnityEngine;

// DEBUG BYPASS: auto-applies a pregen's pre-written level plan during LevelUp().
// Temporary stand-in for player choice until custom characters and the real level-up UI exist.
// At that point these same plans become the UI's pre-selected defaults instead.
public static class PregenProgression
{
    public static void ApplyPlannedLevel(CharacterData data, int level)
    {
        var plan = PregenDatabase.GetPlan(data.characterId);
        if (plan == null) return;

        if (!plan.TryGetValue(level, out var step))
        {
            Warn(data, level, "no plan entry for this level");
            return;
        }

        ApplyFeats(data, level, step);
        ApplyMutations(data, level, step);   //before choices: an upgrade pick can need this level's purchase
        ApplyChoices(data, level, step);
        ApplyASI(data, level, step);
        ApplySkillRanks(data, level, step);
        ApplyPowers(data, level, step);

        if (IsLevelResolved(data, level, step))
            data.levelUpPending = false;
    }

    private static void ApplyFeats(CharacterData data, int level, PregenLevelPlan step)
    {
        foreach (var featId in step.feats)
        {
            var feat = FeatDatabase.Get(featId);
            if (feat == null) { Warn(data, level, $"feat '{featId}' not found"); continue; }

            //Granted feats (background/species, held in unlockedFeatIds) are free; others cost a feat point
            bool granted = data.unlockedFeatIds.Contains(featId);
            if (!granted && data.featPointsAvailable <= 0)
            {
                Warn(data, level, $"no feat point available for '{featId}'");
                continue;
            }

            if (!feat.CanTake(data, out string reason))
            {
                Warn(data, level, $"cannot take '{featId}': {reason}");
                continue;
            }

            data.equippedFeatIds.Add(featId);
            if (granted) data.unlockedFeatIds.Remove(featId);
            else data.featPointsAvailable--;
        }
    }

    private static void ApplyChoices(CharacterData data, int level, PregenLevelPlan step)
    {
        foreach (var optionId in step.choiceOptions)
        {
            string nodeId = null;

            foreach (var pendingId in data.pendingChoiceNodeIds)
            {
                var node = ChoiceNodeDatabase.Get(pendingId);
                if (node != null && node.GetAvailableOptions(data.takenChoiceOptionIds, data)
                        .Exists(o => o.optionId == optionId))
                {
                    nodeId = pendingId;
                    break;
                }
            }

            if (nodeId == null)
            {
                Warn(data, level, $"choice '{optionId}' is not available from any pending node");
                continue;
            }

            data.takenChoiceOptionIds.Add(optionId);
            ChoiceNodeDatabase.Get(nodeId)?.options.Find(o => o.optionId == optionId)?.ApplyPassives(data);
            data.pendingChoiceNodeIds.Remove(nodeId);
        }
    }

    //Helix Warrior mutation purchases: each costs its PassiveDefinition.mutationCost from the Mutation Point pool
    //(the class resource maximum at this level). Recorded as an unlocked passive; spent MP stays on the character
    private static void ApplyMutations(CharacterData data, int level, PregenLevelPlan step)
    {
        if (step.mutations.Count == 0) return;

        var resource = ClassDatabase.Get(data.classId)?.resource;
        int pool = resource != null ? resource.GetMaxAtLevel(level) : 0;

        foreach (var passiveId in step.mutations)
        {
            var passive = PassiveDatabase.Get(passiveId);
            if (passive == null || passive.mutationCost <= 0)
            { Warn(data, level, $"'{passiveId}' is not a purchasable mutation"); continue; }

            if (data.unlockedPassiveIds.Contains(passiveId))
            { Warn(data, level, $"mutation '{passive.displayName}' is already owned"); continue; }

            int left = pool - data.mutationPointsSpent;
            if (passive.mutationCost > left)
            { Warn(data, level, $"mutation '{passive.displayName}' costs {passive.mutationCost} MP, only {left} of {pool} left"); continue; }

            data.mutationPointsSpent += passive.mutationCost;
            data.unlockedPassiveIds.Add(passiveId);
        }
    }

    private static void ApplyASI(CharacterData data, int level, PregenLevelPlan step)
    {
        if (!step.asi.HasValue) return;

        if (!CharacterProgression.IsASILevel(level))
        {
            Warn(data, level, $"ASI planned on a non-ASI level");
            return;
        }

        CharacterProgression.ApplyAbilityScoreIncreases(data, step.asi.Value);
    }

    private static void ApplySkillRanks(CharacterData data, int level, PregenLevelPlan step)
    {
        foreach (var kvp in step.skillRanks)
        {
            SkillType skill = kvp.Key;
            int add = kvp.Value;
            data.skillRanks.TryGetValue(skill, out int current);

            if (!SkillResolver.IsSkillAvailable(data, skill))
            { Warn(data, level, $"{skill} is not available to this character"); continue; }

            if (add > data.skillRanksAvailable)
            { Warn(data, level, $"{skill}: planned {add} rank(s), only {data.skillRanksAvailable} available"); continue; }

            //Pathfinder rule: ranks in a skill can never exceed character level
            if (current + add > level)
            { Warn(data, level, $"{skill}: {current + add} ranks would exceed the level cap of {level}"); continue; }

            data.skillRanks[skill] = current + add;
            data.skillRanksAvailable -= add;
        }
    }

    private static void ApplyPowers(CharacterData data, int level, PregenLevelPlan step)
    {
        foreach (var powerId in step.powers)
            if (!PowerProgression.TryLearn(data, powerId, out string reason))
                Warn(data, level, $"cannot learn '{powerId}': {reason}");
    }

    private static bool IsLevelResolved(CharacterData data, int level, PregenLevelPlan step)
    {
        var unresolved = new List<string>();

        if (data.featPointsAvailable > 0) unresolved.Add($"{data.featPointsAvailable} feat point(s)");
        if (data.skillRanksAvailable > 0) unresolved.Add($"{data.skillRanksAvailable} skill rank(s)");
        if (data.pendingChoiceNodeIds.Count > 0) unresolved.Add($"{data.pendingChoiceNodeIds.Count} choice node(s)");
        if (CharacterProgression.IsASILevel(level) && !step.asi.HasValue) unresolved.Add("ability score increase");
        if (PowerProgression.HasPendingPicks(data)) unresolved.Add("power pick(s)");

        if (unresolved.Count == 0) return true;

        Warn(data, level, "unresolved after plan: " + string.Join(", ", unresolved));
        return false;
    }

    private static void Warn(CharacterData data, int level, string message)
    {
        Debug.LogWarning($"[PregenProgression] {data.name} level {level}: {message}");
    }
}