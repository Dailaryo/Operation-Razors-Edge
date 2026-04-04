using UnityEngine;

public static class ActionFactory
{
    public static BaseAction GetAction(ActionType type)
    {
        switch (type)
        {
            case ActionType.Attack: return new AttackAction();
            case ActionType.FocusShot: return new FocusShotAction();
            case ActionType.Defend: return new DefendAction();
            case ActionType.TacticalBoost: return new TacticalBoostAction();
            case ActionType.Taunt: return new TauntAction();
            case ActionType.Move: return new MoveAction();
            default: return new AttackAction();
        }
    }
}
