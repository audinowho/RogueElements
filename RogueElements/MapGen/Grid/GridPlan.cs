// <copyright file="GridPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Represents a dungeon layout using a rectangular grid of cells, where each cell can contain a room
    /// and cells are connected to adjacent cells via hallways in cardinal directions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The grid plan provides a structured approach to dungeon generation where rooms are placed
    /// within discrete cells of a grid. Each cell has uniform dimensions, and hallways connect
    /// adjacent cells horizontally or vertically.
    /// </para>
    /// <para>
    /// The layout supports multi-cell rooms (rooms that span multiple grid cells), wrapped maps
    /// (where edges connect to opposite edges), and configurable cell dimensions and wall thicknesses.
    /// </para>
    /// <para>
    /// After populating the grid with rooms and halls using <see cref="GridPlanStep{T}"/> subclasses,
    /// call <see cref="PlaceRoomsOnFloor"/> to convert the grid plan into a <see cref="FloorPlan"/>
    /// for actual tile-based generation.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridRoomPlan"/>
    /// <seealso cref="GridHallGroup"/>
    /// <seealso cref="IRoomGridGenContext"/>
    public class GridPlan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridPlan"/> class.
        /// </summary>
        public GridPlan()
        {
        }

        /// <summary>
        /// Gets or sets the thickness of dividers between cells, in tiles.
        /// </summary>
        /// <remarks>
        /// This value determines the space between cells where hallways are placed.
        /// Must be at least 1.
        /// </remarks>
        public int CellWall { get; set; }

        /// <summary>
        /// Gets or sets the width of each cell in the grid, in tiles.
        /// </summary>
        public int WidthPerCell { get; set; }

        /// <summary>
        /// Gets or sets the height of each cell in the grid, in tiles.
        /// </summary>
        public int HeightPerCell { get; set; }

        /// <summary>
        /// Gets the number of columns in the grid.
        /// </summary>
        public int GridWidth => this.Rooms.Length;

        /// <summary>
        /// Gets the number of rows in the grid.
        /// </summary>
        public int GridHeight => this.Rooms[0].Length;

        /// <summary>
        /// Gets the total size of the map in tiles.
        /// </summary>
        /// <remarks>
        /// The size is calculated based on the grid dimensions, cell sizes, and wall thickness.
        /// For wrapped maps, the outer wall is included; for non-wrapped maps, it is excluded.
        /// </remarks>
        public Loc Size
        {
            get
            {
                return new Loc(
                    (this.GridWidth * (this.WidthPerCell + this.CellWall)) - (this.Wrap ? 0 : this.CellWall),
                    (this.GridHeight * (this.HeightPerCell + this.CellWall)) - (this.Wrap ? 0 : this.CellWall));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the map wraps around at the edges.
        /// </summary>
        /// <remarks>
        /// When enabled, the left edge connects to the right edge, and the top edge connects
        /// to the bottom edge, creating a toroidal topology.
        /// </remarks>
        public bool Wrap { get; set; }

        /// <summary>
        /// Gets the total number of rooms in the grid plan.
        /// </summary>
        public int RoomCount => this.ArrayRooms.Count;

        /// <summary>
        /// Gets or sets the 2D array mapping grid coordinates to room indices.
        /// A value of -1 indicates an empty cell.
        /// </summary>
        protected int[][] Rooms { get; set; }

        /// <summary>
        /// Gets or sets the 2D array of vertical hall groups connecting cells vertically.
        /// </summary>
        protected GridHallGroup[][] VHalls { get; set; }

        /// <summary>
        /// Gets or sets the 2D array of horizontal hall groups connecting cells horizontally.
        /// </summary>
        protected GridHallGroup[][] HHalls { get; set; }

        /// <summary>
        /// Gets or sets the list of all rooms in the grid plan.
        /// Each entry represents a unique room that may span one or more cells.
        /// </summary>
        protected List<GridRoomPlan> ArrayRooms { get; set; }

        /// <summary>
        /// Initializes the grid plan with the specified dimensions and cell properties.
        /// </summary>
        /// <param name="width">The number of columns in the grid.</param>
        /// <param name="height">The number of rows in the grid.</param>
        /// <param name="widthPerCell">The width of each cell in tiles.</param>
        /// <param name="heightPerCell">The height of each cell in tiles.</param>
        /// <param name="cellWall">The thickness of dividers between cells, in tiles. Must be at least 1.</param>
        /// <param name="wrap">Whether the map wraps around at the edges.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="cellWall"/> is less than 1.</exception>
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

        /// <summary>
        /// Clears all rooms and halls from the grid while preserving the grid dimensions.
        /// </summary>
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
        /// Generates the position and size of each room and hall, and places them into the floor plan.
        /// </summary>
        /// <param name="map">The floor plan generation context to populate with rooms and halls.</param>
        /// <remarks>
        /// <para>
        /// This method converts the abstract grid plan into concrete room and hall placements.
        /// It performs the following steps:
        /// </para>
        /// <list type="number">
        /// <item>Determines the bounds for each room within its cell(s).</item>
        /// <item>Calculates hall bounds and handles cases where halls need to be split.</item>
        /// <item>Adds all rooms to the floor plan, respecting the <see cref="GridRoomPlan.PreferHall"/> setting.</item>
        /// <item>Connects rooms with the appropriate hallways.</item>
        /// </list>
        /// </remarks>
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
        /// Gets the hall plan at the specified location and direction.
        /// </summary>
        /// <param name="locRay">The grid location and direction of the hall relative to a room.</param>
        /// <returns>The <see cref="GridHallPlan"/> at the specified location, or null if no hall exists.</returns>
        public GridHallPlan GetHall(LocRay4 locRay)
        {
            return this.GetHallGroup(locRay)?.MainHall;
        }

        /// <summary>
        /// Enumerates all room and hall plans in the grid.
        /// </summary>
        /// <returns>An enumerable of all <see cref="IRoomPlan"/> instances, including both rooms and halls.</returns>
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

        /// <summary>
        /// Wraps a grid location to valid coordinates for wrapped maps.
        /// </summary>
        /// <param name="loc">The grid location to wrap.</param>
        /// <returns>The wrapped location within grid bounds.</returns>
        public Loc WrapRoom(Loc loc)
        {
            return Loc.Wrap(loc, new Loc(this.GridWidth, this.GridHeight));
        }

        /// <summary>
        /// Checks if two rectangles collide, accounting for map wrapping if enabled.
        /// </summary>
        /// <param name="rect1">The first rectangle.</param>
        /// <param name="rect2">The second rectangle.</param>
        /// <returns>True if the rectangles collide; otherwise, false.</returns>
        public bool Collides(Rect rect1, Rect rect2)
        {
            if (this.Wrap)
                return WrappedCollision.Collides(this.Size, rect1, rect2);
            else
                return Collision.Collides(rect1, rect2);
        }

        /// <summary>
        /// Checks if a location is within a rectangle, accounting for map wrapping if enabled.
        /// </summary>
        /// <param name="rect">The bounding rectangle.</param>
        /// <param name="loc">The location to check.</param>
        /// <returns>True if the location is within the rectangle; otherwise, false.</returns>
        public bool InBounds(Rect rect, Loc loc)
        {
            if (this.Wrap)
                return WrappedCollision.InBounds(this.Size, rect, loc);
            else
                return Collision.InBounds(rect, loc);
        }

        /// <summary>
        /// Gets the room generator at the specified index.
        /// </summary>
        /// <param name="index">The index of the room in the room list.</param>
        /// <returns>The <see cref="IRoomGen"/> for the room.</returns>
        public IRoomGen GetRoom(int index)
        {
            return this.ArrayRooms[index].RoomGen;
        }

        /// <summary>
        /// Gets the room plan at the specified index.
        /// </summary>
        /// <param name="index">The index of the room in the room list.</param>
        /// <returns>The <see cref="GridRoomPlan"/> for the room.</returns>
        public GridRoomPlan GetRoomPlan(int index)
        {
            return this.ArrayRooms[index];
        }

        /// <summary>
        /// Gets the room plan at the specified grid location.
        /// </summary>
        /// <param name="loc">The grid coordinates of the cell.</param>
        /// <returns>The <see cref="GridRoomPlan"/> at that location, or null if the cell is empty.</returns>
        public GridRoomPlan GetRoomPlan(Loc loc)
        {
            int index = this.GetRoomIndex(loc);
            if (index > -1)
                return this.ArrayRooms[index];
            return null;
        }

        /// <summary>
        /// Gets the index of the room at the specified grid location.
        /// </summary>
        /// <param name="loc">The grid coordinates of the cell.</param>
        /// <returns>The room index, or -1 if the cell is empty or out of bounds.</returns>
        public int GetRoomIndex(Loc loc)
        {
            if (this.Wrap)
                loc = this.WrapRoom(loc);
            else if (!Collision.InBounds(this.GridWidth, this.GridHeight, loc))
                return -1;

            return this.Rooms[loc.X][loc.Y];
        }

        /// <summary>
        /// Removes the room at the specified grid location and updates all room indices.
        /// </summary>
        /// <param name="loc">The grid coordinates of the room to erase.</param>
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

        /// <summary>
        /// Adds a room to a single cell in the grid.
        /// </summary>
        /// <param name="loc">The grid coordinates of the cell.</param>
        /// <param name="gen">The room generator to use.</param>
        /// <param name="components">The components to attach to the room.</param>
        public void AddRoom(Loc loc, IRoomGen gen, ComponentCollection components)
        {
            this.AddRoom(new Rect(loc, new Loc(1)), gen, components, false);
        }

        /// <summary>
        /// Adds a room to a single cell in the grid with hall preference.
        /// </summary>
        /// <param name="loc">The grid coordinates of the cell.</param>
        /// <param name="gen">The room generator to use.</param>
        /// <param name="components">The components to attach to the room.</param>
        /// <param name="preferHall">Whether the room should be treated as a hall when added to the floor plan.</param>
        public void AddRoom(Loc loc, IRoomGen gen, ComponentCollection components, bool preferHall)
        {
            this.AddRoom(new Rect(loc, new Loc(1)), gen, components, preferHall);
        }

        /// <summary>
        /// Adds a multi-cell room to the grid.
        /// </summary>
        /// <param name="rect">The grid rectangle defining which cells the room occupies.</param>
        /// <param name="gen">The room generator to use.</param>
        /// <param name="components">The components to attach to the room.</param>
        public void AddRoom(Rect rect, IRoomGen gen, ComponentCollection components)
        {
            this.AddRoom(rect, gen, components, false);
        }

        /// <summary>
        /// Adds a multi-cell room to the grid with hall preference.
        /// </summary>
        /// <param name="rect">The grid rectangle defining which cells the room occupies.</param>
        /// <param name="gen">The room generator to use.</param>
        /// <param name="components">The components to attach to the room.</param>
        /// <param name="preferHall">Whether the room should be treated as a hall when added to the floor plan.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the room is out of bounds or larger than the grid.</exception>
        /// <exception cref="InvalidOperationException">Thrown when adding on top of an existing room or hall, or when preferHall is true but gen is not permissive.</exception>
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

        /// <summary>
        /// Checks if a room can be added at the specified grid rectangle.
        /// </summary>
        /// <param name="rect">The grid rectangle to check.</param>
        /// <returns>True if a room can be added at the specified location; otherwise, false.</returns>
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
        /// Sets or clears the hall at the specified location and direction.
        /// </summary>
        /// <param name="locRay">The grid location and direction of the hall relative to a room.</param>
        /// <param name="hallGen">The hall generator to use, or null to clear the hall.</param>
        /// <param name="components">The components to attach to the hall.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the hall position is invalid.</exception>
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
        /// Determines the tile-space bounds for a room within its grid cell(s).
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="roomIndex">The index of the room to process.</param>
        /// <remarks>
        /// The room generator proposes a size, which is capped to fit within the cell bounds.
        /// The room is then randomly positioned within the available cell space.
        /// For wrapped maps, results may be unwrapped (extending beyond normal bounds).
        /// </remarks>
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
        /// Determines the tile-space bounds for a hall connecting two adjacent cells.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="x">The X coordinate of the hall in the grid.</param>
        /// <param name="y">The Y coordinate of the hall in the grid.</param>
        /// <param name="vertical">True for vertical halls, false for horizontal halls.</param>
        /// <remarks>
        /// <para>
        /// This method calculates the hall dimensions based on the rooms it connects.
        /// It handles complex cases where rooms may be offset from each other, potentially
        /// splitting the hall into multiple segments if needed.
        /// </para>
        /// <para>
        /// The algorithm considers the fulfillable borders of each room to ensure the hall
        /// connects to valid entry points.
        /// </para>
        /// </remarks>
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

        /// <summary>
        /// Gets the indices of all rooms connected to the specified room via halls.
        /// </summary>
        /// <param name="roomIndex">The index of the room to query.</param>
        /// <returns>A list of room indices that are adjacent (connected by halls) to the specified room.</returns>
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

        /// <summary>
        /// Gets the room index connected via a hall in the specified direction.
        /// </summary>
        /// <param name="locRay">The location and direction to check.</param>
        /// <returns>The index of the connected room, or -1 if no hall exists in that direction.</returns>
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

        /// <summary>
        /// Converts grid cell bounds to tile-space bounds.
        /// </summary>
        /// <param name="bounds">The grid cell rectangle.</param>
        /// <returns>The corresponding tile-space rectangle.</returns>
        public virtual Rect GetCellBounds(Rect bounds)
        {
            return new Rect(
                bounds.X * (this.WidthPerCell + this.CellWall),
                bounds.Y * (this.HeightPerCell + this.CellWall),
                (bounds.Size.X * (this.WidthPerCell + this.CellWall)) - this.CellWall,
                (bounds.Size.Y * (this.HeightPerCell + this.CellWall)) - this.CellWall);
        }

        /// <summary>
        /// Calculates the range along a room's side where a hall can connect.
        /// </summary>
        /// <param name="rect">The tile-space rectangle of the room.</param>
        /// <param name="borderQuery">A function that returns true if a border tile at the given position is fulfillable.</param>
        /// <param name="dir">The direction from the room toward the hall.</param>
        /// <param name="tier">The grid coordinate perpendicular to the hall direction.</param>
        /// <returns>The range of valid tile positions where the hall can connect.</returns>
        /// <remarks>
        /// This method handles the complex case of multi-cell rooms where the hall may need
        /// to extend beyond the current cell to reach a fulfillable border tile.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when no fulfillable border tile exists.</exception>
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

        /// <summary>
        /// Gets the hall group at the specified location and direction.
        /// </summary>
        /// <param name="locRay">The grid location and direction of the hall.</param>
        /// <returns>The <see cref="GridHallGroup"/> at the specified location, or null if out of bounds.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the direction is invalid.</exception>
        /// <exception cref="ArgumentException">Thrown when the direction is None.</exception>
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
