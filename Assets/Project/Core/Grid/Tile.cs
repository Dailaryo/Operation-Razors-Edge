using UnityEngine;

public class Tile : MonoBehaviour
{
    Renderer rend;

    Color baseColor;
    Color hoverColor = Color.yellow;

    TileHighlightType baseHighlight = TileHighlightType.None;
    TileHighlightType overlayHighlight = TileHighlightType.None;

    public Vector2Int gridPosition;

    public enum TileHighlightType
    {
        None, 
        MoveValid, 
        Hover, 
        Ally, 
        Enemy,
        ValidTarget,
        InRange
    }

    public bool IsSelectable()
    {
        return baseHighlight == TileHighlightType.MoveValid
            || baseHighlight == TileHighlightType.ValidTarget;
    }

    void Awake()
    {
        rend = GetComponent<Renderer>();
        baseColor = rend.material.color;
    }

    public void SetHighlight(TileHighlightType type)
    {
        //For backward compatibility, treat this as base highlight
        baseHighlight = type;
        ApplyHighlight();
    }

    public void SetBaseHighlight(TileHighlightType type)
    {
        baseHighlight = type;
        ApplyHighlight();
    }

    public void SetOverlayHighlight(TileHighlightType type)
    {
        overlayHighlight = type;
        ApplyHighlight();
    }

    void ApplyHighlight()
    {
        TileHighlightType finalType = overlayHighlight != TileHighlightType.None
            ? overlayHighlight
            : baseHighlight;

        switch (finalType)
        {
            case TileHighlightType.None:
                rend.material.color = baseColor;
                break;

            case TileHighlightType.MoveValid:
                rend.material.color = Color.cyan;
                break;

            case TileHighlightType.Hover:
                rend.material.color = Color.yellow;
                break;

            case TileHighlightType.Ally:
                rend.material.color = Color.green;
                break;

            case TileHighlightType.Enemy:
                rend.material.color = Color.red;
                break;

            case TileHighlightType.ValidTarget:
                rend.material.color = Color.magenta;
                break;

            case TileHighlightType.InRange:
                rend.material.color = Color.blue;
                break;
        }

        ////Debug.Log($"Tile {gridPosition} | Base: {baseHighlight} | Overlay: {overlayHighlight}");
    }
}
