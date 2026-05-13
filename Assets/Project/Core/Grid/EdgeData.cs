using UnityEngine;

public class EdgeData
{
    public enum EdgeType
    {
        None,
        Wall,
        Door,
        Window
    }

    public EdgeType type;

    public bool doorIsOpen;
    public bool doorIsLocked;
    public bool windowIsBroken;

    public bool BlocksLoS
    {
        get
        {
            switch (type)
            {
                case EdgeType.Wall:
                    return true;

                case EdgeType.Door:
                    return !doorIsOpen;

                case EdgeType.Window:
                    return true;

                default:
                    return false;
            }
        }
    }

    public bool BlocksMovement
    {
        get
        {
            switch (type)
            {
                case EdgeType.Wall:
                    return true;

                case EdgeType.Door:
                    return !doorIsOpen;

                case EdgeType.Window:
                    return !windowIsBroken;

                default:
                    return false;
            }
        }
    }
}
