using UnityEngine;

public class Tile : MonoBehaviour
{
    Renderer rend;

    Color baseColor;
    Color hoverColor = Color.yellow;

    //How strongly location highlights (move, range, threat, planned move, LoS) tint the terrain colour. 0 = terrain only, 1 = solid highlight
    [SerializeField, Range(0f, 1f)] float locationTint = 0.45f;

    TileHighlightType baseHighlight = TileHighlightType.None;
    TileHighlightType overlayHighlight = TileHighlightType.None;

    //Active field zone (Deploy Field): tints the terrain under every other highlight
    bool inFieldZone = false;
    [SerializeField] Color fieldZoneColor = new Color(0.3f, 0.3f, 0.75f);
    [SerializeField, Range(0f, 1f)] float fieldZoneTint = 0.5f;

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
        Threatened,
        PlannedMove,
        GroundTarget //ground-targeted action range (Deploy Field): tinted, selectable, units drawn on top
    }

    private Color terrainColor;

    public bool IsSelectable()
    {
        return baseHighlight == TileHighlightType.MoveValid
            || baseHighlight == TileHighlightType.ValidTarget
            || baseHighlight == TileHighlightType.GroundTarget;
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

        //Active field zones tint the ground; every location highlight blends over that
        Color ground = inFieldZone
            ? Color.Lerp(terrainColor, fieldZoneColor, fieldZoneTint)
            : terrainColor;

        if (finalType == TileHighlightType.None)
        {
            rend.material.color = ground;
            return;
        }

        Color highlight = GetHighlightColor(finalType);

        //Location highlights tint the ground so it stays readable underneath; unit and cursor highlights stay solid
        rend.material.color = IsLocationHighlight(finalType)
            ? Color.Lerp(ground, highlight, locationTint)
            : highlight;
    }

    public void SetFieldZone(bool inZone)
    {
        if (inFieldZone == inZone) return;
        inFieldZone = inZone;
        ApplyHighlight();
    }

    static bool IsLocationHighlight(TileHighlightType type)
    {
        switch (type)
        {
            case TileHighlightType.MoveValid:
            case TileHighlightType.InRange:
            case TileHighlightType.Threatened:
            case TileHighlightType.PlannedMove:
            case TileHighlightType.LoSBlocked:
            case TileHighlightType.GroundTarget:
                return true;

            default:
                return false;
        }
    }

    static Color GetHighlightColor(TileHighlightType type)
    {
        switch (type)
        {
            case TileHighlightType.MoveValid: return Color.cyan;
            case TileHighlightType.Hover: return Color.yellow;
            case TileHighlightType.Ally: return Color.green;
            case TileHighlightType.Enemy: return Color.red;
            case TileHighlightType.ValidTarget: return Color.magenta;
            case TileHighlightType.InRange: return Color.blue;
            case TileHighlightType.LoSBlocked: return Color.black;
            case TileHighlightType.Threatened: return Color.orange;
            case TileHighlightType.PlannedMove: return new Color(0.2f, 0.6f, 0.45f); //darker "ghost" green: where an ally will be
            case TileHighlightType.GroundTarget: return Color.magenta;
            default: return Color.white;
        }
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

        //Repaint with the current highlight (tinted highlights blend over the new terrain colour)
        ApplyHighlight();
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
