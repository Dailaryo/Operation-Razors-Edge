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
            kvp.Value.SetHighlight(Tile.TileHighlightType.None);
        }
    }

    //Highlight valid movement tiles
    public void HighlightValidMoves(Unit unit, CombatManager manager)
    {
        foreach (var kvp in GridVisualizer.Instance.GetAllTiles())
        {
            Vector2Int pos = kvp.Key;
            Tile tile = kvp.Value;

            //Create a test move action
            CombatAction testMove = new CombatAction(
                unit,
                null,
                ActionType.Move,
                manager.GetAllUnits(),
                manager.GetAllUnits(),
                manager
            );

            testMove.SetMovePosition(pos);

            //Reuse validator
            if (ActionValidator.IsValid(testMove))
            {
                tile.SetHighlight(Tile.TileHighlightType.MoveValid);
            }
        }
    }
}
