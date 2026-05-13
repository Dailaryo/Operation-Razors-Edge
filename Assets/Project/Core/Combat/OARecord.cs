using UnityEngine;

public struct OARecord
{
    public Unit attacker;
    public Unit target;
    public Vector2Int fromTile;
    public int priority;

    public OARecord(Unit attacker, Unit target, Vector2Int fromTile)
    {
        this.attacker = attacker;
        this.target = target;
        this.fromTile = fromTile;
        this.priority = attacker.initiativeRoll;
    }
}
