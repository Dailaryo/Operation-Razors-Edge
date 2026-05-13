using UnityEngine;

public class LoSResolver
{
    //Public entry point.  Returns true if 'from' has line of sight to 'to'.
    //LoS is asymmetric: corner-peek applies only to the seer.
    //Target is always measured from tile center.
    public static bool HasLoS(Vector2Int from, Vector2Int to, GridManager grid)
    {
        if (from == to) return true;

        Vector2 origin = ComputeOrigin(from, to, grid);
        Vector2 targetCenter = TileCenter(to);

        //Near face: the side of the target tile closest to the seer
        Vector2 nearFaceStart, nearFaceEnd;
        GetNearFace(to, from, out nearFaceStart, out nearFaceEnd);

        //Check every edge in the grid for occlusion
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                TileData tile = grid.GetTileData(pos);
                if (tile== null) continue;

                //North edge of tile (x, y): runs from (x, y+1) to (x+1, y+1)
                if (tile.northEdge != null && tile.northEdge.BlocksLoS)
                {
                    Vector2 edgeStart = new Vector2(x, y + 1);
                    Vector2 edgeEnd = new Vector2(x + 1, y + 1);

                    if (ComputeShadowFraction(origin, edgeStart, edgeEnd,
                        nearFaceStart, nearFaceEnd) > 0.333f)
                        return false;
                }

                //East edge of tile (x, y): runs from (x+1, y) to (x+1, y+1)
                if (tile.eastEdge != null && tile.eastEdge.BlocksLoS)
                {
                    Vector2 edgeStart = new Vector2(x + 1, y);
                    Vector2 edgeEnd = new Vector2(x + 1, y + 1);

                    if (ComputeShadowFraction(origin, edgeStart, edgeEnd,
                        nearFaceStart, nearFaceEnd) > 0.333f)
                        return false;
                }

                //Impassable tiles block LoS at the tile level
                if (tile.terrain == TileData.TerrainType.Impassable)
                {
                    //Treat all four edges of an impassable tile as blocking
                    //Only check edges that could plausibly lie between seer and target

                    //South edge: from (x, y) to (x+1, y)
                    if (ComputeShadowFraction(origin,
                        new Vector2(x, y), new Vector2(x + 1, y),
                        nearFaceStart, nearFaceEnd) > 0.333f)
                        return false;

                    //North edge (Same as above northEdge)
                    if (ComputeShadowFraction(origin,
                        new Vector2(x, y + 1), new Vector2(x + 1, y + 1),
                        nearFaceStart, nearFaceEnd) > 0.333f)
                        return false;

                    //West edge from (x, y) to (x, y+1)
                    if (ComputeShadowFraction(origin,
                        new Vector2(x, y), new Vector2(x, y + 1),
                        nearFaceStart, nearFaceEnd) > 0.333f)
                        return false;

                    //East edge (Same as above eastEdge)
                    if (ComputeShadowFraction(origin,
                        new Vector2(x + 1, y), new Vector2(x + 1, y + 1),
                        nearFaceStart, nearFaceEnd) > 0.333f)
                        return false;
                }
            }
        }

        return true;
    }

    // Determines the ray origin using the corner-peek rule.
    // Default: tile center.
    // Exception: if seer has a blocking edge on exactly one cardinal side
    // and the neighbor in that direction has no blocking edge facing back,
    // origin shifts to the nearest corner of the seer's tile toward that gap.
    private static Vector2 ComputeOrigin(Vector2Int seerPos, Vector2Int targetPos, GridManager grid)
    {
        Vector2Int[] cardinals = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        int peekCount = 0;
        Vector2Int peekDirection = Vector2Int.zero;

        foreach (var dir in cardinals)
        {
            Vector2Int neighbor = seerPos + dir;

            //Does seer tile have a blocking edge on this side?
            EdgeData seerEdge = grid.GetEdge(seerPos, neighbor);
            if (seerEdge == null || !seerEdge.BlocksLoS) continue;

            //Does neighbor tile have NO blocking edge facing back?
            EdgeData neighborEdge = grid.GetEdge(neighbor, seerPos);
            if (neighborEdge != null && neighborEdge.BlocksLoS) continue;

            //Neighbor is out of bounds: no peek opportunity
            if (!grid.IsWithinBounds(neighbor)) continue;

            peekCount++;
            peekDirection = dir;
        }

        if (peekCount != 1)
        {
            //Zero or two+ gaps: use tile center
            return TileCenter(seerPos);
        }

        //Exactly one gap: shift to nearest corner on opposite side of gap
        // - Furthest from the gap along the gap's axis
        // - Closest to the target along the perpendicular axis
        Vector2 center = TileCenter(seerPos);
        Vector2 target = TileCenter(targetPos);

        //The corner is the one furthest from the target along the perpendicular axis
        if (peekDirection == Vector2Int.up || peekDirection == Vector2Int.down)
        {
            //Gap is north or south: shift in X toward from target
            float cornerX = target.x < center.x
                ? seerPos.x //west corner
                : seerPos.x + 1f; //east corner

            float cornerY = peekDirection == Vector2Int.up
                ? seerPos.y //south side
                : seerPos.y + 1f; //north side

            return new Vector2(cornerX, cornerY);
        }
        else
        {
            // Gap is east or west — shift in Y toward target
            float cornerY = target.y < center.y
                ? seerPos.y // south corner
                : seerPos.y + 1f; // north corner

            float cornerX = peekDirection == Vector2Int.right
                ? seerPos.x // west side
                : seerPos.x + 1f; // east side

            return new Vector2(cornerX, cornerY);
        }
    }

    //Returns the near face of the target tile: the side closest to the seer.
    //The near face is a line segment of 1.0 in world units.
    private static void GetNearFace(Vector2Int targetPos, Vector2Int seerPos,
        out Vector2 start, out Vector2 end)
    {
        Vector2 diff = seerPos - targetPos;

        //Dominant axis determines which face is closest
        if (Mathf.Abs(diff.x) >= Mathf.Abs(diff.y))
        {
            if (diff.x > 0)
            {
                //Seer is to the east: near face is the east edge of target
                start = new Vector2(targetPos.x + 1, targetPos.y);
                end = new Vector2(targetPos.x + 1, targetPos.y + 1);
            }
            else
            {
                //Seer is to the west: near face is the west edge of target
                start = new Vector2(targetPos.x, targetPos.y);
                end = new Vector2(targetPos.x, targetPos.y + 1);
            }
        }
        else
        {
            if (diff.y > 0)
            {
                // Seer is to the north: near face is the north edge of target
                start = new Vector2(targetPos.x, targetPos.y + 1);
                end = new Vector2(targetPos.x + 1, targetPos.y + 1);
            }
            else
            {
                // Seer is to the south: near face is the south edge of target
                start = new Vector2(targetPos.x, targetPos.y);
                end = new Vector2(targetPos.x + 1, targetPos.y);
            }
        }
    }

    // Returns the fraction of the target's near face shadowed by a blocking edge,
    // as seen from the ray origin.
    // Projects the blocking edge endpoints through the origin onto the near face line
    // and measures what length of that face falls in shadow.
    // Returns 0 if the edge casts no shadow on the near face.
    private static float ComputeShadowFraction(
        Vector2 origin,
        Vector2 edgeStart, Vector2 edgeEnd,
        Vector2 faceStart, Vector2 faceEnd)
    {
        //Project edgeStart through origin onto the near face line
        float t0 = ProjectOntoFace(origin, edgeStart, faceStart, faceEnd);
        //Project edgeEnd through origin onto the near face line
        float t1 = ProjectOntoFace(origin, edgeEnd, faceStart, faceEnd);

        if (!t0.HasValue() || !t1.HasValue()) return 0f;

        float shadowMin = Mathf.Min(t0, t1);
        float shadowMax = Mathf.Max(t0, t1);

        //Clamp to [0, 1]: only the portion on the near face counts
        shadowMin = Mathf.Clamp01(shadowMin);
        shadowMax = Mathf.Clamp01(shadowMax);

        return shadowMax - shadowMin;
    }

    // Projects a point through the origin onto the near face line.
    // Returns the parametric position along the face (0 = faceStart, 1 = faceEnd).
    // Returns null equivalent (float.NaN) if the projection is behind the origin
    // or parallel to the face.
    private static float ProjectOntoFace(
        Vector2 origin, Vector2 point,
        Vector2 faceStart, Vector2 faceEnd)
    {
        //Direction from origin through the blocking edge point
        Vector2 rayDir = point - origin;

        //Face direction and normal
        Vector2 faceDir = faceEnd - faceStart;
        float faceLen = faceDir.magnitude;
        if (faceLen < 0.0001f) return float.NaN;

        Vector2 faceDirNorm = faceDir / faceLen;

        //Solve: origin + t * rayDir = faceStart + s * faceDir
        //Using 2D line intersection
        float denom = rayDir.x * faceDir.y - rayDir.y * faceDir.x;

        if (Mathf.Abs(denom) < 0.0001f) return float.NaN; //parallel

        Vector2 delta = faceStart - origin;
        float t = (delta.x * faceDir.y - delta.y * faceDir.x) / denom;

        if (t < 0f) return float.NaN; //projection is behind the origin

        float s = (delta.x * rayDir.y - delta.y * rayDir.x) / denom;

        //s is the parametric position along the full face line
        //Normalize to [0,1] relative to faceStart->faceEnd
        return s / faceLen;
    }

    //Returns the center of a tile in world space.
    // Tile (x, y) occupies the unit square from (x, y) to (x+1, y+1).
    private static Vector2 TileCenter(Vector2Int pos)
    {
        return new Vector2(pos.x + 0.5f, pos.y + 0.5f);
    }
}

//Extension method to make float.NaN checks readable inline
public static class FloatExtensions
{
    public static bool HasValue(this float f)
    {
        return !float.IsNaN(f);
    }
}