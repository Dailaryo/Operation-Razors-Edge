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
        InRange,
        LoSBlocked,
        Threatened
    }

    private Color terrainColor;

    public bool IsSelectable()
    {
        return baseHighlight == TileHighlightType.MoveValid
            || baseHighlight == TileHighlightType.ValidTarget;
    }

    void Awake()
    {
        rend = GetComponent<Renderer>();
        baseColor = rend.material.color;
        terrainColor = baseColor;
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
                rend.material.color = terrainColor;
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

            case TileHighlightType.LoSBlocked:
                rend.material.color = Color.black;
                break;

            case TileHighlightType.Threatened:
                rend.material.color = Color.orangeRed;
                break;
        }

        ////Debug.Log($"Tile {gridPosition} | Base: {baseHighlight} | Overlay: {overlayHighlight}");
    }

    public void SetTerrainColor(TileData.TerrainType terrain)
    {
        switch (terrain)
        {
            case TileData.TerrainType.Open:
                terrainColor = Color.tan;
                break;
            case TileData.TerrainType.Difficult:
                terrainColor = Color.darkGreen;
                break;
            case TileData.TerrainType.HighGround:
                terrainColor = Color.brown;
                break;
            case TileData.TerrainType.Impassable:
                terrainColor = Color.grey;
                break;
            default:
                terrainColor = baseColor;
                break;
        }

        //Only apply immediately if no highlight is active
        if (baseHighlight == TileHighlightType.None && overlayHighlight == TileHighlightType.None)
            rend.material.color = terrainColor;
    }

    public TileHighlightType GetBaseHighlight()
    {
        return baseHighlight;
    }

    public TileHighlightType GetOverlayHighlight()
    {
        return overlayHighlight;
    }
}
