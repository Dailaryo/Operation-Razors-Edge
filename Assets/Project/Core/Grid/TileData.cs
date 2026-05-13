using UnityEngine;

public class TileData
{
    public enum TerrainType
    {
        Open,
        Difficult,
        HighGround,
        Impassable
    }

    public TerrainType terrain;

    public int floorLayer;

    public Unit occupant;

    public EdgeData northEdge;
    public EdgeData eastEdge;

    public int MoveCost
    {
        get
        {
            switch (terrain)
            {
                case TerrainType.Open:
                case TerrainType.HighGround:
                    return 1;

                case TerrainType.Difficult:
                    return 2;

                case TerrainType.Impassable:
                    return int.MaxValue;

                default:
                    return 1;
            }
        }
    }
}
