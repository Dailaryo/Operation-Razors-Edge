using UnityEngine;
using UnityEngine.EventSystems;
using static Tile;

public class TileHighlighter : MonoBehaviour
{
    Tile currentTile;

    [SerializeField] private CombatManager combat;

    private ICombatInputHandler inputHandler;

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

        //Right-click: the single cancel path (targeting first, then Maximize, then the last planned action)
        if (Input.GetMouseButtonDown(1) && combat != null)
            combat.Cancel();
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
                //Hover is a flag on the tile, not a saved overlay: a highlight rebuild while the cursor rests here
                //can't be undone by restoring a stale overlay when it leaves
                if (currentTile != null) currentTile.SetHover(false);

                currentTile = tile;

                if (currentTile != null) currentTile.SetHover(true);
            }
        }
        else
        {
            ClearCurrent();
        }
    }

    void ClearCurrent()
    {
        if (currentTile == null) return;

        currentTile.SetHover(false);
        currentTile = null;
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
