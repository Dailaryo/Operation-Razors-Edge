using System.Collections.Generic;
using UnityEngine;
using static ActionDefinition;

public static class ActionDatabase
{
    public static Dictionary<string, ActionDefinition> Definitions =
        new Dictionary<string, ActionDefinition>()
        {
             //MELEE ATTACK
            { ActionIds.MeleeAttack, new ActionDefinition(
                ActionIds.MeleeAttack,
                TargetType.Enemy,
                true,
                1,
                1,
                cooldown: 0,
                dealsDamage: true,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                category: ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new DamageEffect(),
                    new ScalingDamageEffect()
                },
                attackType: ActionDefinition.AttackType.Melee,
                scalingStat: StatType.MeleePower,
                scalingMultiplier: 1f
            )},

            //RANGED ATTACK (TEMPORARY BASELINE)
            { ActionIds.RangedAttack, new ActionDefinition(
                ActionIds.RangedAttack,
                TargetType.Enemy,
                true,
                2,
                5,
                cooldown: 0,
                dealsDamage: true,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                category: ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new DamageEffect(),
                    new ScalingDamageEffect()
                },
                attackType: ActionDefinition.AttackType.Ranged,
                scalingStat: StatType.Accuracy,
                scalingMultiplier : 1f
            )},

            //MOVE
            { ActionIds.Move, new ActionDefinition(
                ActionIds.Move,
                TargetType.None,
                false,
                0,
                0,
                cooldown: 0,
                dealsDamage: false,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                category: ActionCategory.Move,
                effects: new List<IActionEffect>
                {
                    new MoveEffect()
                },
                attackType: ActionDefinition.AttackType.None
            )},

            //FIGHTING DEFENSIVELY (TOGGLE)
            { ActionIds.FightingDefensively, new ActionDefinition(
                ActionIds.FightingDefensively,
                TargetType.None,
                false,
                0,
                0,
                cooldown: 0,
                dealsDamage: false,
                targetingMode: ActionDefinition.TargetingMode.None,
                category: ActionDefinition.ActionCategory.Free,
                effects: new List<IActionEffect>
                {
                    new ApplySelfStatusEffectEffect(unit =>
                        new DefenseBuffEffect(0.25f, 1)) // placeholder tuning
                },
                attackType: ActionDefinition.AttackType.None
            )},

            //TOTAL DEFENSE (FULL ACTION)
            { ActionIds.TotalDefense, new ActionDefinition(
                ActionIds.TotalDefense,
                TargetType.None,
                false,
                0,
                0,
                cooldown: 0,
                dealsDamage: false,
                targetingMode: ActionDefinition.TargetingMode.None,
                category: ActionDefinition.ActionCategory.Standard,
                consumesCategories: new List<ActionCategory>
                {
                    ActionCategory.Standard,
                    ActionCategory.Move
                },
                effects: new List<IActionEffect>
                {
                    new ApplySelfStatusEffectEffect(unit =>
                        new DefenseBuffEffect(0.75f, 1)) // stronger than defensive
                },
                attackType: ActionDefinition.AttackType.None
            )}
        };


};