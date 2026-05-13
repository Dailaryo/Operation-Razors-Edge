using UnityEngine;

public class WeaponMountActionEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (!resolver.IsSimulation)
        {
            ctx.attacker.AddEffect(new WeaponMountEffect());
            CombatLog.Add($"{ctx.attacker.unitName} activates weapon mount.");
        }
    }
}