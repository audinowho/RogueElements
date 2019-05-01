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


        private static List<Loc> getBackreference(PathTile currentTile)
        {
            List<Loc> path = new List<Loc>();
            path.Add(currentTile.Location);
            while (currentTile.BackReference != null)
            {
                currentTile = currentTile.BackReference;
                path.Add(currentTile.Location);
            }
            return path;
        }

        public static List<Loc> FindPath(Loc rectStart, Loc rectSize, Loc start, Loc end, LocTest checkBlock, LocTest checkDiagBlock)
        {
            Loc[] ends = new Loc[1];
            ends[0] = end;
            return FindAPath(rectStart, rectSize, start, ends, checkBlock, checkDiagBlock);
        }
        public static List<Loc> FindAPath(Loc rectStart, Loc rectSize, Loc start, Loc[] ends, LocTest checkBlock, LocTest checkDiagBlock)
        {
            //searches for one specific path (ends[0]), but doesn't mind hitting the other ones
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
            start_tile.Heuristic = Math.Sqrt((ends[0] - start).DistSquared());
            start_tile.Cost = 0;
            candidates.Enqueue(start_tile.Heuristic + start_tile.Cost, start_tile);

            PathTile farthest_tile = start_tile;
            while (candidates.Count > 0)
            {
                PathTile currentTile = candidates.Dequeue();
                for(int ii = 0; ii < ends.Length; ii++)
                {
                    if (currentTile.Location == ends[ii])
                        return getBackreference(currentTile);
                }
                if (currentTile.Heuristic < farthest_tile.Heuristic)
                    farthest_tile = currentTile;

                currentTile.Traversed = true;

                foreach (Dir8 dir in DirExt.VALID_DIR8)
                {
                    if (!IsDirBlocked(currentTile.Location, dir, checkBlock, checkDiagBlock))
                    {
                        Loc newLoc = currentTile.Location - rectStart + dir.GetLoc();
                        if (Collision.InBounds(rectSize.X, rectSize.Y, newLoc))
                        {
                            PathTile tile = tiles[newLoc.X][newLoc.Y];

                            if (tile.Traversed)
                                continue;

                            int newCost = currentTile.Cost + 1;
                            if (tile.Cost == -1 || newCost < tile.Cost)
                            {
                                tile.Cost = newCost;
                                tile.Heuristic = Math.Sqrt((ends[0] - tile.Location).DistSquared());
                                tile.BackReference = currentTile;
                                candidates.AddOrSetPriority(tile.Heuristic + tile.Cost, tile);
                            }
                        }
                    }
                }

            }
            return getBackreference(farthest_tile);
        }


        public static Loc ResizeJustified<T>(ref T[][] array, int width, int height, Dir8 dir, LocAction newLocOp, LocAction newTileOp)
        {
            DirV vert;
            DirH horiz;
            dir.Separate(out horiz, out vert);

            Loc offset = new Loc();
            if (horiz == DirH.None)
                offset.X = (width - array.Length) / 2;
            else if (horiz == DirH.Left)
                offset.X = (width - array.Length);

            if (vert == DirV.None)
                offset.Y = (height - array[0].Length) / 2;
            else if (vert == DirV.Up)
                offset.Y = (height - array[0].Length);


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
                        newTileOp(new Loc(x, y));
                }
            }

            return offset;
        }



        private static void scanFill(Rect rect, LocTest checkBlock, LocTest checkDiagBlock, LocAction fillOp,
            int min, int max, int range_min, int range_max, int y, bool isNext, DirV dir,
            Stack<ScanLineTile> stack)
        {
            //move y down or up
            int new_y = y + dir.GetLoc().Y;

            //for diagonal checking: check slightly further
            int sub = ((range_min > rect.Start.X) ? 1 : 0);
            int add = ((range_max < rect.Start.X + rect.Size.X - 1) ? 1 : 0);

            int line_start = -1;
            int x = range_min - sub;
            for (; x <= range_max + add; x++)
            {
                bool unblocked = !checkBlock(new Loc(x, new_y));
                //check diagonal if applicable
                if (x < range_min)
                    unblocked &= !IsDirBlocked(new Loc(range_min, y), DirExt.Combine(DirH.Left, dir), checkBlock, checkDiagBlock);
                else if (x > range_max)
                    unblocked &= !IsDirBlocked(new Loc(range_max, y), DirExt.Combine(DirH.Right, dir), checkBlock, checkDiagBlock);

                // skip testing, if testing previous line within previous range
                bool empty = (isNext || (x < min || x > max)) && unblocked;

                if (line_start == -1 && empty)
                    line_start = x;
                else if (line_start > -1 && !empty)
                {
                    stack.Push(new ScanLineTile(line_start, x - 1, new_y, dir, line_start <= range_min, x > range_max));
                    line_start = -1;
                }

                if (line_start > -1)
                    fillOp(new Loc(x, new_y));

                if (!isNext && x == min)
                    x = max;
            }
            if (line_start > -1)
                stack.Push(new ScanLineTile(line_start, x - 1, new_y, dir, line_start <= range_min, true));

        }

        /// <summary>
        /// Traverses a grid. Does not internally handle the state of traversed/untraversed nodes.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="checkBlock"></param>
        /// <param name="checkDiagBlock"></param>
        /// <param name="fillOp"></param>
        /// <param name="loc"></param>
        public static void FloodFill(Rect rect, LocTest checkBlock, LocTest checkDiagBlock, LocAction fillOp, Loc loc)
        {
            Stack<ScanLineTile> stack = new Stack<ScanLineTile>();
            stack.Push(new ScanLineTile(loc.X, loc.X, loc.Y, DirV.None, true, true));
            fillOp(loc);

            while (stack.Count > 0)
            {
                ScanLineTile fillCandidate = stack.Pop();

                int rangeMinX = fillCandidate.MinX;
                if (fillCandidate.GoLeft)
                {
                    while (rangeMinX > rect.Start.X && !checkBlock(new Loc(rangeMinX - 1, fillCandidate.Y)))
                    {
                        rangeMinX--;
                        fillOp(new Loc(rangeMinX, fillCandidate.Y));
                    }
                }

                int rangeMaxX = fillCandidate.MaxX;
                if (fillCandidate.GoRight)
                {
                    while (rangeMaxX + 1 < rect.Start.X + rect.Size.X && !checkBlock(new Loc(rangeMaxX + 1, fillCandidate.Y)))
                    {
                        rangeMaxX++;
                        fillOp(new Loc(rangeMaxX, fillCandidate.Y));
                    }
                }

                if (fillCandidate.Y < rect.Start.Y + rect.Size.Y - 1)
                    scanFill(rect, checkBlock, checkDiagBlock, fillOp, fillCandidate.MinX, fillCandidate.MaxX, rangeMinX, rangeMaxX, fillCandidate.Y, fillCandidate.Dir != DirV.Up, DirV.Down, stack);

                if (fillCandidate.Y > rect.Start.Y)
                    scanFill(rect, checkBlock, checkDiagBlock, fillOp, fillCandidate.MinX, fillCandidate.MaxX, rangeMinX, rangeMaxX, fillCandidate.Y, fillCandidate.Dir != DirV.Down, DirV.Up, stack);
            }
        }

        public static List<Loc> FindConnectedTiles(Loc rectStart, Loc rectSize, LocTest checkOp, LocTest checkBlock, LocTest checkDiagBlock, Loc loc)
        {
            //use efficient fill method
            List<Loc> locList = new List<Loc>();

            LocAction action = (Loc actLoc) =>
            {
                if (checkOp(actLoc))
                    locList.Add(actLoc);
            };

            AffectConnectedTiles(rectStart, rectSize, action, checkBlock, checkDiagBlock, loc);

            return locList;
        }


        public static void AffectConnectedTiles(Loc rectStart, Loc rectSize, LocAction action, LocTest checkBlock, LocTest checkDiagBlock, Loc loc)
        {
            //create an array to cache which tiles were already traversed
            bool[][] fillArray = new bool[rectSize.X][];
            for (int ii = 0; ii < rectSize.X; ii++)
                fillArray[ii] = new bool[rectSize.Y];

            FloodFill(new Rect(rectStart, rectSize),
                (Loc testLoc) =>
                {
                    return (checkBlock(testLoc) || fillArray[testLoc.X - rectStart.X][testLoc.Y - rectStart.Y]);
                },
                (Loc testLoc) =>
                {
                    return (checkDiagBlock(testLoc) || fillArray[testLoc.X - rectStart.X][testLoc.Y - rectStart.Y]);
                },
                (Loc actLoc) =>
                {
                    action(actLoc);

                    fillArray[actLoc.X - rectStart.X][actLoc.Y - rectStart.Y] = true;
                },
                loc);
            
        }


        public static Loc? FindClosestConnectedTile(Loc rectStart, Loc rectSize, LocTest checkFree, LocTest checkBlock, LocTest checkDiagBlock, Loc loc)
        {
            foreach(Loc returnLoc in FindClosestConnectedTiles(rectStart, rectSize, checkFree, checkBlock, checkDiagBlock, loc, 1))
                return returnLoc;
            return null;
        }

        public static IEnumerable<Loc> FindClosestConnectedTiles(Loc rectStart, Loc rectSize, LocTest checkFree, LocTest checkBlock, LocTest checkDiagBlock, Loc loc, int amount)
        {
            //create an array to cache which tiles were already traversed
            bool[][] fillArray = new bool[rectSize.X][];
            for (int ii = 0; ii < rectSize.X; ii++)
                fillArray[ii] = new bool[rectSize.Y];

            //use typical fill method
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
            List<Loc> locList = new List<Loc>();
            for (int x = rectStart.X; x < rectStart.X + rectSize.X; x++)
            {
                for (int y = rectStart.Y; y < rectStart.Y + rectSize.Y; y++)
                {
                    Loc testLoc = new Loc(x,y);
                    if (checkOp(testLoc))
                        locList.Add(testLoc);
                }
            }
            return locList;
        }

        public static bool IsChokePoint(Loc rectStart, Loc rectSize, Loc point, LocTest checkBlock, LocTest checkDiagBlock)
        {
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

            LocTest checkChokeBlock = (Loc loc) =>
            {
                if (fillArray[loc.X - rectStart.X][loc.Y - rectStart.Y])
                    return true;
                return checkBlock(loc);
            };
            LocTest checkDiagChokeBlock = (Loc loc) =>
            {
                if (fillArray[loc.X - rectStart.X][loc.Y - rectStart.Y])
                    return true;
                return checkDiagBlock(loc);
            };
            LocAction fill = (Loc loc) =>
            {
                fillArray[loc.X][loc.Y] = true;
                if (forkList.Contains(loc))
                    forkList.Remove(loc);
            };
            FloodFill(new Rect(rectStart, rectSize), checkChokeBlock, checkDiagChokeBlock, fill, forkList[0]);

            return forkList.Count > 0;
        }

        public static List<Dir8> GetForkDirs(Loc point, LocTest checkBlock, LocTest checkDiagBlock)
        {
            List<Dir8> forks = new List<Dir8>();
            bool prevBlocked = IsDirBlocked(point, Dir8.Down, checkBlock, checkDiagBlock);
            int switches = 0;
            for (int ii = 0; ii < DirExt.VALID_DIR8.Length; ii++)
            {
                Dir8 dir = DirExt.VALID_DIR8[(ii + 1) % 8];
                bool newBlock = IsDirBlocked(point, dir, checkBlock, checkDiagBlock);
                if (newBlock != prevBlocked)
                {
                    switches++;
                    if (!newBlock)
                        forks.Add(dir);
                    prevBlocked = newBlock;
                }
            }
            return forks;
        }


        public static bool IsDirBlocked(Loc loc, Dir8 dir, LocTest checkBlock, LocTest checkDiagBlock)
        {
            return IsDirBlocked(loc, dir, checkBlock, checkDiagBlock, 1);
        }

        public static bool IsDirBlocked(Loc loc, Dir8 dir, LocTest checkBlock, LocTest checkDiagBlock, int distance)
        {
            if (!Enum.IsDefined(typeof(Dir8), dir))
                throw new ArgumentException("Invalid value to check.");
            else if (dir == Dir8.None)
                return false;

            for (int ii = 0; ii < distance; ii++)
            {
                if (dir.IsDiagonal())
                {
                    DirH horiz;
                    DirV vert;
                    dir.Separate(out horiz, out vert);

                    Loc diagLoc = loc + horiz.GetLoc();
                    if (checkDiagBlock(diagLoc))
                        return true;

                    diagLoc = loc + vert.GetLoc();
                    if (checkDiagBlock(diagLoc))
                        return true;
                }
                loc = loc + dir.GetLoc();

                if (checkBlock(loc))
                    return true;
            }

            return false;
        }

    }
}
