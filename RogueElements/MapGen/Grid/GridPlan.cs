using System;
using System.Collections.Generic;

namespace RogueElements
{

    public class GridPlan
    {
        const int CELL_WALL = 1;
        
        public int WidthPerCell;
        public int HeightPerCell;

        protected int[][] rooms;
        protected IPermissiveRoomGen[][] vHalls;
        protected IPermissiveRoomGen[][] hHalls;
        
        public int GridWidth { get { return rooms.Length; } }
        public int GridHeight { get { return rooms[0].Length; } }
        
        public Loc Size
        {
            get
            {
                return new Loc(GridWidth * (WidthPerCell + CELL_WALL) - CELL_WALL,
                    GridHeight * (HeightPerCell + CELL_WALL) - CELL_WALL);
            }
        }


        //list of all rooms on the entire floor
        //each entry is a different room, guaranteed
        protected List<GridRoomPlan> arrayRooms;
        
        public int RoomCount { get { return arrayRooms.Count; } }

        public GridPlan() { }
        
        public void InitSize(int width, int height, int widthPerCell, int heightPerCell)
        {
            rooms = new int[width][];
            vHalls = new IPermissiveRoomGen[width][];
            hHalls = new IPermissiveRoomGen[width - 1][];
            for (int xx = 0; xx < width; xx++)
            {
                rooms[xx] = new int[height];
                vHalls[xx] = new IPermissiveRoomGen[height - 1];
                if (xx < width - 1)
                    hHalls[xx] = new IPermissiveRoomGen[height];
                for (int yy = 0; yy < height; yy++)
                    rooms[xx][yy] = -1;
            }
            arrayRooms = new List<GridRoomPlan>();

            WidthPerCell = widthPerCell;
            HeightPerCell = heightPerCell;
        }

        public void Clear()
        {
            int width = GridWidth;
            int height = GridHeight;

            rooms = new int[width][];
            vHalls = new IPermissiveRoomGen[width][];
            hHalls = new IPermissiveRoomGen[width - 1][];
            for (int xx = 0; xx < width; xx++)
            {
                rooms[xx] = new int[height];
                vHalls[xx] = new IPermissiveRoomGen[height - 1];
                if (xx < width - 1)
                    hHalls[xx] = new IPermissiveRoomGen[height];
                for (int yy = 0; yy < height; yy++)
                    rooms[xx][yy] = -1;
            }
            arrayRooms = new List<GridRoomPlan>();
        }

        public void PlaceRoomsOnFloor(IFloorPlanGenContext map)
        {
            //decide on room sizes
            for (int ii = 0; ii < arrayRooms.Count; ii++)
                ChooseRoomBounds(map.Rand, ii);

            //decide on halls; write to RoomSideReqs
            for (int xx = 0; xx < vHalls.Length; xx++)
            {
                for (int yy = 0; yy < vHalls[xx].Length; yy++)
                    ChooseHallBounds(map.Rand, xx, yy, true);
            }
            for (int xx = 0; xx < hHalls.Length; xx++)
            {
                for (int yy = 0; yy < hHalls[xx].Length; yy++)
                    ChooseHallBounds(map.Rand, xx, yy, false);
            }

            List<RoomHallIndex> roomToHall = new List<RoomHallIndex>();
            for (int ii = 0; ii < arrayRooms.Count; ii++)
            {
                GridRoomPlan plan = arrayRooms[ii];
                IDefaultRoomGen defaultGen = plan.RoomGen as IDefaultRoomGen;
                if (defaultGen != null)
                {
                    roomToHall.Add(new RoomHallIndex(map.RoomPlan.HallCount, true));
                    map.RoomPlan.AddHall(defaultGen);
                }
                else
                {
                    roomToHall.Add(new RoomHallIndex(map.RoomPlan.RoomCount, false));
                    map.RoomPlan.AddRoom(plan.RoomGen, plan.Immutable);
                }
            }

            for (int xx = 0; xx < vHalls.Length; xx++)
            {
                for (int yy = 0; yy < vHalls[xx].Length; yy++)
                {
                    if (vHalls[xx][yy] != null)
                    {
                        List<RoomHallIndex> adj = new List<RoomHallIndex>();
                        int upRoom = rooms[xx][yy];
                        if (upRoom > -1)
                            adj.Add(roomToHall[upRoom]);
                        int downRoom = rooms[xx][yy + 1];
                        if (downRoom > -1)
                            adj.Add(roomToHall[downRoom]);
                        map.RoomPlan.AddHall(vHalls[xx][yy], adj.ToArray());
                    }
                }
            }
            for (int xx = 0; xx < hHalls.Length; xx++)
            {
                for (int yy = 0; yy < hHalls[xx].Length; yy++)
                {
                    if (hHalls[xx][yy] != null)
                    {
                        List<RoomHallIndex> adj = new List<RoomHallIndex>();
                        int leftRoom = rooms[xx][yy];
                        if (leftRoom > -1)
                            adj.Add(roomToHall[leftRoom]);
                        int rightRoom = rooms[xx + 1][yy];
                        if (rightRoom > -1)
                            adj.Add(roomToHall[rightRoom]);
                        map.RoomPlan.AddHall(hHalls[xx][yy], adj.ToArray());
                    }
                }
            }

        }

        public IPermissiveRoomGen GetHall(Loc loc, Dir4 dir)
        {
            switch (dir)
            {
                case Dir4.Down:
                    if (loc.Y < GridHeight - 1)
                        return vHalls[loc.X][loc.Y];
                    break;
                case Dir4.Left:
                    if (loc.X > 0)
                        return hHalls[loc.X - 1][loc.Y];
                    break;
                case Dir4.Up:
                    if (loc.Y > 0)
                        return vHalls[loc.X][loc.Y - 1];
                    break;
                case Dir4.Right:
                    if (loc.X < GridWidth - 1)
                        return hHalls[loc.X][loc.Y];
                    break;
            }
            return null;
        }

        public IRoomGen GetRoom(int index)
        {
            return arrayRooms[index].RoomGen;
        }

        public GridRoomPlan GetRoomPlan(int index)
        {
            return arrayRooms[index];
        }

        public GridRoomPlan GetRoomPlan(int x, int y)
        {
            return rooms[x][y] > -1 ? arrayRooms[rooms[x][y]] : null;
        }

        public int GetRoomIndex(int x, int y)
        {
            return rooms[x][y];
        }

        public void EraseRoom(int x, int y)
        {
            int roomIndex = rooms[x][y];
            GridRoomPlan room = arrayRooms[roomIndex];
            arrayRooms.RemoveAt(roomIndex);
            for (int ii = 0; ii < room.Bounds.Size.X; ii++)
            {
                for (int jj = 0; jj < room.Bounds.Size.Y; jj++)
                    rooms[room.Bounds.X+ii][room.Bounds.Y+jj] = -1;
            }

            for (int ii = 0; ii < GridWidth; ii++)
            {
                for (int jj = 0; jj < GridHeight; jj++)
                {
                    if (rooms[ii][jj] > roomIndex)
                        rooms[ii][jj]--;
                }
            }
        }

        public bool IsRoomOpen(int x, int y)
        {
            return (GetRoomPlan(x,y) != null && !(GetRoomPlan(x,y).RoomGen is IDefaultRoomGen));
        }

        public void SetRoomGen(int index, IRoomGen gen)
        {
            arrayRooms[index].RoomGen = gen.Copy();
        }

        public void SetRoomImmutable(int index, bool immutable)
        {
            arrayRooms[index].Immutable = immutable;
        }

        public void SetRoomGen(int x, int y, IRoomGen gen, bool immutable = false)
        {
            if (rooms[x][y] == -1)
                AddRoom(x, y, gen, immutable);
            else
            {
                arrayRooms[rooms[x][y]].RoomGen = gen.Copy();
                arrayRooms[rooms[x][y]].Immutable = immutable;
            }
        }

        public void AddRoom(int x, int y, IRoomGen gen, bool immutable = false)
        {
            AddRoom(x, y, 1, 1, gen, immutable);
        }

        public void AddRoom(int x, int y, int cellWidth, int cellHeight, IRoomGen gen, bool immutable = false)
        {
            if (x < 0 || y < 0 || x + cellWidth > GridWidth || y + cellHeight > GridHeight)
                throw new ArgumentOutOfRangeException("Cannot add room out of bounds!");

            for (int ii = 0; ii < cellWidth; ii++)
            {
                for (int jj = 0; jj < cellHeight; jj++)
                {
                    if (rooms[x + ii][y + jj] != -1)
                        throw new InvalidOperationException("Tried to add on top of an existing room!");
                    if (ii > 0 && hHalls[x + ii - 1][y + jj] != null)
                        throw new InvalidOperationException("Tried to add on top of an existing hall!");
                    if (jj > 0 && vHalls[x + ii][y + jj - 1] != null)
                        throw new InvalidOperationException("Tried to add on top of an existing hall!");
                }
            }
            GridRoomPlan room = new GridRoomPlan(new Rect(x, y, cellWidth, cellHeight), gen.Copy());
            room.Immutable = immutable;
            arrayRooms.Add(room);
            for (int ii = 0; ii < cellWidth; ii++)
            {
                for (int jj = 0; jj < cellHeight; jj++)
                    rooms[x + ii][y + jj] = arrayRooms.Count - 1;
            }
        }

        public void SetHall(Loc loc, Dir4 dir, IPermissiveRoomGen hallGen)
        {
            if (dir <= Dir4.None || (int)dir >= DirExt.DIR4_COUNT)
                throw new ArgumentException("Invalid direction.");
            IPermissiveRoomGen addHall = null;
            if (hallGen != null)
                addHall = hallGen.Copy();
            switch (dir)
            {
                case Dir4.Down:
                    if (loc.Y < GridHeight - 1)
                        vHalls[loc.X][loc.Y] = addHall;
                    break;
                case Dir4.Left:
                    if (loc.X > 0)
                        hHalls[loc.X - 1][loc.Y] = addHall;
                    break;
                case Dir4.Up:
                    if (loc.Y > 0)
                        vHalls[loc.X][loc.Y - 1] = addHall;
                    break;
                case Dir4.Right:
                    if (loc.X < GridWidth - 1)
                        hHalls[loc.X][loc.Y] = addHall;
                    break;
            }
        }

        public void SetConnectingHall(Loc room1, Loc room2, IPermissiveRoomGen hallGen)
        {
            Loc diff = room2 - room1;
            Dir8 dir = diff.GetDir();
            if (diff.Dist8() != 1)
                throw new ArgumentException("Cannot add hall between " + room1.X + "," + room1.Y + " and " + room2.X + "," + room2.Y);
            else
                SetHall(room1, dir.ToDir4(), hallGen);
        }

        /// <summary>
        /// Decides on the room bounds for each room
        /// </summary>
        /// <param name="map"></param>
        /// <param name="roomIndex"></param>
        public void ChooseRoomBounds(IRandom rand, int roomIndex)
        {
            GridRoomPlan roomPair = arrayRooms[roomIndex];
            //the RoomGens are allowed to choose any size period, but this function will cap them at the cell sizes
            Loc size = roomPair.RoomGen.ProposeSize(rand);
            Rect cellBounds = GetCellBounds(roomPair.Bounds);
            size = new Loc(Math.Min(size.X, cellBounds.Width), Math.Min(size.Y, cellBounds.Height));
            roomPair.RoomGen.PrepareSize(rand, size);

            Loc start = cellBounds.Start + new Loc(rand.Next(cellBounds.Size.X - size.X + 1), rand.Next(cellBounds.Size.Y - size.Y + 1));
            roomPair.RoomGen.SetLoc(start);
        }

        /// <summary>
        /// Decides on the bounds for each hall.  Also writes to the adjacent rooms' SideReqs and tile permissions
        /// </summary>
        /// <param name="map"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="vertical"></param>
        public void ChooseHallBounds(IRandom rand, int x, int y, bool vertical)
        {
            GridRoomPlan startRoom = GetRoomPlan(x, y);
            GridRoomPlan endRoom = GetRoomPlan(x + (vertical ? 0 : 1), y + (vertical ? 1 : 0));
            
            IPermissiveRoomGen hall = vertical ? vHalls[x][y] : hHalls[x][y];
            if (hall != null)//also sets the sidereqs
            {
                int tier = vertical ? x : y;
                Dir4 dir = vertical ? Dir4.Down : Dir4.Right;
                Range startRange = GetHallTouchRange(startRoom.RoomGen, dir, tier);
                Range endRange = GetHallTouchRange(endRoom.RoomGen, dir.Reverse(), tier);
                Range combinedRange = new Range(Math.Min(startRange.Min, endRange.Min), Math.Max(startRange.Max, endRange.Max));
                Loc start = startRoom.RoomGen.Draw.End;
                Loc end = endRoom.RoomGen.Draw.Start;

                Rect bounds = vertical ? new Rect(combinedRange.Min, start.Y, combinedRange.GetRange(), end.Y - start.Y)
                    : new Rect(start.X, combinedRange.Min, end.X - start.X, combinedRange.GetRange());

                hall.PrepareSize(rand, bounds.Size);
                hall.SetLoc(bounds.Start);
            }
        }

        public List<int> GetAdjacentRooms(int roomIndex)
        {
            List<int> returnList = new List<int>();
            GridRoomPlan room = arrayRooms[roomIndex];
            for (int ii = 0; ii < room.Bounds.Size.X; ii++)
            {
                //above
                int up = GetRoomNum(new Loc(room.Bounds.X + ii, room.Bounds.Y), Dir4.Up);
                if (up > -1 && !returnList.Contains(up))
                    returnList.Add(up);
                //below
                int down = GetRoomNum(new Loc(room.Bounds.X + ii, room.Bounds.End.Y - 1), Dir4.Down);
                if (down > -1 && !returnList.Contains(down))
                    returnList.Add(down);
            }
            for (int ii = 0; ii < room.Bounds.Size.Y; ii++)
            {
                //left
                int left = GetRoomNum(new Loc(room.Bounds.X, room.Bounds.Y + ii), Dir4.Left);
                if (left > -1 && !returnList.Contains(left))
                    returnList.Add(left);
                //right
                int right = GetRoomNum(new Loc(room.Bounds.End.X - 1, room.Bounds.Y + ii), Dir4.Right);
                if (right > -1 && !returnList.Contains(right))
                    returnList.Add(right);
            }
            return returnList;
        }


        public int GetRoomNum(Loc loc, Dir4 dir)
        {
            IPermissiveRoomGen hall = GetHall(loc, dir);
            if (hall != null)
            {
                Loc moveLoc = loc + dir.GetLoc();
                return rooms[moveLoc.X][moveLoc.Y];
            }
            return -1;
        }
        
        public bool CheckAccessibility(params int[] rooms)
        {
            //check that, for all starting rooms provided, that all other rooms can be accessed
            bool[] checkList = new bool[RoomCount];
            foreach (int room in rooms)
                TraverseRooms(checkList, room);

            foreach (bool check in checkList)
            {
                if (!check)
                    return false;
            }

            return true;
        }
        protected void TraverseRooms(bool[] checkList, int startRoom)
        {
            if (checkList[startRoom])
                return;
            checkList[startRoom] = true;
            GridRoomPlan room = arrayRooms[startRoom];
            List<int> adjacent = GetAdjacentRooms(startRoom);
            foreach(int adjacentRoom in adjacent)
                TraverseRooms(checkList, adjacentRoom);
        }


        public virtual Rect GetCellBounds(Rect bounds)
        {
            return new Rect(bounds.X * (WidthPerCell + CELL_WALL), bounds.Y * (HeightPerCell + CELL_WALL),
                bounds.Size.X * (WidthPerCell + CELL_WALL) - CELL_WALL, bounds.Size.Y * (HeightPerCell + CELL_WALL) - CELL_WALL);
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
            //The hall will touch the entire side of each room under normal circumstances.
            //Things get tricky for a target room that occupies more than one cell.
            //First, try to cover only the part of the target room that's in the cell.
            //If that's not possible, try to extend the hall until it covers one tile of the target room.
            Range startRange = room.Draw.GetSide(dir.ToAxis());

            //factor possibletiles into this calculation
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
            
            int tierStart = vertical ? tier * (WidthPerCell + CELL_WALL) : tier * (HeightPerCell + CELL_WALL);
            int tierLength = vertical ? WidthPerCell : HeightPerCell;
            Range newRange = new Range(Math.Max(startRange.Min, tierStart), Math.Min(startRange.Max, tierStart+tierLength));
            if (newRange.Max <= newRange.Min)
            {
                //try to extend the hall until it covers one tile of the target room.
                //first, note that the current end range is covering the zone between the tier and the edge of the room (inverted)
                //un-invert this and inflate by 1, and you will have a zone that covers 1 tile with the room
                //get the intersection of this zone and the room.
                newRange = new Range(Math.Max(startRange.Min, newRange.Max-1), Math.Min(startRange.Max, newRange.Min+1));
            }
            return newRange;
        }
    }
    
}
