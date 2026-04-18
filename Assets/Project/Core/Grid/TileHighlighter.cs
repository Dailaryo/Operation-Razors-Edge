using UnityEngine;

public class TileHighlighter : MonoBehaviour
{
    Tile currentTile;

    [SerializeField] private CombatManager combat;

    private ICombatInputHandler inputHandler;

    void Update()
    {
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

    void HandleHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            ////Debug.Log("Hit: " + hit.collider.name);
            
            Tile tile = hit.collider.GetComponent<Tile>();

            if (tile != currentTile)
            {
                //Clear previous hover
                if(currentTile != null)
                {
                    currentTile.SetOverlayHighlight(Tile.TileHighlightType.None);
                }

                currentTile = tile;

                //Apply new hover
                if (currentTile != null)
                {
                    currentTile.SetOverlayHighlight(Tile.TileHighlightType.Hover);
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
            currentTile.SetOverlayHighlight(Tile.TileHighlightType.None);
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
        CombatManager manager = FindAnyObjectByType<CombatManager>();

        if (manager == null) return;

        //Clear pending action
        manager.ClearPendingAction();

        //Reset Highlights
        GridHighlighter.Instance.ClearAll();

        //Restore movement highlights
        if (manager.CurrentUnit != null && !manager.CurrentUnit.IsDead())
        {
            GridHighlighter.Instance.HighlightValidMoves(manager.CurrentUnit, manager);
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
