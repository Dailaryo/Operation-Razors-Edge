using UnityEngine;

public interface IActionEffect
{
    void Execute(ActionContext ctx, CombatResolver resolver);
}
