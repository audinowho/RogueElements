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
                    (this.GridWidth * (this.WidthPerCell + this.CellWall)) - (this.Wrap ? 0 : this.CellWall),
                    (this.GridHeight * (this.HeightPerCell + this.CellWall)) - (this.Wrap ? 0 : this.CellWall));
            }
        }

        public bool Wrap { get; set; }

        public int RoomCount => this.ArrayRooms.Count;

        protected int[][] Rooms { get; set; }

        protected GridHallGroup[][] VHalls { get; set; }

        protected GridHallGroup[][] HHalls { get; set; }

        // list of all rooms on the entire floor
        // each entry is a different room, guaranteed
        protected List<GridRoomPlan> ArrayRooms { get; set; }

        public void InitSize(int width, int height, int widthPerCell, int heightPerCell, int cellWall = 1, bool wrap = false)
        {
            this.Rooms = new int[width][];
            this.VHalls = new GridHallGroup[width][];
            this.HHalls = new GridHallGroup[width][];
            for (int xx = 0; xx < width; xx++)
            {
                this.Rooms[xx] = new int[height];
                this.VHalls[xx] = new GridHallGroup[height];
                this.HHalls[xx] = new GridHallGroup[height];
                for (int yy = 0; yy < height; yy++)
                {
                    this.Rooms[xx][yy] = -1;
                    this.VHalls[xx][yy] = new GridHallGroup();
                    this.HHalls[xx][yy] = new GridHallGroup();
                }
            }

            this.ArrayRooms = new List<GridRoomPlan>();

            this.WidthPerCell = widthPerCell;
            this.HeightPerCell = heightPerCell;
            if (cellWall < 1)
                throw new ArgumentException("Cannot init a grid with cell wall < 1");
            this.CellWall = cellWall;
            this.Wrap = wrap;
        }

        public void Clear()
        {
            int width = this.GridWidth;
            int height = this.GridHeight;

            this.Rooms = new int[width][];
            this.VHalls = new GridHallGroup[width][];
            this.HHalls = new GridHallGroup[width][];
            for (int xx = 0; xx < width; xx++)
            {
                this.Rooms[xx] = new int[height];
                this.VHalls[xx] = new GridHallGroup[height];
                this.HHalls[xx] = new GridHallGroup[height];
                for (int yy = 0; yy < height; yy++)
                {
                    this.Rooms[xx][yy] = -1;
                    this.VHalls[xx][yy] = new GridHallGroup();
                    this.HHalls[xx][yy] = new GridHallGroup();
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

            try
            {
                foreach (var plan in this.ArrayRooms)
                {
                    if (plan.PreferHall)
                    {
                        roomToHall.Add(new RoomHallIndex(map.RoomPlan.HallCount, true));
                        map.RoomPlan.AddHall((IPermissiveRoomGen)plan.RoomGen, plan.Components);
                        GenContextDebug.DebugProgress("Add Hall Room");
                    }
                    else
                    {
                        roomToHall.Add(new RoomHallIndex(map.RoomPlan.RoomCount, false));
                        map.RoomPlan.AddRoom(plan.RoomGen, plan.Components);
                        GenContextDebug.DebugProgress("Added Room");
                    }
                }
            }
            catch (Exception ex)
            {
                GenContextDebug.DebugError(ex);
            }

            GenContextDebug.StepOut();

            GenContextDebug.StepIn("Connecting Halls");

            try
            {
                for (int xx = 0; xx < this.VHalls.Length; xx++)
                {
                    for (int yy = 0; yy < this.VHalls[xx].Length; yy++)
                    {
                        GridHallGroup hallGroup = this.VHalls[xx][yy];
                        for (int ii = 0; ii < hallGroup.HallParts.Count; ii++)
                        {
                            List<RoomHallIndex> adj = new List<RoomHallIndex>();
                            if (ii == 0)
                            {
                                int upRoom = this.GetRoomIndex(new Loc(xx, yy));
                                if (upRoom > -1)
                                    adj.Add(roomToHall[upRoom]);
                            }
                            else
                            {
                                adj.Add(new RoomHallIndex(map.RoomPlan.HallCount - 1, true));
                            }

                            if (ii == hallGroup.HallParts.Count - 1)
                            {
                                int downRoom = this.GetRoomIndex(new Loc(xx, yy + 1));
                                if (downRoom > -1)
                                    adj.Add(roomToHall[downRoom]);
                            }

                            map.RoomPlan.AddHall(hallGroup.HallParts[ii].RoomGen, hallGroup.HallParts[ii].Components, adj.ToArray());
                            GenContextDebug.DebugProgress("Add VHall");
                        }
                    }
                }

                for (int xx = 0; xx < this.HHalls.Length; xx++)
                {
                    for (int yy = 0; yy < this.HHalls[xx].Length; yy++)
                    {
                        GridHallGroup hallGroup = this.HHalls[xx][yy];

                        for (int ii = 0; ii < hallGroup.HallParts.Count; ii++)
                        {
                            List<RoomHallIndex> adj = new List<RoomHallIndex>();
                            if (ii == 0)
                            {
                                int leftRoom = this.GetRoomIndex(new Loc(xx, yy));
                                if (leftRoom > -1)
                                    adj.Add(roomToHall[leftRoom]);
                            }
                            else
                            {
                                adj.Add(new RoomHallIndex(map.RoomPlan.HallCount - 1, true));
                            }

                            if (ii == hallGroup.HallParts.Count - 1)
                            {
                                int rightRoom = this.GetRoomIndex(new Loc(xx + 1, yy));
                                if (rightRoom > -1)
                                    adj.Add(roomToHall[rightRoom]);
                            }

                            map.RoomPlan.AddHall(hallGroup.HallParts[ii].RoomGen, hallGroup.HallParts[ii].Components, adj.ToArray());
                            GenContextDebug.DebugProgress("Add HHall");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GenContextDebug.DebugError(ex);
            }

            GenContextDebug.StepOut();
        }

        /// <summary>
        /// Returns the RoomGen found in the specified hall.
        /// </summary>
        /// <param name="locRay">The location of the room + the direction of the connecting hall relative to the room.</param>
        /// <returns></returns>
        public GridHallPlan GetHall(LocRay4 locRay)
        {
            return this.GetHallGroup(locRay)?.MainHall;
        }

        public IEnumerable<IRoomPlan> GetAllPlans()
        {
            foreach (GridRoomPlan plan in this.ArrayRooms)
                yield return plan;

            for (int xx = 0; xx < this.VHalls.Length; xx++)
            {
                for (int yy = 0; yy < this.VHalls[xx].Length; yy++)
                {
                    for (int ii = 0; ii < this.VHalls[xx][yy].HallParts.Count; ii++)
                        yield return this.VHalls[xx][yy].HallParts[ii];
                }
            }

            for (int xx = 0; xx < this.HHalls.Length; xx++)
            {
                for (int yy = 0; yy < this.HHalls[xx].Length; yy++)
                {
                    for (int ii = 0; ii < this.HHalls[xx][yy].HallParts.Count; ii++)
                        yield return this.HHalls[xx][yy].HallParts[ii];
                }
            }
        }

        public Loc WrapRoom(Loc loc)
        {
            return Loc.Wrap(loc, new Loc(this.GridWidth, this.GridHeight));
        }

        public bool Collides(Rect rect1, Rect rect2)
        {
            if (this.Wrap)
                return WrappedCollision.Collides(this.Size, rect1, rect2);
            else
                return Collision.Collides(rect1, rect2);
        }

        public bool InBounds(Rect rect, Loc loc)
        {
            if (this.Wrap)
                return WrappedCollision.InBounds(this.Size, rect, loc);
            else
                return Collision.InBounds(rect, loc);
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
            int index = this.GetRoomIndex(loc);
            if (index > -1)
                return this.ArrayRooms[index];
            return null;
        }

        public int GetRoomIndex(Loc loc)
        {
            if (this.Wrap)
                loc = this.WrapRoom(loc);
            else if (!Collision.InBounds(this.GridWidth, this.GridHeight, loc))
                return -1;

            return this.Rooms[loc.X][loc.Y];
        }

        public void EraseRoom(Loc loc)
        {
            if (this.Wrap)
                loc = this.WrapRoom(loc);
            int roomIndex = this.Rooms[loc.X][loc.Y];
            GridRoomPlan room = this.ArrayRooms[roomIndex];
            this.ArrayRooms.RemoveAt(roomIndex);
            for (int xx = room.Bounds.Start.X; xx < room.Bounds.End.X; xx++)
            {
                for (int yy = room.Bounds.Start.Y; yy < room.Bounds.End.Y; yy++)
                {
                    Loc subLoc = new Loc(xx, yy);
                    if (this.Wrap)
                        subLoc = this.WrapRoom(subLoc);
                    this.Rooms[subLoc.X][subLoc.Y] = -1;
                }
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

        public void AddRoom(Loc loc, IRoomGen gen, ComponentCollection components)
        {
            this.AddRoom(new Rect(loc, new Loc(1)), gen, components, false);
        }

        public void AddRoom(Loc loc, IRoomGen gen, ComponentCollection components, bool preferHall)
        {
            this.AddRoom(new Rect(loc, new Loc(1)), gen, components, preferHall);
        }

        public void AddRoom(Rect rect, IRoomGen gen, ComponentCollection components)
        {
            this.AddRoom(rect, gen, components, false);
        }

        public void AddRoom(Rect rect, IRoomGen gen, ComponentCollection components, bool preferHall)
        {
            Rect floorRect = new Rect(0, 0, this.GridWidth, this.GridHeight);
            if (!this.Wrap && !floorRect.Contains(rect))
                throw new ArgumentOutOfRangeException(nameof(rect), "Cannot add room out of bounds!");

            if (rect.Size.X > this.GridWidth || rect.Size.Y > this.GridHeight)
                throw new ArgumentOutOfRangeException(nameof(rect), "Cannot add room larger than bounds!");

            for (int xx = rect.Start.X; xx < rect.End.X; xx++)
            {
                for (int yy = rect.Start.Y; yy < rect.End.Y; yy++)
                {
                    Loc subLoc = new Loc(xx, yy);
                    Loc subLocLeft = new Loc(xx - 1, yy);
                    Loc subLocUp = new Loc(xx, yy - 1);
                    if (this.Wrap)
                    {
                        subLoc = this.WrapRoom(subLoc);
                        subLocLeft = this.WrapRoom(subLocLeft);
                        subLocUp = this.WrapRoom(subLocUp);
                    }

                    if (this.Rooms[subLoc.X][subLoc.Y] != -1)
                        throw new InvalidOperationException("Tried to add on top of an existing room!");
                    if (xx > rect.Start.X && this.HHalls[subLocLeft.X][subLocLeft.Y].MainHall != null)
                        throw new InvalidOperationException("Tried to add on top of an existing hall!");
                    if (yy > rect.Start.Y && this.VHalls[subLocUp.X][subLocUp.Y].MainHall != null)
                        throw new InvalidOperationException("Tried to add on top of an existing hall!");
                }
            }

            if (preferHall && !(gen is IPermissiveRoomGen))
                throw new InvalidOperationException("Cannot prefer hall for a non-permissive gen!");

            if (this.Wrap)
                rect = new Rect(this.WrapRoom(rect.Start), rect.Size);

            var room = new GridRoomPlan(rect, gen.Copy(), components)
            {
                PreferHall = preferHall,
            };
            this.ArrayRooms.Add(room);
            for (int xx = rect.Start.X; xx < rect.End.X; xx++)
            {
                for (int yy = rect.Start.Y; yy < rect.End.Y; yy++)
                {
                    Loc subLoc = new Loc(xx, yy);
                    if (this.Wrap)
                        subLoc = this.WrapRoom(subLoc);

                    this.Rooms[subLoc.X][subLoc.Y] = this.ArrayRooms.Count - 1;
                }
            }
        }

        public bool CanAddRoom(Rect rect)
        {
            Rect floorRect = new Rect(0, 0, this.GridWidth, this.GridHeight);
            if (!this.Wrap && !floorRect.Contains(rect))
                return false;

            for (int xx = rect.Start.X; xx < rect.End.X; xx++)
            {
                for (int yy = rect.Start.Y; yy < rect.End.Y; yy++)
                {
                    Loc subLoc = new Loc(xx, yy);
                    Loc subLocLeft = new Loc(xx - 1, yy);
                    Loc subLocUp = new Loc(xx, yy - 1);
                    if (this.Wrap)
                    {
                        subLoc = this.WrapRoom(subLoc);
                        subLocLeft = this.WrapRoom(subLocLeft);
                        subLocUp = this.WrapRoom(subLocUp);
                    }

                    if (this.Rooms[subLoc.X][subLoc.Y] != -1)
                        return false;
                    if (xx > rect.Start.X && this.HHalls[subLocLeft.X][subLocLeft.Y].MainHall != null)
                        return false;
                    if (yy > rect.Start.Y && this.VHalls[subLocUp.X][subLocUp.Y].MainHall != null)
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
        /// <param name="components">components to include in the hall</param>
        public void SetHall(LocRay4 locRay, IPermissiveRoomGen hallGen, ComponentCollection components)
        {
            GridHallPlan plan = null;
            if (hallGen != null)
                plan = new GridHallPlan((IPermissiveRoomGen)hallGen.Copy(), components);

            GridHallGroup group = this.GetHallGroup(locRay);
            if (group != null)
                group.SetHall(plan);
            else
                throw new ArgumentOutOfRangeException("Invalid position for hall.");
        }

        /// <summary>
        /// Decides on the room bounds for each room.  Results will be out of bounds and unwrapped in wrapped floor scenarios.
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
            GridHallGroup hallGroup = vertical ? this.VHalls[x][y] : this.HHalls[x][y];
            if (hallGroup.MainHall == null)
                return;

            Loc loc = new Loc(x, y);
            Dir4 dir = vertical ? Dir4.Down : Dir4.Right;
            Axis4 orth = vertical ? Axis4.Horiz : Axis4.Vert;

            GridRoomPlan startRoom = this.GetRoomPlan(loc);

            // always down or left of the original
            GridRoomPlan endRoom = this.GetRoomPlan(loc + dir.GetLoc());

            // in the case of wrapped rooms, we must push the end room position forward by one whole map size
            Rect wrappedEndDraw = endRoom.RoomGen.Draw;
            Rect wrappedEndBounds = endRoom.Bounds;
            if (this.Wrap)
            {
                // get the unwrapped location of the hall start
                Loc unwrappedGridLoc = Loc.Zero;
                foreach (Loc wrapLoc in WrappedCollision.IteratePointsInBounds(new Loc(this.GridWidth, this.GridHeight), startRoom.Bounds, loc))
                    unwrappedGridLoc = wrapLoc;

                // use it to find the unwrapped rectangles
                Loc multUnit = new Loc(this.WidthPerCell + this.CellWall, this.HeightPerCell + this.CellWall);
                foreach (Rect wrapRect in WrappedCollision.IterateRegionsColliding(new Loc(this.GridWidth, this.GridHeight), new Rect(unwrappedGridLoc + dir.GetLoc(), Loc.One), endRoom.Bounds))
                {
                    wrappedEndDraw.Start = endRoom.RoomGen.Draw.Start + ((wrapRect.Start - endRoom.Bounds.Start) * multUnit);
                    wrappedEndBounds = wrapRect;
                }
            }

            // also sets the sidereqs
            int tier = loc.GetScalar(orth);

            // the tier may need to be wrapped to the start and end bounds
            if (this.Wrap)
            {
                int regionStart = Math.Max(startRoom.Bounds.Start.GetScalar(orth), wrappedEndBounds.Start.GetScalar(orth));
                int regionEnd = Math.Min(startRoom.Bounds.End.GetScalar(orth), wrappedEndBounds.End.GetScalar(orth));
                foreach (int eligibles in WrappedCollision.IteratePointsInBounds(vertical ? this.GridWidth : this.GridHeight, regionStart, regionEnd, tier))
                {
                    tier = eligibles;
                    break;
                }
            }

            IntRange startRange = this.GetHallTouchRange(startRoom.RoomGen.Draw, startRoom.RoomGen.GetFulfillableBorder, dir, tier);
            IntRange endRange = this.GetHallTouchRange(wrappedEndDraw, endRoom.RoomGen.GetFulfillableBorder, dir.Reverse(), tier);
            IntRange combinedRange = new IntRange(Math.Min(startRange.Min, endRange.Min), Math.Max(startRange.Max, endRange.Max));
            Loc start = startRoom.RoomGen.Draw.End;
            Loc end = wrappedEndDraw.Start;

            Rect startCell = this.GetCellBounds(startRoom.Bounds);
            Rect endCell = this.GetCellBounds(wrappedEndBounds);

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
                hallGroup.MainHall.RoomGen.PrepareSize(rand, bounds.Size);
                hallGroup.MainHall.RoomGen.SetLoc(bounds.Start);
            }
            else
            {
                int divPoint = startCell.GetScalar(dir) + 1;
                IntRange startDivRange = startRange;
                IntRange endDivRange = endRange;
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
                    IntRange interCellSide = IntRange.Intersect(startCell.GetSide(dir.ToAxis()), endCell.GetSide(dir.ToAxis()));
                    startDivRange = IntRange.IncludeRange(startDivRange, interCellSide);
                    endDivRange = IntRange.IncludeRange(endDivRange, interCellSide);
                }

                Rect startBox = vertical ? new Rect(startDivRange.Min, start.Y, startDivRange.Length, divPoint - start.Y)
                    : new Rect(start.X, startDivRange.Min, divPoint - start.X, startDivRange.Length);
                Rect endBox = vertical ? new Rect(endDivRange.Min, divPoint, endDivRange.Length, end.Y - divPoint)
                    : new Rect(divPoint, endDivRange.Min, end.X - divPoint, endDivRange.Length);

                GridHallPlan originalHall = hallGroup.MainHall;
                hallGroup.HallParts.Add(new GridHallPlan((IPermissiveRoomGen)originalHall.RoomGen.Copy(), originalHall.Components));
                hallGroup.HallParts[0].RoomGen.PrepareSize(rand, startBox.Size);
                hallGroup.HallParts[0].RoomGen.SetLoc(startBox.Start);
                hallGroup.HallParts[1].RoomGen.PrepareSize(rand, endBox.Size);
                hallGroup.HallParts[1].RoomGen.SetLoc(endBox.Start);
            }
        }

        public List<int> GetAdjacentRooms(int roomIndex)
        {
            List<int> returnList = new List<int>();
            GridRoomPlan room = this.ArrayRooms[roomIndex];
            for (int ii = 0; ii < room.Bounds.Size.X; ii++)
            {
                // above
                int up = this.GetRoomIndex(new LocRay4(room.Bounds.X + ii, room.Bounds.Y, Dir4.Up));
                if (up > -1 && !returnList.Contains(up))
                    returnList.Add(up);

                // below
                int down = this.GetRoomIndex(new LocRay4(room.Bounds.X + ii, room.Bounds.End.Y - 1, Dir4.Down));
                if (down > -1 && !returnList.Contains(down))
                    returnList.Add(down);
            }

            for (int ii = 0; ii < room.Bounds.Size.Y; ii++)
            {
                // left
                int left = this.GetRoomIndex(new LocRay4(room.Bounds.X, room.Bounds.Y + ii, Dir4.Left));
                if (left > -1 && !returnList.Contains(left))
                    returnList.Add(left);

                // right
                int right = this.GetRoomIndex(new LocRay4(room.Bounds.End.X - 1, room.Bounds.Y + ii, Dir4.Right));
                if (right > -1 && !returnList.Contains(right))
                    returnList.Add(right);
            }

            return returnList;
        }

        public int GetRoomIndex(LocRay4 locRay)
        {
            GridHallPlan hall = this.GetHall(locRay);
            if (hall != null)
            {
                Loc moveLoc = locRay.Traverse(1);
                return this.GetRoomIndex(moveLoc);
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
        /// Gets the minimum range along the side of a room that includes all of its fulfillable borders.
        /// Special cases arise if the room is multi-cell.
        /// </summary>
        /// <param name="rect">todo: describe rect parameter on GetHallTouchRange</param>
        /// <param name="borderQuery">todo: describe borderQuery parameter on GetHallTouchRange</param>
        /// <param name="dir">Direction from room to hall.</param>
        /// <param name="tier"></param>
        /// <returns></returns>
        public virtual IntRange GetHallTouchRange(Rect rect, Func<Dir4, int, bool> borderQuery, Dir4 dir, int tier)
        {
            bool vertical = dir.ToAxis() == Axis4.Vert;

            // The hall will touch the whole fulfillable side of each room under normal circumstances.
            // Things get tricky for a target room that occupies more than one cell.
            // First, try to cover only the part of the target room that's in the cell.
            int tierStart = vertical ? tier * (this.WidthPerCell + this.CellWall) : tier * (this.HeightPerCell + this.CellWall);
            int tierLength = vertical ? this.WidthPerCell : this.HeightPerCell;
            IntRange tierRange = new IntRange(tierStart, tierStart + tierLength);
            IntRange roomRange = rect.GetSide(dir.ToAxis());

            // factor possibletiles into this calculation
            int borderStart = tierStart - roomRange.Min;
            if (borderStart < 0)
            {
                tierRange.Min -= borderStart;
                borderStart = 0;
            }

            for (int ii = borderStart; ii < roomRange.Length; ii++)
            {
                if (borderQuery(dir, ii))
                    break;
                else
                    tierRange.Min += 1;
            }

            int borderEnd = tierStart + tierLength - roomRange.Min;
            if (borderEnd > roomRange.Length)
            {
                tierRange.Max += roomRange.Length - borderEnd;
                borderEnd = roomRange.Length;
            }

            for (int ii = borderEnd - 1; ii >= 0; ii--)
            {
                if (borderQuery(dir, ii))
                    break;
                else
                    tierRange.Max -= 1;
            }

            if (tierRange.Max > tierRange.Min)
                return tierRange;

            // If that's not possible, then it means that the room must have fulfillable tiles outside of the current bound.
            // Try to extend the hall until it covers one fulfillable tile of the target room.
            // Easy method: note that the current tierRange range is covering the zone between the tier and the edge of the room (inverted)
            // There will be either a workable range at the start or a workable range at the end, never neither.
            IntRange startRange = new IntRange(tierRange.Max - 1, tierStart + 1);
            IntRange endRange = new IntRange(tierStart + tierLength - 1, tierRange.Min + 1);

            bool chooseStart = true;
            bool chooseEnd = true;

            // if tierRanges reached the absolute limits of the roomRange, then there is no fulfillable tile on that side
            if (startRange.Min < roomRange.Min)
                chooseStart = false;
            else if (endRange.Length > startRange.Length)
                chooseEnd = false;

            if (endRange.Max > roomRange.Max)
                chooseEnd = false;
            else if (startRange.Length > endRange.Length)
                chooseStart = false;

            if (!chooseStart && !chooseEnd)
                throw new ArgumentException("PrepareFulfillableBorders did not open at least one open tile for each direction!");

            if (chooseStart)
                return startRange;
            return endRange;
        }

        private GridHallGroup GetHallGroup(LocRay4 locRay)
        {
            if (!locRay.Dir.Validate())
                throw new ArgumentOutOfRangeException("Invalid enum value.");

            GridHallGroup[][] hallGroup;
            Loc endLoc;
            switch (locRay.Dir)
            {
                case Dir4.Down:
                    hallGroup = this.VHalls;
                    endLoc = locRay.Loc;
                    break;
                case Dir4.Left:
                    hallGroup = this.HHalls;
                    endLoc = locRay.Traverse(1);
                    break;
                case Dir4.Up:
                    hallGroup = this.VHalls;
                    endLoc = locRay.Traverse(1);
                    break;
                case Dir4.Right:
                    hallGroup = this.HHalls;
                    endLoc = locRay.Loc;
                    break;
                case Dir4.None:
                    throw new ArgumentException("Invalid direction.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(locRay.Dir), "Invalid enum value.");
            }

            int arrayWidth = this.GridWidth;
            int arrayHeight = this.GridHeight;
            if (hallGroup == this.HHalls)
                arrayWidth -= 1;
            else if (hallGroup == this.VHalls)
                arrayHeight -= 1;

            if (this.Wrap)
                endLoc = this.WrapRoom(endLoc);
            else if (!Collision.InBounds(arrayWidth, arrayHeight, endLoc))
                return null;

            return hallGroup[endLoc.X][endLoc.Y];
        }
    }
}
