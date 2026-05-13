using UnityEngine;

public struct LastKnownData
{
    public Vector2Int position;
    public int turnRecorded;

    public LastKnownData(Vector2Int pos, int turn)
    {
        position = pos;
        turnRecorded = turn;
    }
}
