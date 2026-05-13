using UnityEngine;

public class RemoteInterfaceEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation) return;

        int result = SkillResolver.GetSkillTotal(ctx.attacker, SkillType.ComputerUse);
        int roll = UnityEngine.Random.Range(1, 21);
        int total = result + roll;

        CombatLog.Add($"{ctx.attacker.unitName}: Remote Interface — ComputerUse {total}.");
        // Device interaction resolved by mission scripting when device system is implemented
    }
}