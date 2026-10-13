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

            // ── HELIX WARRIOR — ACTIVE MUTATIONS ──────────────────────────────────────

            { ActionIds.HelixAdrenalineSurge, new ActionDefinition(
                actionID:       ActionIds.HelixAdrenalineSurge,
                targetType:     TargetType.Self,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Free,
                cooldown:       -1,
                effects: new List<IActionEffect>
                {
                    new ApplySelfStatusEffectEffect(unit =>
                        new StatBonusEffect(StatType.Power, 4, "AdrenalineSurge", StatModifier.BonusType.Racial))
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.HelixAdrenalFrenzy, new ActionDefinition(
                actionID:       ActionIds.HelixAdrenalFrenzy,
                targetType:     TargetType.Self,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Free,
                cooldown:       -1,
                effects: new List<IActionEffect>
                {
                    new ApplySelfStatusEffectEffect(unit =>
                        new StatBonusEffect(StatType.Power, 6, "AdrenalFrenzy_Power", StatModifier.BonusType.Racial)),
                    new ApplySelfStatusEffectEffect(unit =>
                        new StatBonusEffect(StatType.Accuracy, 1, "AdrenalFrenzy_Attack", StatModifier.BonusType.Racial))
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            // ── HELIX WARRIOR — BERSERKER PATH ────────────────────────────────────────

            { ActionIds.HelixFrenziedStrike, new ActionDefinition(
                actionID:       ActionIds.HelixFrenziedStrike,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       1,
                maxRange:       1,
                dealsDamage:    true,
                category:       ActionDefinition.ActionCategory.Standard,
                consumesCategories: new List<ActionDefinition.ActionCategory>
                {
                    ActionDefinition.ActionCategory.Standard,
                    ActionDefinition.ActionCategory.Move
                },
                targetingMode:  ActionDefinition.TargetingMode.None,
                effects: new List<IActionEffect>
                {
                    new ApplyStatusEffectToEnemiesEffect(
                        (attacker, target) =>
                        {
                            if (DistanceUtility.IsAdjacent(attacker.position, target.position))
                                return new StatBonusEffect(StatType.Defense, -2, "FrenziedStrike", StatModifier.BonusType.Untyped);
                            return null;
                        })
                },
                attackType:     ActionDefinition.AttackType.Melee,
                requiresLoS:    false
            )},

            { ActionIds.HelixBerserkApotheosis, new ActionDefinition(
                actionID:       ActionIds.HelixBerserkApotheosis,
                targetType:     TargetType.Self,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Free,
                cooldown:       -1,
                effects: new List<IActionEffect>
                {
                    new ApplySelfStatusEffectEffect(unit =>
                        new StatBonusEffect(StatType.MeleePower, 4, "BerserkApotheosis_Damage", StatModifier.BonusType.Morale)),
                    new ApplySelfStatusEffectEffect(unit =>
                        new StatBonusEffect(StatType.Accuracy, 2, "BerserkApotheosis_Attack", StatModifier.BonusType.Morale))
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.HelixApexMutationBerserker, new ActionDefinition(
                actionID:       ActionIds.HelixApexMutationBerserker,
                targetType:     TargetType.Self,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Free,
                cooldown:       -1,
                effects: new List<IActionEffect>
                {
                    new ApplySelfStatusEffectEffect(unit =>
                        new StatBonusEffect(StatType.MeleePower, 6, "ApexMutation_Berserker", StatModifier.BonusType.Morale)),
                    new ApplySelfStatusEffectEffect(unit =>
                        new StatBonusEffect(StatType.Accuracy, 3, "ApexMutation_BerserkerAcc", StatModifier.BonusType.Morale))
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            // ── HELIX WARRIOR — APEX PREDATOR PATH ───────────────────────────────────

            { ActionIds.HelixPredatoryLeap, new ActionDefinition(
                actionID:       ActionIds.HelixPredatoryLeap,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       2,
                maxRange:       6,
                dealsDamage:    true,
                category:       ActionDefinition.ActionCategory.Standard,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                effects: new List<IActionEffect>
                {
                    new DamageEffect(),
                    new ScalingDamageEffect()
                },
                attackType:     ActionDefinition.AttackType.Melee,
                scalingStat:    StatType.MeleePower,
                scalingMultiplier: 1f,
                requiresLoS:    true
            )},

            { ActionIds.HelixMarkPrey, new ActionDefinition(
                actionID:       ActionIds.HelixMarkPrey,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       8,
                dealsDamage:    false,
                category:       ActionDefinition.ActionCategory.Swift,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                effects: new List<IActionEffect>
                {
                    new ApplyStatusEffectActionEffect(target =>
                        new DiagnosedEffect())
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    true
            )},

            { ActionIds.HelixShadowstep, new ActionDefinition(
                actionID:       ActionIds.HelixShadowstep,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       4,
                dealsDamage:    false,
                category:       ActionDefinition.ActionCategory.Move,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                effects: new List<IActionEffect>
                {
                    new MoveEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false,
                cooldown:       2
            )},

            { ActionIds.HelixPredatorApotheosis, new ActionDefinition(
                actionID:       ActionIds.HelixPredatorApotheosis,
                targetType:     TargetType.Self,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Free,
                cooldown:       -1,
                effects: new List<IActionEffect>
                {
                    new ApplySelfStatusEffectEffect(unit =>
                        new StatBonusEffect(StatType.Accuracy, 4, "PredatorApotheosis_Attack", StatModifier.BonusType.Morale)),
                    new ApplySelfStatusEffectEffect(unit =>
                        new StatBonusEffect(StatType.MeleePower, 4, "PredatorApotheosis_Damage", StatModifier.BonusType.Morale))
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.HelixApexMutationApex, new ActionDefinition(
                actionID:       ActionIds.HelixApexMutationApex,
                targetType:     TargetType.Self,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Free,
                cooldown:       -1,
                effects: new List<IActionEffect>
                {
                    new ApplySelfStatusEffectEffect(unit =>
                        new StatBonusEffect(StatType.Accuracy, 5, "ApexMutation_Apex", StatModifier.BonusType.Morale)),
                    new ApplySelfStatusEffectEffect(unit =>
                        new StatBonusEffect(StatType.MoveSpeed, 2, "ApexMutation_ApexSpeed", StatModifier.BonusType.Morale))
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            // ── HELIX WARRIOR — IRON FLESH PATH ──────────────────────────────────────

            { ActionIds.HelixIronApotheosis, new ActionDefinition(
                actionID:       ActionIds.HelixIronApotheosis,
                targetType:     TargetType.Self,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Free,
                cooldown:       -1,
                effects: new List<IActionEffect>
                {
                    new ApplySelfStatusEffectEffect(unit =>
                        new DamageReductionEffect(6)),
                    new ApplySelfStatusEffectEffect(unit =>
                        new StatBonusEffect(StatType.NaturalArmor, 3, "IronApotheosis_Armor", StatModifier.BonusType.Morale))
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.HelixApexMutationIron, new ActionDefinition(
                actionID:       ActionIds.HelixApexMutationIron,
                targetType:     TargetType.Self,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Free,
                cooldown:       -1,
                effects: new List<IActionEffect>
                {
                    new ApplySelfStatusEffectEffect(unit =>
                        new DamageReductionEffect(8)),
                    new ApplySelfStatusEffectEffect(unit =>
                        new StatBonusEffect(StatType.NaturalArmor, 4, "ApexMutation_Iron", StatModifier.BonusType.Morale))
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            // --- Infiltrator ---

            { ActionIds.DeadAim, new ActionDefinition(
                actionID:          ActionIds.DeadAim,
                targetType:        TargetType.Enemy,
                requiresTarget:    true,
                minRange:          2,
                maxRange:          10,
                cooldown:          0,
                dealsDamage:       true,
                targetingMode:     ActionDefinition.TargetingMode.Tile,
                category:          ActionDefinition.ActionCategory.Standard,
                consumesCategories: new List<ActionDefinition.ActionCategory>
                {
                    ActionDefinition.ActionCategory.Standard,
                    ActionDefinition.ActionCategory.Move
                },
                effects: new List<IActionEffect>
                {
                    new DamageEffect()
                },
                attackType:        ActionDefinition.AttackType.Ranged
            )},

            { ActionIds.InfiltratorVanish, new ActionDefinition(
                actionID:       ActionIds.InfiltratorVanish,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.None,
                category:       ActionDefinition.ActionCategory.Standard
                // Re-enters stealth mid-combat — effect scoped during M23 stealth authoring
            )},

            { ActionIds.InfiltratorSpectralForm, new ActionDefinition(
                actionID:       ActionIds.InfiltratorSpectralForm,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.None,
                category:       ActionDefinition.ActionCategory.Standard
                // 1/combat untargetable for 1 round — effect scoped during Ghost path authoring
            )},

            { ActionIds.InfiltratorDeathFromShadows, new ActionDefinition(
                actionID:       ActionIds.InfiltratorDeathFromShadows,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       2,
                maxRange:       10,
                cooldown:       0,
                dealsDamage:    true,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new DamageEffect()
                },
                attackType:     ActionDefinition.AttackType.Ranged
                // 3/combat guaranteed crit from stealth — crit logic scoped during Ghost path authoring
            )},

            { ActionIds.InfiltratorExposeWeakness, new ActionDefinition(
                actionID:       ActionIds.InfiltratorExposeWeakness,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       6,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard
                // Target suffers -2 Defense until end of round — effect scoped during Operative path authoring
            )},

            { ActionIds.InfiltratorDisablingStrike, new ActionDefinition(
                actionID:       ActionIds.InfiltratorDisablingStrike,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       1,
                cooldown:       0,
                dealsDamage:    true,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new DamageEffect()
                },
                attackType:     ActionDefinition.AttackType.Melee
                // On hit, target loses 1 action next turn — scoped during Operative path authoring
            )},

            { ActionIds.InfiltratorChokePoint, new ActionDefinition(
                actionID:       ActionIds.InfiltratorChokePoint,
                targetType:     TargetType.None,
                requiresTarget: true,
                minRange:       1,
                maxRange:       6,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard
                // Designate tile; enemies entering spend +1 action — scoped during Operative path authoring
            )},

            { ActionIds.InfiltratorCalculatedTakedown, new ActionDefinition(
                actionID:       ActionIds.InfiltratorCalculatedTakedown,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       1,
                cooldown:       0,
                dealsDamage:    true,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new DamageEffect()
                },
                attackType:     ActionDefinition.AttackType.Melee
                // Guaranteed stun if target is debuffed — scoped during Operative path authoring
            )},

            { ActionIds.InfiltratorPsionicFeint, new ActionDefinition(
                actionID:       ActionIds.InfiltratorPsionicFeint,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       6,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard
                // COG-based attack, Confused 1 round on hit — scoped during Phantom path authoring
            )},

            { ActionIds.InfiltratorMentalCloak, new ActionDefinition(
                actionID:       ActionIds.InfiltratorMentalCloak,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.None,
                category:       ActionDefinition.ActionCategory.Standard
                // Immune to Sweep/Detect for 3 rounds — scoped during Phantom path authoring
            )},

            { ActionIds.InfiltratorPsychicDisplacement, new ActionDefinition(
                actionID:       ActionIds.InfiltratorPsychicDisplacement,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       4,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Move
                // Teleport 4 tiles as Move action, 3/combat — scoped during Phantom path authoring
            )},

            { ActionIds.InfiltratorPhaseOut, new ActionDefinition(
                actionID:       ActionIds.InfiltratorPhaseOut,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.None,
                category:       ActionDefinition.ActionCategory.Standard
                // Incorporeal for 2 rounds, 1/combat — scoped during Phantom path authoring
            )},

            // --- Dreadnought ---

            { ActionIds.AbilitySurge, new ActionDefinition(
                ActionIds.AbilitySurge,
                TargetType.None,
                requiresTarget: false,
                minRange: 0,
                maxRange: 0,
                cooldown: 0,
                dealsDamage: false,
                targetingMode: ActionDefinition.TargetingMode.None,
                category: ActionDefinition.ActionCategory.Free,
                effects: new List<IActionEffect>
                {
                    new ApplySelfStatusEffectEffect(unit =>
                    {
                        int classLevel = unit.sourceCharacterData?.level ?? 1;

                        bool upgraded = unit.sourceCharacterData != null &&
                            unit.sourceCharacterData.unlockedPassiveIds
                                .Contains(PassiveIds.JuggernautAbilitySurgeUpgrade);

                        int powBonus = upgraded ? 10 : 8;

                        return new AbilitySurgeEffect(classLevel, powBonus);
                    })
                },
                attackType: ActionDefinition.AttackType.None,
                resourceCost: 1
            )},

            { ActionIds.BastionBulwark, new ActionDefinition(
                ActionIds.BastionBulwark,
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
                    new BastionBulwarkEffect()
                },
                attackType: ActionDefinition.AttackType.None
            )},

            { ActionIds.JuggernautUnstoppableCharge, new ActionDefinition(
                ActionIds.JuggernautUnstoppableCharge,
                TargetType.Enemy,
                requiresTarget: true,
                minRange: 2,
                maxRange: 6,
                cooldown: 0,
                dealsDamage: true,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                category: ActionDefinition.ActionCategory.Standard,
                consumesCategories: new List<ActionDefinition.ActionCategory>
                {
                    ActionDefinition.ActionCategory.Standard,
                    ActionDefinition.ActionCategory.Move
                },
                effects: new List<IActionEffect>
                {
                    new JuggernautUnstoppableChargeEffect()
                },
                attackType: ActionDefinition.AttackType.Melee
            )},

            { ActionIds.JuggernautGroundbreaker, new ActionDefinition(
                ActionIds.JuggernautGroundbreaker,
                TargetType.None,
                requiresTarget: false,
                minRange: 0,
                maxRange: 0,
                cooldown: Unit.CombatCooldown,
                dealsDamage: false,
                targetingMode: ActionDefinition.TargetingMode.None,
                category: ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new JuggernautGroundbreakerEffect()
                },
                attackType: ActionDefinition.AttackType.None
            )},

            { ActionIds.ArtilleristSuppressionFire, new ActionDefinition(
                ActionIds.ArtilleristSuppressionFire,
                TargetType.Enemy,
                requiresTarget: true,
                minRange: 1,
                maxRange: 8,
                cooldown: 0,
                dealsDamage: true,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                category: ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new DamageEffect(),
                    new ArtilleristSuppressionFireEffect()
                },
                attackType: ActionDefinition.AttackType.Ranged
            )},

            { ActionIds.ArtilleristIndirectFire, new ActionDefinition(
                ActionIds.ArtilleristIndirectFire,
                TargetType.Enemy,
                requiresTarget: true,
                minRange: 2,
                maxRange: 10,
                cooldown: 0,
                dealsDamage: true,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                category: ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new DamageEffect(),
                    new ArtilleristIndirectFireEffect()
                },
                attackType: ActionDefinition.AttackType.Ranged
            )},

            { ActionIds.ArtilleristVolleyFire, new ActionDefinition(
                ActionIds.ArtilleristVolleyFire,
                TargetType.Enemy,
                requiresTarget: true,
                minRange: 1,
                maxRange: 3,
                cooldown: 0,
                dealsDamage: true,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                category: ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new ArtilleristVolleyFireEffect()
                },
                attackType: ActionDefinition.AttackType.Ranged
            )},

            { ActionIds.ArtilleristDevastation, new ActionDefinition(
                ActionIds.ArtilleristDevastation,
                TargetType.Enemy,
                requiresTarget: true,
                minRange: 1,
                maxRange: 8,
                cooldown: Unit.CombatCooldown,
                dealsDamage: false,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                category: ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new ArtilleristDevastationEffect()
                },
                attackType: ActionDefinition.AttackType.Ranged
            )},

            { ActionIds.HealAlly, new ActionDefinition(
                actionID:       ActionIds.HealAlly,
                targetType:     TargetType.Ally,
                requiresTarget: true,
                minRange:       1,
                maxRange:       1,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>
                {
                    new HealAllyEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.MinorMedicalMiracle, new ActionDefinition(
                actionID:       ActionIds.MinorMedicalMiracle,
                targetType:     TargetType.Ally,
                requiresTarget: true,
                minRange:       1,
                maxRange:       1,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>
                {
                    new MinorMedicalMiracleEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.MedicalMiracle, new ActionDefinition(
                actionID:       ActionIds.MedicalMiracle,
                targetType:     TargetType.Ally,
                requiresTarget: true,
                minRange:       1,
                maxRange:       1,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>
                {
                    new MedicalMiracleEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.TSStabilize, new ActionDefinition(
                actionID:       ActionIds.TSStabilize,
                targetType:     TargetType.Ally,
                requiresTarget: true,
                minRange:       1,
                maxRange:       1,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Move,
                effects:        new List<IActionEffect>
                {
                    new TSStabilizeEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.TSMassTriage, new ActionDefinition(
                actionID:       ActionIds.TSMassTriage,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.None,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>
                {
                    new TSMassTriageEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.CMBattlefieldPatch, new ActionDefinition(
                actionID:       ActionIds.CMBattlefieldPatch,
                targetType:     TargetType.Ally,
                requiresTarget: true,
                minRange:       1,
                maxRange:       1,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Free,
                effects:        new List<IActionEffect>
                {
                    new CMBattlefieldPatchEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.CMRevive, new ActionDefinition(
                actionID:       ActionIds.CMRevive,
                targetType:     TargetType.Ally,
                requiresTarget: true,
                minRange:       1,
                maxRange:       1,
                cooldown:       Unit.CombatCooldown,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>
                {
                    new CMReviveEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.CMPriorityPatient, new ActionDefinition(
                actionID:       ActionIds.CMPriorityPatient,
                targetType:     TargetType.Ally,
                requiresTarget: true,
                minRange:       1,
                maxRange:       8,
                cooldown:       Unit.CombatCooldown,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>
                {
                    new CMPriorityPatientEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.CMMedicApotheosis, new ActionDefinition(
                actionID:       ActionIds.CMMedicApotheosis,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.None,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>
                {
                    new CMMedicApotheosisEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.FPStimInjection, new ActionDefinition(
                actionID:       ActionIds.FPStimInjection,
                targetType:     TargetType.Ally,
                requiresTarget: true,
                minRange:       1,
                maxRange:       1,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>
                {
                    new FPStimInjectionEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.FPFieldSynthesis, new ActionDefinition(
                actionID:       ActionIds.FPFieldSynthesis,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                cooldown:       Unit.CombatCooldown,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.None,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>
                {
                    new FPFieldSynthesisEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},

            { ActionIds.FPPharmacistApotheosis, new ActionDefinition(
                actionID:       ActionIds.FPPharmacistApotheosis,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       0,
                cooldown:       0,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.None,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>
                {
                    new FPPharmacistApotheosisEffect()
                },
                attackType:     ActionDefinition.AttackType.None,
                requiresLoS:    false
            )},
            //Suggestion against every enemy in a 20-ft. radius. 1/combat.
            //Deferred: Suggestion behavior is a stub; effects, area targeting and saves come with the full power pass
            { ActionIds.EmpathGroupSuggestion, new ActionDefinition(
                actionID:       ActionIds.EmpathGroupSuggestion,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       5,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>(),
                attackType:     ActionDefinition.AttackType.None
            ) { usesPerCombat = 1 } },
            //Move one harmful condition from an ally within 60 ft. to the target enemy. Focus save negates. 2/combat.
            { ActionIds.EmpathSympatheticLink, new ActionDefinition(
                actionID:       ActionIds.EmpathSympatheticLink,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       SympatheticLinkEffect.Range,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>
                {
                    new SympatheticLinkEffect()
                },
                attackType:     ActionDefinition.AttackType.None
            ) { usesPerCombat = 2 } },
            //Touch: reveal a specific memory segment Anyha chooses. Focus save negates.
            //Deferred: memory content and the save come with the full power pass
            { ActionIds.InquisitorTraceMemory, new ActionDefinition(
                actionID:       ActionIds.InquisitorTraceMemory,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       1,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>(),
                attackType:     ActionDefinition.AttackType.None
            )},

            //Tap any active communication within 100 ft. and read it as a free action.
            //Deferred: there is no communication system yet
            { ActionIds.InquisitorPsionicIntercept, new ActionDefinition(
                actionID:       ActionIds.InquisitorPsionicIntercept,
                targetType:     TargetType.None,
                requiresTarget: false,
                minRange:       0,
                maxRange:       20,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.None,
                category:       ActionDefinition.ActionCategory.Free,
                effects:        new List<IActionEffect>(),
                attackType:     ActionDefinition.AttackType.None
            )},

            //Mindprobe with no PP cost; the target can't use power resistance. 1/combat.
            //Deferred: Mindprobe behavior comes with the full power pass. Range is a placeholder (Close at minimum)
            { ActionIds.InquisitorCompleteProbe, new ActionDefinition(
                actionID:       ActionIds.InquisitorCompleteProbe,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       5,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>(),
                attackType:     ActionDefinition.AttackType.None
            ) { usesPerCombat = 1 } },

            //Force a dominated target to make one melee attack against a target Anyha designates.
            //Deferred: needs Domination behavior and a second target. Range is a placeholder
            { ActionIds.DominatorDirectedStrike, new ActionDefinition(
                actionID:       ActionIds.DominatorDirectedStrike,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       20,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>(),
                attackType:     ActionDefinition.AttackType.None
            )},

            //Brain Lock on every enemy in a 15-ft. radius, at double Brain Lock's PP cost. 1/combat.
            //Deferred: area targeting and PP cost (ActionDefinition costs the class resource, not PP).
            //Range is a placeholder (Medium at minimum)
            { ActionIds.DominatorMassBrainLock, new ActionDefinition(
                actionID:       ActionIds.DominatorMassBrainLock,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       20,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects:        new List<IActionEffect>(),
                attackType:     ActionDefinition.AttackType.None
            ) { usesPerCombat = 1 } },

            //After a successful Domination, attempt a second Domination on an adjacent target as a free action. 1/combat.
            //Deferred: needs Domination behavior; likely becomes a trigger rather than a planned action
            { ActionIds.DominatorChainDomination, new ActionDefinition(
                actionID:       ActionIds.DominatorChainDomination,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                minRange:       1,
                maxRange:       1,
                dealsDamage:    false,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Free,
                effects:        new List<IActionEffect>(),
                attackType:     ActionDefinition.AttackType.None
            ) { usesPerCombat = 1 } },
        };

    public static ActionDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out ActionDefinition action)) return action;

        Debug.LogWarning($"[ActionDatabase] Action not found: {id}");
        return null;
    }
}