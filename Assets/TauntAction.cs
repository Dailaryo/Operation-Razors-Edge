using UnityEngine;

public class TauntAction : BaseAction
{
    public override void Execute(CombatAction action, CombatManager manager)
    {
        var enemies = manager.GetAllUnits().FindAll(u => u.isEnemy != action.source.isEnemy);

        foreach (Unit unit in enemies)
        {
            if (!unit.IsDead())
                {
                unit.AddEffect(new TauntEffect(action.source, 2));
            }
        }

        action.source.SetCooldown(ActionType.Taunt, 2);

        CombatLog.Add(action.source.unitName + " uses Taunt!");
    }
}
