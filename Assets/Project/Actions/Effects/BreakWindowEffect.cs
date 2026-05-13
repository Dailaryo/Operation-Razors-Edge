using UnityEngine;

public class BreakWindowEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        ctx.isWindowBreak = true;
    }
}
