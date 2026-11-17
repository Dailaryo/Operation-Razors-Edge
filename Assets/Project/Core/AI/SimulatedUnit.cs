using UnityEngine;

public class SimulatedUnit
{
    public Unit source;

    public int currentHP;

    public Vector2Int position;

    public int moveSpeedBonus = 0; //Planning-only speed bonus from earlier planned actions (Burst on self)

    public bool isDead => source.removedFromEncounter || currentHP <= source.GetDeathThreshold(); //Matches Unit.IsDead(): Disabled/Dying units are still on the board

    public SimulatedUnit(Unit unit)
    {
        source = unit;
        currentHP = unit.currentHP;
        position = unit.position;
    }
}
