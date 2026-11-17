using UnityEngine;
using UnityEngine.EventSystems;
using static Tile;

public class TileHighlighter : MonoBehaviour
{
    Tile currentTile;

    [SerializeField] private CombatManager combat;

    private ICombatInputHandler inputHandler;

    private Tile.TileHighlightType savedOverlay = Tile.TileHighlightType.None;

    void Update()
    {
        //Pointer over the action bar, log or any other UI: no hover, click or cancel reaches the grid
        if (IsPointerOverUI())
        {
            ClearCurrent();
            return;
        }

        HandleHover();

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Mouse clicked");
            HandleClick();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelTargeting();
        }
    }

    //Same check CameraController uses
    bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    void HandleHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            ////Debug.Log("Hit: " + hit.collider.name);
            
            Tile tile = hit.collider.GetComponent<Tile>();

            if (tile != currentTile)
            {
                // Restore previous tile's saved overlay before leaving it
                if (currentTile != null)
                {
                    currentTile.SetOverlayHighlight(savedOverlay);
                }

                currentTile = tile;

                // Save the new tile's current overlay before applying hover
                if (currentTile != null)
                {
                    savedOverlay = currentTile.GetOverlayHighlight();
                    currentTile.SetOverlayHighlight(Tile.TileHighlightType.Hover);
                }
                else
                {
                    savedOverlay = Tile.TileHighlightType.None;
                }
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
            currentTile.SetOverlayHighlight(savedOverlay);
            savedOverlay = Tile.TileHighlightType.None;
            currentTile = null;
        }
    }

    void HandleClick()
    {
        //Debug.Log("HandleClick fired");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            ////Debug.Log("Raycast hit: " + hit.collider.name);

            Tile tile = hit.collider.GetComponent<Tile>();

            if (tile == null)
            {
                ////Debug.Log("Hit something, but NOT a tile");
                return;
            }

            //Block invalid tiles before doing anything
            if (!tile.IsSelectable())
            {
                //Debug.Log("Clicked invalid tile");
                return;
            }

            ////Debug.Log("Tile component: " + tile);

            if (combat == null || !combat.IsPlayerTurn) return;
            if (inputHandler == null) return;

            inputHandler.SelectTile(tile.gridPosition);
        }
    }

    void CancelTargeting()
    {
        if (combat == null) return;

        combat.ClearPendingAction();

        GridHighlighter.Instance.ClearAll();

        if (combat.CurrentUnit != null && !combat.CurrentUnit.IsDead())
        {
            GridHighlighter.Instance.HighlightValidMoves(combat.CurrentUnit, combat);
        }
    }

    void Awake()
    {
        if (combat == null)
        {
            Debug.LogError("TileHighlighter missing CombatManager reference");
            return;
        }

        inputHandler = combat;
    }
}
