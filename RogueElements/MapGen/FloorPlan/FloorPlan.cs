// <copyright file="FloorPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public class FloorPlan
    {
        public FloorPlan()
        {
        }

        public Loc Size { get; private set; }

        public Loc Start { get; private set; }

        public bool Wrap { get; private set; }

        public Rect DrawRect => new Rect(this.Start, this.Size);

        public virtual int RoomCount => this.Rooms.Count;

        public virtual int HallCount => this.Halls.Count;

        protected List<FloorRoomPlan> Rooms { get; private set; }

        protected List<FloorHallPlan> Halls { get; private set; }

        /// <summary>
        /// Gets the amount of tiles that overlap when adding a new room adjacent to an existing room.
        /// </summary>
        /// <param name="roomFrom">The room to have the new room added to.</param>
        /// <param name="room">The new room to add. Its current position is not final.</param>
        /// <param name="candLoc">The proposed location of the new room. Assumes this loc is indeed adjacent to the roomFrom, even in wrapped scenarios.</param>
        /// <param name="expandTo">The direction to expand from the old room to new room.</param>
        /// <returns></returns>
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
        /// Given two rectangles that are meant to be adjacent to each other, with a valid direction of adjacency,
        /// Gets the unwrapped version of the second rectangle that is adjacent to the first.
        /// </summary>
        /// <param name="rectFrom"></param>
        /// <param name="rectTo"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
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
        /// Gets the direction of adjacency.
        /// </summary>
        /// <param name="roomGenFrom"></param>
        /// <param name="roomGenTo"></param>
        /// <returns></returns>
        public Dir4 GetDirAdjacent(IRoomGen roomGenFrom, IRoomGen roomGenTo)
        {
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                if (this.GetAdjacentRect(roomGenFrom.Draw, roomGenTo.Draw, dir) != null)
                    return dir;
            }

            return Dir4.None;
        }

        public void InitSize(Loc size, bool wrap = false)
        {
            this.InitRect(new Rect(Loc.Zero, size), wrap);
        }

        public void InitRect(Rect rect, bool wrap)
        {
            this.Start = rect.Start;
            this.Size = rect.Size;
            this.Wrap = wrap;
            this.Rooms = new List<FloorRoomPlan>();
            this.Halls = new List<FloorHallPlan>();
        }

        public void Clear()
        {
            this.Rooms.Clear();
            this.Halls.Clear();
        }

        public virtual FloorRoomPlan GetRoomPlan(int index)
        {
            return this.Rooms[index];
        }

        public virtual IRoomGen GetRoom(int index)
        {
            return this.Rooms[index].RoomGen;
        }

        public virtual FloorHallPlan GetHallPlan(int index)
        {
            return this.Halls[index];
        }

        public virtual IPermissiveRoomGen GetHall(int index)
        {
            return this.Halls[index].RoomGen;
        }

        public virtual IFloorRoomPlan GetRoomHall(RoomHallIndex room)
        {
            if (!room.IsHall)
                return this.Rooms[room.Index];
            else
                return this.Halls[room.Index];
        }

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
        /// Changes size without changing the start.
        /// </summary>
        /// <param name="newSize"></param>
        /// <param name="dir"></param>
        /// <param name="anchorDir">The anchor point of the initial floor rect.</param>
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
        /// A room's draw has been completed.  It must now signal to its adjacent rooms which of its borders are open.
        /// </summary>
        /// <param name="from"></param>
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

        public IEnumerable<IRoomPlan> GetAllPlans()
        {
            foreach (FloorRoomPlan plan in this.Rooms)
                yield return plan;

            foreach (FloorHallPlan plan in this.Halls)
                yield return plan;
        }

        public virtual List<RoomHallIndex> GetAdjacents(RoomHallIndex nodeIndex)
        {
            return this.GetRoomHall(nodeIndex).Adjacents;
        }
    }
}
