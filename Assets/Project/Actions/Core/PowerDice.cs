using UnityEngine;

//Variable dice for power effects. Maximize Power: every die rolls its maximum
public static class PowerDice
{
    public static int Roll(ActionContext ctx, int count, int sides)
    {
        return Roll(ctx != null && ctx.isMaximized, count, sides);
    }

    public static int Roll(bool maximized, int count, int sides)
    {
        if (maximized) return count * sides;

        int total = 0;
        for (int i = 0; i < count; i++)
            total += Random.Range(1, sides + 1);

        return total;
    }
}