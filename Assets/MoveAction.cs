using UnityEngine;

public class MoveAction : BaseAction
{
    public override void Execute(CombatAction action, CombatManager manager)
    {
        if (action.movePosition == null)
        {
            Debug.LogError("MoveAction missing movePosition");
            return;
        }

        Vector2Int newPos = action.movePosition.Value;

        if (!GridManager.Instance.IsWithinBounds(newPos))
        {
            CombatLog.Add("Invalid move (out of bounds)");
            return;
        }

        if (GridManager.Instance.IsTileOccupied(newPos))
        {
            CombatLog.Add("Invalid move (tile occupied)");
            return;
        }

        GridManager.Instance.MoveUnit(action.source, newPos);

        CombatLog.Add(action.source.unitName + " moved to " + newPos);
    }
}
