using UnityEngine;

// Receives player input from the combat UI and forwards to CombatManager.
// Separated from CombatUIController so display and input logic stay distinct.
// TileHighlighter handles tile clicks and already routes to ICombatInputHandler —
// this class handles button-driven input from the action bar and confirm button.
public class CombatInputHandler : MonoBehaviour
{
    [SerializeField] private CombatManager combatManager;

    public void OnActionSelected(ActionDefinition action)
    {
        if (combatManager == null) return;
        if (!combatManager.IsPlayerTurn) return;

        combatManager.SelectAction(action);
    }

    public void OnConfirmTurn()
    {
        if (combatManager == null) return;
        if (!combatManager.IsPlayerTurn) return;

        combatManager.ConfirmTurn();
    }

    public void OnCancelAction()
    {
        if (combatManager == null) return;

        combatManager.Cancel();
    }
}
