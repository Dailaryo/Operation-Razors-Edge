using UnityEngine;

public class DamageEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        ctx.shouldApplyDamage = true;
    }
}
