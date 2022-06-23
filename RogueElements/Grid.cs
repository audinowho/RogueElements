// <copyright file="Grid.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements
{
    public static class Grid
    {
        public delegate bool LocTest(Loc loc);

        public delegate void LocAction(Loc loc);

        private delegate bool EvalPaths(Loc[] ends, List<Loc>[] resultPaths, PathTile[] farthestTiles, PathTile currentTile);

        private delegate void EvalFallback(List<Loc>[] resultPaths, PathTile[] farthestTiles);

        public static List<Loc> FindPath(Loc rectStart, Loc rectSize, Loc start, Loc end, LocTest checkBlock, LocTest checkDiagBlock)
        {
            Loc[] ends = new Loc[1];
            ends[0] = end;
            return FindAPath(rectStart, rectSize, start, ends, checkBlock, checkDiagBlock);
        }

        /// <summary>
        /// Searches for the fastest path to any of the endpoints.  If multiple are tied it picks the first one.
        /// </summary>
        /// <param name="rectStart"></param>
        /// <param name="rectSize"></param>
        /// <param name="start"></param>
        /// <param name="ends">The list of goal points to path to.  Increase in count increases runtime linearly.</param>
        /// <param name="checkBlock"></param>
        /// <param name="checkDiagBlock"></param>
        /// <returns></returns>
        public static List<Loc> FindAPath(Loc rectStart, Loc rectSize, Loc start, Loc[] ends, LocTest checkBlock, LocTest checkDiagBlock)
        {
            List<Loc>[] multiEnds = FindMultiPaths(rectStart, rectSize, start, ends, checkBlock, checkDiagBlock, EvalPathOne, EvalFallbackOne);
            for (int ii = 0; ii < multiEnds.Length; ii++)
            {
                if (multiEnds[ii] != null)
                    return multiEnds[ii];
            }

            throw new InvalidOperationException("Could not find a path!");
        }

        public static List<Loc>[] FindAllPaths(Loc rectStart, Loc rectSize, Loc start, Loc[] ends, LocTest checkBlock, LocTest checkDiagBlock)
        {
            return FindMultiPaths(rectStart, rectSize, start, ends, checkBlock, checkDiagBlock, EvalPathAll, EvalFallbackAll);
        }

        /// <summary>
        /// Searches for the N fastest paths to any of the endpoints.
        /// </summary>
        /// <param name="rectStart"></param>
        /// <param name="rectSize"></param>
        /// <param name="start"></param>
        /// <param name="ends">The list of goal points to path to.  Increase in count increases runtime linearly.</param>
        /// <param name="checkBlock"></param>
        /// <param name="checkDiagBlock"></param>
        /// <param name="amt">Top N</param>
        /// <param name="multiTie">If multiple are tied it returns all involved in the tie.</param>
        public static List<Loc>[] FindNPaths(Loc rectStart, Loc rectSize, Loc start, Loc[] ends, LocTest checkBlock, LocTest checkDiagBlock, int amt, bool multiTie)
        {
            EvalPaths evalPathTied = (Loc[] evalEnds, List<Loc>[] resultPaths, PathTile[] farthestTiles, PathTile currentTile) =>
            {
                int foundResults = 0;
                int highestBackRefCount = 0;
                for (int ii = 0; ii < resultPaths.Length; ii++)
                {
                    if (resultPaths[ii] != null)
                    {
                        foundResults++;
                        highestBackRefCount = Math.Max(highestBackRefCount, resultPaths[ii].Count);
                    }
                }

                bool addedResult = false;
                for (int ii = 0; ii < evalEnds.Length; ii++)
                {
                    if (currentTile.Location == evalEnds[ii])
                    {
                        List<Loc> proposedBackRef = GetBackreference(currentTile);

                        // in multiTie mode, we are waiting for confirmation that we've found all ties.  This means having found something that fails a tie.
                        if (multiTie && foundResults >= amt)
                        {
                            // if this new proposed backref takes more steps than the current longest, then it is a failed tie
                            if (proposedBackRef.Count > highestBackRefCount)
                                return true;
                        }

                        resultPaths[ii] = proposedBackRef;
                    }

                    if (currentTile.Heuristic[ii] < farthestTiles[ii].Heuristic[ii])
                        farthestTiles[ii] = currentTile;
                }

                // if we're just looking for the amount and not paying attention to tiebreakers, we can call it a day when we reach the amount.
                if (!multiTie && addedResult)
                {
                    if (foundResults + 1 >= amt)
                        return true;
                }

                return false;
            };

            EvalFallback evalFallbackTied = (List<Loc>[] resultPaths, PathTile[] farthestTiles) =>
            {
                int foundResults = 0;
                int highestBackRefCount = 0;
                StablePriorityQueue<int, int> lowestStepsQueue = new StablePriorityQueue<int, int>();
                List<Loc>[] proposedPaths = new List<Loc>[resultPaths.Length];

                for (int ii = 0; ii < resultPaths.Length; ii++)
                {
                    if (resultPaths[ii] != null)
                    {
                        foundResults++;
                        highestBackRefCount = Math.Max(highestBackRefCount, resultPaths[ii].Count);
                    }
                    else
                    {
                        proposedPaths[ii] = GetBackreference(farthestTiles[ii]);
                        lowestStepsQueue.AddOrSetPriority(proposedPaths[ii].Count, ii);
                    }
                }

                while (lowestStepsQueue.Count > 0)
                {
                    int newIdx = lowestStepsQueue.Dequeue();
                    if (multiTie && foundResults >= amt)
                    {
                        // we've run out of ties.  stop adding paths
                        if (proposedPaths[newIdx].Count > highestBackRefCount)
                            break;
                    }

                    resultPaths[newIdx] = proposedPaths[newIdx];
                    highestBackRefCount = proposedPaths[newIdx].Count;
                    foundResults++;

                    if (!multiTie)
                    {
                        if (foundResults >= amt)
                            break;
                    }
                }
            };

            return FindMultiPaths(rectStart, rectSize, start, ends, checkBlock, checkDiagBlock, evalPathTied, evalFallbackTied);
        }

        /// <summary>
        /// Get the offset that the existing grid will have to be moved in, if resized with the specified parameters.
        /// </summary>
        /// <param name="oldWidth"></param>
        /// <param name="oldHeight"></param>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        /// <param name="dir">The direction to expand/shrink in.</param>
        /// <returns></returns>
        public static Loc GetResizeOffset(int oldWidth, int oldHeight, int width, int height, Dir8 dir)
        {
            dir.Separate(out DirH horiz, out DirV vert);

            Loc offset = Loc.Zero;
            if (horiz == DirH.None)
                offset.X = (width - oldWidth) / 2;
            else if (horiz == DirH.Left)
                offset.X = width - oldWidth;

            if (vert == DirV.None)
                offset.Y = (height - oldHeight) / 2;
            else if (vert == DirV.Up)
                offset.Y = height - oldHeight;

            return offset;
        }

        /// <summary>
        /// Resizes a 2-D array in a certain direction.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height.</param>
        /// <param name="dir">The direction to expand/shrink in.</param>
        /// <param name="newLocOp">The operation to perform on the tile when it is moved.</param>
        /// <param name="newTileOp">The operation to perform on the tile when it is completely new.</param>
        /// <returns></returns>
        public static Loc ResizeJustified<T>(ref T[][] array, int width, int height, Dir8 dir, LocAction newLocOp, LocAction newTileOp)
        {
            if (newLocOp == null)
                throw new ArgumentNullException(nameof(newLocOp));
            if (newTileOp == null)
                throw new ArgumentNullException(nameof(newTileOp));

            Loc offset = GetResizeOffset(array.Length, array[0].Length, width, height, dir);

            T[][] prevArray = array;
            array = new T[width][];
            for (int ii = 0; ii < width; ii++)
                array[ii] = new T[height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x >= offset.X && x < offset.X + prevArray.Length && y >= offset.Y && y < offset.Y + prevArray[0].Length)
                    {
                        array[x][y] = prevArray[x - offset.X][y - offset.Y];
                        newLocOp(new Loc(x, y));
                    }
                    else
                    {
                        newTileOp(new Loc(x, y));
                    }
                }
            }

            return offset;
        }

        /// <summary>
        /// Traverses a grid. Does not internally handle the state of traversed/untraversed nodes.
        /// </summary>
        /// <param name="rect">Bounds of the area to fill.</param>
        /// <param name="checkBlock">The operation determining if a tile is nontraversible.</param>
        /// <param name="checkDiagBlock">The operation determining if a tile is nontraversible, diagonally.</param>
        /// <param name="fillOp">The fill operation.</param>
        /// <param name="loc"></param>
        public static void FloodFill(Rect rect, LocTest checkBlock, LocTest checkDiagBlock, LocAction fillOp, Loc loc)
        {
            if (checkBlock == null)
                throw new ArgumentNullException(nameof(checkBlock));
            if (fillOp == null)
                throw new ArgumentNullException(nameof(fillOp));

            Stack<ScanLineTile> stack = new Stack<ScanLineTile>();
            stack.Push(new ScanLineTile(new IntRange(loc.X, loc.X), loc.Y, DirV.None, true, true));
            fillOp(loc);

            while (stack.Count > 0)
            {
                ScanLineTile fillCandidate = stack.Pop();

                int rangeMinX = fillCandidate.X.Min;
                if (fillCandidate.GoLeft)
                {
                    while (rangeMinX > rect.Start.X && !checkBlock(new Loc(rangeMinX - 1, fillCandidate.Y)))
                    {
                        rangeMinX--;
                        fillOp(new Loc(rangeMinX, fillCandidate.Y));
                    }
                }

                int rangeMaxX = fillCandidate.X.Max;
                if (fillCandidate.GoRight)
                {
                    while (rangeMaxX + 1 < rect.Start.X + rect.Size.X && !checkBlock(new Loc(rangeMaxX + 1, fillCandidate.Y)))
                    {
                        rangeMaxX++;
                        fillOp(new Loc(rangeMaxX, fillCandidate.Y));
                    }
                }

                if (fillCandidate.Y < rect.Start.Y + rect.Size.Y - 1)
                    ScanFill(rect, checkBlock, checkDiagBlock, fillOp, fillCandidate.X.Min, fillCandidate.X.Max, rangeMinX, rangeMaxX, fillCandidate.Y, fillCandidate.Dir != DirV.Up, DirV.Down, stack);

                if (fillCandidate.Y > rect.Start.Y)
                    ScanFill(rect, checkBlock, checkDiagBlock, fillOp, fillCandidate.X.Min, fillCandidate.X.Max, rangeMinX, rangeMaxX, fillCandidate.Y, fillCandidate.Dir != DirV.Down, DirV.Up, stack);
            }
        }

        public static List<Loc> FindConnectedTiles(Loc rectStart, Loc rectSize, LocTest checkOp, LocTest checkBlock, LocTest checkDiagBlock, Loc loc)
        {
            if (checkOp == null)
                throw new ArgumentNullException(nameof(checkOp));

            // use efficient fill method
            List<Loc> locList = new List<Loc>();

            void Action(Loc actLoc)
            {
                if (checkOp(actLoc))
                    locList.Add(actLoc);
            }

            AffectConnectedTiles(rectStart, rectSize, Action, checkBlock, checkDiagBlock, loc);

            return locList;
        }

        public static void AffectConnectedTiles(Loc rectStart, Loc rectSize, LocAction action, LocTest checkBlock, LocTest checkDiagBlock, Loc loc)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (checkBlock == null)
                throw new ArgumentNullException(nameof(checkBlock));
            if (checkDiagBlock == null)
                throw new ArgumentNullException(nameof(checkDiagBlock));

            // create an array to cache which tiles were already traversed
            bool[][] fillArray = new bool[rectSize.X][];
            for (int ii = 0; ii < rectSize.X; ii++)
                fillArray[ii] = new bool[rectSize.Y];

            FloodFill(
                new Rect(rectStart, rectSize),
                (Loc testLoc) => (checkBlock(testLoc) || fillArray[testLoc.X - rectStart.X][testLoc.Y - rectStart.Y]),
                (Loc testLoc) => (checkDiagBlock(testLoc) || fillArray[testLoc.X - rectStart.X][testLoc.Y - rectStart.Y]),
                (Loc actLoc) =>
                {
                    action(actLoc);
                    fillArray[actLoc.X - rectStart.X][actLoc.Y - rectStart.Y] = true;
                },
                loc);
        }

        /// <summary>
        /// Finds the tile that fits the specified requirements, starting from an origin point and searching outwards.
        /// </summary>
        /// <param name="rectStart">Start of the rectangle to search in.</param>
        /// <param name="rectSize">Size of the rectangle to search in.</param>
        /// <param name="checkFree">The check to see if the tile is eligible for return.</param>
        /// <param name="checkBlock">The check to see if the tile cannot be traversed.</param>
        /// <param name="checkDiagBlock">The check to see if the tile would prevent a diagonal traversal.</param>
        /// <param name="loc">Origin point to start search from.</param>
        /// <returns></returns>
        public static Loc? FindClosestConnectedTile(Loc rectStart, Loc rectSize, LocTest checkFree, LocTest checkBlock, LocTest checkDiagBlock, Loc loc)
        {
            foreach (Loc returnLoc in FindClosestConnectedTiles(rectStart, rectSize, checkFree, checkBlock, checkDiagBlock, loc, 1))
                return returnLoc;
            return null;
        }

        /// <summary>
        /// Unwrapped but consistent.
        /// </summary>
        /// <param name="rectStart"></param>
        /// <param name="rectSize"></param>
        /// <param name="checkFree"></param>
        /// <param name="checkBlock"></param>
        /// <param name="checkDiagBlock"></param>
        /// <param name="loc"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static IEnumerable<Loc> FindClosestConnectedTiles(Loc rectStart, Loc rectSize, LocTest checkFree, LocTest checkBlock, LocTest checkDiagBlock, Loc loc, int amount)
        {
            if (checkFree == null)
                throw new ArgumentNullException(nameof(checkFree));

            // create an array to cache which tiles were already traversed
            bool[][] fillArray = new bool[rectSize.X][];
            for (int ii = 0; ii < rectSize.X; ii++)
                fillArray[ii] = new bool[rectSize.Y];

            // use typical fill method
            StablePriorityQueue<int, Loc> locList = new StablePriorityQueue<int, Loc>();
            Loc offset_loc = loc - rectStart;
            locList.Enqueue(0, offset_loc);
            fillArray[offset_loc.X][offset_loc.Y] = true;
            int found = 0;
            while (locList.Count > 0)
            {
                Loc candidate = locList.Dequeue();

                if (checkFree(candidate + rectStart))
                {
                    yield return candidate + rectStart;
                    found++;
                    if (found >= amount)
                        yield break;
                }

                foreach (Dir8 dir in DirExt.VALID_DIR8)
                {
                    Loc movedLoc = candidate + dir.GetLoc();
                    if (Collision.InBounds(rectSize.X, rectSize.Y, movedLoc) && !fillArray[movedLoc.X][movedLoc.Y] && !IsDirBlocked(candidate + rectStart, dir, checkBlock, checkDiagBlock))
                    {
                        Loc diff = movedLoc - offset_loc;
                        locList.Enqueue(diff.DistSquared(), movedLoc);
                        fillArray[movedLoc.X][movedLoc.Y] = true;
                    }
                }
            }
        }

        public static List<Loc> FindTilesInBox(Loc rectStart, Loc rectSize, LocTest checkOp)
        {
            if (checkOp == null)
                throw new ArgumentNullException(nameof(checkOp));

            List<Loc> locList = new List<Loc>();
            for (int x = rectStart.X; x < rectStart.X + rectSize.X; x++)
            {
                for (int y = rectStart.Y; y < rectStart.Y + rectSize.Y; y++)
                {
                    Loc testLoc = new Loc(x, y);
                    if (checkOp(testLoc))
                        locList.Add(testLoc);
                }
            }

            return locList;
        }

        /// <summary>
        /// Determines if blocking off a specific tile would cause a path leading through it to become inaccessible.
        /// If a tile is a choke point, there are no alternate paths.
        /// </summary>
        /// <param name="rectStart">Top-Left of the rectangle to search for alternate paths.</param>
        /// <param name="rectSize">Dimensions of the rectangle to search for alternate paths.</param>
        /// <param name="point">The tile to block off.</param>
        /// <param name="checkBlock">Determines if a tile is blocked.</param>
        /// <param name="checkDiagBlock">Determines if a tile checked diagonally is blocked.</param>
        /// <returns></returns>
        public static bool IsChokePoint(Loc rectStart, Loc rectSize, Loc point, LocTest checkBlock, LocTest checkDiagBlock)
        {
            if (checkBlock == null)
                throw new ArgumentNullException(nameof(checkBlock));
            if (checkDiagBlock == null)
                throw new ArgumentNullException(nameof(checkDiagBlock));

            List<Dir8> forks = GetForkDirs(point, checkBlock, checkDiagBlock);
            if (forks.Count < 2)
                return false;

            bool[][] fillArray = new bool[rectSize.X][];
            for (int ii = 0; ii < rectSize.X; ii++)
            {
                fillArray[ii] = new bool[rectSize.Y];
            }

            Loc offset_point = point - rectStart;
            fillArray[offset_point.X][offset_point.Y] = true;

            List<Loc> forkList = new List<Loc>();
            for (int ii = 0; ii < forks.Count; ii++)
            {
                Loc loc = point + forks[ii].GetLoc();
                forkList.Add(loc);
            }

            bool CheckChokeBlock(Loc loc)
            {
                if (fillArray[loc.X - rectStart.X][loc.Y - rectStart.Y])
                    return true;
                return checkBlock(loc);
            }

            bool CheckDiagChokeBlock(Loc loc)
            {
                if (fillArray[loc.X - rectStart.X][loc.Y - rectStart.Y])
                    return true;
                return checkDiagBlock(loc);
            }

            void Fill(Loc loc)
            {
                fillArray[loc.X - rectStart.X][loc.Y - rectStart.Y] = true;
                if (forkList.Contains(loc))
                    forkList.Remove(loc);
            }

            FloodFill(new Rect(rectStart, rectSize), CheckChokeBlock, CheckDiagChokeBlock, Fill, forkList[0]);

            return forkList.Count > 0;
        }

        public static List<Dir8> GetForkDirs(Loc point, LocTest checkBlock, LocTest checkDiagBlock)
        {
            List<Dir8> forks = new List<Dir8>();
            bool prevBlocked = IsDirBlocked(point, Dir8.Down, checkBlock, checkDiagBlock);
            Dir8 dir = Dir8.DownLeft;
            do
            {
                bool newBlock = IsDirBlocked(point, dir, checkBlock, checkDiagBlock);
                if (newBlock != prevBlocked)
                {
                    if (!newBlock)
                        forks.Add(dir);
                    prevBlocked = newBlock;
                }

                dir = dir.Rotate(1);
            }
            while (dir != Dir8.DownLeft);

            return forks;
        }

        public static bool IsDirBlocked(Loc loc, Dir8 dir, LocTest checkBlock, LocTest checkDiagBlock)
        {
            return IsDirBlocked(loc, dir, checkBlock, checkDiagBlock, 1);
        }

        public static bool IsDirBlocked(Loc loc, Dir8 dir, LocTest checkBlock, LocTest checkDiagBlock, int distance)
        {
            if (checkBlock == null)
                throw new ArgumentNullException(nameof(checkBlock));
            if (checkDiagBlock == null)
                throw new ArgumentNullException(nameof(checkDiagBlock));

            if (!dir.Validate())
                throw new ArgumentException("Invalid value to check.");
            else if (dir == Dir8.None)
                return false;

            for (int ii = 0; ii < distance; ii++)
            {
                if (dir.IsDiagonal())
                {
                    dir.Separate(out DirH horiz, out DirV vert);

                    Loc diagLoc = loc + horiz.GetLoc();
                    if (checkDiagBlock(diagLoc))
                        return true;

                    diagLoc = loc + vert.GetLoc();
                    if (checkDiagBlock(diagLoc))
                        return true;
                }

                loc += dir.GetLoc();

                if (checkBlock(loc))
                    return true;
            }

            return false;
        }

        private static List<Loc> GetBackreference(PathTile currentTile)
        {
            List<Loc> path = new List<Loc> { currentTile.Location };
            while (currentTile.BackReference != null)
            {
                currentTile = currentTile.BackReference;
                path.Add(currentTile.Location);
            }

            return path;
        }

        private static void ScanFill(
            Rect rect,
            LocTest checkBlock,
            LocTest checkDiagBlock,
            LocAction fillOp,
            int min,
            int max,
            int range_min,
            int range_max,
            int y,
            bool isNext,
            DirV dir,
            Stack<ScanLineTile> stack)
        {
            if (checkBlock == null)
                throw new ArgumentNullException(nameof(checkBlock));
            if (fillOp == null)
                throw new ArgumentNullException(nameof(fillOp));

            // move y down or up
            int new_y = y + dir.GetLoc().Y;

            // for diagonal checking: check slightly further
            int sub = (range_min > rect.Start.X) ? 1 : 0;
            int add = (range_max < rect.Start.X + rect.Size.X - 1) ? 1 : 0;

            // start x - 2 needs to be initialized to an "invalid" value.
            // use rect.X - 2 because it is 1 less than the minimum of what x can be.
            int invalid_x = rect.X - 2;
            int line_start = invalid_x;
            int x = range_min - sub;
            for (; x <= range_max + add; x++)
            {
                bool unblocked = !checkBlock(new Loc(x, new_y));

                // check diagonal if applicable
                if (x < range_min)
                    unblocked &= !IsDirBlocked(new Loc(range_min, y), DirExt.Combine(DirH.Left, dir), checkBlock, checkDiagBlock);
                else if (x > range_max)
                    unblocked &= !IsDirBlocked(new Loc(range_max, y), DirExt.Combine(DirH.Right, dir), checkBlock, checkDiagBlock);

                // skip testing, if testing previous line within previous range
                bool empty = (isNext || (x < min || x > max)) && unblocked;

                if (line_start == invalid_x && empty)
                {
                    line_start = x;
                }
                else if (line_start > invalid_x && !empty)
                {
                    stack.Push(new ScanLineTile(new IntRange(line_start, x - 1), new_y, dir, line_start <= range_min, x > range_max));
                    line_start = invalid_x;
                }

                if (line_start > invalid_x)
                    fillOp(new Loc(x, new_y));

                if (!isNext && x == min)
                    x = max;
            }

            if (line_start > invalid_x)
                stack.Push(new ScanLineTile(new IntRange(line_start, x - 1), new_y, dir, line_start <= range_min, true));
        }

        /// <summary>
        /// Unwrapped but consistent.
        /// </summary>
        /// <param name="rectStart"></param>
        /// <param name="rectSize"></param>
        /// <param name="start"></param>
        /// <param name="ends"></param>
        /// <param name="checkBlock"></param>
        /// <param name="checkDiagBlock"></param>
        /// <param name="eval"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        private static List<Loc>[] FindMultiPaths(Loc rectStart, Loc rectSize, Loc start, Loc[] ends, LocTest checkBlock, LocTest checkDiagBlock, EvalPaths eval, EvalFallback fallback)
        {
            if (ends.Length == 0)
                return new List<Loc>[0];

            PathTile[][] tiles = new PathTile[rectSize.X][];
            for (int ii = 0; ii < rectSize.X; ii++)
            {
                tiles[ii] = new PathTile[rectSize.Y];
                for (int jj = 0; jj < rectSize.Y; jj++)
                    tiles[ii][jj] = new PathTile(new Loc(rectStart.X + ii, rectStart.Y + jj));
            }

            Loc offset_start = start - rectStart;

            StablePriorityQueue<double, PathTile> candidates = new StablePriorityQueue<double, PathTile>();
            PathTile start_tile = tiles[offset_start.X][offset_start.Y];
            start_tile.UpdateHeuristics(ends);
            start_tile.Cost = 0;
            candidates.Enqueue(start_tile.MinHeuristic + start_tile.Cost, start_tile);

            List<Loc>[] resultPaths = new List<Loc>[ends.Length];

            PathTile[] farthestTiles = new PathTile[ends.Length];
            for (int ii = 0; ii < farthestTiles.Length; ii++)
                farthestTiles[ii] = start_tile;

            while (candidates.Count > 0)
            {
                PathTile currentTile = candidates.Dequeue();

                if (eval(ends, resultPaths, farthestTiles, currentTile))
                    return resultPaths;

                currentTile.Traversed = true;

                foreach (Dir8 dir in DirExt.VALID_DIR8)
                {
                    Loc newLoc = currentTile.Location - rectStart + dir.GetLoc();
                    if (Collision.InBounds(rectSize.X, rectSize.Y, newLoc))
                    {
                        if (!IsDirBlocked(currentTile.Location, dir, checkBlock, checkDiagBlock))
                        {
                            PathTile tile = tiles[newLoc.X][newLoc.Y];

                            if (tile.Traversed)
                                continue;

                            int newCost = currentTile.Cost + 1;
                            if (tile.Cost == -1 || newCost < tile.Cost)
                            {
                                tile.Cost = newCost;
                                tile.UpdateHeuristics(ends);
                                tile.BackReference = currentTile;
                                candidates.AddOrSetPriority(tile.MinHeuristic + tile.Cost, tile);
                            }
                        }
                    }
                }
            }

            fallback(resultPaths, farthestTiles);

            return resultPaths;
        }

        /// <summary>
        /// Only needs one path completed to return.
        /// </summary>
        /// <param name="ends"></param>
        /// <param name="resultPaths"></param>
        /// <param name="farthestTiles"></param>
        /// <param name="currentTile"></param>
        /// <returns></returns>
        private static bool EvalPathOne(Loc[] ends, List<Loc>[] resultPaths, PathTile[] farthestTiles, PathTile currentTile)
        {
            GenericUpdateCompletePaths(ends, resultPaths, farthestTiles, currentTile);

            for (int ii = 0; ii < resultPaths.Length; ii++)
            {
                if (resultPaths[ii] != null)
                    return true;
            }

            return false;
        }

        private static bool EvalPathAll(Loc[] ends, List<Loc>[] resultPaths, PathTile[] farthestTiles, PathTile currentTile)
        {
            GenericUpdateCompletePaths(ends, resultPaths, farthestTiles, currentTile);

            for (int ii = 0; ii < resultPaths.Length; ii++)
            {
                if (resultPaths[ii] == null)
                    return false;
            }

            return true;
        }

        private static void GenericUpdateCompletePaths(Loc[] ends, List<Loc>[] resultPaths, PathTile[] farthestTiles, PathTile currentTile)
        {
            for (int ii = 0; ii < ends.Length; ii++)
            {
                if (currentTile.Location == ends[ii])
                    resultPaths[ii] = GetBackreference(currentTile);

                if (currentTile.Heuristic[ii] < farthestTiles[ii].Heuristic[ii])
                    farthestTiles[ii] = currentTile;
            }
        }

        private static void EvalFallbackOne(List<Loc>[] resultPaths, PathTile[] farthestTiles)
        {
            int minHeuristicIndex = 0;
            for (int ii = 1; ii < resultPaths.Length; ii++)
            {
                if (farthestTiles[ii].Heuristic[ii] < farthestTiles[minHeuristicIndex].Heuristic[minHeuristicIndex])
                    minHeuristicIndex = ii;
            }

            resultPaths[minHeuristicIndex] = GetBackreference(farthestTiles[minHeuristicIndex]);
        }

        private static void EvalFallbackAll(List<Loc>[] resultPaths, PathTile[] farthestTiles)
        {
            for (int ii = 0; ii < resultPaths.Length; ii++)
            {
                if (resultPaths[ii] != null)
                    continue;

                resultPaths[ii] = GetBackreference(farthestTiles[ii]);
            }
        }

        private struct ScanLineTile
        {
            public IntRange X;
            public int Y;
            public DirV Dir;
            public bool GoLeft;
            public bool GoRight;

            public ScanLineTile(IntRange x, int y, DirV dir, bool goLeft, bool goRight)
            {
                this.X = x;
                this.Y = y;
                this.Dir = dir;
                this.GoLeft = goLeft;
                this.GoRight = goRight;
            }
        }

        private class PathTile
        {
            public PathTile(Loc location)
            {
                this.Location = location;
                this.Cost = -1;
            }

            public Loc Location { get; }

            public bool Traversed { get; set; }

            public int Cost { get; set; }

            public double[] Heuristic { get; private set; }

            public double MinHeuristic { get; private set; }

            public PathTile BackReference { get; set; }

            public void UpdateHeuristics(Loc[] ends)
            {
                this.Heuristic = new double[ends.Length];

                this.Heuristic[0] = Math.Sqrt((ends[0] - this.Location).DistSquared());
                this.MinHeuristic = this.Heuristic[0];
                for (int ii = 1; ii < ends.Length; ii++)
                {
                    this.Heuristic[ii] = Math.Sqrt((ends[ii] - this.Location).DistSquared());
                    this.MinHeuristic = Math.Min(this.MinHeuristic, this.Heuristic[ii]);
                }
            }
        }
    }
}
