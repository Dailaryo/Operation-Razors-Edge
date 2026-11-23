using UnityEngine;
using System.Collections.Generic;

public class HideEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        //The Stealth roll changes real detection states: resolve it once, in the execution phase
        if (resolver.IsSimulation) return;

        List<Unit> allUnits = resolver.GetAllUnits();
        List<Unit> enemies = allUnits.FindAll(u => u.isEnemy != ctx.attacker.isEnemy && !u.IsDead());

        DetectionStateManager.AttemptRestealth(ctx.attacker, enemies);
    }
}
