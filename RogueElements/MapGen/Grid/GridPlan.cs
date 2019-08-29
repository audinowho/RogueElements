// <copyright file="GridPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A dungeon layout that uses a rectangular array of rooms, connected to each other in cardinal directions.
    /// </summary>
    public class GridPlan
    {
        public GridPlan()
        {
        }

        public int CellWall { get; set; }

        public int WidthPerCell { get; set; }

        public int HeightPerCell { get; set; }

        public int GridWidth => this.Rooms.Length;

        public int GridHeight => this.Rooms[0].Length;

        public Loc Size
        {
            get
            {
                return new Loc(
                    (this.GridWidth * (this.WidthPerCell + this.CellWall)) - this.CellWall,
                    (this.GridHeight * (this.HeightPerCell + this.CellWall)) - this.CellWall);
            }
        }

        public int RoomCount => this.ArrayRooms.Count;

        protected int[][] Rooms { get; set; }

        protected GridHallPlan[][] VHalls { get; set; }

        protected GridHallPlan[][] HHalls { get; set; }

        // list of all rooms on the entire floor
        // each entry is a different room, guaranteed
        protected List<GridRoomPlan> ArrayRooms { get; set; }

        public void InitSize(int width, int height, int widthPerCell, int heightPerCell, int cellWall = 1)
        {
            this.Rooms = new int[width][];
            this.VHalls = new GridHallPlan[width][];
            this.HHalls = new GridHallPlan[width - 1][];
            for (int xx = 0; xx < width; xx++)
            {
                this.Rooms[xx] = new int[height];
                this.VHalls[xx] = new GridHallPlan[height - 1];
                if (xx < width - 1)
                    this.HHalls[xx] = new GridHallPlan[height];
                for (int yy = 0; yy < height; yy++)
                {
                    this.Rooms[xx][yy] = -1;
                    if (yy < height - 1)
                        this.VHalls[xx][yy] = new GridHallPlan();
                    if (xx < width - 1)
                        this.HHalls[xx][yy] = new GridHallPlan();
                }
            }

            this.ArrayRooms = new List<GridRoomPlan>();

            this.WidthPerCell = widthPerCell;
            this.HeightPerCell = heightPerCell;
            if (cellWall < 1)
                throw new ArgumentException("Cannot init a grid with cell wall < 1");
            this.CellWall = cellWall;
        }

        public void Clear()
        {
            int width = this.GridWidth;
            int height = this.GridHeight;

            this.Rooms = new int[width][];
            this.VHalls = new GridHallPlan[width][];
            this.HHalls = new GridHallPlan[width - 1][];
            for (int xx = 0; xx < width; xx++)
            {
                this.Rooms[xx] = new int[height];
                this.VHalls[xx] = new GridHallPlan[height - 1];
                if (xx < width - 1)
                    this.HHalls[xx] = new GridHallPlan[height];
                for (int yy = 0; yy < height; yy++)
                {
                    this.Rooms[xx][yy] = -1;
                    if (yy < height - 1)
                        this.VHalls[xx][yy] = new GridHallPlan();
                    if (xx < width - 1)
                        this.HHalls[xx][yy] = new GridHallPlan();
                }
            }

            this.ArrayRooms = new List<GridRoomPlan>();
        }

        /// <summary>
        /// Generates the position and size of each room and hall, and places it in the specified IFloorPlanGenContext.
        /// </summary>
        /// <param name="map"></param>
        public void PlaceRoomsOnFloor(IFloorPlanGenContext map)
        {
            // decide on room sizes
            for (int ii = 0; ii < this.ArrayRooms.Count; ii++)
                this.ChooseRoomBounds(map.Rand, ii);

            // decide on halls; write to RoomSideReqs
            for (int xx = 0; xx < this.VHalls.Length; xx++)
            {
                for (int yy = 0; yy < this.VHalls[xx].Length; yy++)
                    this.ChooseHallBounds(map.Rand, xx, yy, true);
            }

            for (int xx = 0; xx < this.HHalls.Length; xx++)
            {
                for (int yy = 0; yy < this.HHalls[xx].Length; yy++)
                    this.ChooseHallBounds(map.Rand, xx, yy, false);
            }

            GenContextDebug.StepIn("Main Rooms");

            List<RoomHallIndex> roomToHall = new List<RoomHallIndex>();
            foreach (var plan in this.ArrayRooms)
            {
                if (plan.CountsAsHall())
                {
                    roomToHall.Add(new RoomHallIndex(map.RoomPlan.HallCount, true));
                    map.RoomPlan.AddHall((IPermissiveRoomGen)plan.RoomGen);
                    GenContextDebug.DebugProgress("Add Hall Room");
                }
                else
                {
                    roomToHall.Add(new RoomHallIndex(map.RoomPlan.RoomCount, false));
                    map.RoomPlan.AddRoom(plan.RoomGen, plan.Immutable);
                    GenContextDebug.DebugProgress("Added Room");
                }
            }

            GenContextDebug.StepOut();

            GenContextDebug.StepIn("Connecting Halls");

            for (int xx = 0; xx < this.VHalls.Length; xx++)
            {
                for (int yy = 0; yy < this.VHalls[xx].Length; yy++)
                {
                    GridHallPlan hall = this.VHalls[xx][yy];
                    if (hall.MainGen != null)
                    {
                        for (int ii = 0; ii < hall.Gens.Count; ii++)
                        {
                            List<RoomHallIndex> adj = new List<RoomHallIndex>();
                            if (ii == 0)
                            {
                                int upRoom = this.Rooms[xx][yy];
                                if (upRoom > -1)
                                    adj.Add(roomToHall[upRoom]);
                            }
                            else
                            {
                                adj.Add(new RoomHallIndex(map.RoomPlan.HallCount - 1, true));
                            }

                            if (ii == hall.Gens.Count - 1)
                            {
                                int downRoom = this.Rooms[xx][yy + 1];
                                if (downRoom > -1)
                                    adj.Add(roomToHall[downRoom]);
                            }

                            map.RoomPlan.AddHall(hall.Gens[ii], adj.ToArray());
                            GenContextDebug.DebugProgress("Add Hall");
                        }
                    }
                }
            }

            for (int xx = 0; xx < this.HHalls.Length; xx++)
            {
                for (int yy = 0; yy < this.HHalls[xx].Length; yy++)
                {
                    GridHallPlan hall = this.HHalls[xx][yy];
                    if (hall.MainGen != null)
                    {
                        for (int ii = 0; ii < hall.Gens.Count; ii++)
                        {
                            List<RoomHallIndex> adj = new List<RoomHallIndex>();
                            if (ii == 0)
                            {
                                int leftRoom = this.Rooms[xx][yy];
                                if (leftRoom > -1)
                                    adj.Add(roomToHall[leftRoom]);
                            }
                            else
                            {
                                adj.Add(new RoomHallIndex(map.RoomPlan.HallCount - 1, true));
                            }

                            if (ii == hall.Gens.Count - 1)
                            {
                                int rightRoom = this.Rooms[xx + 1][yy];
                                if (rightRoom > -1)
                                    adj.Add(roomToHall[rightRoom]);
                            }

                            map.RoomPlan.AddHall(hall.Gens[ii], adj.ToArray());
                            GenContextDebug.DebugProgress("Add Hall");
                        }
                    }
                }
            }

            GenContextDebug.StepOut();
        }

        /// <summary>
        /// Returns the RoomGen found in the specified hall.
        /// </summary>
        /// <param name="locRay">The location of the room + the direction of the connecting hall relative to the room.</param>
        /// <returns></returns>
        public IPermissiveRoomGen GetHall(LocRay4 locRay)
        {
            switch (locRay.Dir)
            {
                case Dir4.Down:
                    if (locRay.Loc.Y < this.GridHeight - 1)
                        return this.VHalls[locRay.Loc.X][locRay.Loc.Y].MainGen;
                    break;
                case Dir4.Left:
                    if (locRay.Loc.X > 0)
                        return this.HHalls[locRay.Loc.X - 1][locRay.Loc.Y].MainGen;
                    break;
                case Dir4.Up:
                    if (locRay.Loc.Y > 0)
                        return this.VHalls[locRay.Loc.X][locRay.Loc.Y - 1].MainGen;
                    break;
                case Dir4.Right:
                    if (locRay.Loc.X < this.GridWidth - 1)
                        return this.HHalls[locRay.Loc.X][locRay.Loc.Y].MainGen;
                    break;
                case Dir4.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(locRay.Dir), "Invalid enum value.");
            }

            return null;
        }

        public IRoomGen GetRoom(int index)
        {
            return this.ArrayRooms[index].RoomGen;
        }

        public GridRoomPlan GetRoomPlan(int index)
        {
            return this.ArrayRooms[index];
        }

        public GridRoomPlan GetRoomPlan(Loc loc)
        {
            return this.Rooms[loc.X][loc.Y] > -1 ? this.ArrayRooms[this.Rooms[loc.X][loc.Y]] : null;
        }

        public int GetRoomIndex(Loc loc)
        {
            return this.Rooms[loc.X][loc.Y];
        }

        public void EraseRoom(Loc loc)
        {
            int roomIndex = this.Rooms[loc.X][loc.Y];
            GridRoomPlan room = this.ArrayRooms[roomIndex];
            this.ArrayRooms.RemoveAt(roomIndex);
            for (int xx = room.Bounds.Start.X; xx < room.Bounds.End.X; xx++)
            {
                for (int yy = room.Bounds.Start.Y; yy < room.Bounds.End.Y; yy++)
                    this.Rooms[xx][yy] = -1;
            }

            for (int xx = 0; xx < this.GridWidth; xx++)
            {
                for (int yy = 0; yy < this.GridHeight; yy++)
                {
                    if (this.Rooms[xx][yy] > roomIndex)
                        this.Rooms[xx][yy]--;
                }
            }
        }

        public void AddRoom(Loc loc, IRoomGen gen)
        {
            this.AddRoom(new Rect(loc, new Loc(1)), gen, false, false);
        }

        public void AddRoom(Loc loc, IRoomGen gen, bool immutable, bool preferHall)
        {
            this.AddRoom(new Rect(loc, new Loc(1)), gen, immutable, preferHall);
        }

        public void AddRoom(Rect rect, IRoomGen gen)
        {
            this.AddRoom(rect, gen, false, false);
        }

        public void AddRoom(Rect rect, IRoomGen gen, bool immutable, bool preferHall)
        {
            Rect floorRect = new Rect(0, 0, this.GridWidth, this.GridHeight);
            if (!floorRect.Contains(rect))
                throw new ArgumentOutOfRangeException(nameof(rect), "Cannot add room out of bounds!");

            for (int xx = rect.Start.X; xx < rect.End.X; xx++)
            {
                for (int yy = rect.Start.Y; yy < rect.End.Y; yy++)
                {
                    if (this.Rooms[xx][yy] != -1)
                        throw new InvalidOperationException("Tried to add on top of an existing room!");
                    if (xx > rect.Start.X && this.HHalls[xx - 1][yy].MainGen != null)
                        throw new InvalidOperationException("Tried to add on top of an existing hall!");
                    if (yy > rect.Start.Y && this.VHalls[xx][yy - 1].MainGen != null)
                        throw new InvalidOperationException("Tried to add on top of an existing hall!");
                }
            }

            var room = new GridRoomPlan(rect, gen.Copy())
            {
                Immutable = immutable,
                PreferHall = preferHall,
            };
            this.ArrayRooms.Add(room);
            for (int xx = rect.Start.X; xx < rect.End.X; xx++)
            {
                for (int yy = rect.Start.Y; yy < rect.End.Y; yy++)
                    this.Rooms[xx][yy] = this.ArrayRooms.Count - 1;
            }
        }

        public bool CanAddRoom(Rect rect)
        {
            Rect floorRect = new Rect(0, 0, this.GridWidth, this.GridHeight);
            if (!floorRect.Contains(rect))
                return false;

            for (int xx = rect.Start.X; xx < rect.End.X; xx++)
            {
                for (int yy = rect.Start.Y; yy < rect.End.Y; yy++)
                {
                    if (this.Rooms[xx][yy] != -1)
                        return false;
                    if (xx > rect.Start.X && this.HHalls[xx - 1][yy].MainGen != null)
                        return false;
                    if (yy > rect.Start.Y && this.VHalls[xx][yy - 1].MainGen != null)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sets the RoomGen found in the specified hall.
        /// </summary>
        /// <param name="locRay">The location of the room + the direction of the connecting hall relative to the room.</param>
        /// <param name="hallGen"></param>
        public void SetHall(LocRay4 locRay, IPermissiveRoomGen hallGen)
        {
            if (locRay.Dir == Dir4.None)
                throw new ArgumentException("Invalid direction.");
            else if (!locRay.Dir.Validate())
                throw new ArgumentOutOfRangeException("Invalid enum value.");

            IPermissiveRoomGen addHall = null;
            if (hallGen != null)
                addHall = (IPermissiveRoomGen)hallGen.Copy();
            switch (locRay.Dir)
            {
                case Dir4.Down:
                    if (locRay.Loc.Y < this.GridHeight - 1)
                        this.VHalls[locRay.Loc.X][locRay.Loc.Y].SetGen(addHall);
                    break;
                case Dir4.Left:
                    if (locRay.Loc.X > 0)
                        this.HHalls[locRay.Loc.X - 1][locRay.Loc.Y].SetGen(addHall);
                    break;
                case Dir4.Up:
                    if (locRay.Loc.Y > 0)
                        this.VHalls[locRay.Loc.X][locRay.Loc.Y - 1].SetGen(addHall);
                    break;
                case Dir4.Right:
                    if (locRay.Loc.X < this.GridWidth - 1)
                        this.HHalls[locRay.Loc.X][locRay.Loc.Y].SetGen(addHall);
                    break;
                case Dir4.None:
                    throw new ArgumentException($"No hall for dir {nameof(Dir4.None)}");
                default:
                    throw new ArgumentOutOfRangeException(nameof(locRay.Dir), "Invalid enum value.");
            }
        }

        /// <summary>
        /// Decides on the room bounds for each room.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="roomIndex"></param>
        public void ChooseRoomBounds(IRandom rand, int roomIndex)
        {
            GridRoomPlan roomPair = this.ArrayRooms[roomIndex];

            // the RoomGens are allowed to choose any size period, but this function will cap them at the cell sizes
            Loc size = roomPair.RoomGen.ProposeSize(rand);
            Rect cellBounds = this.GetCellBounds(roomPair.Bounds);
            size = new Loc(Math.Min(size.X, cellBounds.Width), Math.Min(size.Y, cellBounds.Height));
            roomPair.RoomGen.PrepareSize(rand, size);

            Loc start = cellBounds.Start + new Loc(rand.Next(cellBounds.Size.X - size.X + 1), rand.Next(cellBounds.Size.Y - size.Y + 1));
            roomPair.RoomGen.SetLoc(start);
        }

        /// <summary>
        /// Decides on the bounds for each hall.  Also writes to the adjacent rooms' SideReqs and tile permissions
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="vertical"></param>
        /// <param name="rand">todo: describe rand parameter on ChooseHallBounds</param>
        public void ChooseHallBounds(IRandom rand, int x, int y, bool vertical)
        {
            GridRoomPlan startRoom = this.GetRoomPlan(new Loc(x, y));
            GridRoomPlan endRoom = this.GetRoomPlan(new Loc(x + (vertical ? 0 : 1), y + (vertical ? 1 : 0)));

            GridHallPlan hall = vertical ? this.VHalls[x][y] : this.HHalls[x][y];
            if (hall.MainGen != null)
            {
                // also sets the sidereqs
                int tier = vertical ? x : y;
                Dir4 dir = vertical ? Dir4.Down : Dir4.Right;
                Range startRange = this.GetHallTouchRange(startRoom.RoomGen, dir, tier);
                Range endRange = this.GetHallTouchRange(endRoom.RoomGen, dir.Reverse(), tier);
                Range combinedRange = new Range(Math.Min(startRange.Min, endRange.Min), Math.Max(startRange.Max, endRange.Max));
                Loc start = startRoom.RoomGen.Draw.End;
                Loc end = endRoom.RoomGen.Draw.Start;

                Rect startCell = this.GetCellBounds(startRoom.Bounds);
                Rect endCell = this.GetCellBounds(endRoom.Bounds);

                Rect bounds = vertical ? new Rect(combinedRange.Min, start.Y, combinedRange.Length, end.Y - start.Y)
                    : new Rect(start.X, combinedRange.Min, end.X - start.X, combinedRange.Length);

                // a side constitutes intruding bound when the rectangle moves forward enough to go to the other side
                // and the other side touched is outside of side B's bound (including borders)

                // startRange intrudes if startRange goes outside end's tier (borders included)
                bool startIntrude = !endCell.GetSide(dir.ToAxis()).Contains(startRange);

                // and end is greater than the edge (borders excluded)
                bool endTouch = bounds.GetScalar(dir) == endCell.GetScalar(dir.Reverse());

                bool endIntrude = !startCell.GetSide(dir.ToAxis()).Contains(endRange);

                // and end is greater than the edge (borders excluded)
                bool startTouch = bounds.GetScalar(dir.Reverse()) == startCell.GetScalar(dir);

                // neither side intrudes bound: use the computed rectangle
                if ((!startIntrude && !endIntrude) || (endTouch && startTouch) ||
                    (!(startIntrude && endIntrude) && ((startIntrude && endTouch) || (endIntrude && startTouch))))
                {
                    hall.MainGen.PrepareSize(rand, bounds.Size);
                    hall.MainGen.SetLoc(bounds.Start);
                }
                else
                {
                    int divPoint = startCell.GetScalar(dir) + 1;
                    Range startDivRange = startRange;
                    Range endDivRange = endRange;
                    if (startIntrude && !endIntrude)
                    {
                        // side A intrudes bound, side B does not: divide A and B; doesn't matter who gets border
                        // side A touches border, side B does not: divide A and B; A gets border
                        //
                        // side A does not, side B touches border: A gets border; don't need B - this cannot happen
                        // side A touches border, side B touches border: A gets border; don't need B - this cannot happen
                        //
                        // in short, divide with start getting the border
                        // startDivRange needs to contain endRange
                        startDivRange = combinedRange;
                    }
                    else if (!startIntrude && endIntrude)
                    {
                        // side A does not, side B intrudes bound: divide A and B; doesn't matter who gets border
                        // side A does not, side B touches border: divide A and B; B gets border
                        //
                        // side A touches border, side B does not: B gets border; don't need A - this cannot happen
                        // side A touches border, side B touches border: B gets border; don't need B - this cannot happen
                        //
                        // in short, divide with end getting the border
                        // endDivRange needs to contain startRange
                        divPoint = startCell.GetScalar(dir);
                        endDivRange = combinedRange;
                    }
                    else
                    {
                        // side A intrudes bound, side B intrudes bound: divide A and B; doesn't matter who gets border
                        if (startTouch)
                        {
                            // side A touches border, side B does not: divide A and B; A gets border
                        }

                        if (endTouch)
                        {
                            // side A does not, side B touches border: divide A and B; B gets border
                            divPoint = startCell.GetScalar(dir);
                        }

                        // side A touches border, side B touches border: A gets border; don't need B -  this cannot happen
                        // both sides need to cover the intersection of their cells
                        Range interCellSide = Range.Intersect(startCell.GetSide(dir.ToAxis()), endCell.GetSide(dir.ToAxis()));
                        startDivRange = Range.IncludeRange(startDivRange, interCellSide);
                        endDivRange = Range.IncludeRange(endDivRange, interCellSide);
                    }

                    Rect startBox = vertical ? new Rect(startDivRange.Min, start.Y, startDivRange.Length, divPoint - start.Y)
                        : new Rect(start.X, startDivRange.Min, divPoint - start.X, startDivRange.Length);
                    Rect endBox = vertical ? new Rect(endDivRange.Min, divPoint, endDivRange.Length, end.Y - divPoint)
                        : new Rect(divPoint, endDivRange.Min, end.X - divPoint, endDivRange.Length);

                    hall.Gens.Add((IPermissiveRoomGen)hall.MainGen.Copy());
                    hall.Gens[0].PrepareSize(rand, startBox.Size);
                    hall.Gens[0].SetLoc(startBox.Start);
                    hall.Gens[1].PrepareSize(rand, endBox.Size);
                    hall.Gens[1].SetLoc(endBox.Start);
                }
            }
        }

        public List<int> GetAdjacentRooms(int roomIndex)
        {
            List<int> returnList = new List<int>();
            GridRoomPlan room = this.ArrayRooms[roomIndex];
            for (int ii = 0; ii < room.Bounds.Size.X; ii++)
            {
                // above
                int up = this.GetRoomNum(new LocRay4(room.Bounds.X + ii, room.Bounds.Y, Dir4.Up));
                if (up > -1 && !returnList.Contains(up))
                    returnList.Add(up);

                // below
                int down = this.GetRoomNum(new LocRay4(room.Bounds.X + ii, room.Bounds.End.Y - 1, Dir4.Down));
                if (down > -1 && !returnList.Contains(down))
                    returnList.Add(down);
            }

            for (int ii = 0; ii < room.Bounds.Size.Y; ii++)
            {
                // left
                int left = this.GetRoomNum(new LocRay4(room.Bounds.X, room.Bounds.Y + ii, Dir4.Left));
                if (left > -1 && !returnList.Contains(left))
                    returnList.Add(left);

                // right
                int right = this.GetRoomNum(new LocRay4(room.Bounds.End.X - 1, room.Bounds.Y + ii, Dir4.Right));
                if (right > -1 && !returnList.Contains(right))
                    returnList.Add(right);
            }

            return returnList;
        }

        public int GetRoomNum(LocRay4 locRay)
        {
            IPermissiveRoomGen hall = this.GetHall(locRay);
            if (hall != null)
            {
                Loc moveLoc = locRay.Traverse(1);
                return this.Rooms[moveLoc.X][moveLoc.Y];
            }

            return -1;
        }

        public virtual Rect GetCellBounds(Rect bounds)
        {
            return new Rect(
                bounds.X * (this.WidthPerCell + this.CellWall),
                bounds.Y * (this.HeightPerCell + this.CellWall),
                (bounds.Size.X * (this.WidthPerCell + this.CellWall)) - this.CellWall,
                (bounds.Size.Y * (this.HeightPerCell + this.CellWall)) - this.CellWall);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="room"></param>
        /// <param name="dir">Direction from room to hall.</param>
        /// <param name="tier"></param>
        /// <returns></returns>
        public virtual Range GetHallTouchRange(IRoomGen room, Dir4 dir, int tier)
        {
            bool vertical = dir.ToAxis() == Axis4.Vert;

            // The hall will touch the entire side of each room under normal circumstances.
            // Things get tricky for a target room that occupies more than one cell.
            // First, try to cover only the part of the target room that's in the cell.
            // If that's not possible, try to extend the hall until it covers one tile of the target room.
            Range startRange = room.Draw.GetSide(dir.ToAxis());

            // factor possibletiles into this calculation
            int borderLength = room.GetBorderLength(dir);
            for (int ii = 0; ii < borderLength; ii++)
            {
                if (room.GetFulfillableBorder(dir, ii))
                {
                    startRange.Min += ii;
                    break;
                }
            }

            for (int ii = 0; ii < borderLength; ii++)
            {
                if (room.GetFulfillableBorder(dir, borderLength - 1 - ii))
                {
                    startRange.Max -= ii;
                    break;
                }
            }

            int tierStart = vertical ? tier * (this.WidthPerCell + this.CellWall) : tier * (this.HeightPerCell + this.CellWall);
            int tierLength = vertical ? this.WidthPerCell : this.HeightPerCell;
            Range newRange = new Range(Math.Max(startRange.Min, tierStart), Math.Min(startRange.Max, tierStart + tierLength));
            if (newRange.Max <= newRange.Min)
            {
                // try to extend the hall until it covers one tile of the target room.
                // first, note that the current end range is covering the zone between the tier and the edge of the room (inverted)
                // un-invert this and inflate by 1, and you will have a zone that covers 1 tile with the room
                // get the intersection of this zone and the room.
                newRange = new Range(Math.Max(startRange.Min, newRange.Max - 1), Math.Min(startRange.Max, newRange.Min + 1));
            }

            return newRange;
        }
    }
}
