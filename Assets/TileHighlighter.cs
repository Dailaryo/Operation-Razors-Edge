using UnityEngine;

public class TileHighlighter : MonoBehaviour
{
    Tile currentTile;

    void Update()
    {
        HandleHover();

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    void HandleHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Debug.Log("Hit: " + hit.collider.name);
            
            Tile tile = hit.collider.GetComponent<Tile>();

            if (tile != currentTile)
            {
                //if (currentTile != null) currentTile.SetHighlight(false);

                currentTile = tile;

                //if (currentTile != null) currentTile.SetHighlight(true);
            }
        }
        else
        {
            ClearCurrent();
        }
    }

    void ClearCurrent()
    {
        if (currentTile != null)
        {
            //currentTile.SetHighlight(false);
            currentTile = null;
        }
    }

    void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile tile = hit.collider.GetComponent<Tile>();

            if (tile != null)
            {
                Debug.Log("Clicked tile: " + tile.gridPosition);
                TryMovePlayer(tile);
            }
        }
    }

    void TryMovePlayer (Tile tile)
    {
        CombatManager manager = FindAnyObjectByType<CombatManager>();

        //Only allow during player turn
        if (!manager.IsPlayerTurn) return;

        Unit unit = manager.CurrentUnit;

        if (unit == null || unit.IsDead()) return;

        CombatAction moveAction = new CombatAction(
            unit,
            null,
            ActionType.Move,
            manager.GetAllUnits(), //allies/enemies not important here
            manager.GetAllUnits(),
            manager
        );

        moveAction.SetMovePosition(tile.gridPosition);

        if (!ActionValidator.IsValid(moveAction))
        {
            Debug.Log("Invalid move");
            return;
        }

        AddMoveToPlan(manager, moveAction);
    }

    void AddMoveToPlan(CombatManager manager, CombatAction action)
    {
        //We need access to plannedActions -> so we call into manager
        manager.AddPlannedAction(action);
    }
}
