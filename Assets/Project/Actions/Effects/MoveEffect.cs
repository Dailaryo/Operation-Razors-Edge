using UnityEngine;

public class MoveEffect : IActionEffect
{
    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        ctx.isMovement = true;
        ctx.movePosition = ctx.action.movePosition;
    }
}
