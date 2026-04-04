using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    Attack,
    Defend,
    TacticalBoost,
    Taunt,
    FocusShot,
    Move
}

public class CombatAction
{
    public Unit source;
    public Unit target;

    public ActionType actionType;

    CombatManager manager;

    public int contextBonusDamage = 0;

    public List<StatusEffect> temporaryEffects = new List<StatusEffect>();

    public Vector2Int targetPosition;
    public Vector2Int? movePosition;

    public CombatAction(Unit sourceUnit, Unit targetUnit, ActionType type, List<Unit> allyList, List<Unit> enemyList, CombatManager managerRef)
    {
        source = sourceUnit;
        target = targetUnit;
        actionType = type;
        manager = managerRef;
    }

    public void Execute()
    {
        if (!ActionValidator.IsValid(this))
        {
            CombatLog.Add($"{source.unitName} failed to execute {actionType} (invalid target)");
            return;
        }
        
        BaseAction action = ActionFactory.GetAction(actionType);
        action.Execute(this, manager);
    }

    public void SetMovePosition(Vector2Int pos)
    {
        movePosition = pos;
    }
}
