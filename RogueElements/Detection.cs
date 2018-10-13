using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements
{
    public static class Detection
    {
        public static BlobMap DetectBlobs(bool[][] grid)
        {
            int width = grid.Length;
            int height = grid[0].Length;
            BlobMap blobMap = new BlobMap(width, height);

            for (int xx = 0; xx < width; xx++)
            {
                for (int yy = 0; yy < height; yy++)
                {
                    if (grid[xx][yy] && blobMap.Map[xx][yy] == 0)
                    {
                        blobMap.Blobs.Add(new Rect(xx, yy, 1, 1));
                        blobMap.Sizes.Add(0);

                        //fill the area, keeping track of the total area and blob bounds
                        Grid.FloodFill(new Rect(0, 0, width, height),
                        (Loc testLoc) =>
                        {
                            return (!grid[testLoc.X][testLoc.Y] || blobMap.Map[testLoc.X][testLoc.Y] != 0);
                        },
                        (Loc testLoc) =>
                        {
                            return true;
                        },
                        (Loc fillLoc) =>
                        {
                            blobMap.Map[fillLoc.X][fillLoc.Y] = blobMap.Blobs.Count;
                            blobMap.Sizes[blobMap.Blobs.Count - 1]++;
                            blobMap.Blobs[blobMap.Blobs.Count - 1] = Rect.IncludeLoc(blobMap.Blobs[blobMap.Blobs.Count - 1], fillLoc);
                        },
                        new Loc(xx, yy));
                    }
                }
            }

            return blobMap;
        }

        /// <summary>
        /// Returns a list of wall edges with definite 4-directional normals, connected to a start position
        /// </summary>
        /// <param name="start"></param>
        /// <param name="rect"></param>
        /// <param name="checkBlock">Determines if this is ground that can be burrowed into.</param>
        /// <param name="checkGround">Determines if this is ground that can reach a wall.</param>
        /// <returns></returns>
        public static List<LocRay4> DetectWalls(Loc start, Rect rect, Grid.LocTest checkBlock, Grid.LocTest checkGround)
        {
            bool[][] checkGrid = new bool[rect.Width][];
            bool[][] fillGrid = new bool[rect.Width][];
            for (int xx = 0; xx < rect.Width; xx++)
            {
                checkGrid[xx] = new bool[rect.Height];
                fillGrid[xx] = new bool[rect.Height];
            }

            List<LocRay4> walls = new List<LocRay4>();

            //scan and find solely-facing walls
            //cache already-checked walls since flood fill occasionally checks twice
            Grid.FloodFill(rect, (Loc testLoc) =>
            {
                if (fillGrid[testLoc.X][testLoc.Y])
                    return true;
                if (!checkGrid[testLoc.X][testLoc.Y] && checkBlock(testLoc))
                {
                    LocRay4 ray = GetWallDir(testLoc, checkBlock, checkGround);
                    if (ray.Dir != Dir4.None)
                        walls.Add(ray);
                }
                checkGrid[testLoc.X][testLoc.Y] = true;
                return !checkGround(testLoc);

            },
            (Loc testLoc) => { return true; },
            (Loc fillLoc) => { fillGrid[fillLoc.X][fillLoc.Y] = true; }, start);

            return walls;
        }


        /// <summary>
        /// Returns a list of wall edges with definite 4-directional normals
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="checkBlock">Determines if this is ground that can be burrowed into.</param>
        /// <param name="checkGround">Determines if this is ground that can reach a wall.</param>
        /// <returns></returns>
        public static List<LocRay4> DetectWalls(Rect rect, Grid.LocTest checkBlock, Grid.LocTest checkGround)
        {
            List<LocRay4> walls = new List<LocRay4>();

            for (int xx = rect.X; xx < rect.Width; xx++)
            {
                for (int yy = rect.Y; yy < rect.Height; yy++)
                {
                    Loc testLoc = new Loc(xx, yy);
                    if (checkBlock(testLoc))
                    {
                        LocRay4 ray = GetWallDir(testLoc, checkBlock, checkGround);
                        if (ray.Dir != Dir4.None)
                            walls.Add(ray);
                    }
                }
            }

            return walls;
        }

        public static LocRay4 GetWallDir(Loc loc, Grid.LocTest checkBlock, Grid.LocTest checkGround)
        {
            //check the four directions
            Dir4 chosenDir = Dir4.None;
            //ensure that there is only one direction where it is unblocked
            for (int ii = 0; ii < 4; ii++)
            {
                Loc newLoc = loc + ((Dir4)ii).GetLoc();
                if (checkGround(newLoc))
                {
                    if (chosenDir != Dir4.None)
                        return new LocRay4(loc);
                    else
                        chosenDir = ((Dir4)ii).Reverse();
                }
                else if (!checkBlock(newLoc))//all four directions must be valid ground, or valid block
                    return new LocRay4(loc);
            }
            if (chosenDir == Dir4.None)
                return new LocRay4(loc);

            //then check to make sure that the left and right diagonal of this direction are also valid blocked
            Loc lLoc = loc + DirExt.AddAngles(chosenDir.ToDir8(), Dir8.DownLeft).GetLoc();
            if (!checkBlock(lLoc))
                return new LocRay4(loc);
            Loc rLoc = loc + DirExt.AddAngles(chosenDir.ToDir8(), Dir8.DownRight).GetLoc();
            if (!checkBlock(rLoc))
                return new LocRay4(loc);

            return new LocRay4(loc, chosenDir);
        }



        /// <summary>
        /// Gets the N largest rectangles in the grid that are not a subset of a larger rectangle.  Modifies the grid in-place.
        /// </summary>
        /// <param name="grid">2D array of 0's and 1's</param>
        /// <param name="amount">Max number of rectangles to return.</param>
        /// <returns></returns>
        public static List<Rect> DetectNLargestRects(bool[][] grid, int amount)
        {
            StablePriorityQueue<int, Rect> queue = new StablePriorityQueue<int, Rect>();

            foreach (Rect rect in FindAllRects(grid))
                queue.Enqueue(-rect.Width * rect.Height, rect);

            List<Rect> results = new List<Rect>();
            while (results.Count < amount && queue.Count > 0)
                results.Add(queue.Dequeue());
            return results;
        }
        

        public delegate void RectFunc(Rect rect);

        /// <summary>
        /// Gets all rectangles in the grid that are not a subset of a larger rectangle.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="action"></param>
        public static List<Rect> FindAllRects(bool[][] grid)
        {
            List<Rect> resultRects = new List<Rect>();
            //create a pre-process where each grid value actually indicates how many 1's are below it
            int[][] intGrid = new int[grid.Length][];
            for (int xx = 0; xx < grid.Length; xx++)
            {
                intGrid[xx] = new int[grid[0].Length];

                //first line (bottom)
                if (grid[xx][grid[0].Length - 1])
                    intGrid[xx][grid[0].Length - 1] = 1;
                //all lines above the first line
                for (int yy = grid[0].Length - 2; yy >= 0; yy--)
                {
                    if (grid[xx][yy])
                        intGrid[xx][yy] = intGrid[xx][yy + 1] + 1;
                }
            }

            //go through each row with the histogram approach; capacity is known so might as well specify it here
            Stack<int> prevShadow = new Stack<int>(intGrid.Length);
            for (int yy = 0; yy < intGrid[0].Length; yy++)
            {
                prevShadow.Clear();
                prevShadow.Push(-1);
                int prevZero = -1;
                for (int xx = 0; xx < intGrid.Length; xx++)
                {
                    //encountered a smaller number? compute rectangles with all stack values greater than the current number
                    int prevHistogram = prevShadow.Peek() > -1 ? intGrid[prevShadow.Peek()][yy] : 0;
                    while (intGrid[xx][yy] <= prevHistogram)
                    {
                        prevShadow.Pop();
                        if (prevHistogram == intGrid[xx][yy])
                            break;
                        //this new rectangle is not a subset of a larger rectangle if...
                        //for the row above this rectangle, the position of 1's going back to the start of the rectangle
                        //is GREATER than the end border (exclusive) of this rectangle.
                        //if it's equal or less, this rectangle is a subset and should not be counted
                        //so just keep track of the last zero
                        if (prevZero > prevShadow.Peek())
                            resultRects.Add(new Rect(prevShadow.Peek() + 1, yy, xx - (prevShadow.Peek() + 1), prevHistogram));

                        prevHistogram = prevShadow.Peek() > -1 ? intGrid[prevShadow.Peek()][yy] : 0;
                    }

                    prevShadow.Push(xx);
                    if (yy <= 0 || intGrid[xx][yy - 1] == 0)
                        prevZero = xx;
                }

                //close out with one last 0.
                int lastHistogram = prevShadow.Peek() > -1 ? intGrid[prevShadow.Peek()][yy] : 0;
                while (0 < lastHistogram)
                {
                    prevShadow.Pop();

                    //check on prevZero here too
                    if (prevZero > prevShadow.Peek())
                        resultRects.Add(new Rect(prevShadow.Peek() + 1, yy, intGrid.Length - (prevShadow.Peek() + 1), lastHistogram));

                    lastHistogram = prevShadow.Peek() > -1 ? intGrid[prevShadow.Peek()][yy] : 0;
                }
            }
            return resultRects;
        }
    }
}
