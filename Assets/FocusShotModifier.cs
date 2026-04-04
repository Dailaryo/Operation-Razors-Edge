using UnityEngine;

public class FocusShotModifier : StatusEffect
{
    int bonus;

    public FocusShotModifier (int amount) : base("Focus Shot Bonus", 0)
    {
        bonus = amount;
        Priority = 60;
    }

    public override void OnBeforeAction(ActionContext ctx)
    {
        if (ctx.actionType == ActionType.FocusShot && ctx.attacker == Target)
        {
            ctx.modifiedDamage += bonus;
            CombatLog.Add($"{Target.unitName} gains +{bonus} damage from Focus Shot");
        }
    }
}
