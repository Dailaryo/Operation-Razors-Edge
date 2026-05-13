using UnityEngine;

public class SimulatedUnit
{
    public Unit source;

    public int currentHP;

    public Vector2Int position;

    public bool isDead => currentHP <= 0; //Any log message referencing a simulated unit will need to refer back to the source

    public SimulatedUnit(Unit unit)
    {
        source = unit;
        currentHP = unit.currentHP;
        position = unit.position;
    }
}
