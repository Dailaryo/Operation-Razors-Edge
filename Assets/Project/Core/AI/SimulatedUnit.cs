using UnityEngine;

public class SimulatedUnit
{
    public Unit source;

    public int currentHP;

    public Vector2Int position;

    public bool isDead => currentHP <= 0;

    public SimulatedUnit(Unit unit)
    {
        source = unit;
        currentHP = unit.currentHP;
        position = unit.position;
    }
}
