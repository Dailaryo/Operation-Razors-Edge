using UnityEngine;
using System.Collections.Generic;

//What each unit knows about each opposing unit. Passive check: 10 + Knowledge (Human, Known Alien),
//Xenology (Unknown Alien) or Computer Use (Construct) against DC 10 + target HD.
//Meet = Basic, beat by 5 = Detailed, beat by 10 = Full. Checked at encounter start and when a target is
//first detected; never lowered during the encounter. The squad sees the best level any member has.
public static class AwarenessManager
{
    private static Dictionary<(Unit observer, Unit target), AwarenessLevel> levels =
        new Dictionary<(Unit observer, Unit target), AwarenessLevel>();

    //Called by CombatManager.StartCombat after DetectionStateManager.Initialize
    public static void Initialize(List<Unit> allUnits)
    {
        levels.Clear();

        foreach (var observer in allUnits)
            foreach (var target in allUnits)
                if (observer.isEnemy != target.isEnemy && DetectionStateManager.CanTarget(observer, target))
                    Evaluate(observer, target);
    }

    public static AwarenessLevel Get(Unit observer, Unit target)
    {
        return levels.TryGetValue((observer, target), out var level) ? level : AwarenessLevel.Unknown;
    }

    //Best level any unit on the observing side has on the target (the squad-wide view)
    public static AwarenessLevel GetForSide(bool observersAreEnemies, Unit target)
    {
        var best = AwarenessLevel.Unknown;

        foreach (var kvp in levels)
        {
            if (kvp.Key.target != target || kvp.Key.observer.isEnemy != observersAreEnemies) continue;
            if (kvp.Value > best) best = kvp.Value;
        }

        return best;
    }

    //Never lowers an existing level
    public static void Raise(Unit observer, Unit target, AwarenessLevel level)
    {
        if (level > Get(observer, target))
            levels[(observer, target)] = level;
    }

    //Passive knowledge check for one observer/target pair
    public static void Evaluate(Unit observer, Unit target)
    {
        if (observer.IsDead() || target.IsDead()) return;

        var data = observer.sourceCharacterData;
        if (data != null && data.unlockedPassiveIds.Contains(PassiveIds.EmpathCrowdRead))
        {
            Raise(observer, target, AwarenessLevel.Full);
            return;
        }

        Raise(observer, target, RollPassive(observer, target));
    }

    //Called by DetectionStateManager.Set when a target first becomes Detected or Exposed
    public static void OnDetected(Unit observer, Unit target)
    {
        Evaluate(observer, target);
    }

    public const int ApotheosisRange = 12;  //60 ft.

    //Empath Apotheosis: Full awareness of every living opposing unit within 60 ft., whether or not it is seen.
    //Called at encounter start and at the start of every round.
    public static void ApplyApotheosis(List<Unit> allUnits)
    {
        foreach (var observer in allUnits)
        {
            var data = observer.sourceCharacterData;
            if (data == null || observer.IsDead()) continue;
            if (!data.unlockedPassiveIds.Contains(PassiveIds.EmpathApotheosis)) continue;

            foreach (var target in allUnits)
            {
                if (target.isEnemy == observer.isEnemy || target.IsDead()) continue;
                if (DistanceUtility.MovementCostDistance(observer.position, target.position) > ApotheosisRange)
                    continue;

                Raise(observer, target, AwarenessLevel.Full);
            }
        }
    }

    public static KnowledgeCategory GetCategory(Unit unit)
    {
        var data = unit.sourceCharacterData;
        if (data == null) return KnowledgeCategory.Human;

        if (!string.IsNullOrEmpty(data.speciesId))
        {
            var species = SpeciesDatabase.Get(data.speciesId);
            if (species != null) return species.knowledgeCategory;
        }

        if (data.isEnemy)
        {
            var def = EnemyDatabase.Get(data.characterId);
            if (def != null) return def.knowledgeCategory;
        }

        return KnowledgeCategory.Human;
    }

    private static AwarenessLevel RollPassive(Unit observer, Unit target)
    {
        if (observer.sourceCharacterData == null) return AwarenessLevel.Unknown;

        int score = 10 + SkillResolver.GetSkillTotal(observer, GetSkill(GetCategory(target)));
        int dc = 10 + (target.sourceCharacterData != null ? target.sourceCharacterData.level : 0);

        if (score >= dc + 10) return AwarenessLevel.Full;
        if (score >= dc + 5) return AwarenessLevel.Detailed;
        if (score >= dc) return AwarenessLevel.Basic;
        return AwarenessLevel.Unknown;
    }

    private static SkillType GetSkill(KnowledgeCategory category)
    {
        switch (category)
        {
            case KnowledgeCategory.UnknownAlien: return SkillType.Xenology;
            case KnowledgeCategory.Construct: return SkillType.ComputerUse;
            default: return SkillType.Knowledge;
        }
    }
}