using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class RoomGenAngledHall<T> : PermissiveRoomGen<T> where T : ITiledGenContext
    {
        public int HallTurnBias;

        public RandRange Width;
        public RandRange Height;

        public RoomGenAngledHall() { }

        public RoomGenAngledHall(int turnBias)
        {
            HallTurnBias = turnBias;
        }

        public RoomGenAngledHall(int turnBias, RandRange width, RandRange height)
        {
            HallTurnBias = turnBias;
            Width = width;
            Height = height;
        }
        protected RoomGenAngledHall(RoomGenAngledHall<T> other)
        {
            HallTurnBias = other.HallTurnBias;
            Width = other.Width;
            Height = other.Height;
        }
        public override RoomGen<T> Copy() { return new RoomGenAngledHall<T>(this); }

        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(Width.Pick(rand), Height.Pick(rand));
        }

        public override void DrawOnMap(T map)
        {
            //check if there are any sides that have intersections such that straight lines are possible
            List<HashSet<int>>[] possibleStarts = new List<HashSet<int>>[4];
            for (int ii = 0; ii < 4; ii++)
            {
                Dir4 dir = (Dir4)ii;
                int scalarStart = Draw.Start.GetScalar(dir.ToAxis().Orth());
                possibleStarts[ii] = ChoosePossibleStartRanges(map.Rand, scalarStart, borderToFulfill[(int)dir], roomSideReqs[ii]);
            }

            if ((possibleStarts[(int)Dir4.Down].Count == 0) != (possibleStarts[(int)Dir4.Up].Count == 0) &&
                (possibleStarts[(int)Dir4.Left].Count == 0) != (possibleStarts[(int)Dir4.Right].Count == 0))
            {
                //right angle situation
                //HallTurnBias holds no sway here
                //Get the two directions
                List<Dir4> dirs = new List<Dir4>();
                List<int[]> dirStarts = new List<int[]>();
                for (int ii = 0; ii < possibleStarts.Length; ii++)
                {
                    //choose vertical starts if vertical, horiz starts if otherwise
                    if (possibleStarts[ii].Count > 0)
                    {
                        int[] starts = new int[possibleStarts[ii].Count];
                        //choose their start points at random
                        for (int jj = 0; jj < starts.Length; jj++)
                            starts[jj] = MathUtils.ChooseFromHash(possibleStarts[ii][jj], map.Rand);

                        dirs.Add((Dir4)ii);
                        dirStarts.Add(starts);
                    }
                }
                //make the one side extend up to the point where the closest halls would meet if they went straight
                //make the other side extend up to the point where the farthest halls would meet if they went straight
                //flip a coin to see which gets which
                bool extendFar = map.Rand.Next(2) == 0;
                int minMax1 = getHallMinMax(dirStarts[1], dirs[0].Reverse(), extendFar);
                DrawCombinedHall(map, dirs[0], minMax1, dirStarts[0]);
                int minMax2 = getHallMinMax(dirStarts[0], dirs[1].Reverse(), !extendFar);
                DrawCombinedHall(map, dirs[1], minMax2, dirStarts[1]);
                
                //TODO: a better way to resolve right-angle multi-halls
                //if there are NO opposite sides of sideReqs at all, we have a right angle situation
                //always connect the closest lines
                //for any additional lines, randomly select one on a side, and choose, up to the current connection point for the other dimension, where to add this line
                //which would then increase the connection point for this dimension.
                //then repeat the process
            }
            else
            {
                bool up = possibleStarts[(int)Dir4.Up].Count > 0;
                bool down = possibleStarts[(int)Dir4.Down].Count > 0;
                bool left = possibleStarts[(int)Dir4.Left].Count > 0;
                bool right = possibleStarts[(int)Dir4.Right].Count > 0;
                bool horiz = left && right;
                bool vert = down && up;
                if (!horiz && !vert)
                {
                    //if not a right angle situation, and no opposites here, then there's either 0 or 1 direction that the halls are coming from
                    bool hasHall = false;
                    //iterate through to find the one hall direction, and combine all of the halls (if applicable)
                    for (int ii = 0; ii < possibleStarts.Length; ii++)
                    {
                        //choose vertical starts if vertical, horiz starts if otherwise
                        if (possibleStarts[ii].Count > 0)
                        {
                            Dir4 dir = (Dir4)ii;
                            Range side = Draw.GetSide(dir.ToAxis().Orth());
                            int forwardEnd = map.Rand.Next(side.Min + 1, side.Max - 1);

                            //choose the starts
                            int[] starts = new int[possibleStarts[ii].Count];
                            for (int jj = 0; jj < possibleStarts[ii].Count; jj++)
                            {
                                hasHall = true;
                                starts[jj] = MathUtils.ChooseFromHash(possibleStarts[ii][jj], map.Rand);
                            }

                            DrawCombinedHall(map, dir, forwardEnd, starts);
                        }
                    }

                    //if there is no one hall, throw an error.  this room is MEANT to tie together adjacent rooms
                    if (!hasHall)
                        throw new ArgumentException("No rooms to connect.");
                }
                else
                {
                    //2-way (with opposites) to 4-way intersection
                    bool horizTurn = map.Rand.Next(100) < HallTurnBias;
                    bool vertTurn = map.Rand.Next(100) < HallTurnBias;

                    //force a turn if the sides cannot be connected by a straight line
                    //force a straight line if the sides both land on a single aligned tile
                    HashSet<int> horizCross = getIntersectedTiles(possibleStarts[(int)Dir4.Left], possibleStarts[(int)Dir4.Right]);
                    if (horizCross.Count == 0)
                        horizTurn = true;
                    else if (possibleStarts[(int)Dir4.Left].Count == 1 && possibleStarts[(int)Dir4.Right].Count == 1
                        && possibleStarts[(int)Dir4.Left][0].Count == 1 && possibleStarts[(int)Dir4.Right][0].Count == 1
                        && possibleStarts[(int)Dir4.Left][0].SetEquals(possibleStarts[(int)Dir4.Right][0]))
                        horizTurn = false;

                    HashSet<int> vertCross = getIntersectedTiles(possibleStarts[(int)Dir4.Down], possibleStarts[(int)Dir4.Up]);
                    if (vertCross.Count == 0)
                        vertTurn = true;
                    else if (possibleStarts[(int)Dir4.Down].Count == 1 && possibleStarts[(int)Dir4.Up].Count == 1
                        && possibleStarts[(int)Dir4.Down][0].Count == 1 && possibleStarts[(int)Dir4.Up][0].Count == 1
                        && possibleStarts[(int)Dir4.Down][0].SetEquals(possibleStarts[(int)Dir4.Up][0]))
                        vertTurn = false;

                    //in the case where one hall is crossed and another hall isn't, draw the crossed hall first
                    if (horiz && !vert)
                    {
                        drawPrimaryHall(map, horizCross, possibleStarts, false, horizTurn);
                        drawSecondaryHall(map, vertCross, possibleStarts, true, vertTurn);
                    }
                    else if (!horiz && vert)
                    {
                        drawPrimaryHall(map, vertCross, possibleStarts, true, vertTurn);
                        drawSecondaryHall(map, horizCross, possibleStarts, false, horizTurn);
                    }
                    else
                    {
                        //in the case where one hall is straight and another is angled, draw the straight one first
                        //if both are angled, you can draw any first (horiz by default)
                        //if both are straight, you can draw any first (horiz by default)
                        //if horiz is straight and vert is angled, draw horiz first
                        //if vert is straight and horiz is angled, draw vert first
                        if (!vertTurn && horizTurn)
                        {
                            drawPrimaryHall(map, vertCross, possibleStarts, true, vertTurn);
                            drawSecondaryHall(map, horizCross, possibleStarts, false, horizTurn);
                        }
                        else
                        {
                            drawPrimaryHall(map, horizCross, possibleStarts, false, horizTurn);
                            drawSecondaryHall(map, vertCross, possibleStarts, true, vertTurn);
                        }
                    }
                }
            }

            SetRoomBorders(map);
        }

        /// <summary>
        /// In a 4- or 3-way hall situation, this method is called to add the remaining ways after the first two have been added.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="room"></param>
        /// <param name="cross"></param>
        /// <param name="possibleStarts"></param>
        /// <param name="vertical"></param>
        /// <param name="turn"></param>
        private void drawSecondaryHall(T map, HashSet<int> cross, List<HashSet<int>>[] possibleStarts, bool vertical, bool turn)
        {
            if (!turn)
            {
                //if not turning, use the cross variables
                drawStraightHall(map, cross, vertical);
            }
            else
            {
                Dir4 forwardDir = vertical ? Dir4.Up : Dir4.Left;
                List<HashSet<int>> starts = possibleStarts[(int)forwardDir];
                List<HashSet<int>> ends = possibleStarts[(int)forwardDir.Reverse()];

                //the chosen tiles to start digging the hall from
                int[] startTiles = new int[starts.Count];
                int[] endTiles = new int[ends.Count];

                if (starts.Count == 1 && ends.Count == 1)
                {
                    choose1on1BentHallStarts(map, starts[0], ends[0], startTiles, endTiles);

                    //forward until hit
                    {
                        Loc forwardStart = Draw.Start;
                        Loc startLoc = new Loc(vertical ? startTiles[0] : forwardStart.X, vertical ? forwardStart.Y : startTiles[0]);
                        Loc endLoc = startLoc;
                        //the assumption is that there is already roomterrain to cross over at another point in this room
                        while (!map.GetTile(endLoc).TileEquivalent(map.RoomTerrain))
                            endLoc = endLoc + forwardDir.Reverse().GetLoc();
                        drawHall(map, startLoc, endLoc, map.RoomTerrain);
                    }

                    //backward until hit
                    {
                        Loc backwardStart = Draw.End - new Loc(1);
                        Loc startLoc = new Loc(vertical ? endTiles[0] : backwardStart.X, vertical ? backwardStart.Y : endTiles[0]);
                        Loc endLoc = startLoc;
                        //the assumption is that there is already roomterrain to cross over at another point in this room
                        while (!map.GetTile(endLoc).TileEquivalent(map.RoomTerrain))
                            endLoc = endLoc + forwardDir.GetLoc();
                        drawHall(map, startLoc, endLoc, map.RoomTerrain);
                    }
                }
                else
                {
                    //if turning, use the respective possible starts and draw until the primary lines are hit
                    for (int ii = 0; ii < possibleStarts.Length; ii++)
                    {
                        //choose vertical starts if vertical, horiz starts if otherwise
                        if ((((Dir4)ii).ToAxis() == Axis4.Vert) == vertical)
                        {
                            for (int jj = 0; jj < possibleStarts[ii].Count; jj++)
                            {
                                int[] crossArray = new int[possibleStarts[ii][jj].Count];
                                possibleStarts[ii][jj].CopyTo(crossArray);
                                int startSideDist = crossArray[map.Rand.Next(crossArray.Length)];
                                Loc forwardStart = (ii == (int)Dir4.Up || ii == (int)Dir4.Left) ? Draw.Start : Draw.End - new Loc(1);
                                Loc startLoc = new Loc(vertical ? startSideDist : forwardStart.X, vertical ? forwardStart.Y : startSideDist);
                                Loc endLoc = startLoc;
                                //the assumption is that there is already roomterrain to cross over at another point in this room
                                while (!map.GetTile(endLoc).TileEquivalent(map.RoomTerrain))
                                    endLoc = endLoc + ((Dir4)ii).Reverse().GetLoc();
                                drawHall(map, startLoc, endLoc, map.RoomTerrain);
                            }
                        }
                    }
                    //there is no guarantee that both sides will have an open bordertile; they'll just come in from their respective directions
                }
            }
        }

        /// <summary>
        /// Draws the hall connecting the first opposite pair of sides.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="room"></param>
        /// <param name="cross"></param>
        /// <param name="possibleStarts"></param>
        /// <param name="vertical"></param>
        /// <param name="turn"></param>
        private void drawPrimaryHall(T map, HashSet<int> cross, List<HashSet<int>>[] possibleStarts, bool vertical, bool turn)
        {
            if (!turn)
            {
                //if not turning, use the cross variables
                drawStraightHall(map, cross, vertical);
            }
            else
            {
                //if turning, use the respective possible starts
                //there is a guarantee that both sides have at least one open bordertile
                Dir4 forwardDir = vertical ? Dir4.Up : Dir4.Left;
                List<HashSet<int>> starts = possibleStarts[(int)forwardDir];
                List<HashSet<int>> ends = possibleStarts[(int)forwardDir.Reverse()];

                //the chosen tiles to start digging the hall from
                int[] startTiles = new int[starts.Count];
                int[] endTiles = new int[ends.Count];

                //TODO: when rolling start tiles or end tiles, prefer not to use the edge.
                if (starts.Count == 1 && ends.Count == 1)
                {
                    choose1on1BentHallStarts(map, starts[0], ends[0], startTiles, endTiles);
                }
                else
                {
                    //simply roll for all starts and ends
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

                //make the hall turn at a place not close to the edge, if possible
                int forwardTurn;
                if ((vertical ? Draw.End.Y - Draw.Y : Draw.End.X - Draw.X) > 2)
                    forwardTurn = map.Rand.Next((vertical ? Draw.Y : Draw.X) + 1, (vertical ? Draw.End.Y : Draw.End.X) - 1);
                else //otherwise, just use the full extent
                    forwardTurn = map.Rand.Next(vertical ? Draw.Y : Draw.X, vertical ? Draw.End.Y : Draw.End.X);

                //min and max used to determine the range of the turn hall
                int globalMin = startTiles[0];
                int globalMax = startTiles[0];

                for (int ii = 0; ii < startTiles.Length; ii++)
                {
                    globalMin = Math.Min(globalMin, startTiles[ii]);
                    globalMax = Math.Max(globalMax, startTiles[ii]);
                    Loc startLoc = new Loc(vertical ? startTiles[ii] : Draw.X, vertical ? Draw.Y : startTiles[ii]);
                    Loc endLoc = new Loc(vertical ? startTiles[ii] : forwardTurn, vertical ? forwardTurn : startTiles[ii]);
                    drawHall(map, startLoc, endLoc, map.RoomTerrain);
                }

                for (int ii = 0; ii < endTiles.Length; ii++)
                {
                    globalMin = Math.Min(globalMin, endTiles[ii]);
                    globalMax = Math.Max(globalMax, endTiles[ii]);
                    Loc startLoc = new Loc(vertical ? endTiles[ii] : Draw.End.X - 1, vertical ? Draw.End.Y - 1 : endTiles[ii]);
                    Loc endLoc = new Loc(vertical ? endTiles[ii] : forwardTurn, vertical ? forwardTurn : endTiles[ii]);
                    drawHall(map, startLoc, endLoc, map.RoomTerrain);
                }

                Loc startMid = new Loc(vertical ? globalMin : forwardTurn, vertical ? forwardTurn : globalMin);
                Loc endMid = new Loc(vertical ? globalMax : forwardTurn, vertical ? forwardTurn : globalMax);
                drawHall(map, startMid, endMid, map.RoomTerrain);
            }
        }


        private int getHallMinMax(int[] choices, Dir4 dir, bool extendFar)
        {
            bool useMax = extendFar;
            if (dir == Dir4.Up || dir == Dir4.Left)
                useMax = !useMax;
            int result = choices[0];
            for (int ii = 1; ii < choices.Length; ii++)
            {
                if (useMax)
                    result = Math.Max(choices[ii], result);
                else
                    result = Math.Min(choices[ii], result);
            }
            return result;
        }

        private void choose1on1BentHallStarts(T map, HashSet<int> starts, HashSet<int> ends, int[] startTiles, int[] endTiles)
        {
            //special case; make sure that start and end are NOT aligned to each other because we want a bend
            //This method is run with the assumption that the following is never true:
            //that there is both 1 start and end, and
            //there is only one tile for the start and the end, and
            //that one tile is the same tile

            //therefore, we must start off by choosing a tile from the pool that has less tiles
            //start by default, end if it's smaller.
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
        /// Draws a bundle of halls from one direction, going up to the specified point, and connects them.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="room"></param>
        /// <param name="dir"></param>
        /// <param name="forwardEnd"></param>
        /// <param name="starts"></param>
        public void DrawCombinedHall(ITiledGenContext map, Dir4 dir, int forwardEnd, int[] starts)
        {
            bool vertical = (dir.ToAxis() == Axis4.Vert);

            //Choose whether to start at the min X/Y, or the max X/Y
            Loc forwardStartLoc = (dir == Dir4.Up || dir == Dir4.Left) ? Draw.Start : Draw.End - new Loc(1);

            Range start = Draw.GetSide(dir.ToAxis());
            start.Max -= 1;
            start = new Range(start.Max, start.Min);
            //draw the halls
            for (int jj = 0; jj < starts.Length; jj++)
            {
                start.Min = Math.Min(start.Min, starts[jj]);
                start.Max = Math.Max(start.Max, starts[jj]);

                Loc startLoc = new Loc(vertical ? starts[jj] : forwardStartLoc.X, vertical ? forwardStartLoc.Y : starts[jj]);
                Loc endLoc = new Loc(vertical ? starts[jj] : forwardEnd, vertical ? forwardEnd : starts[jj]);
                drawHall(map, startLoc, endLoc, map.RoomTerrain);
            }

            //combine the halls
            Loc combineStart = new Loc(vertical ? start.Min : forwardEnd, vertical ? forwardEnd : start.Min);
            Loc combineEnd = new Loc(vertical ? start.Max : forwardEnd, vertical ? forwardEnd : start.Max);
            drawHall(map, combineStart, combineEnd, map.RoomTerrain);
        }

        /// <summary>
        /// Draws a single straight hall in the specified direction, choosing ONE of the scalars provided in cross.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="room"></param>
        /// <param name="cross"></param>
        /// <param name="vertical"></param>
        private void drawStraightHall(T map, HashSet<int> cross, bool vertical)
        {
            int startSideDist = MathUtils.ChooseFromHash(cross, map.Rand);
            Loc startLoc = new Loc(vertical ? startSideDist : Draw.X, vertical ? Draw.Y : startSideDist);
            Loc endLoc = new Loc(vertical ? startSideDist : Draw.End.X - 1, vertical ? Draw.End.Y - 1 : startSideDist);
            drawHall(map, startLoc, endLoc, map.RoomTerrain);
        }
        

        /// <summary>
        /// Returns the intersection of two hashsets IF they both contain only one hashset.  Returns an empty hashset otherwise.
        /// </summary>
        /// <param name="opening1"></param>
        /// <param name="opening2"></param>
        /// <returns></returns>
        private HashSet<int> getIntersectedTiles(List<HashSet<int>> opening1, List<HashSet<int>> opening2)
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
        
        /// <summary>
        /// Draws a hall in a straight cardinal direction, starting with one point and ending with another (inclusive).
        /// </summary>
        /// <param name="map"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="terrain"></param>
        private void drawHall(ITiledGenContext map, Loc point1, Loc point2, ITile terrain)
        {
            if (point1 == point2)
                map.SetTile(point1, terrain.Copy());
            else if (point1.X == point2.X)
            {
                if (point2.Y > point1.Y)
                {
                    for (int ii = point1.Y; ii <= point2.Y; ii++)
                        map.SetTile(new Loc(point1.X, ii), terrain.Copy());
                }
                else if (point2.Y < point1.Y)
                {
                    for (int ii = point1.Y; ii >= point2.Y; ii--)
                        map.SetTile(new Loc(point1.X, ii), terrain.Copy());
                }
            }
            else if (point1.Y == point2.Y)
            {
                if (point2.X > point1.X)
                {
                    for (int ii = point1.X; ii <= point2.X; ii++)
                        map.SetTile(new Loc(ii, point1.Y), terrain.Copy());
                }
                else if (point2.X < point1.X)
                {
                    for (int ii = point1.X; ii >= point2.X; ii--)
                        map.SetTile(new Loc(ii, point1.Y), terrain.Copy());
                }
            }
            GenContextDebug.DebugProgress("Hall Line");
        }
    }
}
