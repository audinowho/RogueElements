using System;
using System.Collections.Generic;

namespace RogueElements
{

    public class FloorPlan
    {
        protected List<FloorRoomPlan> rooms;
        protected List<FloorHallPlan> halls;
        
        public Loc Size;
        public Loc Start { get; private set; }

        public Rect DrawRect { get { return new Rect(Start, Size); } }

        public virtual int RoomCount { get { return rooms.Count; } }
        public virtual int HallCount { get { return halls.Count; } }

        public FloorPlan()
        { }


        public void InitSize(Loc size)
        {
            Size = size;
            rooms = new List<FloorRoomPlan>();
            halls = new List<FloorHallPlan>();
        }

        public void Clear()
        {
            rooms.Clear();
            halls.Clear();
        }


        public FloorRoomPlan GetRoomPlan(int index)
        {
            return rooms[index];
        }


        public virtual IRoomGen GetRoom(int index)
        {
            return rooms[index].RoomGen;
        }

        public virtual IPermissiveRoomGen GetHall(int index)
        {
            return halls[index].RoomGen;
        }


        public virtual BaseFloorRoomPlan GetRoomHall(RoomHallIndex room)
        {
            if (!room.IsHall)
                return rooms[room.Index];
            else
                return halls[room.Index];
        }


        public void AddRoom(IRoomGen gen, bool immutable, params RoomHallIndex[] attached)
        {
            //check against colliding on other rooms (and not halls)
            for (int ii = 0; ii < rooms.Count; ii++)
            {
                FloorRoomPlan room = rooms[ii];
                if (Collision.Collides(room.RoomGen.Draw, gen.Draw))
                    throw new InvalidOperationException("Tried to add on top of an existing room!");
            }
            for (int ii = 0; ii < halls.Count; ii++)
            {
                FloorHallPlan hall = halls[ii];
                if (Collision.Collides(hall.RoomGen.Draw, gen.Draw))
                    throw new InvalidOperationException("Tried to add on top of an existing hall!");
            }
            //check against rooms that go out of bounds
            if (!DrawRect.Contains(gen.Draw))
                throw new InvalidOperationException("Tried to add out of range!");
            //we expect that the room has already been given a size
            //and that its fulfillables match up with its adjacent's fulfillables.
            FloorRoomPlan plan = new FloorRoomPlan(gen);
            plan.Immutable = immutable;
            //attach everything
            plan.Adjacents.AddRange(attached);
            foreach (RoomHallIndex fromRoom in attached)
            {
                BaseFloorRoomPlan fromPlan = GetRoomHall(fromRoom);
                fromPlan.Adjacents.Add(new RoomHallIndex(rooms.Count, false));
            }

            rooms.Add(plan);
        }

        public void AddHall(IPermissiveRoomGen gen, params RoomHallIndex[] attached)
        {
            //we expect that the hall has already been given a size...
            //check against colliding on other rooms (and not halls)
            for (int ii = 0; ii < rooms.Count; ii++)
            {
                FloorRoomPlan room = rooms[ii];
                if (Collision.Collides(room.RoomGen.Draw, gen.Draw))
                    throw new InvalidOperationException("Tried to add on top of an existing room!");
            }
            //check against rooms that go out of bounds
            if (!DrawRect.Contains(gen.Draw))
                throw new InvalidOperationException("Tried to add out of range!");
            FloorHallPlan plan = new FloorHallPlan(gen);
            //attach everything
            plan.Adjacents.AddRange(attached);
            foreach (RoomHallIndex fromRoom in attached)
            {
                BaseFloorRoomPlan fromPlan = GetRoomHall(fromRoom);
                fromPlan.Adjacents.Add(new RoomHallIndex(halls.Count, true));
            }
            halls.Add(plan);
        }

        public void EraseRoomHall(RoomHallIndex roomHall)
        {
            if (!roomHall.IsHall)
                rooms.RemoveAt(roomHall.Index);
            else
                halls.RemoveAt(roomHall.Index);

            //go through the rest of the rooms, removing the removed listroomhall from adjacents
            //also correcting their indices
            for (int ii = 0; ii < rooms.Count; ii++)
            {
                FloorRoomPlan plan = rooms[ii];
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
            for (int ii = 0; ii < halls.Count; ii++)
            {
                FloorHallPlan plan = halls[ii];
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
            //skips halls
            //every listroomplan keeps a list of adjacents for easy traversal
            //just because two rooms are next to each other doesn't mean they will be adjacents
            //their openings may not align and therefore have free reign to block the path off from each other
            //the rules of this generator only say that if you park two rooms next to each other,
            //you must prepare for the possibility that they become connected.
            //once again, the philosophy that some setups may be cheesable,
            //but all setups are completable.
            List<int> returnList = new List<int>();

            Graph.DistNodeAction nodeAct = (int nodeIndex, int distance) =>
            {
                //only add nodes that are
                //A) Not the start node
                if (nodeIndex == roomIndex)
                    return;
                //B) Not a hall node
                if (nodeIndex >= rooms.Count)
                    return;

                returnList.Add(nodeIndex);
            };
            Graph.GetAdjacents getAdjacents = (int nodeIndex) =>
            {
                List<int> adjacents = new List<int>();
                List<RoomHallIndex> roomAdjacents = new List<RoomHallIndex>();

                //do not add adjacents if we arrive on a room
                //unless it's the first one.
                if (nodeIndex == roomIndex)
                    roomAdjacents = rooms[roomIndex].Adjacents;
                else if (nodeIndex >= rooms.Count)
                    roomAdjacents = halls[nodeIndex - rooms.Count].Adjacents;
                
                foreach (RoomHallIndex adjacentRoom in roomAdjacents)
                {
                    if (adjacentRoom.IsHall)
                        adjacents.Add(adjacentRoom.Index + rooms.Count);
                    else
                        adjacents.Add(adjacentRoom.Index);
                }

                return adjacents;
            };

            Graph.TraverseBreadthFirst(rooms.Count + halls.Count, roomIndex, nodeAct, getAdjacents);

            return returnList;
        }



        public int GetDistance(RoomHallIndex roomFrom, RoomHallIndex roomTo)
        {
            int returnValue = -1;
            int startIndex = roomFrom.Index + (roomFrom.IsHall ? rooms.Count : 0);
            int endIndex = roomTo.Index + (roomTo.IsHall ? rooms.Count : 0);

            Graph.DistNodeAction nodeAct = (int nodeIndex, int distance) =>
            {
                if (nodeIndex == endIndex)
                    returnValue = distance;
            };

            Graph.TraverseBreadthFirst(rooms.Count + halls.Count, startIndex, nodeAct, getBreadthFirstAdjacents);
            
            return returnValue;
        }

        public bool IsChokePoint(RoomHallIndex room)
        {
            int roomsHit = 0;
            int hallsHit = 0;

            Graph.DistNodeAction nodeAct = (int nodeIndex, int distance) =>
            {
                if (nodeIndex < rooms.Count)
                    roomsHit++;
                else
                    hallsHit++;
            };

            int startIndex = room.Index + (room.IsHall ? rooms.Count : 0);

            Graph.TraverseBreadthFirst(rooms.Count + halls.Count, startIndex, nodeAct, getBreadthFirstAdjacents);

            int totalRooms = roomsHit;
            int totalHalls = hallsHit;

            roomsHit = 0;
            hallsHit = 0;
            if (!room.IsHall)
                roomsHit++;
            else
                hallsHit++;


            Graph.GetAdjacents getChokeAdjacents = (int nodeIndex) =>
            {
                List<int> adjacents = new List<int>();
                List<RoomHallIndex> roomAdjacents = new List<RoomHallIndex>();

                //do not add adjacents if we arrive on a room
                //unless it's the first one.
                if (nodeIndex < rooms.Count)
                    roomAdjacents = rooms[nodeIndex].Adjacents;
                else
                    roomAdjacents = halls[nodeIndex - rooms.Count].Adjacents;

                foreach (RoomHallIndex adjacentRoom in roomAdjacents)
                {
                    //do not count the origin room
                    if (adjacentRoom == room)
                        continue;
                    if (adjacentRoom.IsHall)
                        adjacents.Add(adjacentRoom.Index + rooms.Count);
                    else
                        adjacents.Add(adjacentRoom.Index);
                }

                return adjacents;
            };

            BaseFloorRoomPlan plan = GetRoomHall(room);
            if (plan.Adjacents.Count > 0)
            {
                int adjIndex = plan.Adjacents[0].Index + (plan.Adjacents[0].IsHall ? rooms.Count : 0);

                Graph.TraverseBreadthFirst(rooms.Count + halls.Count, adjIndex, nodeAct, getChokeAdjacents);
            }

            return (roomsHit != totalRooms) || (hallsHit != totalHalls);
        }

        public void MoveStart(Loc offset)
        {
            Loc diff = offset - Start;
            Start = offset;
            for (int ii = 0; ii < rooms.Count; ii++)
                rooms[ii].Gen.SetLoc(rooms[ii].Gen.Draw.Start + diff);

            for (int ii = 0; ii < halls.Count; ii++)
                halls[ii].Gen.SetLoc(halls[ii].Gen.Draw.Start + diff);
        }

        public void DrawOnMap(ITiledGenContext map)
        {

            GenContextDebug.StepIn("Main Rooms");
            for (int ii = 0; ii < rooms.Count; ii++)
            {
                //take in the broad fulfillables from adjacent rooms that have not yet drawn
                BaseFloorRoomPlan plan = rooms[ii];
                foreach (RoomHallIndex adj in plan.Adjacents)
                {
                    if (adj.IsHall || adj.Index > ii)
                    {
                        IRoomGen adjacentGen = GetRoomHall(adj).Gen;
                        plan.Gen.ReceiveFulfillableBorder(adjacentGen, GetDirAdjacent(plan.Gen, adjacentGen));
                    }
                }
                plan.Gen.DrawOnMap(map);
                TransferBorderToAdjacents(new RoomHallIndex(ii, false));
                GenContextDebug.DebugProgress("Draw Room");
            }
            GenContextDebug.StepOut();

            GenContextDebug.StepIn("Connecting Halls");
            for (int ii = 0; ii < halls.Count; ii++)
            {
                //take in the broad fulfillables from adjacent rooms that have not yet drawn
                BaseFloorRoomPlan plan = halls[ii];
                foreach (RoomHallIndex adj in plan.Adjacents)
                {
                    if (adj.IsHall && adj.Index > ii)
                    {
                        IRoomGen adjacentGen = GetRoomHall(adj).Gen;
                        plan.Gen.ReceiveFulfillableBorder(adjacentGen, GetDirAdjacent(plan.Gen, adjacentGen));
                    }
                }
                plan.Gen.DrawOnMap(map);
                TransferBorderToAdjacents(new RoomHallIndex(ii, true));
                GenContextDebug.DebugProgress("Draw Hall");
            }
            GenContextDebug.StepOut();
        }

        private List<int> getBreadthFirstAdjacents(int nodeIndex)
        {
            List<int> adjacents = new List<int>();
            List<RoomHallIndex> roomAdjacents = new List<RoomHallIndex>();

            //do not add adjacents if we arrive on a room
            //unless it's the first one.
            if (nodeIndex < rooms.Count)
                roomAdjacents = rooms[nodeIndex].Adjacents;
            else
                roomAdjacents = halls[nodeIndex - rooms.Count].Adjacents;

            foreach (RoomHallIndex adjacentRoom in roomAdjacents)
            {
                if (adjacentRoom.IsHall)
                    adjacents.Add(adjacentRoom.Index + rooms.Count);
                else
                    adjacents.Add(adjacentRoom.Index);
            }

            return adjacents;
        }

        public void TransferBorderToAdjacents(RoomHallIndex from)
        {
            BaseFloorRoomPlan basePlan = GetRoomHall(from);
            IRoomGen roomGen = basePlan.Gen;
            List<RoomHallIndex> adjacents = basePlan.Adjacents;
            foreach (RoomHallIndex adjacent in adjacents)
            {
                //first determine if this adjacent should be receiving info
                if (!from.IsHall && adjacent.IsHall ||
                    from.IsHall == adjacent.IsHall && from.Index < adjacent.Index)
                {
                    IRoomGen adjacentGen = GetRoomHall(adjacent).Gen;
                    adjacentGen.ReceiveOpenedBorder(roomGen, GetDirAdjacent(adjacentGen, roomGen));
                }
            }
        }

        public Dir4 GetDirAdjacent(IRoomGen roomGenFrom, IRoomGen roomGenTo)
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

        public List<RoomHallIndex> CheckCollision(Rect rect)
        {
            //gets all rooms/halls colliding with the rectangle
            List<RoomHallIndex> results = new List<RoomHallIndex>();
            for (int ii = 0; ii < rooms.Count; ii++)
            {
                FloorRoomPlan room = rooms[ii];
                if (Collision.Collides(room.RoomGen.Draw, rect))
                    results.Add(new RoomHallIndex(ii, false));
            }
            for (int ii = 0; ii < halls.Count; ii++)
            {
                FloorHallPlan hall = halls[ii];
                if (Collision.Collides(hall.RoomGen.Draw, rect))
                    results.Add(new RoomHallIndex(ii, true));
            }
            return results;
        }
        

        public static int GetBorderMatch(IRoomGen roomFrom, IRoomGen room, Loc candLoc, Dir4 expandTo)
        {
            int totalMatch = 0;

            Loc diff = roomFrom.Draw.Start - candLoc;//how far ahead the start of source is to dest
            int offset = diff.GetScalar(expandTo.ToAxis().Orth());

            //Traverse the region that both borders touch
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

    }
    
}
