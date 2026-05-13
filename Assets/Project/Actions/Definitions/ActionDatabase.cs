using System.Collections.Generic;
using UnityEngine;

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
                category: ActionDefinition.ActionCategory.Move,
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
                consumesCategories: new List<ActionDefinition.ActionCategory>
                {
                    ActionDefinition.ActionCategory.Standard,
                    ActionDefinition.ActionCategory.Move
                },
                effects: new List<IActionEffect>
                {
                    new ApplySelfStatusEffectEffect(unit =>
                        new DefenseBuffEffect(0.75f, 1)) // stronger than defensive // placeholder tuning
                },
                attackType: ActionDefinition.AttackType.None
            )},

            { ActionIds.Feint, new ActionDefinition(
                ActionIds.Feint,
                TargetType.Enemy,
                requiresTarget: true,
                minRange: 1,
                maxRange: 1,
                cooldown: 0,
                dealsDamage: false,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                category: ActionDefinition.ActionCategory.Swift,
                effects: new List<IActionEffect> { new FeintEffect() },
                attackType: ActionDefinition.AttackType.None
            )},

            { ActionIds.AcrobaticMovement, new ActionDefinition(
                ActionIds.AcrobaticMovement,
                TargetType.None,
                requiresTarget: false,
                minRange: 0,
                maxRange: 0,
                cooldown: 0,
                dealsDamage: false,
                targetingMode: ActionDefinition.TargetingMode.None,
                category: ActionDefinition.ActionCategory.Swift,
                effects: new List<IActionEffect> { new AcrobaticMovementActionEffect() },
                attackType: ActionDefinition.AttackType.None
            )},

            { ActionIds.Demoralize, new ActionDefinition(
                ActionIds.Demoralize,
                TargetType.Enemy,
                requiresTarget: true,
                minRange: 1,
                maxRange: 1,
                cooldown: 2,
                dealsDamage: false,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                category: ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect> { new DemoralizeEffect() },
                attackType: ActionDefinition.AttackType.None
            )},
            { ActionIds.UseConsumable, new ActionDefinition(
                ActionIds.UseConsumable,
                TargetType.None,
                requiresTarget: false,
                minRange: 0,
                maxRange: 0,
                cooldown: 0,
                dealsDamage: false,
                targetingMode: ActionDefinition.TargetingMode.None,
                category: ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new UseConsumableEffect()
                },
                attackType: ActionDefinition.AttackType.None
            )},
            { ActionIds.BreakWindow, new ActionDefinition(
                ActionIds.BreakWindow,
                TargetType.None,
                requiresTarget: false,
                minRange: 1,
                maxRange: 1,
                cooldown: 0,
                dealsDamage: false,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                category: ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new BreakWindowEffect()
                },
                attackType: ActionDefinition.AttackType.None,
                requiresLoS: false
            )},
            { ActionIds.TreatInjury, new ActionDefinition(
                ActionIds.TreatInjury,
                TargetType.Ally,
                requiresTarget: true,
                minRange: 1,
                maxRange: 1,
                cooldown: 0,
                dealsDamage: false,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                category: ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new TreatInjuryEffect()
                },
                attackType: ActionDefinition.AttackType.None
            )},
            { ActionIds.Hide, new ActionDefinition(
                ActionIds.Hide,
                TargetType.None,
                requiresTarget: false,
                minRange: 0,
                maxRange: 0,
                cooldown: 0,
                dealsDamage: false,
                targetingMode: ActionDefinition.TargetingMode.None,
                category: ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new HideEffect()
                },
                attackType: ActionDefinition.AttackType.None,
                requiresLoS: false
            )},

            { ActionIds.PowerSurge, new ActionDefinition(
                actionID:       ActionIds.PowerSurge,
                targetType:     TargetType.Self,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Free,
                effects:        new List<IActionEffect>
                {
                    new RestoreResourceEffect(2)
                },
                requiresLoS:    false
            )},

            { ActionIds.Overcharge, new ActionDefinition(
                actionID: ActionIds.Overcharge,
                targetType: TargetType.Self,
                requiresTarget: false,
                minRange: 0,
                maxRange: 0,
                category: ActionDefinition.ActionCategory.Free,
                effects: new List<IActionEffect>
                {
                    new ApplySelfStatusEffectEffect(unit =>
                    new OverchargeEffect(unit.GetModifier(StatType.Cognition)))
                },
                attackType: ActionDefinition.AttackType.None,
                requiresLoS: false,
                resourceCost: 1
            )},

            { ActionIds.DiagnoseWeakness, new ActionDefinition(
                actionID:       ActionIds.DiagnoseWeakness,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       8,
                category:       ActionDefinition.ActionCategory.Standard,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                effects: new List<IActionEffect>
                {
                    new ApplyStatusEffectActionEffect(target =>
                        new DiagnosedEffect())
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    true
            )},

            { ActionIds.DeployField, new ActionDefinition(
                actionID:       ActionIds.DeployField,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       6,
                category:       ActionDefinition.ActionCategory.Standard,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                effects: new List<IActionEffect>
                {
                    new DeployFieldEffect(new List<System.Func<StatusEffect>>
                    {
                        () => new StunPulseEffect(),
                        () => new SensorNetEffect()
                    })
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false,
                resourceCost:   2
            )},

            { ActionIds.SystemOverride, new ActionDefinition(
                actionID:       ActionIds.SystemOverride,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       4,
                category:       ActionDefinition.ActionCategory.Standard,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                effects: new List<IActionEffect>
                {
                    new SystemOverrideEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    true,
                resourceCost:   2
            )},

            { ActionIds.EmergencyPatch, new ActionDefinition(
                actionID:       ActionIds.EmergencyPatch,
                targetType:     TargetType.Ally,
                requiresTarget: true,
                minRange:       0,
                maxRange:       1,
                category:       ActionDefinition.ActionCategory.Standard,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                effects: new List<IActionEffect>
                {
                    new HealEffect(1, 6, StatType.Cognition)
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false,
                resourceCost:   1
            )},

            { ActionIds.FullSpectrumInterface, new ActionDefinition(
                actionID:       ActionIds.FullSpectrumInterface,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                cooldown:       2,
                category:       ActionDefinition.ActionCategory.Standard,
                targetingMode:  ActionDefinition.TargetingMode.None,
                effects: new List<IActionEffect>
                {
                    new ApplyStatusEffectToUnitsInRangeEffect(
                        attacker => new FullSpectrumEffect(
                            attacker.activeEffects.Exists(e => e is DistributedProcessingEffect) ? 1 : 0),
                        range: 6,
                        targetsAllies: true)
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false,
                resourceCost:   3
            )},

            { ActionIds.CommandOverrideAction, new ActionDefinition(
                actionID:       ActionIds.CommandOverrideAction,
                targetType:     TargetType.Ally,
                requiresTarget: true,
                minRange:       1,
                maxRange:       6,
                category:       ActionDefinition.ActionCategory.Free,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                effects: new List<IActionEffect>
                {
                    new CommandOverrideActionEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},
            
            { ActionIds.WeaponMount, new ActionDefinition(
                actionID:       ActionIds.WeaponMount,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new WeaponMountActionEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false,
                resourceCost:   1
            )},

            { ActionIds.OverdriveProtocol, new ActionDefinition(
                actionID:       ActionIds.OverdriveProtocol,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Free,
                effects: new List<IActionEffect>
                {
                    new OverdriveProtocolActionEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false,
                resourceCost:   3
            )},

            { ActionIds.ArmamentBurst, new ActionDefinition(
                actionID:       ActionIds.ArmamentBurst,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                cooldown:       0,
                dealsDamage:    true,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new ArmamentBurstEffect()
                },
                attackType:     ActionDefinition.AttackType.Ranged,
                requiresLoS:    false,
                resourceCost:   3
            )},

            { ActionIds.TerminalForce, new ActionDefinition(
                actionID:       ActionIds.TerminalForce,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       2,
                maxRange:       5,
                dealsDamage:    true,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new TerminalForceEffect()
                },
                attackType:     ActionDefinition.AttackType.Ranged,
                requiresLoS:    true,
                resourceCost:   4
            )},

            // ── GHOST PROTOCOL ─────────────────────────────────────────────────────────
            { ActionIds.RemoteInterface, new ActionDefinition(
                actionID:       ActionIds.RemoteInterface,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new RemoteInterfaceEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false,
                resourceCost:   1
            )},

            { ActionIds.NeuralIntrusion, new ActionDefinition(
                actionID:       ActionIds.NeuralIntrusion,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       6,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new NeuralIntrusionEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    true,
                resourceCost:   2
            )},

            { ActionIds.CascadeBurst, new ActionDefinition(
                actionID:       ActionIds.CascadeBurst,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new CascadeBurstEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false,
                resourceCost:   3
            )},

            { ActionIds.GridCollapse, new ActionDefinition(
                actionID:       ActionIds.GridCollapse,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new GridCollapseEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false,
                resourceCost:   4
            )},

            // ── VANGUARD ───────────────────────────────────────────────────────────────
            { ActionIds.NeuralSync, new ActionDefinition(
                actionID:       ActionIds.NeuralSync,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                cooldown:       0,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new NeuralSyncActionEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false,
                resourceCost:   2
            )},

            { ActionIds.CoordinatedFire, new ActionDefinition(
                actionID:       ActionIds.CoordinatedFire,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       8,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new CoordinatedFireEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    true,
                resourceCost:   1
            )},

            { ActionIds.ShieldProtocol, new ActionDefinition(
                actionID:       ActionIds.ShieldProtocol,
                targetType:     TargetType.Ally,
                requiresTarget: true,
                minRange:       0,
                maxRange:       4,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new ShieldProtocolActionEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false,
                resourceCost:   2
            )},

            { ActionIds.HighCommand, new ActionDefinition(
                actionID:       ActionIds.HighCommand,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new HighCommandEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false,
                resourceCost:   4
            )},

            { ActionIds.CommandOverrideAction, new ActionDefinition(
                actionID:       ActionIds.CommandOverrideAction,
                targetType:     TargetType.Ally,
                requiresTarget: true,
                minRange:       1,
                maxRange:       6,
                category:       ActionDefinition.ActionCategory.Free,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                effects: new List<IActionEffect>
                {
                    new CommandOverrideActionEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.DiagnoseWeakness, new ActionDefinition(
                actionID:       ActionIds.DiagnoseWeakness,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       8,
                category:       ActionDefinition.ActionCategory.Standard,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                effects: new List<IActionEffect>
                {
                    new DiagnoseWeaknessEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    true
            )},

            { ActionIds.ActivateElectronicShroud, new ActionDefinition(
                actionID:       ActionIds.ActivateElectronicShroud,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Free,
                effects: new List<IActionEffect>
                {
                    new ActivateElectronicShroudEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.SensorLinkShare, new ActionDefinition(
                actionID:       ActionIds.SensorLinkShare,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Free,
                effects: new List<IActionEffect>
                {
                    new SensorLinkShareEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false,
                cooldown:       0
            )},
        };

    public static ActionDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out ActionDefinition action)) return action;

        Debug.LogWarning($"[ActionDatabase] Action not found: {id}");
        return null;
    }
}