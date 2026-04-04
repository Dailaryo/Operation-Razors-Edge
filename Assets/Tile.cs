using UnityEngine;

public class Tile : MonoBehaviour
{
    Renderer rend;

    Color baseColor;
    Color hoverColor = Color.yellow;

    public Vector2Int gridPosition;

    public enum TileHighlightType
    {
        None, 
        MoveValid, 
        Hover, 
        Ally, 
        Enemy
    }

    void Awake()
    {
        rend = GetComponent<Renderer>();
        baseColor = rend.material.color;
    }

    public void SetHighlight(TileHighlightType type)
    {
        switch (type)
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
        }
    }
}
