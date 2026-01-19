// <copyright file="FloorPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Represents the high-level layout structure of a dungeon floor, managing rooms and halls
    /// with their spatial relationships and connectivity.
    /// </summary>
    /// <remarks>
    /// <para>
    /// FloorPlan is the core data structure for freeform room-based map generation. Unlike grid-based
    /// layouts (<see cref="GridPlan"/>), FloorPlan allows rooms to be placed at arbitrary positions
    /// and sizes, with halls connecting them in a graph structure.
    /// </para>
    /// <para>
    /// The typical workflow is:
    /// <list type="number">
    /// <item><description>Initialize with <see cref="InitSize"/> or <see cref="InitRect"/></description></item>
    /// <item><description>Add rooms using <see cref="AddRoom"/> and halls using <see cref="AddHall"/></description></item>
    /// <item><description>Connect rooms by specifying adjacency relationships</description></item>
    /// <item><description>Draw the final tiles using <see cref="DrawOnMap"/></description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The floor plan supports optional wrapping for toroidal maps where edges connect to opposite sides.
    /// </para>
    /// </remarks>
    /// <seealso cref="FloorRoomPlan"/>
    /// <seealso cref="FloorHallPlan"/>
    /// <seealso cref="IFloorPlanGenContext"/>
    public class FloorPlan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloorPlan"/> class.
        /// </summary>
        public FloorPlan()
        {
        }

        /// <summary>
        /// Gets the total size of the floor plan in tiles.
        /// </summary>
        public Loc Size { get; private set; }

        /// <summary>
        /// Gets the starting location of the floor space.
        /// </summary>
        /// <remarks>
        /// Room coordinates are currently NOT relative to this value and their draw locs are universal.
        /// This value is used for computing the drawable area and for padding when rendering to tiles.
        /// </remarks>
        public Loc Start { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the floor plan wraps around its edges (toroidal topology).
        /// </summary>
        /// <remarks>
        /// When true, rooms near edges can connect to rooms on the opposite side, and collision
        /// detection accounts for wraparound. This enables creation of maps where walking off
        /// one edge brings the player to the opposite side.
        /// </remarks>
        public bool Wrap { get; private set; }

        /// <summary>
        /// Gets the bounding rectangle of the floor plan, combining <see cref="Start"/> and <see cref="Size"/>.
        /// </summary>
        public Rect DrawRect => new Rect(this.Start, this.Size);

        /// <summary>
        /// Gets the total number of rooms in the floor plan.
        /// </summary>
        public virtual int RoomCount => this.Rooms.Count;

        /// <summary>
        /// Gets the total number of halls in the floor plan.
        /// </summary>
        public virtual int HallCount => this.Halls.Count;

        /// <summary>
        /// Gets the internal list of room plans.
        /// </summary>
        protected List<FloorRoomPlan> Rooms { get; private set; }

        /// <summary>
        /// Gets the internal list of hall plans.
        /// </summary>
        protected List<FloorHallPlan> Halls { get; private set; }

        /// <summary>
        /// Calculates the number of border tiles that can connect between two adjacent rooms.
        /// </summary>
        /// <param name="roomFrom">The existing room to expand from.</param>
        /// <param name="room">The new room to add. Its current position is not final.</param>
        /// <param name="candLoc">The proposed location of the new room. Assumes this location is adjacent to roomFrom.</param>
        /// <param name="expandTo">The direction from the existing room toward the new room.</param>
        /// <returns>The count of border tiles where both rooms can create openings (fulfillable borders).</returns>
        /// <remarks>
        /// This method is used to determine how well two rooms can connect at a given placement.
        /// A higher return value indicates more potential connection points, which is used
        /// for weighted random selection when placing rooms.
        /// </remarks>
        public static int GetBorderMatch(IRoomGen roomFrom, IRoomGen room, Loc candLoc, Dir4 expandTo)
        {
            Loc diff = roomFrom.Draw.Start - candLoc; // how far ahead the start of source is to dest
            int offset = diff.GetScalar(expandTo.ToAxis().Orth());

            // Traverse the region that both borders touch
            int sourceLength = roomFrom.Draw.GetBorderLength(expandTo);
            int destLength = room.Draw.GetBorderLength(expandTo.Reverse());

            int totalMatch = 0;
            for (int ii = Math.Max(0, offset); ii - offset < sourceLength && ii < destLength; ii++)
            {
                bool sourceFulfill = roomFrom.GetFulfillableBorder(expandTo, ii - offset);
                bool destFulfill = room.GetFulfillableBorder(expandTo.Reverse(), ii);
                if (sourceFulfill && destFulfill)
                    totalMatch++;
            }

            return totalMatch;
        }

        /// <summary>
        /// Gets the unwrapped version of a rectangle that is adjacent to another in the specified direction.
        /// </summary>
        /// <param name="rectFrom">The reference rectangle.</param>
        /// <param name="rectTo">The rectangle to check for adjacency and potentially unwrap.</param>
        /// <param name="dir">The direction of expected adjacency from rectFrom to rectTo.</param>
        /// <returns>
        /// The unwrapped rectangle if the two rectangles are adjacent in the specified direction;
        /// otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// In wrapped floor plans, a rectangle near one edge may be adjacent to a rectangle on the
        /// opposite edge. This method handles the coordinate transformation needed to compute
        /// the effective position for border calculations.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when <paramref name="dir"/> is <see cref="Dir4.None"/>.</exception>
        public Rect? GetAdjacentRect(Rect rectFrom, Rect rectTo, Dir4 dir)
        {
            if (dir == Dir4.None)
                throw new ArgumentException("Invalid direction.");
            int scalarFrom = rectFrom.GetScalar(dir);
            int scalarTo = rectTo.GetScalar(dir.Reverse());
            Axis4 orth = dir.ToAxis().Orth();
            if (this.Wrap)
            {
                int newTo = WrappedCollision.GetClosestWrap(this.Size.GetScalar(dir.ToAxis()), scalarFrom, scalarTo);
                int diff = newTo - scalarTo;

                Loc newStart = rectTo.Start + DirExt.CreateLoc(dir.ToAxis(), diff, 0);
                rectTo = new Rect(newStart, rectTo.Size);
                scalarTo = newTo;

                // also get the correct orthogonal dimension using IterateRegionsColliding
                int startOrth = rectTo.Start.GetScalar(orth);
                IntRange? workingRange = null;
                foreach (IntRange range in WrappedCollision.IterateRegionsColliding(this.Size.GetScalar(orth), rectFrom.Start.GetScalar(orth), rectFrom.Size.GetScalar(orth), rectTo.Start.GetScalar(orth), rectTo.Size.GetScalar(orth)))
                {
                    workingRange = range;
                    if (range.Min == startOrth)
                        break;
                }

                // The rectangles are touching in-direction, however they are not aligned to each other in the orthogonal direction.
                if (!workingRange.HasValue)
                    return null;

                int orthDiff = workingRange.Value.Min - startOrth;
                Loc orthStart = rectTo.Start + DirExt.CreateLoc(orth, orthDiff, 0);
                rectTo = new Rect(orthStart, rectTo.Size);
            }
            else
            {
                if (!Collision.Collides(rectFrom.Start.GetScalar(orth), rectFrom.Size.GetScalar(orth), rectTo.Start.GetScalar(orth), rectTo.Size.GetScalar(orth)))
                    return null;
            }

            if (scalarFrom == scalarTo)
                return rectTo;

            return null;
        }

        /// <summary>
        /// Determines the direction in which one room is adjacent to another.
        /// </summary>
        /// <param name="roomGenFrom">The reference room generator.</param>
        /// <param name="roomGenTo">The room generator to find the direction to.</param>
        /// <returns>
        /// The <see cref="Dir4"/> from <paramref name="roomGenFrom"/> to <paramref name="roomGenTo"/>
        /// if they are adjacent; otherwise, <see cref="Dir4.None"/>.
        /// </returns>
        public Dir4 GetDirAdjacent(IRoomGen roomGenFrom, IRoomGen roomGenTo)
        {
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                if (this.GetAdjacentRect(roomGenFrom.Draw, roomGenTo.Draw, dir) != null)
                    return dir;
            }

            return Dir4.None;
        }

        /// <summary>
        /// Initializes the floor plan with a specified size starting at the origin.
        /// </summary>
        /// <param name="size">The size of the floor plan in tiles.</param>
        /// <param name="wrap">Whether the floor plan should wrap around edges.</param>
        public void InitSize(Loc size, bool wrap = false)
        {
            this.InitRect(new Rect(Loc.Zero, size), wrap);
        }

        /// <summary>
        /// Initializes the floor plan with a specified bounding rectangle.
        /// </summary>
        /// <param name="rect">The bounding rectangle defining the floor area.</param>
        /// <param name="wrap">Whether the floor plan should wrap around edges.</param>
        public void InitRect(Rect rect, bool wrap)
        {
            this.Start = rect.Start;
            this.Size = rect.Size;
            this.Wrap = wrap;
            this.Rooms = new List<FloorRoomPlan>();
            this.Halls = new List<FloorHallPlan>();
        }

        /// <summary>
        /// Removes all rooms and halls from the floor plan while preserving its dimensions.
        /// </summary>
        public void Clear()
        {
            this.Rooms.Clear();
            this.Halls.Clear();
        }

        /// <summary>
        /// Gets the room plan at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the room.</param>
        /// <returns>The <see cref="FloorRoomPlan"/> at the specified index.</returns>
        public virtual FloorRoomPlan GetRoomPlan(int index)
        {
            return this.Rooms[index];
        }

        /// <summary>
        /// Gets the room generator at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the room.</param>
        /// <returns>The <see cref="IRoomGen"/> for the room at the specified index.</returns>
        public virtual IRoomGen GetRoom(int index)
        {
            return this.Rooms[index].RoomGen;
        }

        /// <summary>
        /// Gets the hall plan at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the hall.</param>
        /// <returns>The <see cref="FloorHallPlan"/> at the specified index.</returns>
        public virtual FloorHallPlan GetHallPlan(int index)
        {
            return this.Halls[index];
        }

        /// <summary>
        /// Gets the hall generator at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the hall.</param>
        /// <returns>The <see cref="IPermissiveRoomGen"/> for the hall at the specified index.</returns>
        public virtual IPermissiveRoomGen GetHall(int index)
        {
            return this.Halls[index].RoomGen;
        }

        /// <summary>
        /// Gets a room or hall plan by its combined index.
        /// </summary>
        /// <param name="room">The index identifying either a room or hall.</param>
        /// <returns>The <see cref="IFloorRoomPlan"/> for the specified room or hall.</returns>
        public virtual IFloorRoomPlan GetRoomHall(RoomHallIndex room)
        {
            if (!room.IsHall)
                return this.Rooms[room.Index];
            else
                return this.Halls[room.Index];
        }

        /// <summary>
        /// Adds a new room to the floor plan with the specified adjacencies.
        /// </summary>
        /// <param name="gen">The room generator defining the room's shape and size.</param>
        /// <param name="components">The component collection to attach to the room for filtering and identification.</param>
        /// <param name="attached">The rooms and halls that this new room is adjacent to.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the room would overlap an existing room or hall, or when it falls outside
        /// the floor plan bounds (in non-wrapped mode).
        /// </exception>
        public void AddRoom(IRoomGen gen, ComponentCollection components, params RoomHallIndex[] attached)
        {
            // check against colliding on other rooms (and not halls)
            foreach (var room in this.Rooms)
            {
                if (this.Collides(room.RoomGen.Draw, gen.Draw))
                    throw new InvalidOperationException("Tried to add on top of an existing room!");
            }

            foreach (var hall in this.Halls)
            {
                if (this.Collides(hall.RoomGen.Draw, gen.Draw))
                    throw new InvalidOperationException("Tried to add on top of an existing hall!");
            }

            // check against rooms that go out of bounds
            if (!this.Wrap && !this.DrawRect.Contains(gen.Draw))
                throw new InvalidOperationException("Tried to add out of range!");

            // we expect that the room has already been given a size
            // and that its fulfillables match up with its adjacent's fulfillables.
            var plan = new FloorRoomPlan(gen, components);

            // attach everything
            plan.Adjacents.AddRange(attached);
            foreach (RoomHallIndex fromRoom in attached)
            {
                IFloorRoomPlan fromPlan = this.GetRoomHall(fromRoom);
                fromPlan.Adjacents.Add(new RoomHallIndex(this.Rooms.Count, false));
            }

            this.Rooms.Add(plan);
        }

        /// <summary>
        /// Adds a new hall to the floor plan with the specified adjacencies.
        /// </summary>
        /// <param name="gen">The permissive room generator defining the hall's shape and size.</param>
        /// <param name="components">The component collection to attach to the hall for filtering and identification.</param>
        /// <param name="attached">The rooms and halls that this new hall is adjacent to.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the hall would overlap an existing room, or when it falls outside
        /// the floor plan bounds (in non-wrapped mode).
        /// </exception>
        public void AddHall(IPermissiveRoomGen gen, ComponentCollection components, params RoomHallIndex[] attached)
        {
            // we expect that the hall has already been given a size...
            // check against colliding on other rooms (and not halls)
            foreach (var room in this.Rooms)
            {
                if (this.Collides(room.RoomGen.Draw, gen.Draw))
                    throw new InvalidOperationException("Tried to add on top of an existing room!");
            }

            // check against rooms that go out of bounds
            if (!this.Wrap && !this.DrawRect.Contains(gen.Draw))
                throw new InvalidOperationException("Tried to add out of range!");
            var plan = new FloorHallPlan(gen, components);

            // attach everything
            plan.Adjacents.AddRange(attached);
            foreach (RoomHallIndex fromRoom in attached)
            {
                IFloorRoomPlan fromPlan = this.GetRoomHall(fromRoom);
                fromPlan.Adjacents.Add(new RoomHallIndex(this.Halls.Count, true));
            }

            this.Halls.Add(plan);
        }

        /// <summary>
        /// Removes a room or hall from the floor plan and updates all adjacency references.
        /// </summary>
        /// <param name="roomHall">The index of the room or hall to remove.</param>
        /// <remarks>
        /// After removal, all indices greater than the removed index are decremented to maintain
        /// a contiguous index space. Adjacency lists of remaining rooms and halls are updated
        /// to reflect the removal and index changes.
        /// </remarks>
        public void EraseRoomHall(RoomHallIndex roomHall)
        {
            if (!roomHall.IsHall)
                this.Rooms.RemoveAt(roomHall.Index);
            else
                this.Halls.RemoveAt(roomHall.Index);

            // go through the rest of the rooms, removing the removed listroomhall from adjacents
            // also correcting their indices
            foreach (var plan in this.Rooms)
            {
                for (int jj = plan.Adjacents.Count - 1; jj >= 0; jj--)
                {
                    RoomHallIndex adj = plan.Adjacents[jj];
                    if (adj.IsHall == roomHall.IsHall)
                    {
                        if (adj.Index == roomHall.Index)
                            plan.Adjacents.RemoveAt(jj);
                        else if (adj.Index > roomHall.Index)
                            plan.Adjacents[jj] = new RoomHallIndex(adj.Index - 1, adj.IsHall);
                    }
                }
            }

            foreach (var plan in this.Halls)
            {
                for (int jj = plan.Adjacents.Count - 1; jj >= 0; jj--)
                {
                    RoomHallIndex adj = plan.Adjacents[jj];
                    if (adj.IsHall == roomHall.IsHall)
                    {
                        if (adj.Index == roomHall.Index)
                            plan.Adjacents.RemoveAt(jj);
                        else if (adj.Index > roomHall.Index)
                            plan.Adjacents[jj] = new RoomHallIndex(adj.Index - 1, adj.IsHall);
                    }
                }
            }
        }

        /// <summary>
        /// Gets all rooms that are reachable from the specified room, traversing through halls.
        /// </summary>
        /// <param name="roomIndex">The index of the starting room.</param>
        /// <returns>A list of room indices that are adjacent to the specified room, possibly through halls.</returns>
        /// <remarks>
        /// This method performs a breadth-first traversal starting from the given room, following
        /// adjacency links through halls but stopping at rooms. It returns all rooms reachable
        /// without passing through another room.
        /// </remarks>
        public virtual List<int> GetAdjacentRooms(int roomIndex)
        {
            RoomHallIndex fullIndex = new RoomHallIndex(roomIndex, false);

            // skips halls
            // every listroomplan keeps a list of adjacents for easy traversal
            // just because two rooms are next to each other doesn't mean they will be adjacents
            // their openings may not align and therefore have free reign to block the path off from each other
            // the rules of this generator only say that if you park two rooms next to each other,
            // you must prepare for the possibility that they become connected.
            // once again, the philosophy that some setups may be cheesable,
            // but all setups are completable.
            List<int> returnList = new List<int>();

            void NodeAct(RoomHallIndex nodeIndex, int distance)
            {
                // only add nodes that are
                if (nodeIndex.IsHall)
                    return; // Not a hall node

                if (nodeIndex == fullIndex)
                    return; // Not the start node

                returnList.Add(nodeIndex.Index);
            }

            List<RoomHallIndex> GetAdjacents(RoomHallIndex nodeIndex)
            {
                // do not add adjacents if we arrive on a room
                // unless it's the first one.
                if (nodeIndex == fullIndex)
                    return this.Rooms[roomIndex].Adjacents;
                else if (nodeIndex.IsHall)
                    return this.Halls[nodeIndex.Index].Adjacents;

                return new List<RoomHallIndex>();
            }

            Graph.TraverseBreadthFirst(fullIndex, NodeAct, GetAdjacents);

            return returnList;
        }

        /// <summary>
        /// Calculates the shortest path distance between two rooms or halls in terms of adjacency hops.
        /// </summary>
        /// <param name="roomFrom">The starting room or hall index.</param>
        /// <param name="roomTo">The destination room or hall index.</param>
        /// <returns>
        /// The number of adjacency hops between the two locations, or -1 if they are not connected.
        /// </returns>
        public int GetDistance(RoomHallIndex roomFrom, RoomHallIndex roomTo)
        {
            int returnValue = -1;
            void NodeAct(RoomHallIndex nodeIndex, int distance)
            {
                if (nodeIndex == roomTo)
                    returnValue = distance;
            }

            Graph.TraverseBreadthFirst(roomFrom, NodeAct, this.GetAdjacents);

            return returnValue;
        }

        /// <summary>
        /// Determines whether removing the specified room or hall would disconnect the floor plan.
        /// </summary>
        /// <param name="room">The room or hall index to test.</param>
        /// <returns>
        /// <c>true</c> if the room or hall is a choke point (its removal would split the graph);
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This is useful for identifying critical paths and ensuring that floor layouts remain
        /// fully connected. A room that is not a choke point can potentially be removed or
        /// replaced without breaking connectivity.
        /// </remarks>
        public bool IsChokePoint(RoomHallIndex room)
        {
            int roomsHit = 0;
            int hallsHit = 0;

            void NodeAct(RoomHallIndex nodeIndex, int distance)
            {
                if (!nodeIndex.IsHall)
                    roomsHit++;
                else
                    hallsHit++;
            }

            Graph.TraverseBreadthFirst(room, NodeAct, this.GetAdjacents);

            int totalRooms = roomsHit;
            int totalHalls = hallsHit;

            roomsHit = 0;
            hallsHit = 0;
            if (!room.IsHall)
                roomsHit++;
            else
                hallsHit++;

            List<RoomHallIndex> GetChokeAdjacents(RoomHallIndex nodeIndex)
            {
                List<RoomHallIndex> adjacents = new List<RoomHallIndex>();
                List<RoomHallIndex> roomAdjacents = this.GetRoomHall(nodeIndex).Adjacents;

                // do not add adjacents if we arrive on a room
                // unless it's the first one.
                foreach (RoomHallIndex adjacentRoom in roomAdjacents)
                {
                    // do not count the origin room
                    if (adjacentRoom == room)
                        continue;
                    adjacents.Add(adjacentRoom);
                }

                return adjacents;
            }

            IFloorRoomPlan plan = this.GetRoomHall(room);
            if (plan.Adjacents.Count > 0)
                Graph.TraverseBreadthFirst(plan.Adjacents[0], NodeAct, GetChokeAdjacents);

            return (roomsHit != totalRooms) || (hallsHit != totalHalls);
        }

        /// <summary>
        /// Moves the floor plan's starting position and adjusts all room and hall positions accordingly.
        /// </summary>
        /// <param name="offset">The new starting location for the floor plan.</param>
        public void MoveStart(Loc offset)
        {
            Loc diff = offset - this.Start;
            this.Start = offset;
            for (int ii = 0; ii < this.Rooms.Count; ii++)
                this.Rooms[ii].RoomGen.SetLoc(this.Rooms[ii].RoomGen.Draw.Start + diff);

            for (int ii = 0; ii < this.Halls.Count; ii++)
                this.Halls[ii].RoomGen.SetLoc(this.Halls[ii].RoomGen.Draw.Start + diff);
        }

        /// <summary>
        /// Resizes the floor plan without changing the start position.
        /// </summary>
        /// <param name="newSize">The new size for the floor plan.</param>
        /// <param name="dir">The direction in which to expand or contract the floor space.</param>
        /// <param name="anchorDir">The anchor point around which existing rooms maintain their relative positions.</param>
        /// <remarks>
        /// This method adjusts the floor plan dimensions and repositions all rooms and halls
        /// to maintain their relative positions according to the anchor direction.
        /// </remarks>
        public void Resize(Loc newSize, Dir8 dir, Dir8 anchorDir)
        {
            Loc diff = Grid.GetResizeOffset(this.Size.X, this.Size.Y, newSize.X, newSize.Y, dir);
            Loc anchorDiff = Grid.GetResizeOffset(this.Size.X, this.Size.Y, newSize.X, newSize.Y, anchorDir.Reverse());
            this.Start -= diff;
            this.Size = newSize;
            for (int ii = 0; ii < this.Rooms.Count; ii++)
                this.Rooms[ii].RoomGen.SetLoc(this.Rooms[ii].RoomGen.Draw.Start + anchorDiff - diff);

            for (int ii = 0; ii < this.Halls.Count; ii++)
                this.Halls[ii].RoomGen.SetLoc(this.Halls[ii].RoomGen.Draw.Start + anchorDiff - diff);
        }

        /// <summary>
        /// Renders all rooms and halls in the floor plan to the tile map.
        /// </summary>
        /// <param name="map">The tiled generation context to draw tiles onto.</param>
        /// <remarks>
        /// <para>
        /// This method is the final step in floor plan generation, converting the abstract
        /// room and hall layout into actual tiles. It processes rooms first, then halls,
        /// ensuring proper border negotiation between adjacent elements.
        /// </para>
        /// <para>
        /// During drawing, each room queries its adjacent rooms and halls for their fulfillable
        /// borders to determine where openings can be placed, then draws its tiles including
        /// walls and floor terrain.
        /// </para>
        /// </remarks>
        public void DrawOnMap(ITiledGenContext map)
        {
            GenContextDebug.StepIn("Main Rooms");
            try
            {
                for (int ii = 0; ii < this.Rooms.Count; ii++)
                {
                    // take in the broad fulfillables from adjacent rooms that have not yet drawn
                    IFloorRoomPlan plan = this.Rooms[ii];
                    foreach (RoomHallIndex adj in plan.Adjacents)
                    {
                        if (adj.IsHall || adj.Index > ii)
                        {
                            IRoomGen adjacentGen = this.GetRoomHall(adj).RoomGen;

                            Dir4 adjDir = this.GetDirAdjacent(plan.RoomGen, adjacentGen);
                            Rect wrapRect = this.GetAdjacentRect(plan.RoomGen.Draw, adjacentGen.Draw, adjDir).Value;
                            plan.RoomGen.AskBorderFromRoom(wrapRect, adjacentGen.GetFulfillableBorder, adjDir);
                        }
                    }

                    plan.RoomGen.DrawOnMap(map);
                    this.TransferBorderToAdjacents(new RoomHallIndex(ii, false));
                    GenContextDebug.DebugProgress("Draw Room");
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
                for (int ii = 0; ii < this.Halls.Count; ii++)
                {
                    // take in the broad fulfillables from adjacent rooms that have not yet drawn
                    IFloorRoomPlan plan = this.Halls[ii];
                    foreach (RoomHallIndex adj in plan.Adjacents)
                    {
                        if (adj.IsHall && adj.Index > ii)
                        {
                            IRoomGen adjacentGen = this.GetRoomHall(adj).RoomGen;

                            Dir4 adjDir = this.GetDirAdjacent(plan.RoomGen, adjacentGen);
                            Rect wrapRect = this.GetAdjacentRect(plan.RoomGen.Draw, adjacentGen.Draw, adjDir).Value;
                            plan.RoomGen.AskBorderFromRoom(wrapRect, adjacentGen.GetFulfillableBorder, adjDir);
                        }
                    }

                    plan.RoomGen.DrawOnMap(map);
                    this.TransferBorderToAdjacents(new RoomHallIndex(ii, true));
                    GenContextDebug.DebugProgress("Draw Hall");
                }
            }
            catch (Exception ex)
            {
                GenContextDebug.DebugError(ex);
            }

            GenContextDebug.StepOut();
        }

        /// <summary>
        /// Transfers border opening information from a drawn room to its adjacent rooms and halls.
        /// </summary>
        /// <param name="from">The room or hall that has just been drawn.</param>
        /// <remarks>
        /// After a room is drawn, its adjacent rooms and halls need to know which of its border
        /// tiles have openings. This method propagates that information so that adjacent elements
        /// can properly connect when they are drawn.
        /// </remarks>
        public void TransferBorderToAdjacents(RoomHallIndex from)
        {
            IFloorRoomPlan basePlan = this.GetRoomHall(from);
            IRoomGen roomGen = basePlan.RoomGen;
            List<RoomHallIndex> adjacents = basePlan.Adjacents;
            foreach (RoomHallIndex adjacent in adjacents)
            {
                // first determine if this adjacent should be receiving info
                if ((!from.IsHall && adjacent.IsHall) ||
                    (from.IsHall == adjacent.IsHall && from.Index < adjacent.Index))
                {
                    IRoomGen adjacentGen = this.GetRoomHall(adjacent).RoomGen;

                    Dir4 adjDir = this.GetDirAdjacent(adjacentGen, roomGen);
                    Rect wrapRect = this.GetAdjacentRect(adjacentGen.Draw, basePlan.RoomGen.Draw, adjDir).Value;
                    adjacentGen.AskBorderFromRoom(wrapRect, roomGen.GetOpenedBorder, adjDir);
                }
            }
        }

        /// <summary>
        /// Determines whether two rectangles collide, accounting for wrapping if enabled.
        /// </summary>
        /// <param name="rect1">The first rectangle.</param>
        /// <param name="rect2">The second rectangle.</param>
        /// <returns><c>true</c> if the rectangles overlap; otherwise, <c>false</c>.</returns>
        public bool Collides(Rect rect1, Rect rect2)
        {
            if (this.Wrap)
            {
                rect1 = new Rect(rect1.Start - this.Start, rect1.Size);
                rect2 = new Rect(rect2.Start - this.Start, rect2.Size);
                return WrappedCollision.Collides(this.Size, rect1, rect2);
            }
            else
            {
                return Collision.Collides(rect1, rect2);
            }
        }

        /// <summary>
        /// Determines whether a location is within a rectangle, accounting for wrapping if enabled.
        /// </summary>
        /// <param name="rect">The bounding rectangle.</param>
        /// <param name="loc">The location to test.</param>
        /// <returns><c>true</c> if the location is within the rectangle; otherwise, <c>false</c>.</returns>
        public bool InBounds(Rect rect, Loc loc)
        {
            if (this.Wrap)
            {
                rect = new Rect(rect.Start - this.Start, rect.Size);
                loc = loc - this.Start;
                return WrappedCollision.InBounds(this.Size, rect, loc);
            }
            else
            {
                return Collision.InBounds(rect, loc);
            }
        }

        /// <summary>
        /// Finds all rooms and halls that collide with the specified rectangle.
        /// </summary>
        /// <param name="rect">The rectangle to check for collisions.</param>
        /// <returns>A list of all room and hall indices that overlap with the rectangle.</returns>
        public List<RoomHallIndex> CheckCollision(Rect rect)
        {
            // gets all rooms/halls colliding with the rectangle
            List<RoomHallIndex> results = new List<RoomHallIndex>();
            for (int ii = 0; ii < this.Rooms.Count; ii++)
            {
                FloorRoomPlan room = this.Rooms[ii];
                if (this.Collides(room.RoomGen.Draw, rect))
                    results.Add(new RoomHallIndex(ii, false));
            }

            for (int ii = 0; ii < this.Halls.Count; ii++)
            {
                FloorHallPlan hall = this.Halls[ii];
                if (this.Collides(hall.RoomGen.Draw, rect))
                    results.Add(new RoomHallIndex(ii, true));
            }

            return results;
        }

        /// <summary>
        /// Enumerates all room and hall plans in the floor plan.
        /// </summary>
        /// <returns>An enumerable sequence of all room and hall plans.</returns>
        public IEnumerable<IRoomPlan> GetAllPlans()
        {
            foreach (FloorRoomPlan plan in this.Rooms)
                yield return plan;

            foreach (FloorHallPlan plan in this.Halls)
                yield return plan;
        }

        /// <summary>
        /// Gets the list of adjacent rooms and halls for a given room or hall.
        /// </summary>
        /// <param name="nodeIndex">The index of the room or hall.</param>
        /// <returns>The list of adjacent room and hall indices.</returns>
        /// <remarks>
        /// This method is designed for use with graph traversal algorithms like
        /// <see cref="Graph.TraverseBreadthFirst{T}"/>.
        /// </remarks>
        public virtual List<RoomHallIndex> GetAdjacents(RoomHallIndex nodeIndex)
        {
            return this.GetRoomHall(nodeIndex).Adjacents;
        }
    }
}
