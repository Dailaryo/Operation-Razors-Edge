using UnityEngine;

//Last entry in a placeholder's effects list when its targeting and saves are authored but its effect isn't yet
//(Mass Cloud Mind). Powers and actions with an empty effects list get the same line from CombatResolver
public class DeferredEffectNotice : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        if (resolver.IsSimulation) return;
        CombatLog.Add($"{ctx.definition.DisplayName}: effect not implemented yet.");
    }
}