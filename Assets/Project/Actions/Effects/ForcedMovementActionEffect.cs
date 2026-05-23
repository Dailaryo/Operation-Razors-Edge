using UnityEngine;

// Sets forced movement fields on ActionContext so CombatResolver.ExecuteAction()
// can call ResolveForcedMove() after other effects resolve.
// direction: normalized Vector2Int (e.g. Vector2Int.right for push east)
// magnitude: number of tiles to displace
public class ForcedMovementActionEffect : IActionEffect
{
    private Unit target;
    private Vector2Int direction;
    private int magnitude;

    public ForcedMovementActionEffect(Unit target, Vector2Int direction, int magnitude)
    {
        this.target = target;
        this.direction = direction;
        this.magnitude = magnitude;
    }

    public void Execute(ActionContext ctx, CombatResolver resolver)
    {
        ctx.isForcedMovement = true;
        ctx.forcedMoveTarget = target;
        ctx.forcedMoveDelta = direction * magnitude;
    }
}
