// <copyright file="RoomGenAngledHall.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A room that connects its exits with a narrow hallway.
    /// It is able to handle all combinations of exits from all combination of directions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RoomGenAngledHall<T> : PermissiveRoomGen<T>
        where T : ITiledGenContext
    {
        public RoomGenAngledHall()
        {
            this.Brush = new DefaultHallBrush();
        }

        public RoomGenAngledHall(int turnBias)
        {
            this.HallTurnBias = turnBias;
            this.Brush = new DefaultHallBrush();
        }

        public RoomGenAngledHall(int turnBias, BaseHallBrush brush)
        {
            this.HallTurnBias = turnBias;
            this.Brush = brush;
        }

        public RoomGenAngledHall(int turnBias, RandRange width, RandRange height)
        {
            this.HallTurnBias = turnBias;
            this.Brush = new DefaultHallBrush();
            this.Width = width;
            this.Height = height;
        }

        protected RoomGenAngledHall(RoomGenAngledHall<T> other)
        {
            this.HallTurnBias = other.HallTurnBias;
            this.Brush = other.Brush.Clone();
            this.Width = other.Width;
            this.Height = other.Height;
        }

        /// <summary>
        /// A percentage chance 0 to 100 for the hall making a turn.
        /// </summary>
        public int HallTurnBias { get; set; }

        /// <summary>
        /// The brush to draw the hall with.
        /// </summary>
        public BaseHallBrush Brush { get; set; }

        /// <summary>
        /// The preferred width of the area covered by the hall.
        /// </summary>
        public RandRange Width { get; set; }

        /// <summary>
        /// The preferred height of the area covered by the hall.
        /// </summary>
        public RandRange Height { get; set; }

        public override RoomGen<T> Copy() => new RoomGenAngledHall<T>(this);

        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(this.Width.Pick(rand), this.Height.Pick(rand));
        }

        public override void DrawOnMap(T map)
        {
            // check if there are any sides that have intersections such that straight lines are possible
            var possibleStarts = new Dictionary<Dir4, List<HashSet<int>>>();
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                int scalarStart = this.Draw.Start.GetScalar(dir.ToAxis().Orth());

                // modify the sidereqs: shorten them to accomodate the brush size
                // if the sidereq cannot be shortened further than a width of 1, just use 1
                // the result will have to a degenerate hall, but the important thing is that it will still be functional.
                int center = this.Brush.Center.GetScalar(dir.ToAxis().Orth());
                int width = this.Brush.Size.GetScalar(dir.ToAxis().Orth());
                List<IntRange> origReqs = this.RoomSideReqs[dir];
                List<IntRange> moddedReqs = new List<IntRange>();
                foreach (IntRange range in origReqs)
                {
                    IntRange newRange = range;
                    newRange.Min = Math.Min(newRange.Min + center, newRange.Max - 1);
                    newRange.Max = Math.Max(newRange.Max + center + 1 - width, newRange.Min + 1);
                    moddedReqs.Add(newRange);
                }

                possibleStarts[dir] = this.ChoosePossibleStartRanges(map.Rand, scalarStart, this.BorderToFulfill[dir], moddedReqs);
            }

            if ((possibleStarts[Dir4.Down].Count == 0) != (possibleStarts[Dir4.Up].Count == 0) &&
                (possibleStarts[Dir4.Left].Count == 0) != (possibleStarts[Dir4.Right].Count == 0))
            {
                // right angle situation
                // HallTurnBias holds no sway here
                // Get the two directions
                List<Dir4> dirs = new List<Dir4>();
                List<int[]> dirStarts = new List<int[]>();
                foreach (Dir4 dir in DirExt.VALID_DIR4)
                {
                    // choose vertical starts if vertical, horiz starts if otherwise
                    if (possibleStarts[dir].Count > 0)
                    {
                        int[] starts = new int[possibleStarts[dir].Count];

                        // choose their start points at random
                        for (int jj = 0; jj < starts.Length; jj++)
                            starts[jj] = MathUtils.ChooseFromHash(possibleStarts[dir][jj], map.Rand);

                        dirs.Add(dir);
                        dirStarts.Add(starts);
                    }
                }

                // make the one side extend up to the point where the closest halls would meet if they went straight
                // make the other side extend up to the point where the farthest halls would meet if they went straight
                // flip a coin to see which gets which
                bool extendFar = map.Rand.Next(2) == 0;
                int minMax1 = GetHallMinMax(dirStarts[1], dirs[0].Reverse(), extendFar);
                this.DrawCombinedHall(map, dirs[0], minMax1, dirStarts[0]);
                int minMax2 = GetHallMinMax(dirStarts[0], dirs[1].Reverse(), !extendFar);
                this.DrawCombinedHall(map, dirs[1], minMax2, dirStarts[1]);

                // TODO: a better way to resolve right-angle multi-halls
                // if there are NO opposite sides of sideReqs at all, we have a right angle situation
                // always connect the closest lines
                // for any additional lines, randomly select one on a side, and choose, up to the current connection point for the other dimension, where to add this line
                // which would then increase the connection point for this dimension.
                // then repeat the process
            }
            else
            {
                bool up = possibleStarts[Dir4.Up].Count > 0;
                bool down = possibleStarts[Dir4.Down].Count > 0;
                bool left = possibleStarts[Dir4.Left].Count > 0;
                bool right = possibleStarts[Dir4.Right].Count > 0;
                bool horiz = left && right;
                bool vert = down && up;
                if (!horiz && !vert)
                {
                    // if not a right angle situation, and no opposites here, then there's either 0 or 1 direction that the halls are coming from
                    bool hasHall = false;

                    // iterate through to find the one hall direction, and combine all of the halls (if applicable)
                    foreach (Dir4 dir in DirExt.VALID_DIR4)
                    {
                        // choose vertical starts if vertical, horiz starts if otherwise
                        if (possibleStarts[dir].Count > 0)
                        {
                            IntRange side = this.Draw.GetSide(dir.ToAxis().Orth());
                            int forwardEnd = map.Rand.Next(side.Min + 1, side.Max - 1);

                            // choose the starts
                            int[] starts = new int[possibleStarts[dir].Count];
                            for (int jj = 0; jj < possibleStarts[dir].Count; jj++)
                            {
                                hasHall = true;
                                starts[jj] = MathUtils.ChooseFromHash(possibleStarts[dir][jj], map.Rand);
                            }

                            this.DrawCombinedHall(map, dir, forwardEnd, starts);
                        }
                    }

                    // if there is no one hall, throw an error.  this room is MEANT to tie together adjacent rooms
                    if (!hasHall)
                        throw new ArgumentException("No rooms to connect.");
                }
                else
                {
                    // 2-way (with opposites) to 4-way intersection
                    bool horizTurn = map.Rand.Next(100) < this.HallTurnBias;
                    bool vertTurn = map.Rand.Next(100) < this.HallTurnBias;

                    // force a turn if the sides cannot be connected by a straight line
                    // force a straight line if the sides both land on a single aligned tile
                    HashSet<int> horizCross = GetIntersectedTiles(possibleStarts[Dir4.Left], possibleStarts[Dir4.Right]);
                    if (horizCross.Count == 0)
                        horizTurn = true;
                    else if (possibleStarts[Dir4.Left].Count == 1 && possibleStarts[Dir4.Right].Count == 1
                        && possibleStarts[Dir4.Left][0].Count == 1 && possibleStarts[Dir4.Right][0].Count == 1
                        && possibleStarts[Dir4.Left][0].SetEquals(possibleStarts[Dir4.Right][0]))
                        horizTurn = false;

                    HashSet<int> vertCross = GetIntersectedTiles(possibleStarts[Dir4.Down], possibleStarts[Dir4.Up]);
                    if (vertCross.Count == 0)
                        vertTurn = true;
                    else if (possibleStarts[Dir4.Down].Count == 1 && possibleStarts[Dir4.Up].Count == 1
                        && possibleStarts[Dir4.Down][0].Count == 1 && possibleStarts[Dir4.Up][0].Count == 1
                        && possibleStarts[Dir4.Down][0].SetEquals(possibleStarts[Dir4.Up][0]))
                        vertTurn = false;

                    // in the case where one hall is crossed and another hall isn't, draw the crossed hall first
                    if (horiz && !vert)
                    {
                        this.DrawPrimaryHall(map, horizCross, possibleStarts, false, horizTurn);
                        this.DrawSecondaryHall(map, vertCross, possibleStarts, true, vertTurn);
                    }
                    else if (!horiz && vert)
                    {
                        this.DrawPrimaryHall(map, vertCross, possibleStarts, true, vertTurn);
                        this.DrawSecondaryHall(map, horizCross, possibleStarts, false, horizTurn);
                    }
                    else
                    {
                        // in the case where one hall is straight and another is angled, draw the straight one first
                        // if both are angled, you can draw any first (horiz by default)
                        // if both are straight, you can draw any first (horiz by default)
                        // if horiz is straight and vert is angled, draw horiz first
                        // if vert is straight and horiz is angled, draw vert first
                        if (!vertTurn && horizTurn)
                        {
                            this.DrawPrimaryHall(map, vertCross, possibleStarts, true, vertTurn);
                            this.DrawSecondaryHall(map, horizCross, possibleStarts, false, horizTurn);
                        }
                        else
                        {
                            this.DrawPrimaryHall(map, horizCross, possibleStarts, false, horizTurn);
                            this.DrawSecondaryHall(map, vertCross, possibleStarts, true, vertTurn);
                        }
                    }
                }
            }

            this.SetRoomBorders(map);
        }

        /// <summary>
        /// Draws a bundle of halls from one direction, going up to the specified point, and connects them.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="dir"></param>
        /// <param name="forwardEnd"></param>
        /// <param name="starts"></param>
        public void DrawCombinedHall(ITiledGenContext map, Dir4 dir, int forwardEnd, int[] starts)
        {
            bool vertical = dir.ToAxis() == Axis4.Vert;

            // Choose whether to start at the min X/Y, or the max X/Y
            Loc forwardStartLoc = (dir == Dir4.Up || dir == Dir4.Left) ? this.Draw.Start : this.Draw.End - new Loc(1);

            IntRange start = this.Draw.GetSide(dir.ToAxis());
            start.Max -= 1;
            start = new IntRange(start.Max, start.Min);

            // draw the halls
            for (int jj = 0; jj < starts.Length; jj++)
            {
                start.Min = Math.Min(start.Min, starts[jj]);
                start.Max = Math.Max(start.Max, starts[jj]);

                Loc startLoc = new Loc(vertical ? starts[jj] : forwardStartLoc.X, vertical ? forwardStartLoc.Y : starts[jj]);
                Loc endLoc = new Loc(vertical ? starts[jj] : forwardEnd, vertical ? forwardEnd : starts[jj]);
                this.DrawHall(map, startLoc, endLoc, vertical);
            }

            // combine the halls
            Loc combineStart = new Loc(vertical ? start.Min : forwardEnd, vertical ? forwardEnd : start.Min);
            Loc combineEnd = new Loc(vertical ? start.Max : forwardEnd, vertical ? forwardEnd : start.Max);
            this.DrawHall(map, combineStart, combineEnd, !vertical);
        }

        public override string ToString()
        {
            return string.Format("{0}: Angle:{1}%", this.GetType().Name, this.HallTurnBias);
        }

        private static void Choose1on1BentHallStarts(T map, HashSet<int> starts, HashSet<int> ends, int[] startTiles, int[] endTiles)
        {
            // special case; make sure that start and end are NOT aligned to each other because we want a bend
            // This method is run with the assumption that the following is never true:
            // that there is both 1 start and end, and
            // there is only one tile for the start and the end, and
            // that one tile is the same tile
            //
            // therefore, we must start off by choosing a tile from the pool that has less tiles
            // start by default, end if it's smaller.
            if (starts.Count > ends.Count)
            {
                int[] crossArray = new int[ends.Count];
                ends.CopyTo(crossArray);
                endTiles[0] = crossArray[map.Rand.Next(crossArray.Length)];

                HashSet<int> new_start = new HashSet<int>(starts);
                new_start.Remove(endTiles[0]);
                crossArray = new int[new_start.Count];
                new_start.CopyTo(crossArray);
                startTiles[0] = crossArray[map.Rand.Next(crossArray.Length)];
            }
            else
            {
                int[] crossArray = new int[starts.Count];
                starts.CopyTo(crossArray);
                startTiles[0] = crossArray[map.Rand.Next(crossArray.Length)];

                HashSet<int> new_end = new HashSet<int>(ends);
                new_end.Remove(startTiles[0]);
                crossArray = new int[new_end.Count];
                new_end.CopyTo(crossArray);
                endTiles[0] = crossArray[map.Rand.Next(crossArray.Length)];
            }
        }

        /// <summary>
        /// Returns the intersection of two hashsets IF they both contain only one hashset.  Returns an empty hashset otherwise.
        /// </summary>
        /// <param name="opening1"></param>
        /// <param name="opening2"></param>
        /// <returns></returns>
        private static HashSet<int> GetIntersectedTiles(List<HashSet<int>> opening1, List<HashSet<int>> opening2)
        {
            HashSet<int> intersect = new HashSet<int>();
            if (opening1.Count == 1 && opening2.Count == 1)
            {
                foreach (int element in opening1[0])
                    intersect.Add(element);
                intersect.IntersectWith(opening2[0]);
            }

            return intersect;
        }

        private static int GetHallMinMax(int[] choices, Dir4 dir, bool extendFar)
        {
            bool useMax = extendFar;
            if (dir == Dir4.Up || dir == Dir4.Left)
                useMax = !useMax;
            int result = choices[0];
            for (int ii = 1; ii < choices.Length; ii++)
            {
                result = useMax ? Math.Max(choices[ii], result) : Math.Min(choices[ii], result);
            }

            return result;
        }

        /// <summary>
        /// Draws a hall in a straight cardinal direction, starting with one point and ending with another (inclusive).
        /// </summary>
        /// <param name="map"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="vertical"></param>
        private void DrawHall(ITiledGenContext map, Loc point1, Loc point2, bool vertical)
        {
            if (point1 == point2)
            {
                this.Brush.DrawHallBrush(map, this.Draw, point1, vertical);
            }
            else if (point1.X == point2.X)
            {
                if (point2.Y > point1.Y)
                {
                    for (int ii = point1.Y; ii <= point2.Y; ii++)
                        this.Brush.DrawHallBrush(map, this.Draw, new Loc(point1.X, ii), vertical);
                }
                else if (point2.Y < point1.Y)
                {
                    for (int ii = point1.Y; ii >= point2.Y; ii--)
                        this.Brush.DrawHallBrush(map, this.Draw, new Loc(point1.X, ii), vertical);
                }
            }
            else if (point1.Y == point2.Y)
            {
                if (point2.X > point1.X)
                {
                    for (int ii = point1.X; ii <= point2.X; ii++)
                        this.Brush.DrawHallBrush(map, this.Draw, new Loc(ii, point1.Y), vertical);
                }
                else if (point2.X < point1.X)
                {
                    for (int ii = point1.X; ii >= point2.X; ii--)
                        this.Brush.DrawHallBrush(map, this.Draw, new Loc(ii, point1.Y), vertical);
                }
            }

            GenContextDebug.DebugProgress("Hall Line");
        }

        /// <summary>
        /// In a 4- or 3-way hall situation, this method is called to add the remaining ways after the first two have been added.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="cross"></param>
        /// <param name="possibleStarts"></param>
        /// <param name="vertical"></param>
        /// <param name="turn"></param>
        private void DrawSecondaryHall(T map, HashSet<int> cross, Dictionary<Dir4, List<HashSet<int>>> possibleStarts, bool vertical, bool turn)
        {
            if (!turn)
            {
                // if not turning, use the cross variables
                this.DrawStraightHall(map, cross, vertical);
            }
            else
            {
                Dir4 forwardDir = vertical ? Dir4.Up : Dir4.Left;
                List<HashSet<int>> starts = possibleStarts[forwardDir];
                List<HashSet<int>> ends = possibleStarts[forwardDir.Reverse()];

                // the chosen tiles to start digging the hall from
                int[] startTiles = new int[starts.Count];
                int[] endTiles = new int[ends.Count];

                if (starts.Count == 1 && ends.Count == 1)
                {
                    Choose1on1BentHallStarts(map, starts[0], ends[0], startTiles, endTiles);

                    // forward until hit
                    {
                        Loc forwardStart = this.Draw.Start;
                        Loc startLoc = new Loc(vertical ? startTiles[0] : forwardStart.X, vertical ? forwardStart.Y : startTiles[0]);
                        Loc endLoc = startLoc;

                        // the assumption is that there is already roomterrain to cross over at another point in this room
                        while (!map.RoomTerrain.TileEquivalent(map.GetTile(endLoc)))
                            endLoc += forwardDir.Reverse().GetLoc();
                        this.DrawHall(map, startLoc, endLoc, vertical);
                    }

                    // backward until hit
                    {
                        Loc backwardStart = this.Draw.End - new Loc(1);
                        Loc startLoc = new Loc(vertical ? endTiles[0] : backwardStart.X, vertical ? backwardStart.Y : endTiles[0]);
                        Loc endLoc = startLoc;

                        // the assumption is that there is already roomterrain to cross over at another point in this room
                        while (!map.RoomTerrain.TileEquivalent(map.GetTile(endLoc)))
                            endLoc += forwardDir.GetLoc();
                        this.DrawHall(map, startLoc, endLoc, vertical);
                    }
                }
                else
                {
                    // if turning, use the respective possible starts and draw until the primary lines are hit
                    foreach (Dir4 dir in DirExt.VALID_DIR4)
                    {
                        // choose vertical starts if vertical, horiz starts if otherwise
                        if ((dir.ToAxis() == Axis4.Vert) == vertical)
                        {
                            for (int jj = 0; jj < possibleStarts[dir].Count; jj++)
                            {
                                int[] crossArray = new int[possibleStarts[dir][jj].Count];
                                possibleStarts[dir][jj].CopyTo(crossArray);
                                int startSideDist = crossArray[map.Rand.Next(crossArray.Length)];
                                Loc forwardStart = (dir == Dir4.Up || dir == Dir4.Left) ? this.Draw.Start : this.Draw.End - new Loc(1);
                                Loc startLoc = new Loc(vertical ? startSideDist : forwardStart.X, vertical ? forwardStart.Y : startSideDist);
                                Loc endLoc = startLoc;

                                // the assumption is that there is already roomterrain to cross over at another point in this room
                                while (!map.RoomTerrain.TileEquivalent(map.GetTile(endLoc)))
                                    endLoc += dir.Reverse().GetLoc();

                                this.DrawHall(map, startLoc, endLoc, vertical);
                            }
                        }
                    }

                    // there is no guarantee that both sides will have an open bordertile; they'll just come in from their respective directions
                }
            }
        }

        /// <summary>
        /// Draws the hall connecting the first opposite pair of sides.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="cross"></param>
        /// <param name="possibleStarts"></param>
        /// <param name="vertical"></param>
        /// <param name="turn"></param>
        private void DrawPrimaryHall(T map, HashSet<int> cross, Dictionary<Dir4, List<HashSet<int>>> possibleStarts, bool vertical, bool turn)
        {
            if (!turn)
            {
                // if not turning, use the cross variables
                this.DrawStraightHall(map, cross, vertical);
            }
            else
            {
                // if turning, use the respective possible starts
                // there is a guarantee that both sides have at least one open bordertile
                Dir4 forwardDir = vertical ? Dir4.Up : Dir4.Left;
                List<HashSet<int>> starts = possibleStarts[forwardDir];
                List<HashSet<int>> ends = possibleStarts[forwardDir.Reverse()];

                // the chosen tiles to start digging the hall from
                int[] startTiles = new int[starts.Count];
                int[] endTiles = new int[ends.Count];

                // TODO: when rolling start tiles or end tiles, prefer not to use the edge.
                if (starts.Count == 1 && ends.Count == 1)
                {
                    Choose1on1BentHallStarts(map, starts[0], ends[0], startTiles, endTiles);
                }
                else
                {
                    // simply roll for all starts and ends
                    for (int jj = 0; jj < starts.Count; jj++)
                    {
                        int[] crossArray = new int[starts[jj].Count];
                        starts[jj].CopyTo(crossArray);
                        startTiles[jj] = crossArray[map.Rand.Next(crossArray.Length)];
                    }

                    for (int jj = 0; jj < ends.Count; jj++)
                    {
                        int[] crossArray = new int[ends[jj].Count];
                        ends[jj].CopyTo(crossArray);
                        endTiles[jj] = crossArray[map.Rand.Next(crossArray.Length)];
                    }
                }

                // make the hall turn at a place not close to the edge, if possible
                int forwardTurn;
                if ((vertical ? this.Draw.End.Y - this.Draw.Y : this.Draw.End.X - this.Draw.X) > 2)
                    forwardTurn = map.Rand.Next((vertical ? this.Draw.Y : this.Draw.X) + 1, (vertical ? this.Draw.End.Y : this.Draw.End.X) - 1);
                else // otherwise, just use the full extent
                    forwardTurn = map.Rand.Next(vertical ? this.Draw.Y : this.Draw.X, vertical ? this.Draw.End.Y : this.Draw.End.X);

                // min and max used to determine the range of the turn hall
                int globalMin = startTiles[0];
                int globalMax = startTiles[0];

                for (int ii = 0; ii < startTiles.Length; ii++)
                {
                    globalMin = Math.Min(globalMin, startTiles[ii]);
                    globalMax = Math.Max(globalMax, startTiles[ii]);
                    Loc startLoc = new Loc(vertical ? startTiles[ii] : this.Draw.X, vertical ? this.Draw.Y : startTiles[ii]);
                    Loc endLoc = new Loc(vertical ? startTiles[ii] : forwardTurn, vertical ? forwardTurn : startTiles[ii]);
                    this.DrawHall(map, startLoc, endLoc, vertical);
                }

                for (int ii = 0; ii < endTiles.Length; ii++)
                {
                    globalMin = Math.Min(globalMin, endTiles[ii]);
                    globalMax = Math.Max(globalMax, endTiles[ii]);
                    Loc startLoc = new Loc(vertical ? endTiles[ii] : this.Draw.End.X - 1, vertical ? this.Draw.End.Y - 1 : endTiles[ii]);
                    Loc endLoc = new Loc(vertical ? endTiles[ii] : forwardTurn, vertical ? forwardTurn : endTiles[ii]);
                    this.DrawHall(map, startLoc, endLoc, vertical);
                }

                Loc startMid = new Loc(vertical ? globalMin : forwardTurn, vertical ? forwardTurn : globalMin);
                Loc endMid = new Loc(vertical ? globalMax : forwardTurn, vertical ? forwardTurn : globalMax);
                this.DrawHall(map, startMid, endMid, !vertical);
            }
        }

        /// <summary>
        /// Draws a single straight hall in the specified direction, choosing ONE of the scalars provided in cross.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="cross"></param>
        /// <param name="vertical"></param>
        private void DrawStraightHall(T map, HashSet<int> cross, bool vertical)
        {
            int startSideDist = MathUtils.ChooseFromHash(cross, map.Rand);
            Loc startLoc = new Loc(vertical ? startSideDist : this.Draw.X, vertical ? this.Draw.Y : startSideDist);
            Loc endLoc = new Loc(vertical ? startSideDist : this.Draw.End.X - 1, vertical ? this.Draw.End.Y - 1 : startSideDist);
            this.DrawHall(map, startLoc, endLoc, vertical);
        }
    }
}
