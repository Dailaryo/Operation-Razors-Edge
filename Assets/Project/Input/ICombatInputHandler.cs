using UnityEngine;

public interface ICombatInputHandler
{
    void SelectAction(ActionDefinition actionId);
    void SelectTarget(Unit target);
    void SelectTile(Vector2Int position);
    void Cancel();
    void ConfirmTurn();
}
