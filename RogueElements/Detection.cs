// <copyright file="Detection.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements
{
    public static class Detection
    {
        public delegate void RectFunc(Rect rect);

        public static BlobMap DetectBlobs(Rect rect, Grid.LocTest isValid)
        {
            if (isValid == null)
                throw new ArgumentNullException(nameof(isValid));

            var blobMap = new BlobMap(rect.Width, rect.Height);

            for (int xx = rect.X; xx < rect.End.X; xx++)
            {
                for (int yy = rect.Y; yy < rect.End.Y; yy++)
                {
                    if (isValid(new Loc(xx, yy)) && blobMap.Map[xx][yy] == -1)
                    {
                        var blob = new BlobMap.Blob(new Rect(xx, yy, 1, 1), 0);

                        // fill the area, keeping track of the total area and blob bounds
                        Grid.FloodFill(
                            rect,
                            (Loc testLoc) => (!isValid(testLoc) || blobMap.Map[testLoc.X][testLoc.Y] != -1),
                            (Loc testLoc) => true,
                            (Loc fillLoc) =>
                            {
                                blobMap.Map[fillLoc.X][fillLoc.Y] = blobMap.Blobs.Count;
                                blob.Bounds = Rect.IncludeLoc(blob.Bounds, fillLoc);
                                blob.Area += 1;
                            },
                            new Loc(xx, yy));

                        blobMap.Blobs.Add(blob);
                    }
                }
            }

            return blobMap;
        }

        /// <summary>
        /// Detects if an added blob of tiles disconnects the map's existing connectivity.
        /// </summary>
        /// <param name="mapBounds"></param>
        /// <param name="isMapValid">Checks for a valid path tile.  Returns true if the tile is unblocked.</param>
        /// <param name="blobDest">Position to draw the blob at.</param>
        /// <param name="blobSize"></param>
        /// <param name="isBlobValid">Checks for a valid blob tile. Loc is with respect to the top right of the blob rect.  Returns true if the tile will be made blocked.</param>
        /// <param name="countErasures">Whether a completely erased graph counts as disconnected or not.</param>
        /// <returns></returns>
        public static bool DetectDisconnect(Rect mapBounds, Grid.LocTest isMapValid, Loc blobDest, Loc blobSize, Grid.LocTest isBlobValid, bool countErasures)
        {
            if (isMapValid == null)
                throw new ArgumentNullException(nameof(isMapValid));
            if (isBlobValid == null)
                throw new ArgumentNullException(nameof(isBlobValid));

            List<int> mapBlobCounts = new List<int>();
            int[][] fullGrid = new int[mapBounds.Width][];
            int[][] splitGrid = new int[mapBounds.Width][];
            for (int xx = 0; xx < mapBounds.Width; xx++)
            {
                fullGrid[xx] = new int[mapBounds.Height];
                splitGrid[xx] = new int[mapBounds.Height];
                for (int yy = 0; yy < mapBounds.Height; yy++)
                {
                    fullGrid[xx][yy] = -1;
                    splitGrid[xx][yy] = -1;
                }
            }

            // iterate the map and flood fill when finding a walkable.
            // Count the number of times a flood fill is required.  This is the blob count.
            for (int xx = 0; xx < mapBounds.Width; xx++)
            {
                for (int yy = 0; yy < mapBounds.Height; yy++)
                {
                    if (isMapValid(mapBounds.Start + new Loc(xx, yy)) && fullGrid[xx][yy] == -1)
                    {
                        int totalFill = 0;
                        Grid.FloodFill(
                            new Rect(0, 0, mapBounds.Width, mapBounds.Height),
                            (Loc testLoc) => (fullGrid[testLoc.X][testLoc.Y] == mapBlobCounts.Count) || !isMapValid(mapBounds.Start + testLoc),
                            (Loc testLoc) => true,
                            (Loc fillLoc) =>
                            {
                                fullGrid[fillLoc.X][fillLoc.Y] = mapBlobCounts.Count;
                                totalFill++;
                            },
                            new Loc(xx, yy));
                        mapBlobCounts.Add(totalFill);
                    }
                }
            }

            // we've passed in a boolean grid containing a blob, with an offset of where to render it to
            for (int xx = Math.Max(mapBounds.X, blobDest.X); xx < Math.Min(mapBounds.End.X, blobDest.X + blobSize.X); xx++)
            {
                for (int yy = Math.Max(mapBounds.Y, blobDest.Y); yy < Math.Min(mapBounds.End.Y, blobDest.Y + blobSize.Y); yy++)
                {
                    Loc mapLoc = new Loc(xx, yy) - mapBounds.Start;
                    int blobIndex = fullGrid[mapLoc.X][mapLoc.Y];
                    if (blobIndex > -1 && isBlobValid(new Loc(xx, yy) - blobDest))
                    {
                        mapBlobCounts[blobIndex] = mapBlobCounts[blobIndex] - 1;
                        fullGrid[mapLoc.X][mapLoc.Y] = -1;
                    }
                }
            }

            // remove the blobs that have been entirely erased; return false if entirely erased.
            for (int ii = mapBlobCounts.Count - 1; ii >= 0; ii--)
            {
                if (mapBlobCounts[ii] == 0)
                {
                    if (countErasures)
                        return true;
                    mapBlobCounts.RemoveAt(ii);
                }
            }

            // iterate the map and flood fill when finding a walkable (needs a new bool grid), this time discounting tiles involved in the blob.  count times needed for this
            int blobsFound = 0;
            for (int xx = 0; xx < mapBounds.Width; xx++)
            {
                for (int yy = 0; yy < mapBounds.Height; yy++)
                {
                    if (fullGrid[xx][yy] > -1 && splitGrid[xx][yy] == -1)
                    {
                        Grid.FloodFill(
                            new Rect(0, 0, mapBounds.Width, mapBounds.Height),
                            (Loc testLoc) => (splitGrid[testLoc.X][testLoc.Y] == blobsFound) || (fullGrid[testLoc.X][testLoc.Y] == -1),
                            (Loc testLoc) => true,
                            (Loc fillLoc) => splitGrid[fillLoc.X][fillLoc.Y] = blobsFound,
                            new Loc(xx, yy));

                        blobsFound++;
                    }
                }
            }

            // more times = more blobs = failure
            return blobsFound != mapBlobCounts.Count;
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
            if (checkBlock == null)
                throw new ArgumentNullException(nameof(checkBlock));
            if (checkGround == null)
                throw new ArgumentNullException(nameof(checkGround));

            bool[][] checkGrid = new bool[rect.Width][];
            bool[][] fillGrid = new bool[rect.Width][];
            for (int xx = 0; xx < rect.Width; xx++)
            {
                checkGrid[xx] = new bool[rect.Height];
                fillGrid[xx] = new bool[rect.Height];
            }

            List<LocRay4> walls = new List<LocRay4>();

            // scan and find solely-facing walls
            // cache already-checked walls since flood fill occasionally checks twice
            Grid.FloodFill(
                rect,
                (Loc testLoc) =>
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
                (Loc testLoc) => true,
                (Loc fillLoc) => fillGrid[fillLoc.X][fillLoc.Y] = true,
                start);

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
            if (checkBlock == null)
                throw new ArgumentNullException(nameof(checkBlock));

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
            if (checkBlock == null)
                throw new ArgumentNullException(nameof(checkBlock));
            if (checkGround == null)
                throw new ArgumentNullException(nameof(checkGround));

            // check the four directions
            Dir4 chosenDir = Dir4.None;

            // ensure that there is only one direction where it is unblocked
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                Loc newLoc = loc + dir.GetLoc();
                if (checkGround(newLoc))
                {
                    if (chosenDir != Dir4.None)
                        return new LocRay4(loc);
                    else
                        chosenDir = dir.Reverse();
                }
                else if (!checkBlock(newLoc))
                {
                    // all four directions must be valid ground, or valid block
                    return new LocRay4(loc);
                }
            }

            if (chosenDir == Dir4.None)
                return new LocRay4(loc);

            // then check to make sure that the left and right diagonal of this direction are also valid blocked
            Loc lLoc = loc + DirExt.AddAngles(chosenDir.ToDir8(), Dir8.DownLeft).GetLoc();
            if (!checkBlock(lLoc))
                return new LocRay4(loc);
            Loc rLoc = loc + DirExt.AddAngles(chosenDir.ToDir8(), Dir8.DownRight).GetLoc();
            if (!checkBlock(rLoc))
                return new LocRay4(loc);

            return new LocRay4(loc, chosenDir);
        }

        /// <summary>
        /// Gets the N largest rectangles in the grid that are not a subset of a larger rectangle.
        /// </summary>
        /// <param name="grid">2D array of booleans</param>
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

        /// <summary>
        /// Gets all rectangles in the grid that are not a subset of a larger rectangle.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static List<Rect> FindAllRects(bool[][] grid)
        {
            List<Rect> resultRects = new List<Rect>();

            // create a pre-process where each grid value actually indicates how many 1's are below it
            int[][] intGrid = new int[grid.Length][];
            for (int xx = 0; xx < grid.Length; xx++)
            {
                intGrid[xx] = new int[grid[0].Length];

                // first line (bottom)
                if (grid[xx][grid[0].Length - 1])
                    intGrid[xx][grid[0].Length - 1] = 1;

                // all lines above the first line
                for (int yy = grid[0].Length - 2; yy >= 0; yy--)
                {
                    if (grid[xx][yy])
                        intGrid[xx][yy] = intGrid[xx][yy + 1] + 1;
                }
            }

            // go through each row with the histogram approach; capacity is known so might as well specify it here
            Stack<int> prevShadow = new Stack<int>(intGrid.Length);
            for (int yy = 0; yy < intGrid[0].Length; yy++)
            {
                prevShadow.Clear();
                prevShadow.Push(-1);
                int prevZero = -1;
                for (int xx = 0; xx < intGrid.Length; xx++)
                {
                    // encountered a smaller number? compute rectangles with all stack values greater than the current number
                    int prevHistogram = prevShadow.Peek() > -1 ? intGrid[prevShadow.Peek()][yy] : 0;
                    while (intGrid[xx][yy] <= prevHistogram)
                    {
                        prevShadow.Pop();
                        if (prevHistogram == intGrid[xx][yy])
                            break;

                        // this new rectangle is not a subset of a larger rectangle if...
                        // for the row above this rectangle, the position of 1's going back to the start of the rectangle
                        // is GREATER than the end border (exclusive) of this rectangle.
                        // if it's equal or less, this rectangle is a subset and should not be counted
                        // so just keep track of the last zero
                        if (prevZero > prevShadow.Peek())
                            resultRects.Add(new Rect(prevShadow.Peek() + 1, yy, xx - (prevShadow.Peek() + 1), prevHistogram));

                        prevHistogram = prevShadow.Peek() > -1 ? intGrid[prevShadow.Peek()][yy] : 0;
                    }

                    prevShadow.Push(xx);
                    if (yy <= 0 || intGrid[xx][yy - 1] == 0)
                        prevZero = xx;
                }

                // close out with one last 0.
                int lastHistogram = prevShadow.Peek() > -1 ? intGrid[prevShadow.Peek()][yy] : 0;
                while (lastHistogram > 0)
                {
                    prevShadow.Pop();

                    // check on prevZero here too
                    if (prevZero > prevShadow.Peek())
                        resultRects.Add(new Rect(prevShadow.Peek() + 1, yy, intGrid.Length - (prevShadow.Peek() + 1), lastHistogram));

                    lastHistogram = prevShadow.Peek() > -1 ? intGrid[prevShadow.Peek()][yy] : 0;
                }
            }

            return resultRects;
        }
    }
}
