using UnityEngine;

public class TacticalBoostAction : BaseAction
{
    public override void Execute(CombatAction action, CombatManager manager)
    {
        var allies = manager.GetAllUnits().FindAll(u => u.isEnemy == action.source.isEnemy);

        foreach (Unit unit in allies)
        {
            if (!unit.IsDead())
            {
                unit.AddEffect(new TacticalBoostEffect(5, 1));
            }
        }

        action.source.SetCooldown(ActionType.TacticalBoost, 2);

        CombatLog.Add(action.source.unitName + " uses Tactical Boost");
    }
}
