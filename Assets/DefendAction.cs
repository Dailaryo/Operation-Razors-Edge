using UnityEngine;

public class DefendAction : BaseAction
{
    public override void Execute(CombatAction action, CombatManager manager)
    {
        action.source.AddEffect(new DefenseBuffEffect(0.5f, 1));
        CombatLog.Add(action.source.unitName + " is Defending");
    }
}
