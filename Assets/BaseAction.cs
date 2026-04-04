using UnityEngine;

public abstract class BaseAction
{
    public abstract void Execute(CombatAction action, CombatManager manager);
}
