using UnityEngine;

public abstract class BaseAction
{
    public abstract void Execute(ActionContext ctx, CombatResolver resolver);
}
