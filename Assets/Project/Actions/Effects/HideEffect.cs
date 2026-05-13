using UnityEngine;
using System.Collections.Generic;

public class HideEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        List<Unit> allUnits = resolver.GetAllUnits();
        List<Unit> enemies = allUnits.FindAll(u => u.isEnemy != ctx.attacker.isEnemy && !u.IsDead());

        DetectionStateManager.AttemptRestealth(ctx.attacker, enemies);
    }
}
