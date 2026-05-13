using UnityEngine;

public static class DistanceUtility
{
    //Max of absolute dx and dy.
    //Used only for fast pre-filter culling and adjacency checks.
    //Never used as the final range check
    public static int Chebyshev(Vector2Int a, Vector2Int b)
    {
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
    }

    //Returns true if two tiles are cardinal or diagonal neighbors
    public static bool IsAdjacent(Vector2Int a, Vector2Int b)
    {
        return Chebyshev(a, b) == 1;
    }

    //Returns the movement cost of a single step from 'from' to 'to'
    //using the alternating 1/2 diagonal rule.
    //'nextDiagonalCheap tracks parity and is mutated by this call.
    //Cardinal steps always cost 1 and do not mutate parity.
    public static int DiagonalMoveCost(Vector2Int from, Vector2Int to, ref bool nextDiagonalIsCheap)
    {
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);

        bool isDiagonal = dx == 1 && dy == 1;

        if (!isDiagonal)
        {
            return 1;
        }

        if (nextDiagonalIsCheap)
        {
            nextDiagonalIsCheap = false;
            return 1;
        }
        else
        {
            nextDiagonalIsCheap = true;
            return 2;
        }
    }

    //Returns the total diagonal-adjusted movement cost of traveling
    //in a straight line between two tiles.
    //This is the authoritative measurement for range and AoE checks.
    //Produces cicular-ish footprints rather than squares.
    public static int MovementCostDistance(Vector2Int from, Vector2Int to)
    {
        int dx = Mathf.Abs(to.x - from.x);
        int dy = Mathf.Abs(to.y - from.y);

        int diagonalSteps = Mathf.Min(dx, dy);
        int cardinalSteps = Mathf.Abs(dx - dy);

        //Diagonal steps alternate 1/2/1/2...
        //Each pair of diagonal steps costs 3 total (1+2).
        //An odd leftover cheap step costs 1.
        int diagonalCost = (diagonalSteps / 2) * 3 + (diagonalSteps % 2) * 1;

        return diagonalCost + cardinalSteps;
    }
}
