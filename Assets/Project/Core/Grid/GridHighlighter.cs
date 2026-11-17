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
        //Live resolver: ActionValidator builds a fresh simulation of this unit's planned actions for each check
        //(same as HighlightValidTargets). Passing a simulation resolver made the validator replay the plan again
        //onto it for every tile, stacking planning-only bonuses (Burst) across tiles.
        CombatResolver resolver = new CombatResolver(manager, false);

        var plannedActions = manager.GetPlannedActions(unit);

        //Only offer move tiles if the unit can still afford a move this turn (Move, or a Standard converted to Move).
        //The probe is a throwaway action; TryAssignCategories only writes to it, never to the planned list
        var moveDefinition = ActionDatabase.Definitions[ActionIds.Move];
        CombatAction economyProbe = new CombatAction(unit, null, moveDefinition, manager);
        bool canAffordMove = ActionEconomyResolver.TryAssignCategories(
            unit, plannedActions, economyProbe, ActionEconomyResolver.GetLimitsForUnit(unit));

        foreach (var kvp in GridVisualizer.Instance.GetAllTiles())
        {
            Vector2Int pos = kvp.Key;
            Tile tile = kvp.Value;

            Unit occupyingUnit = GridManager.Instance.GetUnitAt(pos);

            //Units stay on their origin tile until the plan executes, so keep showing them there.
            //(Moving back to the origin is done by cancelling the move, not by selecting the tile.)
            if (occupyingUnit != null)
            {
                tile.SetBaseHighlight(
                    occupyingUnit.isEnemy == unit.isEnemy
                    ? Tile.TileHighlightType.Ally
                    : Tile.TileHighlightType.Enemy
                );
                continue;
            }

            //Planned destinations (this unit's or another ally's) block movement and get their own colour
            if (manager.IsTilePlannedOccupied(pos))
            {
                tile.SetBaseHighlight(Tile.TileHighlightType.PlannedMove);

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
            if (canAffordMove && ActionValidator.IsValid(testMove, resolver))
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

    public void HighlightValidTargets(Unit unit, ActionDefinition def, CombatManager manager)
    {
        Vector2Int plannedOrigin = manager.GetPlannedPosition(unit);

        //Range bands (Close/Medium/Long powers) scale with manifester level; same call ActionValidator uses
        int maxRange = def.GetMaxRange(unit);

        CombatResolver resolver = new CombatResolver(manager, false);

        //Ground-targeted actions highlight tiles, not units
        if (def.IsGroundTargeted())
        {
            HighlightGroundTiles(unit, def, manager);
            return;
        }

        //If action doesn't use targets -> don't use
        if (!def.requiresTarget)
        {
            //Fallback to default state
            HighlightValidMoves(unit, manager);
            return;
        }

        foreach (var kvp in GridVisualizer.Instance.GetAllTiles())
        {
            Vector2Int pos = kvp.Key;
            Tile tile = kvp.Value;

            if (DistanceUtility.Chebyshev(plannedOrigin, pos) > maxRange) continue;
            int distance = DistanceUtility.MovementCostDistance(plannedOrigin, pos);

            //Check if tile is within range
            bool inRange = distance >= def.minRange && distance <= maxRange;

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
                    tile.SetBaseHighlight(manager.IsTilePlannedOccupied(pos)
                        ? Tile.TileHighlightType.PlannedMove
                        : Tile.TileHighlightType.None);
                }

                continue;
            }

            //Tile is within range
            Unit targetUnit = GridManager.Instance.GetUnitAt(pos);

            //The acting unit still stands on its origin until the plan executes, so it shows as Ally there.
            //Empty planned destinations show as PlannedMove and aren't targetable
            if (targetUnit == null && manager.IsTilePlannedOccupied(pos))
            {
                tile.SetBaseHighlight(Tile.TileHighlightType.PlannedMove);
                continue;
            }

            //empty tile -> show range
            if (targetUnit == null)
            {
                tile.SetBaseHighlight(Tile.TileHighlightType.InRange);
                continue;
            }

            bool isAlly = targetUnit.isEnemy == unit.isEnemy;

            //Ally on a non-ally action: shown, never selectable
            if (isAlly && def.targetType != TargetType.Ally)
            {
                tile.SetBaseHighlight(Tile.TileHighlightType.Ally);
                continue;
            }

            //Enemy, or ally on an ally-targeting action (Treat Injury, heals) -> validate
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
                tile.SetBaseHighlight(isAlly ? Tile.TileHighlightType.Ally : Tile.TileHighlightType.Enemy);
            }
        }
    }

    //Ground-targeted actions (Deploy Field, Break Window): every tile in range is selectable, occupied or not
    void HighlightGroundTiles(Unit unit, ActionDefinition def, CombatManager manager)
    {
        CombatResolver resolver = new CombatResolver(manager, false);

        foreach (var kvp in GridVisualizer.Instance.GetAllTiles())
        {
            Vector2Int pos = kvp.Key;
            Tile tile = kvp.Value;

            Unit occupant = GridManager.Instance.GetUnitAt(pos);

            CombatAction testAction = new CombatAction(unit, null, def, manager);
            testAction.SetMovePosition(pos);

            //In-range tiles tint the terrain, so it stays readable, and are clickable
            if (ActionValidator.IsValid(testAction, resolver))
                tile.SetBaseHighlight(Tile.TileHighlightType.GroundTarget);
            else
                tile.SetBaseHighlight(occupant == null && manager.IsTilePlannedOccupied(pos)
                    ? Tile.TileHighlightType.PlannedMove
                    : Tile.TileHighlightType.None);

            //Units stay visible on top; the tile underneath is still selectable when in range
            tile.SetOverlayHighlight(occupant == null
                ? Tile.TileHighlightType.None
                : occupant.isEnemy == unit.isEnemy
                    ? Tile.TileHighlightType.Ally
                    : Tile.TileHighlightType.Enemy);
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

    //Field zones tint the ground until they expire; called when a zone is placed, expires or is cleared
    public void RefreshFieldZones()
    {
        if (GridVisualizer.Instance == null) return;

        foreach (var kvp in GridVisualizer.Instance.GetAllTiles())
            kvp.Value.SetFieldZone(FieldZoneManager.IsTileInActiveZone(kvp.Key));
    }
}
