using UnityEngine;
using System.Collections.Generic;

public static class EnemyDatabase
{
    public static Dictionary<string, EnemyDefinition> Definitions =
        new Dictionary<string, EnemyDefinition>()
        {
            {
                EnemyIds.BasicDrone,
                new EnemyDefinition(
                    enemy: EnemyIds.BasicDrone,
                    name: "Drone"
                )
                {
                    baseMaxHP       = 60,
                    babProgression  = ClassDefinition.BABProgression.Full, //placeholder enemy; removed when real enemies are authored
                    baseInitiative  = 4,
                    moveSpeed       = 6,
                    power           = 12,
                    agility         = 10,
                    endurance       = 10,
                    cognition       = 1,
                    insight         = 1,
                    presence        = 1,
                    xpValue         = 50,
                    lootTableId     = LootTableIds.BasicDrone,
                    additionalActionIds = new List<string> { ActionIds.RangedAttack },
                }
            },
            
            // ===== Training dummies (Milestone 28) =====
            // All: ignore difficulty scaling, no XP, no loot. Attributes still gain +0.5 per level above 1
            // (EnemyScaler pass 1), so expected saves/Defense at level N use the scaled attribute.

            // Target (Static): hit/miss, damage, crits, DR interaction. Invulnerable, never acts.
            {
                EnemyIds.TrainingTargetStatic,
                new EnemyDefinition(
                    enemy: EnemyIds.TrainingTargetStatic,
                    name: "Target (Static)"
                )
                {
                    baseMaxHP               = 100,
                    baseInitiative          = 0,
                    power = 10, agility = 10, endurance = 10,
                    cognition = 10, insight = 10, presence = 10,
                    resolveProgression      = ClassDefinition.SaveProgression.Poor,
                    responseProgression     = ClassDefinition.SaveProgression.Poor,
                    focusProgression        = ClassDefinition.SaveProgression.Poor,
                    ignoreDifficultyScaling = true,
                    isInvulnerableTarget    = true,
                    xpValue                 = 0,
                    attackPattern = new List<PatternStep> { new PatternStep(null, PatternStep.TargetRule.None) },
                }
            },

            // Target (Mindless): as Static but COG 1, so it is immune to mind-affecting effects (G8).
            {
                EnemyIds.TrainingTargetMindless,
                new EnemyDefinition(
                    enemy: EnemyIds.TrainingTargetMindless,
                    name: "Target (Mindless)"
                )
                {
                    baseMaxHP               = 100,
                    baseInitiative          = 0,
                    power = 10, agility = 10, endurance = 10,
                    cognition = 1, insight = 10, presence = 1,
                    resolveProgression      = ClassDefinition.SaveProgression.Poor,
                    responseProgression     = ClassDefinition.SaveProgression.Poor,
                    focusProgression        = ClassDefinition.SaveProgression.Poor,
                    ignoreDifficultyScaling = true,
                    isInvulnerableTarget    = true,
                    xpValue                 = 0,
                    attackPattern = new List<PatternStep> { new PatternStep(null, PatternStep.TargetRule.None) },
                }
            },

            // Hardened — Resolve: Resolve-gated effects. Good Resolve, Poor others. Killable (Save Gauntlet).
            {
                EnemyIds.TrainingHardenedResolve,
                new EnemyDefinition(
                    enemy: EnemyIds.TrainingHardenedResolve,
                    name: "Hardened (Resolve)"
                )
                {
                    baseMaxHP               = 40,
                    baseInitiative          = 0,
                    power = 10, agility = 10, endurance = 10,
                    cognition = 10, insight = 10, presence = 10,
                    resolveProgression      = ClassDefinition.SaveProgression.Good,
                    responseProgression     = ClassDefinition.SaveProgression.Poor,
                    focusProgression        = ClassDefinition.SaveProgression.Poor,
                    ignoreDifficultyScaling = true,
                    xpValue                 = 0,
                    attackPattern = new List<PatternStep> { new PatternStep(null, PatternStep.TargetRule.None) },
                }
            },

            // Hardened — Response: Response-gated effects. Good Response, Poor others. Killable.
            {
                EnemyIds.TrainingHardenedResponse,
                new EnemyDefinition(
                    enemy: EnemyIds.TrainingHardenedResponse,
                    name: "Hardened (Response)"
                )
                {
                    baseMaxHP               = 40,
                    baseInitiative          = 0,
                    power = 10, agility = 10, endurance = 10,
                    cognition = 10, insight = 10, presence = 10,
                    resolveProgression      = ClassDefinition.SaveProgression.Poor,
                    responseProgression     = ClassDefinition.SaveProgression.Good,
                    focusProgression        = ClassDefinition.SaveProgression.Poor,
                    ignoreDifficultyScaling = true,
                    xpValue                 = 0,
                    attackPattern = new List<PatternStep> { new PatternStep(null, PatternStep.TargetRule.None) },
                }
            },

            // Hardened — Focus: Focus-gated and mind-affecting powers. Good Focus, Poor others. Killable.
            // Phase 1 check: at level 10, INS 14 (+2) + Good save 7 = Focus +9 on any difficulty.
            {
                EnemyIds.TrainingHardenedFocus,
                new EnemyDefinition(
                    enemy: EnemyIds.TrainingHardenedFocus,
                    name: "Hardened (Focus)"
                )
                {
                    baseMaxHP               = 40,
                    baseInitiative          = 0,
                    power = 10, agility = 10, endurance = 10,
                    cognition = 10, insight = 10, presence = 10,
                    resolveProgression      = ClassDefinition.SaveProgression.Poor,
                    responseProgression     = ClassDefinition.SaveProgression.Poor,
                    focusProgression        = ClassDefinition.SaveProgression.Good,
                    ignoreDifficultyScaling = true,
                    xpValue                 = 0,
                    attackPattern = new List<PatternStep> { new PatternStep(null, PatternStep.TargetRule.None) },
                }
            },

            // Striker: defensive features, OAs, concentration on damage. Full BAB, melees the nearest
            // squad member every round, approaching to reach (D5). Killable.
            {
                EnemyIds.TrainingStriker,
                new EnemyDefinition(
                    enemy: EnemyIds.TrainingStriker,
                    name: "Striker"
                )
                {
                    baseMaxHP               = 30,
                    baseInitiative          = 1,
                    power = 14, agility = 12, endurance = 12,
                    cognition = 10, insight = 10, presence = 10,
                    babProgression          = ClassDefinition.BABProgression.Full,
                    resolveProgression      = ClassDefinition.SaveProgression.Poor,
                    responseProgression     = ClassDefinition.SaveProgression.Poor,
                    focusProgression        = ClassDefinition.SaveProgression.Poor,
                    ignoreDifficultyScaling = true,
                    xpValue                 = 0,
                    attackPattern = new List<PatternStep>
                    {
                        new PatternStep(ActionIds.MeleeAttack, PatternStep.TargetRule.Nearest, approach: true),
                    },
                }
            },

            // Marksman: LoS, cover, windows, Hide, detection. Full BAB, shoots the lowest-HP squad member,
            // never moves (no approach). RangedAttack range 2–5; no weapon items exist yet (unarmed 1d4).
            {
                EnemyIds.TrainingMarksman,
                new EnemyDefinition(
                    enemy: EnemyIds.TrainingMarksman,
                    name: "Marksman"
                )
                {
                    baseMaxHP               = 25,
                    baseInitiative          = 2,
                    power = 10, agility = 14, endurance = 10,
                    cognition = 10, insight = 12, presence = 10,
                    babProgression          = ClassDefinition.BABProgression.Full,
                    resolveProgression      = ClassDefinition.SaveProgression.Poor,
                    responseProgression     = ClassDefinition.SaveProgression.Poor,
                    focusProgression        = ClassDefinition.SaveProgression.Poor,
                    ignoreDifficultyScaling = true,
                    xpValue                 = 0,
                    additionalActionIds     = new List<string> { ActionIds.RangedAttack },
                    attackPattern = new List<PatternStep>
                    {
                        new PatternStep(ActionIds.RangedAttack, PatternStep.TargetRule.LowestHP),
                    },
                }
            },

            // Bruiser: forces Disabled / Dying for triage. High attack (Full BAB, POW 18, +2 BAB offset)
            // and high HP; melees the nearest squad member, approaching to reach. Killable.
            {
                EnemyIds.TrainingBruiser,
                new EnemyDefinition(
                    enemy: EnemyIds.TrainingBruiser,
                    name: "Bruiser"
                )
                {
                    baseMaxHP               = 80,
                    baseInitiative          = 0,
                    power = 18, agility = 10, endurance = 16,
                    cognition = 10, insight = 10, presence = 10,
                    babProgression          = ClassDefinition.BABProgression.Full,
                    resolveProgression      = ClassDefinition.SaveProgression.Good,
                    responseProgression     = ClassDefinition.SaveProgression.Poor,
                    focusProgression        = ClassDefinition.SaveProgression.Poor,
                    statBonuses = new List<StatModifier>
                    {
                        new StatModifier(StatType.BaseAttackBonus, 2, 1f, "training_bruiser_attack"),
                    },
                    ignoreDifficultyScaling = true,
                    xpValue                 = 0,
                    attackPattern = new List<PatternStep>
                    {
                        new PatternStep(ActionIds.MeleeAttack, PatternStep.TargetRule.Nearest, approach: true),
                    },
                }
            },

            // Large Target: 2x2 footprint, reach 2, threat zones, multi-tile targeting. Never acts. Killable.
            {
                EnemyIds.TrainingLargeTarget,
                new EnemyDefinition(
                    enemy: EnemyIds.TrainingLargeTarget,
                    name: "Large Target"
                )
                {
                    baseMaxHP               = 60,
                    baseInitiative          = 0,
                    power = 14, agility = 8, endurance = 14,
                    cognition = 10, insight = 10, presence = 10,
                    size                    = Unit.SizeCategory.Large,
                    resolveProgression      = ClassDefinition.SaveProgression.Poor,
                    responseProgression     = ClassDefinition.SaveProgression.Poor,
                    focusProgression        = ClassDefinition.SaveProgression.Poor,
                    ignoreDifficultyScaling = true,
                    xpValue                 = 0,
                    attackPattern = new List<PatternStep> { new PatternStep(null, PatternStep.TargetRule.None) },
                }
            },
            
            // Striker (Weak): Crowd Control (TR-07). Striker profile with 6 HP so bursts and area effects
            // can clear several at once. Approaches and melees the nearest squad member. Killable.
            {
                EnemyIds.TrainingStrikerWeak,
                new EnemyDefinition(
                    enemy: EnemyIds.TrainingStrikerWeak,
                    name: "Striker (Weak)"
                )
                {
                    baseMaxHP               = 6,
                    baseInitiative          = 1,
                    power = 14, agility = 12, endurance = 12,
                    cognition = 10, insight = 10, presence = 10,
                    babProgression          = ClassDefinition.BABProgression.Full,
                    resolveProgression      = ClassDefinition.SaveProgression.Poor,
                    responseProgression     = ClassDefinition.SaveProgression.Poor,
                    focusProgression        = ClassDefinition.SaveProgression.Poor,
                    ignoreDifficultyScaling = true,
                    xpValue                 = 0,
                    attackPattern = new List<PatternStep>
                    {
                        new PatternStep(ActionIds.MeleeAttack, PatternStep.TargetRule.Nearest, approach: true),
                    },
                }
            },
        };

    public static EnemyDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out EnemyDefinition enemy)) return enemy;

        Debug.LogWarning($"[EnemyDatabase] Enemy not found: {id}");
        return null;
    }
}
