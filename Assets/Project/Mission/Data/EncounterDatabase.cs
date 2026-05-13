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
        };

    public static EncounterData Get(string id)
    {
        if (Definitions.TryGetValue(id, out EncounterData encounter)) return encounter;

        Debug.LogWarning($"[EncounterDatabase] Encounter not found: {id}");
        return null;
    }
}