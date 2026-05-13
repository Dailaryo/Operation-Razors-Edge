using UnityEngine;
using System.Collections.Generic;

public class GridHighlighter : MonoBehaviour
{
    public static GridHighlighter Instance;

    void Awake()
    {
        Instance = this;
    }

    //Clear all tiles
    public void ClearAll()
    {
        foreach (var kvp in GridVisualizer.Instance.GetAllTiles())
        {
            kvp.Value.SetBaseHighlight(Tile.TileHighlightType.None);
            kvp.Value.SetOverlayHighlight(Tile.TileHighlightType.None);
        }

        ////Debug.Log("Clearing highlights...");
    }

    //Highlight valid movement tiles
    public void HighlightValidMoves(Unit unit, CombatManager manager)
    {
        CombatResolver resolver = new CombatResolver(manager, true);

        //Simulate this unit's planned actions
        var plannedActions = manager.GetPlannedActions(unit);

        foreach (var prior in plannedActions)
        {
            ActionContext ctx = new ActionContext
            {
                attacker = prior.source,
                originalTarget = prior.target,
                finalTarget = prior.target,
                baseDamage = prior.source.attackRating,
                modifiedDamage = prior.source.attackRating,
                action = prior,
                temporaryEffects = prior.temporaryEffects
            };

            var def = prior.definition;
            BaseAction baseAction = new GenericAction(def);
            if (baseAction == null) continue;

            baseAction.Execute(ctx, resolver);

            if (ctx.isMovement)
                resolver.ResolveMove(ctx);
            else if (ctx.shouldApplyDamage)
                resolver.ApplyFinalResult(ctx);
        }

        foreach (var kvp in GridVisualizer.Instance.GetAllTiles())
        {
            Vector2Int pos = kvp.Key;
            Tile tile = kvp.Value;

            Unit occupyingUnit = GridManager.Instance.GetUnitAt(pos);

            //If this unit is moving away, treat tile as empty
            if (occupyingUnit == unit && manager.IsUnitLeavingTile(unit, pos))
            {
                occupyingUnit = null;
            }

            //If another unit plans to move here, treat as occupied
            bool isPlannedOccupied = manager.IsTilePlannedOccupied(pos);

            //Detect unit presence
            if (occupyingUnit != null || isPlannedOccupied)
            {
                if (occupyingUnit != null)
                {
                    tile.SetBaseHighlight(
                        occupyingUnit.isEnemy == unit.isEnemy
                        ? Tile.TileHighlightType.Ally
                        : Tile.TileHighlightType.Enemy
                    );
                }
                else
                {
                    //Planned occupied -> block movement but don't fake a unit
                    tile.SetBaseHighlight(Tile.TileHighlightType.None);
                }

                continue;
            }

            //Create a test move action
            var moveDef = ActionDatabase.Definitions[ActionIds.Move];
            CombatAction testMove = new CombatAction(
                unit,
                null,
                moveDef,
                manager
            );

            testMove.SetMovePosition(pos);

            //Reuse validator
            if (ActionValidator.IsValid(testMove, resolver))
            {
                tile.SetBaseHighlight(Tile.TileHighlightType.MoveValid);
            }
            else
            {
                tile.SetBaseHighlight(Tile.TileHighlightType.None);
            }
        }

        // Overlay threatened tiles so the player can see OA risk during movement planning
        foreach (var kvp in GridVisualizer.Instance.GetAllTiles())
        {
            Vector2Int pos = kvp.Key;
            Tile tile = kvp.Value;

            // Only warn about tiles the player could actually move to
            if (tile.GetBaseHighlight() != Tile.TileHighlightType.MoveValid) continue;

            List<Unit> threateners = ThreatZoneManager.GetThreateningUnits(pos, isEnemy: true);

            if (threateners.Count > 0)
                tile.SetOverlayHighlight(Tile.TileHighlightType.Threatened);
        }

        //Debug.Log("Highlighting valid tiles...");
    }

    public void HighlightValidTargets(Unit unit, string actionType, CombatManager manager)
    {
        ////Debug.Log("HighlightValidTargets CALLED");

        Vector2Int plannedOrigin = manager.GetPlannedPosition(unit);

        var def = ActionDatabase.Definitions[actionType];

        CombatResolver resolver = new CombatResolver(manager, false);

        //If action doesn't use targets -> don't use
        if(!def.requiresTarget)
        {
            //Fallback to default state
            HighlightValidMoves(unit, manager);
            return;
        }

        foreach (var kvp in GridVisualizer.Instance.GetAllTiles())
        {
            Vector2Int pos = kvp.Key;
            Tile tile = kvp.Value;

            if (DistanceUtility.Chebyshev(plannedOrigin, pos) > def.maxRange) continue;
            int distance = DistanceUtility.MovementCostDistance(plannedOrigin, pos);

            //Check if tile is within range
            bool inRange = distance >= def.minRange && distance <= def.maxRange;

            if (!inRange)
            {
                //Outside range -> still show units
                Unit u = GridManager.Instance.GetUnitAt(pos);

                if (u != null)
                {
                    tile.SetBaseHighlight(
                        u.isEnemy == unit.isEnemy
                        ? Tile.TileHighlightType.Ally
                        : Tile.TileHighlightType.Enemy
                    );
                }
                else
                {
                    tile.SetBaseHighlight(Tile.TileHighlightType.None);
                }

                continue;
            }

            //Tile is within range
            Unit targetUnit = GridManager.Instance.GetUnitAt(pos);

            //Treat origin tile as empty if unit is moving away
            if (targetUnit == unit && manager.IsUnitLeavingTile(unit, pos))
            {
                targetUnit = null;
            }

            //Treat planned destination as occupied
            if (manager.IsTilePlannedOccupied(pos))
            {
                targetUnit = unit; //dummy to mark occupied
            }

            //empty tile -> show range
            if (targetUnit == null)
            {
                tile.SetBaseHighlight(Tile.TileHighlightType.InRange);
                continue;
            }

            //Ally
            if (targetUnit.isEnemy == unit.isEnemy)
            {
                tile.SetBaseHighlight(Tile.TileHighlightType.Ally);
                continue;
            }

            //Enemy -> validate
            CombatAction testAction = new CombatAction(
                unit,
                targetUnit,
                def,
                manager
            );

            bool isValid = ActionValidator.IsValid(testAction, resolver);

            if (isValid)
            {
                tile.SetBaseHighlight(Tile.TileHighlightType.ValidTarget);
            }
            else
            {
                tile.SetBaseHighlight(Tile.TileHighlightType.Enemy);
            }
        }
    }

    public void RefreshUnitHighlights(CombatManager manager)
    {
        ClearAll();

        foreach (var kvp in GridVisualizer.Instance.GetAllTiles())
        {
            Tile tile = kvp.Value;
            Vector2Int pos = kvp.Key;

            Unit unit = GridManager.Instance.GetUnitAt(pos);

            if (unit == null) continue;

            tile.SetBaseHighlight(
                unit.isEnemy
                ? Tile.TileHighlightType.Enemy
                : Tile.TileHighlightType.Ally
            );
        }
    }
}
