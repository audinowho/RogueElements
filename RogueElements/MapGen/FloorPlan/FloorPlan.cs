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

        public Rect DrawRect => new Rect(this.Start, this.Size);

        public virtual int RoomCount => this.Rooms.Count;

        public virtual int HallCount => this.Halls.Count;

        protected List<FloorRoomPlan> Rooms { get; private set; }

        protected List<FloorHallPlan> Halls { get; private set; }

        public static Dir4 GetDirAdjacent(IRoomGen roomGenFrom, IRoomGen roomGenTo)
        {
            Dir4 result = Dir4.None;
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                if (roomGenFrom.Draw.GetScalar(dir) == roomGenTo.Draw.GetScalar(dir.Reverse()))
                {
                    if (result == Dir4.None)
                        result = dir;
                    else
                        return Dir4.None;
                }
            }

            return result;
        }

        public static int GetBorderMatch(IRoomGen roomFrom, IRoomGen room, Loc candLoc, Dir4 expandTo)
        {
            int totalMatch = 0;

            Loc diff = roomFrom.Draw.Start - candLoc; // how far ahead the start of source is to dest
            int offset = diff.GetScalar(expandTo.ToAxis().Orth());

            // Traverse the region that both borders touch
            int sourceLength = roomFrom.GetBorderLength(expandTo);
            int destLength = room.GetBorderLength(expandTo.Reverse());
            for (int ii = Math.Max(0, offset); ii - offset < sourceLength && ii < destLength; ii++)
            {
                bool sourceFulfill = roomFrom.GetFulfillableBorder(expandTo, ii - offset);
                bool destFulfill = room.GetFulfillableBorder(expandTo.Reverse(), ii);
                if (sourceFulfill && destFulfill)
                    totalMatch++;
            }

            return totalMatch;
        }

        public void InitSize(Loc size)
        {
            this.Size = size;
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

        public void AddRoom(IRoomGen gen, bool immutable, ComponentCollection components, params RoomHallIndex[] attached)
        {
            // check against colliding on other rooms (and not halls)
            foreach (var room in this.Rooms)
            {
                if (Collision.Collides(room.RoomGen.Draw, gen.Draw))
                    throw new InvalidOperationException("Tried to add on top of an existing room!");
            }

            foreach (var hall in this.Halls)
            {
                if (Collision.Collides(hall.RoomGen.Draw, gen.Draw))
                    throw new InvalidOperationException("Tried to add on top of an existing hall!");
            }

            // check against rooms that go out of bounds
            if (!this.DrawRect.Contains(gen.Draw))
                throw new InvalidOperationException("Tried to add out of range!");

            // we expect that the room has already been given a size
            // and that its fulfillables match up with its adjacent's fulfillables.
            var plan = new FloorRoomPlan(gen, components, immutable);

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
                if (Collision.Collides(room.RoomGen.Draw, gen.Draw))
                    throw new InvalidOperationException("Tried to add on top of an existing room!");
            }

            // check against rooms that go out of bounds
            if (!this.DrawRect.Contains(gen.Draw))
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
            // skips halls
            // every listroomplan keeps a list of adjacents for easy traversal
            // just because two rooms are next to each other doesn't mean they will be adjacents
            // their openings may not align and therefore have free reign to block the path off from each other
            // the rules of this generator only say that if you park two rooms next to each other,
            // you must prepare for the possibility that they become connected.
            // once again, the philosophy that some setups may be cheesable,
            // but all setups are completable.
            List<int> returnList = new List<int>();

            void NodeAct(int nodeIndex, int distance)
            {
                // only add nodes that are
                if (nodeIndex == roomIndex)
                    return; // A) Not the start node
                if (nodeIndex >= this.Rooms.Count)
                    return; // B) Not a hall node

                returnList.Add(nodeIndex);
            }

            List<int> GetAdjacents(int nodeIndex)
            {
                List<int> adjacents = new List<int>();
                List<RoomHallIndex> roomAdjacents = new List<RoomHallIndex>();

                // do not add adjacents if we arrive on a room
                // unless it's the first one.
                if (nodeIndex == roomIndex)
                    roomAdjacents = this.Rooms[roomIndex].Adjacents;
                else if (nodeIndex >= this.Rooms.Count)
                    roomAdjacents = this.Halls[nodeIndex - this.Rooms.Count].Adjacents;

                foreach (RoomHallIndex adjacentRoom in roomAdjacents)
                {
                    if (adjacentRoom.IsHall)
                        adjacents.Add(adjacentRoom.Index + this.Rooms.Count);
                    else
                        adjacents.Add(adjacentRoom.Index);
                }

                return adjacents;
            }

            Graph.TraverseBreadthFirst(this.Rooms.Count + this.Halls.Count, roomIndex, NodeAct, GetAdjacents);

            return returnList;
        }

        public int GetDistance(RoomHallIndex roomFrom, RoomHallIndex roomTo)
        {
            int returnValue = -1;
            int startIndex = roomFrom.Index + (roomFrom.IsHall ? this.Rooms.Count : 0);
            int endIndex = roomTo.Index + (roomTo.IsHall ? this.Rooms.Count : 0);

            void NodeAct(int nodeIndex, int distance)
            {
                if (nodeIndex == endIndex)
                    returnValue = distance;
            }

            Graph.TraverseBreadthFirst(this.Rooms.Count + this.Halls.Count, startIndex, NodeAct, this.GetBreadthFirstAdjacents);

            return returnValue;
        }

        public bool IsChokePoint(RoomHallIndex room)
        {
            int roomsHit = 0;
            int hallsHit = 0;

            void NodeAct(int nodeIndex, int distance)
            {
                if (nodeIndex < this.Rooms.Count)
                    roomsHit++;
                else
                    hallsHit++;
            }

            int startIndex = room.Index + (room.IsHall ? this.Rooms.Count : 0);

            Graph.TraverseBreadthFirst(this.Rooms.Count + this.Halls.Count, startIndex, NodeAct, this.GetBreadthFirstAdjacents);

            int totalRooms = roomsHit;
            int totalHalls = hallsHit;

            roomsHit = 0;
            hallsHit = 0;
            if (!room.IsHall)
                roomsHit++;
            else
                hallsHit++;

            List<int> GetChokeAdjacents(int nodeIndex)
            {
                List<int> adjacents = new List<int>();
                List<RoomHallIndex> roomAdjacents = new List<RoomHallIndex>();

                // do not add adjacents if we arrive on a room
                // unless it's the first one.
                if (nodeIndex < this.Rooms.Count)
                    roomAdjacents = this.Rooms[nodeIndex].Adjacents;
                else
                    roomAdjacents = this.Halls[nodeIndex - this.Rooms.Count].Adjacents;

                foreach (RoomHallIndex adjacentRoom in roomAdjacents)
                {
                    // do not count the origin room
                    if (adjacentRoom == room)
                        continue;
                    if (adjacentRoom.IsHall)
                        adjacents.Add(adjacentRoom.Index + this.Rooms.Count);
                    else
                        adjacents.Add(adjacentRoom.Index);
                }

                return adjacents;
            }

            IFloorRoomPlan plan = this.GetRoomHall(room);
            if (plan.Adjacents.Count > 0)
            {
                int adjIndex = plan.Adjacents[0].Index + (plan.Adjacents[0].IsHall ? this.Rooms.Count : 0);

                Graph.TraverseBreadthFirst(this.Rooms.Count + this.Halls.Count, adjIndex, NodeAct, GetChokeAdjacents);
            }

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

        public void DrawOnMap(ITiledGenContext map)
        {
            GenContextDebug.StepIn("Main Rooms");
            for (int ii = 0; ii < this.Rooms.Count; ii++)
            {
                // take in the broad fulfillables from adjacent rooms that have not yet drawn
                IFloorRoomPlan plan = this.Rooms[ii];
                foreach (RoomHallIndex adj in plan.Adjacents)
                {
                    if (adj.IsHall || adj.Index > ii)
                    {
                        IRoomGen adjacentGen = this.GetRoomHall(adj).RoomGen;
                        plan.RoomGen.ReceiveFulfillableBorder(adjacentGen, GetDirAdjacent(plan.RoomGen, adjacentGen));
                    }
                }

                plan.RoomGen.DrawOnMap(map);
                this.TransferBorderToAdjacents(new RoomHallIndex(ii, false));
                GenContextDebug.DebugProgress("Draw Room");
            }

            GenContextDebug.StepOut();

            GenContextDebug.StepIn("Connecting Halls");
            for (int ii = 0; ii < this.Halls.Count; ii++)
            {
                // take in the broad fulfillables from adjacent rooms that have not yet drawn
                IFloorRoomPlan plan = this.Halls[ii];
                foreach (RoomHallIndex adj in plan.Adjacents)
                {
                    if (adj.IsHall && adj.Index > ii)
                    {
                        IRoomGen adjacentGen = this.GetRoomHall(adj).RoomGen;
                        plan.RoomGen.ReceiveFulfillableBorder(adjacentGen, GetDirAdjacent(plan.RoomGen, adjacentGen));
                    }
                }

                plan.RoomGen.DrawOnMap(map);
                this.TransferBorderToAdjacents(new RoomHallIndex(ii, true));
                GenContextDebug.DebugProgress("Draw Hall");
            }

            GenContextDebug.StepOut();
        }

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
                    adjacentGen.ReceiveOpenedBorder(roomGen, GetDirAdjacent(adjacentGen, roomGen));
                }
            }
        }

        public List<RoomHallIndex> CheckCollision(Rect rect)
        {
            // gets all rooms/halls colliding with the rectangle
            List<RoomHallIndex> results = new List<RoomHallIndex>();
            for (int ii = 0; ii < this.Rooms.Count; ii++)
            {
                FloorRoomPlan room = this.Rooms[ii];
                if (Collision.Collides(room.RoomGen.Draw, rect))
                    results.Add(new RoomHallIndex(ii, false));
            }

            for (int ii = 0; ii < this.Halls.Count; ii++)
            {
                FloorHallPlan hall = this.Halls[ii];
                if (Collision.Collides(hall.RoomGen.Draw, rect))
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

        private List<int> GetBreadthFirstAdjacents(int nodeIndex)
        {
            List<int> adjacents = new List<int>();
            List<RoomHallIndex> roomAdjacents;

            // do not add adjacents if we arrive on a room
            // unless it's the first one.
            if (nodeIndex < this.Rooms.Count)
                roomAdjacents = this.Rooms[nodeIndex].Adjacents;
            else
                roomAdjacents = this.Halls[nodeIndex - this.Rooms.Count].Adjacents;

            foreach (RoomHallIndex adjacentRoom in roomAdjacents)
            {
                if (adjacentRoom.IsHall)
                    adjacents.Add(adjacentRoom.Index + this.Rooms.Count);
                else
                    adjacents.Add(adjacentRoom.Index);
            }

            return adjacents;
        }
    }
}
