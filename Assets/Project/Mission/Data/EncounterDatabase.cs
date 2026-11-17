using System.Collections.Generic;
using UnityEngine;

public static class EncounterDatabase
{
    public static Dictionary<string, EncounterData> Definitions =
        new Dictionary<string, EncounterData>()
        {
            // Scout Encounter
            { EncounterIds.ScoutEncounter, new EncounterData(
                EncounterIds.ScoutEncounter,
                "Scout Encounter",
                2,
                "Eliminate all enemies"
            )
            {
                isRequired = true,
                factionId = "coalition",
                factionContext = "Coalition scouts have identified an Imperial patrol in the area.",
                enemyTypeName = "Imperial Patrol Drones",
                primaryObjective = new ObjectiveDefinition(
                    objectiveId: "scout_primary",
                    description: "Eliminate all enemies",
                    condition: new EliminationCondition(),
                    reward: new MissionReward(credits: 150),
                    isPrimary: true
                ),
                secondaryObjectives = new List<ObjectiveDefinition>
                {
                    new ObjectiveDefinition(
                        objectiveId: "scout_secondary_holdtheline",
                        description: "Hold the line — survive 2 turns before eliminating all enemies",
                        condition: new CompositeCondition(
                            new List<IObjectiveCondition>
                            {
                                new SurvivalCondition(requiredTurns: 2),
                                new EliminationCondition()
                            },
                            CompositeCondition.Mode.All
                        ),
                        reward: new MissionReward(credits: 50)
                    )
                }
            }},

            // Hold Position
            { EncounterIds.HoldPosition, new EncounterData(
                EncounterIds.HoldPosition,
                "Hold Position",
                3,
                "Survive 3 turns"
            )
            {
                isRequired = false,
                endsWhenPrimaryComplete = true, //G19: "Survive 3 turns" ends at turn 3
                factionId = "coalition",
                factionContext = "Hold the perimeter while Coalition forces withdraw.",
                enemyTypeName = "Imperial Assault Drones",
                primaryObjective = new ObjectiveDefinition(
                    objectiveId: "hold_primary",
                    description: "Survive 3 turns",
                    condition: new SurvivalCondition(requiredTurns: 3),
                    reward: new MissionReward(credits: 150),
                    isPrimary: true
                ),
                secondaryObjectives = new List<ObjectiveDefinition>
                {
                    new ObjectiveDefinition(
                        objectiveId: "hold_secondary_wipeout",
                        description: "Wipe them out — eliminate all enemies before time expires",
                        condition: new CompositeCondition(
                            new List<IObjectiveCondition>
                            {
                                new EliminationCondition(),
                                new TurnLimitCondition(turnLimit: 3)
                            },
                            CompositeCondition.Mode.All
                        ),
                        reward: new MissionReward(credits: 75)
                    )
                }
            }},

            // Assassination
            { EncounterIds.Assassination, new EncounterData(
                EncounterIds.Assassination,
                "Assassination",
                4,
                "Eliminate priority target"
            )
            {
                isRequired = true,
                factionId = "imperium",
                factionContext = "An Imperial officer has gone rogue. Command wants them silenced.",
                enemyTypeName = "Imperial Drones + Priority Target",
                primaryObjective = new ObjectiveDefinition(
                    objectiveId: "assassination_primary",
                    description: "Eliminate the priority target",
                    condition: new TargetEliminationCondition(targetUnitId: "assassination_target"),
                    reward: new MissionReward(credits: 200),
                    isPrimary: true
                ),
                secondaryObjectives = new List<ObjectiveDefinition>
                {
                    new ObjectiveDefinition(
                        objectiveId: "assassination_secondary_nocasualties",
                        description: "No casualties — finish with full squad",
                        condition: new SquadStrengthCondition(minimumUnits: 6),
                        reward: new MissionReward(credits: 100)
                    )
                }
            }},
            
            // ===== Training drills (Milestone 28) — isTraining: no campaign record, XP, rewards, loot or saves =====

            // TR-01 Target Range: to-hit vs Defense, damage, crits, DR. Static target never dies, so it ends on survival.
            { EncounterIds.TrainingTargetRange, new EncounterData(
                EncounterIds.TrainingTargetRange,
                "TR-01 Target Range",
                1,
                "Training: attack the static target for 5 rounds"
            )
            {
                isTraining = true,
                endsWhenPrimaryComplete = true,
                mapId = MapIds.Default,
                enemyTypeName = "Training Target",
                primaryObjective = new ObjectiveDefinition(
                    objectiveId: "training_target_range_primary",
                    description: "Survive 5 rounds",
                    condition: new SurvivalCondition(requiredTurns: 5),
                    reward: new MissionReward(),
                    isPrimary: true
                )
            }},

            // TR-02 Save Gauntlet: Resolve / Response / Focus-gated actions and powers. Ends on elimination.
            { EncounterIds.TrainingSaveGauntlet, new EncounterData(
                EncounterIds.TrainingSaveGauntlet,
                "TR-02 Save Gauntlet",
                3,
                "Training: defeat one Hardened target per save"
            )
            {
                isTraining = true,
                mapId = MapIds.Default,
                enemyTypeName = "Hardened Targets",
                primaryObjective = new ObjectiveDefinition(
                    objectiveId: "training_save_gauntlet_primary",
                    description: "Eliminate all targets",
                    condition: new EliminationCondition(),
                    reward: new MissionReward(),
                    isPrimary: true
                )
            }},

            // TR-03 Mind Lab: mind-affecting immunity (Mindless target), Aura of Calm, Sympathetic Link, concentration.
            { EncounterIds.TrainingMindLab, new EncounterData(
                EncounterIds.TrainingMindLab,
                "TR-03 Mind Lab",
                2,
                "Training: compare a sentient and a mindless target for 4 rounds"
            )
            {
                isTraining = true,
                endsWhenPrimaryComplete = true,
                mapId = MapIds.Default,
                enemyTypeName = "Training Targets",
                primaryObjective = new ObjectiveDefinition(
                    objectiveId: "training_mind_lab_primary",
                    description: "Survive 4 rounds",
                    condition: new SurvivalCondition(requiredTurns: 4),
                    reward: new MissionReward(),
                    isPrimary: true
                )
            }},

            // TR-04 Sparring Ring: Defense buffs, DR, temp HP, Resilience / Last Stand, OAs, concentration on damage.
            { EncounterIds.TrainingSparringRing, new EncounterData(
                EncounterIds.TrainingSparringRing,
                "TR-04 Sparring Ring",
                2,
                "Training: hold out against two Strikers for 3 rounds"
            )
            {
                isTraining = true,
                endsWhenPrimaryComplete = true,
                mapId = MapIds.Default,
                enemyTypeName = "Strikers",
                primaryObjective = new ObjectiveDefinition(
                    objectiveId: "training_sparring_ring_primary",
                    description: "Survive 3 rounds",
                    condition: new SurvivalCondition(requiredTurns: 3),
                    reward: new MissionReward(),
                    isPrimary: true
                )
            }},

            // TR-05 Firing Line: range bands, line of sight, Hide, detection, stealth approach. Open 10x8 map;
            // walls, doors and windows are descoped until edges have visual indicators (TestRoom keeps them for later).
            { EncounterIds.TrainingFiringLine, new EncounterData(
                EncounterIds.TrainingFiringLine,
                "TR-05 Firing Line",
                2,
                "Training: clear two Marksmen using cover and line of sight"
            )
            {
                isTraining = true,
                mapId = MapIds.FiringLine,
                enemyTypeName = "Marksmen",
                primaryObjective = new ObjectiveDefinition(
                    objectiveId: "training_firing_line_primary",
                    description: "Eliminate all targets",
                    condition: new EliminationCondition(),
                    reward: new MissionReward(),
                    isPrimary: true
                )
            }},
            
            // TR-06 Triage Drill: Disabled / Dying / Stabilized, healing, revive, miracles. Expect G14 on miracles.
            { EncounterIds.TrainingTriage, new EncounterData(
                EncounterIds.TrainingTriage,
                "TR-06 Triage Drill",
                1,
                "Training: keep the squad standing against a Bruiser for 4 rounds"
            )
            {
                isTraining = true,
                endsWhenPrimaryComplete = true,
                mapId = MapIds.Default,
                enemyTypeName = "Bruiser",
                primaryObjective = new ObjectiveDefinition(
                    objectiveId: "training_triage_primary",
                    description: "Survive 4 rounds",
                    condition: new SurvivalCondition(requiredTurns: 4),
                    reward: new MissionReward(),
                    isPrimary: true
                )
            }},

            // TR-07 Crowd Control: bursts, multi-target and area effects, forced movement.
            { EncounterIds.TrainingCrowdControl, new EncounterData(
                EncounterIds.TrainingCrowdControl,
                "TR-07 Crowd Control",
                4,
                "Training: clear four weak Strikers with area and multi-target effects"
            )
            {
                isTraining = true,
                mapId = MapIds.TrainingRange,
                enemyTypeName = "Weak Strikers",
                primaryObjective = new ObjectiveDefinition(
                    objectiveId: "training_crowd_control_primary",
                    description: "Eliminate all targets",
                    condition: new EliminationCondition(),
                    reward: new MissionReward(),
                    isPrimary: true
                )
            }},

            // TR-08 Escalation: reinforcement placement, usesPerCombat, cooldowns, resource regen, power points,
            // material pool. Wave 1 (TurnBased) lands at the end of round 2; wave 2 (EnemyCount) when one enemy remains.
            { EncounterIds.TrainingEscalation, new EncounterData(
                EncounterIds.TrainingEscalation,
                "TR-08 Escalation",
                2,
                "Training: hold out for 6 rounds as reinforcements arrive"
            )
            {
                isTraining = true,
                endsWhenPrimaryComplete = true,
                mapId = MapIds.TrainingRange,
                enemyTypeName = "Strikers + reinforcements",
                primaryObjective = new ObjectiveDefinition(
                    objectiveId: "training_escalation_primary",
                    description: "Survive 6 rounds",
                    condition: new SurvivalCondition(requiredTurns: 6),
                    reward: new MissionReward(),
                    isPrimary: true
                ),
                escalationWaves = new List<EscalationWaveDefinition>
                {
                    new EscalationWaveDefinition(
                        waveId: "training_escalation_wave_turn",
                        trigger: EscalationWaveDefinition.TriggerCondition.TurnBased,
                        triggerValue: 2, //TurnBased fires in the end phase of this turn: end of Turn 2
                        enemies: new List<EscalationWaveDefinition.EnemySpawnEntry>
                        {
                            new EscalationWaveDefinition.EnemySpawnEntry(EnemyIds.TrainingStriker, 2)
                        },
                        spawn: EscalationWaveDefinition.SpawnPattern.EdgeSpawn,
                        edge: EscalationWaveDefinition.SpawnEdge.Right,
                        level: 0), //0 = match the squad
                    new EscalationWaveDefinition(
                        waveId: "training_escalation_wave_count",
                        trigger: EscalationWaveDefinition.TriggerCondition.EnemyCount,
                        triggerValue: 1, //fires at the first end phase with one or no living enemies
                        enemies: new List<EscalationWaveDefinition.EnemySpawnEntry>
                        {
                            new EscalationWaveDefinition.EnemySpawnEntry(EnemyIds.TrainingMarksman, 1)
                        },
                        spawn: EscalationWaveDefinition.SpawnPattern.EdgeSpawn,
                        edge: EscalationWaveDefinition.SpawnEdge.Right,
                        level: 0)
                }
            }},

            // TR-09 Large Target: multi-tile footprint, reach, threat zones.
            { EncounterIds.TrainingLargeTarget, new EncounterData(
                EncounterIds.TrainingLargeTarget,
                "TR-09 Large Target",
                1,
                "Training: defeat a Large target (2x2 footprint, reach 2)"
            )
            {
                isTraining = true,
                mapId = MapIds.TrainingRange,
                enemyTypeName = "Large Target",
                primaryObjective = new ObjectiveDefinition(
                    objectiveId: "training_large_target_primary",
                    description: "Eliminate the target",
                    condition: new EliminationCondition(),
                    reward: new MissionReward(),
                    isPrimary: true
                )
            }},
        };

    public static EncounterData Get(string id)
    {
        if (Definitions.TryGetValue(id, out EncounterData encounter)) return encounter;

        Debug.LogWarning($"[EncounterDatabase] Encounter not found: {id}");
        return null;
    }
}