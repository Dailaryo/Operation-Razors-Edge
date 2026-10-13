using System.Collections.Generic;
using UnityEngine;

public static class PowerDatabase
{
    public static Dictionary<string, PsionicPowerDefinition> Definitions =
        new Dictionary<string, PsionicPowerDefinition>()
        {
            // ===== 0-level =====
            { PowerIds.Daze, new PsionicPowerDefinition(
                actionId:       PowerIds.Daze,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                maxRange:       0,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new TargetLimitEffect(new List<IActionEffect>
                    {
                        new PowerSaveGateEffect(new List<IActionEffect>
                        {
                            new ApplyStatusEffectActionEffect(target => new LoseNextActionEffect())
                        })
                    }, maxLevel: 4, maxSize: Unit.SizeCategory.Medium)
                },
                powerLevel:            0,
                discipline:            PsionicDiscipline.Telepathy,
                basePowerPointCost:    1,
                keyAbility:            StatType.Presence,
                saveType:              Unit.SaveType.Focus,
                allowsPowerResistance: true,
                descriptors:           EffectDescriptor.Compulsion | EffectDescriptor.MindAffecting,
                rangeBand:             PsionicPowerDefinition.PowerRange.Close
            )},

            { PowerIds.Distract, new PsionicPowerDefinition(
                actionId:       PowerIds.Distract,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                maxRange:       0,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new PowerSaveGateEffect(new List<IActionEffect>
                    {
                        //Listen/Spot -> Perception, Search -> Investigate
                        new ApplyStatusEffectActionEffect(target => new TimedStatModifierEffect(PowerIds.Distract, 0,
                            new StatModifier(StatType.SkillPerception,  -1, 1f, PowerIds.Distract, StatModifier.BonusType.Untyped),
                            new StatModifier(StatType.SkillInvestigate, -1, 1f, PowerIds.Distract, StatModifier.BonusType.Untyped),
                            new StatModifier(StatType.SkillSenseMotive, -1, 1f, PowerIds.Distract, StatModifier.BonusType.Untyped))
                            {isHarmful = true })
                    })
                },
                powerLevel:             0,
                discipline:             PsionicDiscipline.Telepathy,
                basePowerPointCost:     1,
                keyAbility:             StatType.Presence,
                concentrationRequired:  true,
                saveType:               Unit.SaveType.Focus,
                allowsPowerResistance:  true,
                descriptors:            EffectDescriptor.MindAffecting,
                rangeBand:              PsionicPowerDefinition.PowerRange.Close,
                concentrationMaxRounds: ml => 10 * ml   //up to 1 minute/level
            )},

            { PowerIds.Verve, new PsionicPowerDefinition(
                actionId:       PowerIds.Verve,
                targetType:     TargetType.Self,
                requiresTarget: false,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new ApplySelfStatusEffectEffect(self => new TemporaryHitPointsEffect(PowerIds.Verve, 1, 10)) //1 minute
                },
                requiresLoS:        false,
                powerLevel:         0,
                discipline:         PsionicDiscipline.Psychometabolism,
                basePowerPointCost: 1,
                keyAbility:         StatType.Power,
                rangeBand:          PsionicPowerDefinition.PowerRange.Personal
            )},

            //Burst on self: free action, speed bonus this round. The known/learnable entry
            { PowerIds.Burst, new PsionicPowerDefinition(
                actionId:       PowerIds.Burst,
                targetType:     TargetType.Self,
                requiresTarget: false,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Free,
                effects: new List<IActionEffect> { new BurstEffect(onSelf: true) },
                requiresLoS:        false,
                powerLevel:         0,
                discipline:         PsionicDiscipline.Psychoportation,
                basePowerPointCost: 1,
                keyAbility:         StatType.Agility,
                rangeBand:          PsionicPowerDefinition.PowerRange.Personal,
                variantPowerIds:    new List<string> { PowerIds.BurstOther }
            )},

            //Burst on another creature: attack action, speed bonus on the target's next turn. Variant of Burst
            { PowerIds.BurstOther, new PsionicPowerDefinition(
                actionId:       PowerIds.BurstOther,
                targetType:     TargetType.Ally,
                requiresTarget: true,
                maxRange:       0,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect> { new BurstEffect(onSelf: false) },
                powerLevel:         0,
                discipline:         PsionicDiscipline.Psychoportation,
                basePowerPointCost: 1,
                keyAbility:         StatType.Agility,
                rangeBand:          PsionicPowerDefinition.PowerRange.Close
            )},

            //Deferred: Telempathic Projection — shift attitude one step; ±1 on Bluff/Diplomacy/Intimidate (dialogue system). 1 min/level
            { PowerIds.TelempathicProjection, new PsionicPowerDefinition(
                actionId: PowerIds.TelempathicProjection, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 0, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 1, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true, descriptors: EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Medium
            )},

            //Deferred: Missive — one-way telepathic message of up to ten words (squad communication)
            { PowerIds.Missive, new PsionicPowerDefinition(
                actionId: PowerIds.Missive, targetType: TargetType.Ally, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 0, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 1, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true, descriptors: EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Deferred: Detect Psionics — 60-ft quarter-circle; aura presence, count/strength, then location (detection/exploration)
            { PowerIds.DetectPsionics, new PsionicPowerDefinition(
                actionId: PowerIds.DetectPsionics, targetType: TargetType.None, requiresTarget: false, maxRange: 12,
                requiresLoS: false,
                powerLevel: 0, discipline: PsionicDiscipline.Clairsentience, basePowerPointCost: 1, keyAbility: StatType.Insight,
                concentrationRequired: true, concentrationMaxRounds: ml => 10 * ml
            )},

            //Deferred: Far Hand — move an unattended object up to 5 lb. up to 15 ft. as a move action
            { PowerIds.FarHand, new PsionicPowerDefinition(
                actionId: PowerIds.FarHand, targetType: TargetType.None, requiresTarget: false, maxRange: 0,
                powerLevel: 0, discipline: PsionicDiscipline.Psychokinesis, basePowerPointCost: 1, keyAbility: StatType.Endurance,
                concentrationRequired: true, rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            // ===== 1st-level =====
            //Deferred: Lesser Mindlink — telepathic bond with one willing ally, 10 min/level (squad communication)
            { PowerIds.LesserMindlink, new PsionicPowerDefinition(
                actionId: PowerIds.LesserMindlink, targetType: TargetType.Ally, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 1, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 1, keyAbility: StatType.Presence,
                rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Deferred: Conceal Thoughts — +20 Bluff vs Sense Motive on intentions; +4 saves vs mind-reading powers. 1 hour/level
            { PowerIds.ConcealThoughts, new PsionicPowerDefinition(
                actionId: PowerIds.ConcealThoughts, targetType: TargetType.Ally, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 1, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 1, keyAbility: StatType.Presence,
                rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Deferred: Object Reading — learn previous owners' details round by round (investigation)
            { PowerIds.ObjectReading, new PsionicPowerDefinition(
                actionId: PowerIds.ObjectReading, targetType: TargetType.None, requiresTarget: false, maxRange: 0,
                powerLevel: 1, discipline: PsionicDiscipline.Clairsentience, basePowerPointCost: 1, keyAbility: StatType.Insight,
                concentrationRequired: true, allowsPowerResistance: true, rangeBand: PsionicPowerDefinition.PowerRange.Touch
            )},

            //Deferred: Attraction — implanted attraction to a person/object/action/event; +4 Presence modifier when dealing with the subject. 1 hour/level
            { PowerIds.Attraction, new PsionicPowerDefinition(
                actionId: PowerIds.Attraction, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 1, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 1, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true,
                descriptors: EffectDescriptor.Compulsion | EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Deferred: White Noise — 5-ft/level radius jams wireless transmissions, 1 min/level (electronics/comms system)
            { PowerIds.WhiteNoise, new PsionicPowerDefinition(
                actionId: PowerIds.WhiteNoise, targetType: TargetType.Self, requiresTarget: false, maxRange: 0,
                requiresLoS: false,
                powerLevel: 1, discipline: PsionicDiscipline.Psychokinesis, basePowerPointCost: 1, keyAbility: StatType.Endurance,
                rangeBand: PsionicPowerDefinition.PowerRange.Personal
            )},

            //Stub: Charm Person — regards you as a trusted ally; +5 save if threatened. 1 hour/level (lastsUntilHub when built)
            { PowerIds.CharmPerson, new PsionicPowerDefinition(
                actionId: PowerIds.CharmPerson, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 1, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 1, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true,
                descriptors: EffectDescriptor.Compulsion | EffectDescriptor.MindAffecting | EffectDescriptor.Charm,
                rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Stub: Finger of Fire — ranged touch attack, 1d3 fire
            { PowerIds.FingerOfFire, new PsionicPowerDefinition(
                actionId: PowerIds.FingerOfFire, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 0, discipline: PsionicDiscipline.Psychokinesis, basePowerPointCost: 1, keyAbility: StatType.Cognition,
                allowsPowerResistance: true, rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Stub: Control Object — animate an object up to 100 lb.; slam +0, 1d4. Concentration, up to 1 round/level
            { PowerIds.ControlObject, new PsionicPowerDefinition(
                actionId: PowerIds.ControlObject, targetType: TargetType.None, requiresTarget: false, maxRange: 0,
                powerLevel: 1, discipline: PsionicDiscipline.Psychokinesis, basePowerPointCost: 1, keyAbility: StatType.Endurance,
                concentrationRequired: true, concentrationMaxRounds: ml => ml,
                rangeBand: PsionicPowerDefinition.PowerRange.Medium
            )},

            //Stub: Lesser Body Adjustment — heal 1d8, or +1 next Fort save vs poison/disease, or heal 1 ability damage. Full-round
            { PowerIds.LesserBodyAdjustment, new PsionicPowerDefinition(
                actionId: PowerIds.LesserBodyAdjustment, targetType: TargetType.Self, requiresTarget: false, maxRange: 0,
                consumesCategories: new List<ActionDefinition.ActionCategory>
                    { ActionDefinition.ActionCategory.Standard, ActionDefinition.ActionCategory.Move },
                requiresLoS: false,
                powerLevel: 1, discipline: PsionicDiscipline.Psychometabolism, basePowerPointCost: 1, keyAbility: StatType.Power,
                rangeBand: PsionicPowerDefinition.PowerRange.Personal
            )},

            // ===== 2nd-level =====
            { PowerIds.BrainLock, new PsionicPowerDefinition(
                actionId:       PowerIds.BrainLock,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                maxRange:       0,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new TargetLimitEffect(new List<IActionEffect>
                    {
                        new PowerSaveGateEffect(new List<IActionEffect>
                        {
                            //1 round/level
                            new ApplyContextStatusEffect(ctx =>
                                new BrainLockedEffect(PsionicPowerDefinition.GetManifesterLevel(ctx.attacker)))
                        })
                    }, maxSize: Unit.SizeCategory.Medium)
                },
                powerLevel:            2,
                discipline:            PsionicDiscipline.Telepathy,
                basePowerPointCost:    3,
                keyAbility:            StatType.Presence,
                saveType:              Unit.SaveType.Focus,
                allowsPowerResistance: true,
                descriptors:           EffectDescriptor.MindAffecting,
                rangeBand:             PsionicPowerDefinition.PowerRange.Medium
            )},

            { PowerIds.InflictPain, new PsionicPowerDefinition(
                actionId:       PowerIds.InflictPain,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                maxRange:       0,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                dealsDamage:    true,
                effects: new List<IActionEffect>
                {
                    new PowerSaveGateEffect(new List<IActionEffect>
                    {
                        new PowerDamageEffect(3, 6),
                        //Repeats 3d6 (new save each time) while concentration holds, up to 5 rounds total
                        new ApplyContextStatusEffect(ctx => new InflictPainEffect(ctx.attacker,
                            ((PsionicPowerDefinition)ctx.definition).GetSaveDC(ctx.attacker), ctx.isMaximized))
                    })
                },
                powerLevel:            2,
                discipline:            PsionicDiscipline.Telepathy,
                basePowerPointCost:    3,
                keyAbility:            StatType.Presence,
                concentrationRequired: true,     //no cap: InflictPainEffect ends itself after its 4 repeats
                saveType:              Unit.SaveType.Focus,
                allowsPowerResistance: true,
                descriptors:           EffectDescriptor.MindAffecting,
                rangeBand:             PsionicPowerDefinition.PowerRange.Long
            )},

            // ===== 3rd-level =====
            { PowerIds.MentalBlast, new PsionicPowerDefinition(
                actionId:       PowerIds.MentalBlast,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                maxRange:       12,   //60 ft (fixed in the source)
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new PowerSaveGateEffect(new List<IActionEffect>
                    {
                        //Stunned 3d4 rounds
                        new ApplyContextStatusEffect(ctx => new StunnedEffect(PowerDice.Roll(ctx, 3, 4)))
                    })
                },
                powerLevel:            3,
                discipline:            PsionicDiscipline.Telepathy,
                basePowerPointCost:    5,
                keyAbility:            StatType.Presence,
                saveType:              Unit.SaveType.Focus,
                allowsPowerResistance: false
            )},

            // ===== 4th-level =====
            { PowerIds.InertialBarrier, new PsionicPowerDefinition(
                actionId:       PowerIds.InertialBarrier,
                targetType:     TargetType.Ally,   //includes self
                requiresTarget: true,
                maxRange:       0,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new PowerSaveGateEffect(new List<IActionEffect>
                    {
                        //DR 10/— until it has prevented 10 x ML (max 150). 10 minutes/level: until Hub return
                        new ApplyContextStatusEffect(ctx => new InertialBarrierEffect(
                            Mathf.Min(10 * PsionicPowerDefinition.GetManifesterLevel(ctx.attacker), 150)))
                    })
                },
                powerLevel:            4,
                discipline:            PsionicDiscipline.Psychokinesis,
                basePowerPointCost:    7,
                keyAbility:            StatType.Endurance,
                saveType:              Unit.SaveType.Focus,   //harmless: allies take NoSave
                allowsPowerResistance: true,
                rangeBand:             PsionicPowerDefinition.PowerRange.Touch
            )},

            { PowerIds.Mindwipe, new PsionicPowerDefinition(
                actionId:       PowerIds.Mindwipe,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                maxRange:       0,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new PowerSaveGateEffect(new List<IActionEffect>
                    {
                        //One negative level per two manifester levels (max 5)
                        new ApplyContextStatusEffect(ctx => new NegativeLevelsEffect(
                            Mathf.Clamp(PsionicPowerDefinition.GetManifesterLevel(ctx.attacker) / 2, 1, 5)))
                    })
                },
                powerLevel:            4,
                discipline:            PsionicDiscipline.Telepathy,
                basePowerPointCost:    7,
                keyAbility:            StatType.Presence,
                saveType:              Unit.SaveType.Resolve,   //Fortitude negates
                allowsPowerResistance: true,
                rangeBand:             PsionicPowerDefinition.PowerRange.Close
            )},

            // ===== 5th-level =====
            { PowerIds.Catapsi, new PsionicPowerDefinition(
                actionId:       PowerIds.Catapsi,
                targetType:     TargetType.Self,
                requiresTarget: false,
                maxRange:       0,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect> { new CatapsiActionEffect() },
                requiresLoS:           false,
                powerLevel:            5,
                discipline:            PsionicDiscipline.Telepathy,
                basePowerPointCost:    9,
                keyAbility:            StatType.Presence,
                saveType:              Unit.SaveType.Focus,   //rolled by CatapsiEffect each time an opponent manifests
                allowsPowerResistance: true,                  //checked by CatapsiEffect
                descriptors:           EffectDescriptor.MindAffecting,
                rangeBand:             PsionicPowerDefinition.PowerRange.Personal
            )},

            { PowerIds.PowerResistance, new PsionicPowerDefinition(
                actionId:       PowerIds.PowerResistance,
                targetType:     TargetType.Ally,   //includes self
                requiresTarget: true,
                maxRange:       0,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new PowerSaveGateEffect(new List<IActionEffect>
                    {
                        //PR 12 + ML for 1 minute/level
                        new ApplyContextStatusEffect(ctx =>
                        {
                            int ml = PsionicPowerDefinition.GetManifesterLevel(ctx.attacker);
                            return new PowerResistanceEffect(12 + ml, 10 * ml);
                        })
                    })
                },
                powerLevel:            5,
                discipline:            PsionicDiscipline.Clairsentience,
                basePowerPointCost:    9,
                keyAbility:            StatType.Insight,
                saveType:              Unit.SaveType.Focus,   //harmless: allies take NoSave
                allowsPowerResistance: true,
                rangeBand:             PsionicPowerDefinition.PowerRange.Touch
            )},

            //Deferred: Detect Thoughts — 60-ft quarter-circle; thoughts present, count/strength, then surface thoughts (Will negates)
            { PowerIds.DetectThoughts, new PsionicPowerDefinition(
                actionId: PowerIds.DetectThoughts, targetType: TargetType.None, requiresTarget: false, maxRange: 12,
                requiresLoS: false,
                powerLevel: 2, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 3, keyAbility: StatType.Presence,
                concentrationRequired: true, concentrationMaxRounds: ml => 10 * ml,
                saveType: Unit.SaveType.Focus, descriptors: EffectDescriptor.MindAffecting
            )},

            //Deferred: Clairaudience/Clairvoyance — hear or see at a chosen locale. 1 min/level (scouting/exploration)
            { PowerIds.ClairaudienceClairvoyance, new PsionicPowerDefinition(
                actionId: PowerIds.ClairaudienceClairvoyance, targetType: TargetType.None, requiresTarget: false, maxRange: 0,
                requiresLoS: false,
                powerLevel: 2, discipline: PsionicDiscipline.Clairsentience, basePowerPointCost: 3, keyAbility: StatType.Insight
            )},

            //Deferred: Sensitivity to Psychic Impressions — visions of strong-emotion events in the area (investigation)
            { PowerIds.SensitivityToPsychicImpressions, new PsionicPowerDefinition(
                actionId: PowerIds.SensitivityToPsychicImpressions, targetType: TargetType.None, requiresTarget: false, maxRange: 0,
                requiresLoS: false,
                powerLevel: 2, discipline: PsionicDiscipline.Clairsentience, basePowerPointCost: 2, keyAbility: StatType.Insight,
                concentrationRequired: true, rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Deferred: False Sensory Input — falsify one of the target's senses; can force an enemy manifester's
            //concentration check (DC = save DC + 3). Full-round. Concentration, up to 1 min/level
            { PowerIds.FalseSensoryInput, new PsionicPowerDefinition(
                actionId: PowerIds.FalseSensoryInput, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                consumesCategories: new List<ActionDefinition.ActionCategory>
                    { ActionDefinition.ActionCategory.Standard, ActionDefinition.ActionCategory.Move },
                powerLevel: 3, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 5, keyAbility: StatType.Presence,
                concentrationRequired: true, concentrationMaxRounds: ml => 10 * ml,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true, descriptors: EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Long
            )},

            //Deferred: Mindlink — telepathic bond with one willing creature/level. 10 min/level (squad communication)
            { PowerIds.Mindlink, new PsionicPowerDefinition(
                actionId: PowerIds.Mindlink, targetType: TargetType.Ally, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 3, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 5, keyAbility: StatType.Presence,
                rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Deferred: Forced Mindlink — as Lesser Mindlink, against an unwilling target (interrogation)
            { PowerIds.ForcedMindlink, new PsionicPowerDefinition(
                actionId: PowerIds.ForcedMindlink, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 4, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 7, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true,
                rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Deferred: Tailor Memory — insert a false memory (Medium-size or smaller; dialogue/narrative)
            { PowerIds.TailorMemory, new PsionicPowerDefinition(
                actionId: PowerIds.TailorMemory, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 4, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 7, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true, descriptors: EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Medium
            )},

            //Deferred: Mindprobe — one question answered per round from the target's memories. 1-minute manifestation;
            //1 min/level (interrogation)
            { PowerIds.Mindprobe, new PsionicPowerDefinition(
                actionId: PowerIds.Mindprobe, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 5, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 9, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Resolve, allowsPowerResistance: true,
                rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Deferred: Signal Feed — false images/sounds on one communication device, 1 round/level (electronics system)
            { PowerIds.SignalFeed, new PsionicPowerDefinition(
                actionId: PowerIds.SignalFeed, targetType: TargetType.None, requiresTarget: false, maxRange: 0,
                powerLevel: 5, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 9, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true, descriptors: EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},
            //Stub: Suggestion — target follows a reasonable suggested course of action. 1 hour/level (until Hub when built)
            { PowerIds.Suggestion, new PsionicPowerDefinition(
                actionId: PowerIds.Suggestion, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 2, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 3, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true,
                descriptors: EffectDescriptor.Compulsion | EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Stub: Charm Creature — as Charm Person, any creature type/size, no shared language. 1 day/level
            { PowerIds.CharmCreature, new PsionicPowerDefinition(
                actionId: PowerIds.CharmCreature, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 3, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 5, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true,
                descriptors: EffectDescriptor.Compulsion | EffectDescriptor.MindAffecting | EffectDescriptor.Charm,
                rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Stub: Lesser Domination — as Domination, requires a shared language
            { PowerIds.LesserDomination, new PsionicPowerDefinition(
                actionId: PowerIds.LesserDomination, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 3, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 5, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true,
                descriptors: EffectDescriptor.Compulsion | EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Medium
            )},

            //Stub: Domination — control a Medium-size or smaller creature; new save vs against-nature orders. 1 day/level
            { PowerIds.Domination, new PsionicPowerDefinition(
                actionId: PowerIds.Domination, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 4, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 7, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true,
                descriptors: EffectDescriptor.Compulsion | EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Medium
            )},

            //Stub: Electronic Fog — scramble electronic signals (Urban Arcana; verify the full stat block when built)
            { PowerIds.ElectronicFog, new PsionicPowerDefinition(
                actionId: PowerIds.ElectronicFog, targetType: TargetType.None, requiresTarget: false, maxRange: 0,
                powerLevel: 2, discipline: PsionicDiscipline.Psychokinesis, basePowerPointCost: 3, keyAbility: StatType.Endurance
            )},

            //Stub: Levitate — move self, a willing creature or an object up/down. 10 min/level
            { PowerIds.Levitate, new PsionicPowerDefinition(
                actionId: PowerIds.Levitate, targetType: TargetType.Ally, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 2, discipline: PsionicDiscipline.Psychoportation, basePowerPointCost: 3, keyAbility: StatType.Agility,
                rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Stub: Negate Psionics — end ongoing powers (targeted or 30-ft area negation checks)
            { PowerIds.NegatePsionics, new PsionicPowerDefinition(
                actionId: PowerIds.NegatePsionics, targetType: TargetType.None, requiresTarget: false, maxRange: 0,
                powerLevel: 3, discipline: PsionicDiscipline.Psychokinesis, basePowerPointCost: 5, keyAbility: StatType.Endurance,
                rangeBand: PsionicPowerDefinition.PowerRange.Medium
            )},

            //Stub: Telekinesis — sustained force or violent thrust. Concentration, up to 1 round/level
            { PowerIds.Telekinesis, new PsionicPowerDefinition(
                actionId: PowerIds.Telekinesis, targetType: TargetType.None, requiresTarget: false, maxRange: 0,
                powerLevel: 4, discipline: PsionicDiscipline.Psychokinesis, basePowerPointCost: 7, keyAbility: StatType.Endurance,
                concentrationRequired: true, concentrationMaxRounds: ml => ml,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true,
                rangeBand: PsionicPowerDefinition.PowerRange.Long
            )},

            //Stub: Sending — 25-word message to a known individual anywhere. Full-round
            { PowerIds.Sending, new PsionicPowerDefinition(
                actionId: PowerIds.Sending, targetType: TargetType.None, requiresTarget: false, maxRange: 0,
                consumesCategories: new List<ActionDefinition.ActionCategory>
                    { ActionDefinition.ActionCategory.Standard, ActionDefinition.ActionCategory.Move },
                requiresLoS: false,
                powerLevel: 5, discipline: PsionicDiscipline.Psychoportation, basePowerPointCost: 9, keyAbility: StatType.Agility
            )},

            // ===== 6th-level =====
            { PowerIds.MassInflictPain, new PsionicPowerDefinition(
                actionId:       PowerIds.MassInflictPain,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                maxRange:       0,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                dealsDamage:    true,
                effects: new List<IActionEffect>
                {
                    //Inflict Pain on one creature per manifester level, no two more than 30 ft. apart
                    new MultiTargetEffect(new List<IActionEffect>
                    {
                        new PowerSaveGateEffect(new List<IActionEffect>
                        {
                            new PowerDamageEffect(3, 6),
                            new ApplyContextStatusEffect(ctx => new InflictPainEffect(ctx.attacker,
                                ((PsionicPowerDefinition)ctx.definition).GetSaveDC(ctx.attacker), ctx.isMaximized))
                        })
                    }, maxTargets: ctx => PsionicPowerDefinition.GetManifesterLevel(ctx.attacker))
                },
                powerLevel:            6,
                discipline:            PsionicDiscipline.Telepathy,
                basePowerPointCost:    11,
                keyAbility:            StatType.Presence,
                concentrationRequired: true,     //one concentration holds every target's repeats
                saveType:              Unit.SaveType.Focus,
                allowsPowerResistance: true,
                descriptors:           EffectDescriptor.MindAffecting,
                rangeBand:             PsionicPowerDefinition.PowerRange.Close
            )},

            { PowerIds.PrimalRegression, new PsionicPowerDefinition(
                actionId:       PowerIds.PrimalRegression,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                maxRange:       0,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    //One creature per two manifester levels, no two more than 30 ft. apart. 1 minute/level
                    new MultiTargetEffect(new List<IActionEffect>
                    {
                        new PowerSaveGateEffect(new List<IActionEffect>
                        {
                            new ApplyContextStatusEffect(ctx => new PrimalRegressionEffect(
                                ((PsionicPowerDefinition)ctx.definition).GetSaveDC(ctx.attacker),
                                10 * PsionicPowerDefinition.GetManifesterLevel(ctx.attacker))),
                            //2d8 temporary HP, lost when the regression ends
                            new ApplyContextStatusEffect(ctx => new TemporaryHitPointsEffect("PrimalRegression",
                                PowerDice.Roll(ctx, 2, 8),
                                10 * PsionicPowerDefinition.GetManifesterLevel(ctx.attacker)))
                        })
                    }, maxTargets: ctx => Mathf.Max(1, PsionicPowerDefinition.GetManifesterLevel(ctx.attacker) / 2))
                },
                powerLevel:            6,
                discipline:            PsionicDiscipline.Telepathy,
                basePowerPointCost:    11,
                keyAbility:            StatType.Presence,
                saveType:              Unit.SaveType.Focus,
                allowsPowerResistance: true,
                descriptors:           EffectDescriptor.Compulsion | EffectDescriptor.MindAffecting,
                rangeBand:             PsionicPowerDefinition.PowerRange.Close
            )},

            //Stub (deferred): Withdraw Affliction (PF) — touch, one afflicted creature; no save, PR. Cures one
            //affliction (as remove disease / remove curse / neutralize poison) and holds it as a charge; a
            //touch attack passes it to another creature, which saves against the original DC.
            //Needs an affliction system (disease, curse, poison) first.
            { PowerIds.WithdrawAffliction, new PsionicPowerDefinition(
                actionId: PowerIds.WithdrawAffliction, targetType: TargetType.Ally, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 6, discipline: PsionicDiscipline.Psychometabolism, basePowerPointCost: 11, keyAbility: StatType.Presence,
                allowsPowerResistance: true,
                rangeBand: PsionicPowerDefinition.PowerRange.Touch
            )},

            //Stub (deferred): Mass Cloud Mind (3.5) — one creature/level, Focus negates, PR; 1 minute/level.
            //Each affected creature cannot perceive the manifester at all. Ends for a creature the manifester
            //attacks; others get a new save when the manifester attacks or moves a visible object, or when an
            //ally spends a move action to warn them. Needs per-observer detection integration
            //(DetectionStateManager)
            { PowerIds.MassCloudMind, new PsionicPowerDefinition(
                actionId: PowerIds.MassCloudMind, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 6, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 11, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true,
                descriptors: EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Stub: Co-opt Concentration (3.5) — one creature, Focus negates, PR; concentration up to 1 round/level.
            //Takes over one concentration power the target is maintaining: let it run, control it, redirect a
            //personal power onto the manifester, or end it. The taken power ends when this one does
            { PowerIds.CoOptConcentration, new PsionicPowerDefinition(
                actionId: PowerIds.CoOptConcentration, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 6, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 11, keyAbility: StatType.Presence,
                concentrationRequired: true, concentrationMaxRounds: ml => ml,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true,
                descriptors: EffectDescriptor.Compulsion | EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Medium
            )},

            // ===== 7th-level =====
            { PowerIds.MindBlank, new PsionicPowerDefinition(
                actionId:       PowerIds.MindBlank,
                targetType:     TargetType.Ally,   //includes self
                requiresTarget: true,
                maxRange:       0,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new PowerSaveGateEffect(new List<IActionEffect>
                    {
                        new ApplyContextStatusEffect(ctx => new MindBlankEffect())
                    })
                },
                powerLevel:            7,
                discipline:            PsionicDiscipline.Telepathy,
                basePowerPointCost:    13,
                keyAbility:            StatType.Presence,
                saveType:              Unit.SaveType.Focus,   //harmless: allies take NoSave
                allowsPowerResistance: true,
                rangeBand:             PsionicPowerDefinition.PowerRange.Close
            )},

            { PowerIds.MassSynesthesia, new PsionicPowerDefinition(
                actionId:       PowerIds.MassSynesthesia,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                maxRange:       0,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    //One or more creatures, no two more than 30 ft. apart. 1 round/level
                    new MultiTargetEffect(new List<IActionEffect>
                    {
                        new PowerSaveGateEffect(new List<IActionEffect>
                        {
                            new ApplyContextStatusEffect(ctx => new SynesthesiaEffect(
                                ((PsionicPowerDefinition)ctx.definition).GetSaveDC(ctx.attacker),
                                PsionicPowerDefinition.GetManifesterLevel(ctx.attacker)))
                        })
                    })
                },
                powerLevel:            7,
                discipline:            PsionicDiscipline.Telepathy,
                basePowerPointCost:    13,
                keyAbility:            StatType.Presence,
                saveType:              Unit.SaveType.Focus,
                allowsPowerResistance: true,
                descriptors:           EffectDescriptor.MindAffecting,
                rangeBand:             PsionicPowerDefinition.PowerRange.Medium
            )},

            { PowerIds.UnshakableZeal, new PsionicPowerDefinition(
                actionId:       PowerIds.UnshakableZeal,
                targetType:     TargetType.Ally,   //includes self
                requiresTarget: true,
                maxRange:       0,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    new PowerSaveGateEffect(new List<IActionEffect>
                    {
                        new ApplyContextStatusEffect(ctx => new UnshakableZealEffect())
                    })
                },
                powerLevel:            7,
                discipline:            PsionicDiscipline.Telepathy,
                basePowerPointCost:    13,
                keyAbility:            StatType.Presence,
                saveType:              Unit.SaveType.Focus,   //harmless: allies take NoSave
                allowsPowerResistance: true,
                descriptors:           EffectDescriptor.Compulsion | EffectDescriptor.Emotion | EffectDescriptor.MindAffecting,
                rangeBand:             PsionicPowerDefinition.PowerRange.Touch
            )},

            //Stub: Insanity — one living creature, Focus negates, PR. The target is permanently confused.
            //Deferred: Confused condition (d% each turn: act normally / babble / harm self / attack nearest;
            //attacks whoever attacked it)
            { PowerIds.Insanity, new PsionicPowerDefinition(
                actionId: PowerIds.Insanity, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 7, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 13, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true,
                descriptors: EffectDescriptor.Compulsion | EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Medium
            )},

            //Stub: Crisis of Life (3.5) — one creature, Resolve partial, PR. 11 HD or fewer: dies on a failed save.
            //Saved, or more than 11 HD: 7d6 damage. Augment: +1 HD killable per extra PP
            { PowerIds.CrisisOfLife, new PsionicPowerDefinition(
                actionId: PowerIds.CrisisOfLife, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile, dealsDamage: true,
                powerLevel: 7, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 13, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Resolve, allowsPowerResistance: true,
                descriptors: EffectDescriptor.Death | EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Medium
            )},

            // ===== 8th-level =====
            { PowerIds.RecallDeath, new PsionicPowerDefinition(
                actionId:       PowerIds.RecallDeath,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                maxRange:       0,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                dealsDamage:    true,
                effects: new List<IActionEffect>
                {
                    //Focus partial: dies on a failed save, 5d6 on a success
                    new PowerSaveGateEffect(
                        new List<IActionEffect> { new KillTargetEffect() },
                        new List<IActionEffect> { new PowerDamageEffect(5, 6) })
                },
                powerLevel:            8,
                discipline:            PsionicDiscipline.Clairsentience,
                basePowerPointCost:    15,
                keyAbility:            StatType.Presence,
                saveType:              Unit.SaveType.Focus,
                allowsPowerResistance: true,
                descriptors:           EffectDescriptor.Death | EffectDescriptor.MindAffecting,
                rangeBand:             PsionicPowerDefinition.PowerRange.Medium
            )},

            //Stub: Irresistible Dance (PF) — touch, one living creature, Focus partial, PR; 1d4+1 rounds (1 round
            //on a save). The target dances and can take no other actions: -4 Defense, -10 Response saves, no
            //shield bonus, and provokes attacks of opportunity every turn
            { PowerIds.IrresistibleDance, new PsionicPowerDefinition(
                actionId: PowerIds.IrresistibleDance, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 8, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 15, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true,
                descriptors: EffectDescriptor.Compulsion | EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Touch
            )},

            // ===== 9th-level =====
            { PowerIds.Microcosm, new PsionicPowerDefinition(
                actionId:       PowerIds.Microcosm,
                targetType:     TargetType.Enemy,
                requiresTarget: true,
                maxRange:       0,
                targetingMode:  ActionDefinition.TargetingMode.Tile,
                category:       ActionDefinition.ActionCategory.Standard,
                effects: new List<IActionEffect>
                {
                    //One or more creatures with a combined 30 HD or fewer
                    new MultiTargetEffect(new List<IActionEffect> { new MicrocosmEffect() }, hdBudget: 30)
                },
                powerLevel:            9,
                discipline:            PsionicDiscipline.Telepathy,
                basePowerPointCost:    17,
                keyAbility:            StatType.Presence,
                saveType:              Unit.SaveType.Focus,
                allowsPowerResistance: true,
                descriptors:           EffectDescriptor.MindAffecting,
                rangeBand:             PsionicPowerDefinition.PowerRange.Close
            )},

            //Stub: Divide Mind (PF) — personal, 1 minute. Roll twice and take the better result on Focus saves,
            //Cognition checks and Cognition-based skill checks. Each round, one extra purely mental action as a
            //swift action (manifest a power of 5th level or lower, or concentrate)
            { PowerIds.DivideMind, new PsionicPowerDefinition(
                actionId: PowerIds.DivideMind, targetType: TargetType.Self, requiresTarget: false, maxRange: 0,
                requiresLoS: false,
                powerLevel: 9, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 17, keyAbility: StatType.Presence,
                descriptors: EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Personal
            )},

            //Stub: Dominate Monster (PF) — one creature of any type, Focus negates, PR; manifesting time 1 round.
            //Domination as Domination (4th), on any creature type. Waits on the domination system
            { PowerIds.DominateMonster, new PsionicPowerDefinition(
                actionId: PowerIds.DominateMonster, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 9, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 17, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Focus, allowsPowerResistance: true,
                descriptors: EffectDescriptor.Compulsion | EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Stub: Affinity Field (3.5) — 20-ft. radius around the manifester, Resolve negates, PR; 1 round/level.
            //Each linked creature takes all damage (including ability damage) and healing the manifester takes,
            //and shares effects of 3rd level or lower that affect the manifester (saving as if targeted).
            //Immunities the manifester gains are not shared
            { PowerIds.AffinityField, new PsionicPowerDefinition(
                actionId: PowerIds.AffinityField, targetType: TargetType.Self, requiresTarget: false, maxRange: 0,
                requiresLoS: false,
                powerLevel: 9, discipline: PsionicDiscipline.Psychometabolism, basePowerPointCost: 17, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Resolve, allowsPowerResistance: true,
                rangeBand: PsionicPowerDefinition.PowerRange.Personal
            )},

            //Stub: Apopsi (3.5) — one living psionic creature, Resolve negates, PR; manifesting time 1 round.
            //Permanently deletes 1d4 powers (the manifester names the levels).
            //Cost: 250 credits per power level deleted (50 XP x 5). It varies with the levels chosen, so the
            //flat creditCost stays 0 until this is built
            { PowerIds.Apopsi, new PsionicPowerDefinition(
                actionId: PowerIds.Apopsi, targetType: TargetType.Enemy, requiresTarget: true, maxRange: 0,
                targetingMode: ActionDefinition.TargetingMode.Tile,
                powerLevel: 9, discipline: PsionicDiscipline.Telepathy, basePowerPointCost: 17, keyAbility: StatType.Presence,
                saveType: Unit.SaveType.Resolve, allowsPowerResistance: true,
                descriptors: EffectDescriptor.MindAffecting,
                rangeBand: PsionicPowerDefinition.PowerRange.Close
            )},

            //Stub (non-combat): Metafaculty (3.5) — personal, manifesting time 1 hour. Learn about a creature the
            //manifester has met: name, species, class, level band, location, significant items, and the last
            //8 hours of activity. Bypasses Mind Blank with a manifester level check.
            //Cost: 5,000 credits (1,000 XP x 5)
            { PowerIds.Metafaculty, new PsionicPowerDefinition(
                actionId: PowerIds.Metafaculty, targetType: TargetType.Self, requiresTarget: false, maxRange: 0,
                requiresLoS: false,
                powerLevel: 9, discipline: PsionicDiscipline.Clairsentience, basePowerPointCost: 17, keyAbility: StatType.Insight,
                creditCost: 5000,
                rangeBand: PsionicPowerDefinition.PowerRange.Personal
            )},

            // ===== Tier chains (builders below) =====
            { PowerIds.PsychicCrushI,         PsychicCrush(PowerIds.PsychicCrushI, 1) },
            { PowerIds.PsychicCrushII,        PsychicCrush(PowerIds.PsychicCrushII, 2) },
            { PowerIds.PsychicCrushIII,       PsychicCrush(PowerIds.PsychicCrushIII, 3) },
            { PowerIds.PsychicCrushIV,        PsychicCrush(PowerIds.PsychicCrushIV, 4) },
            { PowerIds.PsychicCrushV,         PsychicCrush(PowerIds.PsychicCrushV, 5) },

            { PowerIds.TowerOfIronWillI,      TowerOfIronWill(PowerIds.TowerOfIronWillI, 1) },
            { PowerIds.TowerOfIronWillII,     TowerOfIronWill(PowerIds.TowerOfIronWillII, 2) },
            { PowerIds.TowerOfIronWillIII,    TowerOfIronWill(PowerIds.TowerOfIronWillIII, 3) },
            { PowerIds.TowerOfIronWillIV,     TowerOfIronWill(PowerIds.TowerOfIronWillIV, 4) },
            { PowerIds.TowerOfIronWillV,      TowerOfIronWill(PowerIds.TowerOfIronWillV, 5) },

            { PowerIds.IntellectFortressI,    IntellectFortress(PowerIds.IntellectFortressI, 1) },
            { PowerIds.IntellectFortressII,   IntellectFortress(PowerIds.IntellectFortressII, 2) },
            { PowerIds.IntellectFortressIII,  IntellectFortress(PowerIds.IntellectFortressIII, 3) },

            { PowerIds.ThoughtShieldI,        ThoughtShield(PowerIds.ThoughtShieldI, 1) },
            { PowerIds.ThoughtShieldII,       ThoughtShield(PowerIds.ThoughtShieldII, 2) },
            { PowerIds.ThoughtShieldIII,      ThoughtShield(PowerIds.ThoughtShieldIII, 3) },
            { PowerIds.ThoughtShieldIV,       ThoughtShield(PowerIds.ThoughtShieldIV, 4) },
            { PowerIds.ThoughtShieldV,        ThoughtShield(PowerIds.ThoughtShieldV, 5) },

            { PowerIds.EgoWhipI,          EgoWhip(PowerIds.EgoWhipI, 1) },
            { PowerIds.EgoWhipII,         EgoWhip(PowerIds.EgoWhipII, 2) },
            { PowerIds.EgoWhipIII,        EgoWhip(PowerIds.EgoWhipIII, 3) },
            { PowerIds.EgoWhipIV,         EgoWhip(PowerIds.EgoWhipIV, 4) },
            { PowerIds.EgoWhipV,          EgoWhip(PowerIds.EgoWhipV, 5) },

            { PowerIds.MindThrustI,       MindThrust(PowerIds.MindThrustI, 1) },
            { PowerIds.MindThrustII,      MindThrust(PowerIds.MindThrustII, 2) },
            { PowerIds.MindThrustIII,     MindThrust(PowerIds.MindThrustIII, 3) },
            { PowerIds.MindThrustIV,      MindThrust(PowerIds.MindThrustIV, 4) },
            { PowerIds.MindThrustV,       MindThrust(PowerIds.MindThrustV, 5) },
            { PowerIds.MindThrustVI,      MindThrust(PowerIds.MindThrustVI, 6) },

            { PowerIds.MentalBarrierI,    MentalBarrier(PowerIds.MentalBarrierI, 1) },
            { PowerIds.MentalBarrierII,   MentalBarrier(PowerIds.MentalBarrierII, 2) },
            { PowerIds.MentalBarrierIII,  MentalBarrier(PowerIds.MentalBarrierIII, 3) },
            { PowerIds.MentalBarrierIV,   MentalBarrier(PowerIds.MentalBarrierIV, 4) },
            { PowerIds.MentalBarrierV,    MentalBarrier(PowerIds.MentalBarrierV, 5) },

        };

    public static PsionicPowerDefinition Get(string id)
    {
        if (Definitions.TryGetValue(id, out PsionicPowerDefinition power)) return power;

        Debug.LogWarning($"[PowerDatabase] Power not found: {id}");
        return null;
    }

    // ===== Tier chain builders: one entry per tier, each at its own power level =====

    static int PowerPointCost(int level) => 2 * level - 1;

    //Psychic Crush I–V (PF psychic 5–9): Will, then Fortitude. See PsychicCrushEffect
    static PsionicPowerDefinition PsychicCrush(string id, int tier)
    {
        int level = tier + 4;
        return new PsionicPowerDefinition(
            actionId: id,
            targetType: TargetType.Enemy,
            requiresTarget: true,
            maxRange: 0,
            targetingMode: ActionDefinition.TargetingMode.Tile,
            category: ActionDefinition.ActionCategory.Standard,
            dealsDamage: true,
            effects: new List<IActionEffect> { new PsychicCrushEffect(tier) },
            powerLevel: level,
            discipline: PsionicDiscipline.Telepathy,
            basePowerPointCost: PowerPointCost(level),
            keyAbility: StatType.Presence,
            saveType: Unit.SaveType.Focus,
            allowsPowerResistance: true,
            descriptors: EffectDescriptor.MindAffecting,
            tierGroupId: PowerIds.TierPsychicCrush,
            tier: tier,
            rangeBand: PsionicPowerDefinition.PowerRange.Close);
    }

    //Tower of Iron Will I–V (PF psychic 5–9): immediate. Allies within 10 ft. (self included) gain
    //PR 10 + 2 x level against mind-affecting powers for 1–5 rounds
    static PsionicPowerDefinition TowerOfIronWill(string id, int tier)
    {
        int level = tier + 4;
        return new PsionicPowerDefinition(
            actionId: id,
            targetType: TargetType.Self,
            requiresTarget: false,
            maxRange: 0,
            category: ActionDefinition.ActionCategory.Swift,
            effects: new List<IActionEffect>
            {
                new ApplyStatusEffectToUnitsInRangeEffect(caster => new PowerResistanceEffect(
                    10 + 2 * level, tier, EffectDescriptor.MindAffecting, "TowerOfIronWill"),
                    range: 2, targetsAllies: true)
            },
            requiresLoS: false,
            powerLevel: level,
            discipline: PsionicDiscipline.Telepathy,
            basePowerPointCost: PowerPointCost(level),
            keyAbility: StatType.Presence,
            provokesOA: false,
            tierGroupId: PowerIds.TierTowerOfIronWill,
            tier: tier,
            rangeBand: PsionicPowerDefinition.PowerRange.Personal)
        { reactionTrigger = ReactionTrigger.TargetedByMindAffecting };
    }

    //Intellect Fortress I–III (PF psychic 4–6): immediate. Allies within 20 ft. (self included), 1 round
    static PsionicPowerDefinition IntellectFortress(string id, int tier)
    {
        int level = tier + 3;
        return new PsionicPowerDefinition(
            actionId: id,
            targetType: TargetType.Self,
            requiresTarget: false,
            maxRange: 0,
            category: ActionDefinition.ActionCategory.Swift,
            effects: new List<IActionEffect>
            {
                new ApplyStatusEffectToUnitsInRangeEffect(caster => new IntellectFortressEffect(tier),
                    range: 4, targetsAllies: true)
            },
            requiresLoS: false,
            powerLevel: level,
            discipline: PsionicDiscipline.Telepathy,
            basePowerPointCost: PowerPointCost(level),
            keyAbility: StatType.Presence,
            provokesOA: false,
            tierGroupId: PowerIds.TierIntellectFortress,
            tier: tier,
            rangeBand: PsionicPowerDefinition.PowerRange.Personal)
        { reactionTrigger = ReactionTrigger.TargetedByMindAffecting };
    }

    //Thought Shield I–V (PF psychic 2–6): immediate, personal. 1 round; V lasts 1 round/level
    static PsionicPowerDefinition ThoughtShield(string id, int tier)
    {
        int level = tier + 1;
        return new PsionicPowerDefinition(
            actionId: id,
            targetType: TargetType.Self,
            requiresTarget: false,
            maxRange: 0,
            category: ActionDefinition.ActionCategory.Swift,
            effects: new List<IActionEffect>
            {
                new ApplySelfStatusEffectEffect(self => new ThoughtShieldEffect(tier,
                    tier >= 5 ? PsionicPowerDefinition.GetManifesterLevel(self) : 1))
            },
            requiresLoS: false,
            powerLevel: level,
            discipline: PsionicDiscipline.Telepathy,
            basePowerPointCost: PowerPointCost(level),
            keyAbility: StatType.Presence,
            provokesOA: false,
            tierGroupId: PowerIds.TierThoughtShield,
            tier: tier,
            rangeBand: PsionicPowerDefinition.PowerRange.Personal)
        { reactionTrigger = ReactionTrigger.TargetedByMindAffecting };
    }

    //Stub: Ego Whip I–V (PF psychic 3–7) — Focus partial, PR; 1 round/level. Penalty to Cognition, Insight or
    //Presence (-2/-4/-6/-8/-10) and staggered 1 round / 1d4 / 1d6 / 1d8 / 1d10 rounds. A successful save
    //means no stagger and a 1-round penalty. Needs the Staggered condition
    static PsionicPowerDefinition EgoWhip(string id, int tier)
    {
        int level = tier + 2;
        return new PsionicPowerDefinition(
            actionId: id,
            targetType: TargetType.Enemy,
            requiresTarget: true,
            maxRange: 0,
            targetingMode: ActionDefinition.TargetingMode.Tile,
            powerLevel: level,
            discipline: PsionicDiscipline.Telepathy,
            basePowerPointCost: PowerPointCost(level),
            keyAbility: StatType.Presence,
            saveType: Unit.SaveType.Focus,
            allowsPowerResistance: true,
            descriptors: EffectDescriptor.Compulsion | EffectDescriptor.Emotion | EffectDescriptor.MindAffecting,
            tierGroupId: PowerIds.TierEgoWhip,
            tier: tier,
            rangeBand: PsionicPowerDefinition.PowerRange.Close);
    }

    //Stub: Mind Thrust I–VI (PF psychic 1–6) — Focus half, PR; no effect on mindless creatures.
    //Damage per ML: I 1d6 (max 5d6), II 1d8 (max 5d8), III 1d8 (max 10d8), IV–V 1d8 (max 15d8), VI 1d8 (max 20d8).
    //Failed save: IV fatigued 1 round; V exhausted 1 round (fatigued on a success); VI exhausted and stunned 1 round.
    //Needs the Fatigued and Exhausted conditions
    static PsionicPowerDefinition MindThrust(string id, int tier)
    {
        return new PsionicPowerDefinition(
            actionId: id,
            targetType: TargetType.Enemy,
            requiresTarget: true,
            maxRange: 0,
            targetingMode: ActionDefinition.TargetingMode.Tile,
            dealsDamage: true,
            powerLevel: tier,
            discipline: PsionicDiscipline.Telepathy,
            basePowerPointCost: PowerPointCost(tier),
            keyAbility: StatType.Presence,
            saveType: Unit.SaveType.Focus,
            allowsPowerResistance: true,
            descriptors: EffectDescriptor.MindAffecting,
            tierGroupId: PowerIds.TierMindThrust,
            tier: tier,
            rangeBand: PsionicPowerDefinition.PowerRange.Close);
    }

    //Stub: Mental Barrier I–V (PF psychic 2–6) — immediate, personal, 1 round. Shield bonus to Defense
    //(+4/+6/+8/+8/+8). IV: 25% chance to negate critical hits and sneak attacks; V: 50%. Halves Mind Thrust
    //damage within a tier range and grants immunity to lower tiers (a quarter on a successful save)
    static PsionicPowerDefinition MentalBarrier(string id, int tier)
    {
        int level = tier + 1;
        return new PsionicPowerDefinition(
            actionId: id,
            targetType: TargetType.Self,
            requiresTarget: false,
            maxRange: 0,
            category: ActionDefinition.ActionCategory.Swift,
            requiresLoS: false,
            powerLevel: level,
            discipline: PsionicDiscipline.Telepathy,
            basePowerPointCost: PowerPointCost(level),
            keyAbility: StatType.Presence,
            provokesOA: false,
            tierGroupId: PowerIds.TierMentalBarrier,
            tier: tier,
            rangeBand: PsionicPowerDefinition.PowerRange.Personal)
        { reactionTrigger = ReactionTrigger.TargetedByAttack };
    }
}