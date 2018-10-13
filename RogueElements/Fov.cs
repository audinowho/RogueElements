using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements
{
    public static class Fov
    {
        //  \3|4/
        //  2\|/5
        //  --+--
        //  1/|\6
        //  /0|7\

        //xx, xy, yx, yy
        private static int[,] OctantTransform = {
            { 0, -1,  1,  0 },   // 0 S-SW
            {-1,  0,  0,  1 },   // 1 SW-W
            {-1,  0,  0, -1 },   // 2 W-NW
            { 0, -1, -1,  0 },   // 3 NW-N
            { 0,  1, -1,  0 },   // 4 N-NE
            { 1,  0,  0, -1 },   // 5 NE-E
            { 1,  0,  0,  1 },   // 6 E-SE
            { 0,  1,  1,  0 },   // 7 SE-S
        };

        public static bool IsCardinalPathBlocked(Loc start, Loc diff, Grid.LocTest checkBlock)
        {
            int sgn_x = Math.Sign(diff.X);
            int sgn_y = Math.Sign(diff.Y);

            int dx = Math.Abs(diff.X);
            int dy = Math.Abs(diff.Y);

            if (dx > 0)
            {
                for (int x = 1; x < dx; x++)
                {
                    if (checkBlock(start + new Loc(x * sgn_x, 0)))
                        return true;
                }
            }
            else
            {
                for (int y = 1; y < dy; y++)
                {
                    if (checkBlock(start + new Loc(0, y * sgn_y)))
                        return true;
                }
            }

            return false;
        }

        //if you factor this number, you get
        //four 2's, two 3's, a 5, 7, 11, 13, and 17
        const int SLOPE_GRANULARITY = 12252240;

        public static bool IsPathClear(Loc start, int startCol, int endCol, int endRow, int xx, int xy, int yx, int yy, int startSlope, int endSlope, Grid.LocTest checkBlock)
        {
            bool prevBlocked = false;

            int savedRightSlope = -SLOPE_GRANULARITY;

            for (int currentCol = startCol; currentCol <= endCol; currentCol++)
            {
                int xc = currentCol;

                for (int yc = currentCol; yc >= 0; yc--)
                {
                    int gridX = start.X + xc * xx + yc * xy;
                    int gridY = start.Y + xc * yx + yc * yy;


                    int leftBlockSlope = (yc * SLOPE_GRANULARITY + SLOPE_GRANULARITY / 2) * 2 / (xc * 2 - 1);
                    int rightBlockSlope = (yc * SLOPE_GRANULARITY - SLOPE_GRANULARITY / 2) * 2 / (xc * 2 + 1);

                    if (rightBlockSlope > startSlope) // Block is above the left edge of our view area; skip.
                        continue;
                    else if (leftBlockSlope < endSlope) // Block is below the right edge of our view area; we're done.
                        break;

                    if (currentCol == endCol && yc == endRow)
                        return true;

                    bool curBlocked = checkBlock(new Loc(gridX, gridY));

                    if (prevBlocked)
                    {
                        if (curBlocked)
                            savedRightSlope = rightBlockSlope;
                        else
                        {
                            prevBlocked = false;
                            startSlope = savedRightSlope;
                        }
                    }
                    else
                    {
                        if (curBlocked)
                        {

                            if (leftBlockSlope <= startSlope)
                            {
                                if (IsPathClear(start, currentCol + 1, endCol, endRow, xx, xy, yx, yy, startSlope, leftBlockSlope, checkBlock))
                                    return true;
                            }

                            prevBlocked = true;
                            savedRightSlope = rightBlockSlope;
                        }
                    }
                }

                if (prevBlocked)
                    break;
            }
            return false;
        }


        public static bool IsInFOV(Loc start, Loc end, Grid.LocTest checkBlock)
        {
            Loc diff = end - start;
            int dx = Math.Abs(diff.X);
            int dy = Math.Abs(diff.Y);

            if (dx <= 1 && dy <= 1)
                return true;

            if (start.Y == end.Y || start.X == end.X)
                return !IsCardinalPathBlocked(start, diff, checkBlock);
            else
            {
                //signs
                int sgn_x = Math.Sign(diff.X);
                int sgn_y = Math.Sign(diff.Y);
                //xy swiz
                
                if (dy > dx)
                    return IsPathClear(start, 1, dy, dx, 0, sgn_x, sgn_y, 0,
                        (dx * SLOPE_GRANULARITY + SLOPE_GRANULARITY / 2) * 2 / (dy * 2 - 1),
                        (dx * SLOPE_GRANULARITY - SLOPE_GRANULARITY / 2) * 2 / (dy * 2 + 1), checkBlock);
                else
                    return IsPathClear(start, 1, dx, dy, sgn_x, 0, 0, sgn_y,
                        (dy * SLOPE_GRANULARITY + SLOPE_GRANULARITY / 2) * 2 / (dx * 2 - 1),
                        (dy * SLOPE_GRANULARITY - SLOPE_GRANULARITY / 2) * 2 / (dx * 2 + 1), checkBlock);
            }
        }



        public delegate void LightOperation(int locX, int locY, float lighting);

        public static void CalculateAnalogFOV(Loc rectStart, Loc rectSize, Loc start, Grid.LocTest checkBlock, LightOperation lightOp)
        {
            // Viewer's cell is always visible.
            if (Collision.InBounds(rectStart, rectSize, start))
                lightOp(start.X, start.Y, 1f);

            for (int dir = 0; dir < 8; dir++)
                CastPartialLight(rectStart, rectSize, start, checkBlock, lightOp, 1, 3 * SLOPE_GRANULARITY, -SLOPE_GRANULARITY, (Dir8)dir);
        }

        private static void CastPartialLight(Loc rectStart, Loc rectSize, Loc start, Grid.LocTest checkBlock, LightOperation lightOp, int startColumn, int startSlope, int endSlope, Dir8 dir)
        {
            // Set true if the previous cell we encountered was blocked.
            bool prevBlocked = false;

            int savedRightSlope = -SLOPE_GRANULARITY;

            int colVal = OctantTransform[(int)dir, 0] != 0 ? OctantTransform[(int)dir, 0] : OctantTransform[(int)dir, 2];
            
            Loc colDiff = new Loc();
            if (colVal > 0)
                colDiff = rectStart + rectSize - start - new Loc(1);
            else
                colDiff = start - rectStart;

            int maxCol = OctantTransform[(int)dir, 0] != 0 ? colDiff.X : colDiff.Y;

            int rowVal = OctantTransform[(int)dir, 3] != 0 ? OctantTransform[(int)dir, 3] : OctantTransform[(int)dir, 1];
            Loc rowDiff = new Loc();
            if (rowVal > 0)
                rowDiff = rectStart + rectSize - start - new Loc(1);
            else
                rowDiff = start - rectStart;
            int maxRow = OctantTransform[(int)dir, 3] != 0 ? rowDiff.Y : rowDiff.X;

            for (int currentCol = startColumn; currentCol <= maxCol; currentCol++)
            {
                int xc = currentCol;

                for (int yc = Math.Min(maxRow, currentCol); yc >= 0; yc--)
                {
                    int gridX = start.X + xc * OctantTransform[(int)dir, 0] + yc * OctantTransform[(int)dir, 1];
                    int gridY = start.Y + xc * OctantTransform[(int)dir, 2] + yc * OctantTransform[(int)dir, 3];

                    //due to safeguards, this block should never be hit
                    if (!Collision.InBounds(rectStart, rectSize, new Loc(gridX, gridY)))
                        continue;

                    int leftBlockSlope = (yc * SLOPE_GRANULARITY + SLOPE_GRANULARITY / 2) * 2 / (xc * 2 - 1);
                    int rightBlockSlope = (yc * SLOPE_GRANULARITY - SLOPE_GRANULARITY / 2) * 2 / (xc * 2 + 1);

                    if (rightBlockSlope > startSlope) // Block is above the left edge of our view area; skip.
                        continue;
                    else if (leftBlockSlope < endSlope) // Block is below the right edge of our view area; we're done.
                        break;

                    float lighting = 1f;
                    if (leftBlockSlope > startSlope) // Block is above the left edge of our view area; skip.
                        lighting = 0.5f;
                    else if (rightBlockSlope < endSlope) // Block is below the right edge of our view area; we're done.
                        lighting = 0.5f;

                    if (((int)dir % 2 == 0 || yc != 0) && ((int)dir % 2 == 1 || yc != currentCol))
                        lightOp(gridX, gridY, lighting);

                    bool curBlocked = checkBlock(new Loc(gridX, gridY));

                    if (prevBlocked)
                    {
                        if (curBlocked)
                            savedRightSlope = rightBlockSlope;
                        else
                        {
                            prevBlocked = false;
                            startSlope = savedRightSlope;
                        }
                    }
                    else
                    {
                        if (curBlocked)
                        {
                            if (leftBlockSlope <= startSlope)
                                CastPartialLight(rectStart, rectSize, start, checkBlock, lightOp, currentCol + 1, startSlope, leftBlockSlope, dir);

                            prevBlocked = true;
                            savedRightSlope = rightBlockSlope;
                        }
                    }
                }

                if (prevBlocked)
                    break;
            }

        }


    }
}
