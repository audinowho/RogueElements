using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IGridPathTreads
    {
        bool Vertical { get; set; }
        RandRange RoomPercent { get; set; }
        RandRange ConnectPercent { get; set; }
        ComponentCollection LargeRoomComponents { get; set; }
    }
    
    /// <summary>
    /// Creates a grid plan with two large "tread" rooms along the sides and a set of rooms in the middle.
    /// Inverse of GridPathBeetle.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class GridPathTreads<T> : GridPathStartStepGeneric<T>, IGridPathTreads
        where T : class, IRoomGridGenContext
    {
        /// <summary>
        /// Choose a horizontal or vertical orientation.
        /// </summary>
        public bool Vertical { get; set; }

        /// <summary>
        /// The number of small rooms attached to the main large rooms, as a percent of the rooms possible.
        /// </summary>
        public RandRange RoomPercent { get; set; }

        /// <summary>
        /// The number of connections between adjacent small rooms, as a percent of the connections possible.
        /// </summary>
        public RandRange ConnectPercent { get; set; }  
        
        /// <summary>
        /// The room types that can be used for the giant rooms in the layout.
        /// </summary>
        public SpawnList<RoomGen<T>> GiantRoomsGen;
        
        /// <summary>
        /// Components that the giant rooms will be labeled with.
        /// </summary>
        public ComponentCollection LargeRoomComponents { get; set; }
        
        
        public GridPathTreads()
            : base()
        {
            GiantRoomsGen = new SpawnList<RoomGen<T>>();
            LargeRoomComponents = new ComponentCollection();
        }
        
        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            int mainLength = Vertical ? floorPlan.GridHeight : floorPlan.GridWidth;
            int sideLength = Vertical ? floorPlan.GridWidth : floorPlan.GridHeight;

            if (mainLength < 3 || sideLength < 2)
            {
                CreateErrorPath(rand, floorPlan);
                return;
            }

            //add the two edge rooms
            int firstRoomIndex = 0;
            int secondRoomIndex = mainLength - 1;

            RoomGen<T> roomGen1 = GiantRoomsGen.Pick(rand);
            if (roomGen1 == null)
                roomGen1 = GenericRooms.Pick(rand);
            floorPlan.AddRoom(new Rect(Vertical ? 0 : firstRoomIndex, Vertical ? firstRoomIndex : 0, Vertical ? sideLength : 1, Vertical ? 1 : sideLength), roomGen1, this.LargeRoomComponents.Clone());

            
            RoomGen<T> roomGen2 = GiantRoomsGen.Pick(rand);
            if (roomGen2 == null)
                roomGen2 = GenericRooms.Pick(rand);
            floorPlan.AddRoom(new Rect(Vertical ? 0 : secondRoomIndex, Vertical ? secondRoomIndex : 0, Vertical ? sideLength : 1, Vertical ? 1 : sideLength), roomGen2, this.LargeRoomComponents.Clone());
            
            GenContextDebug.DebugProgress("Side Rooms");

            //add the rooms in the middle
            SpawnList<Loc> possibleRoomLocs = new SpawnList<Loc>();
            List<Loc> rooms = new List<Loc>();

            for (int i = 1; i < mainLength - 1; i++)
            {
                for (int j = 0; j < sideLength; j++)
                {
                    possibleRoomLocs.Add(Vertical ? new Loc(j, i) : new Loc(i, j), 1);
                }
            }

            int numMiddleRooms = (int)(possibleRoomLocs.Count * (RoomPercent.Pick(rand) / 100f));

            if (numMiddleRooms < 1)
            {
                numMiddleRooms = 1;
            }

            for (int i = 0; i < numMiddleRooms; i++)
            {
                rooms.Add(possibleRoomLocs.Pick(rand, true));
            }
            
            foreach (Loc room in rooms)
            {
                floorPlan.AddRoom(room, GenericRooms.Pick(rand), this.RoomComponents.Clone());
            }

            //Make hallway connections after all rooms have spawned
            foreach (Loc room in rooms)
            {
                AddHallwayConnections(rand, floorPlan, room, mainLength);
            }
            
            //Add additional side hallway connections based on the component connection
            AddAdditionalHallwayConnections(rand, floorPlan, rooms);
        }

        public override string ToString()
        {
            return string.Format("{0}: Vert:{1} Room:{2}% Connect:{3}%", this.GetType().GetFormattedTypeName(), this.Vertical, this.RoomPercent, this.ConnectPercent);
        }

        protected void AddHallwayConnections(IRandom rand, GridPlan floorPlan, Loc room, int mainLength)
        {
            int roomTier = Vertical ? room.Y : room.X;
            int roomSideIndex = Vertical ? room.X : room.Y;
            
            //Connect up
            int hasRoom = -1;
            for (int jj = roomTier - 1; jj >= 0; jj--)
            {
                if (floorPlan.GetRoomPlan(new Loc(Vertical ? roomSideIndex : jj, Vertical ? jj : roomSideIndex)) != null)
                {
                    hasRoom = jj;
                    break;
                }
            }
            if (roomTier > 0 && hasRoom > -1)
            {
                for (int jj = roomTier; jj > hasRoom; jj--)
                {
                    Loc curLoc = new Loc(Vertical ? roomSideIndex : jj, Vertical ? jj : roomSideIndex);

                    if (jj != roomTier && floorPlan.GetRoomPlan(curLoc) == null)
                    {
                        floorPlan.AddRoom(curLoc,this.GenericHalls.Pick(rand), this.HallComponents.Clone(), true);
                    }
                    SafeAddHall(new LocRay4(curLoc, Vertical ? Dir4.Up : Dir4.Left), 
                        floorPlan, GenericHalls.Pick(rand), GetDefaultGen(), this.RoomComponents, this.HallComponents,
                        true);
                }
            }
            
            GenContextDebug.DebugProgress("Connect Leg Up");
            
            //Connect down with the bottom room if possible
            
             hasRoom = -1;
            for (int jj = roomTier + 1; jj < mainLength; jj++)
            {
                if (floorPlan.GetRoomPlan(new Loc(Vertical ? roomSideIndex : jj, Vertical ? jj : roomSideIndex)) != null)
                {
                    hasRoom = jj;
                    break;
                }
            }
            if (roomTier > 0 && hasRoom == (mainLength - 1))
            {
                for (int jj = roomTier; jj < hasRoom; jj++)
                {
                    Loc curLoc = new Loc(Vertical ? roomSideIndex : jj, Vertical ? jj : roomSideIndex);

                    if (jj != roomTier && floorPlan.GetRoomPlan(curLoc) == null)
                    {
                        floorPlan.AddRoom(curLoc,this.GenericHalls.Pick(rand), this.HallComponents.Clone(), true);
                    }

                    SafeAddHall(new LocRay4(curLoc, Vertical ? Dir4.Down : Dir4.Right), 
                        floorPlan, GenericHalls.Pick(rand), GetDefaultGen(), this.RoomComponents, this.HallComponents,
                        true);

                }
            }
            
            GenContextDebug.DebugProgress("Connect Leg Down");
        }

        protected void AddAdditionalHallwayConnections(IRandom rand, GridPlan floorPlan, List<Loc> rooms)
        {
            //This stores a list of locs that can generate a left or up migration for a side hallway connection.
            SpawnList<Tuple<Loc, int>> possibleSideHallwaySources = new SpawnList<Tuple<Loc, int>>();
            List<Tuple<Loc, int>> sideHallwaySources = new List<Tuple<Loc, int>>();
            
            foreach (Loc room in rooms)
            {
                int hasRoom = -1;
                int roomTier = Vertical ? room.Y : room.X;
                int roomSideIndex = Vertical ? room.X : room.Y;

                if (roomSideIndex == 0)
                {
                    //Do not try to form connections from the leftmost rooms
                    continue;
                }
                
                for (int i = roomSideIndex - 1; i >= 0; i--)
                {
                    if (floorPlan.GetRoomPlan(new Loc(Vertical ? i : roomTier, Vertical ? roomTier : i)) !=
                        null)
                    {
                        hasRoom = i;
                        break;
                    }
                }

                if (hasRoom > -1)
                {
                    possibleSideHallwaySources.Add(new Tuple<Loc, int>(room, hasRoom), 1);
                }
            }

            int totalNumPossibleConnections = possibleSideHallwaySources.Count;

            int numConnections = (int)(totalNumPossibleConnections * (ConnectPercent.Pick(rand) / 100f));

            for (int i = 0; i < numConnections; i++)
            {
                sideHallwaySources.Add(possibleSideHallwaySources.Pick(rand, true));
            }
            
            foreach (Tuple<Loc, int> sideHallwaySource in sideHallwaySources)
            {
                Loc sourceLoc = sideHallwaySource.Item1;
                int targetSideIndex = sideHallwaySource.Item2;
                
                int sourceRoomTier = Vertical ? sourceLoc.Y : sourceLoc.X;
                int sourceRoomSideIndex = Vertical ? sourceLoc.X : sourceLoc.Y;
                
                for (int jj = sourceRoomSideIndex; jj > targetSideIndex; jj--)
                {
                    Loc curLoc = new Loc(Vertical ? jj : sourceRoomTier, Vertical ? sourceRoomTier : jj);

                    if (jj != sourceRoomSideIndex && floorPlan.GetRoomPlan(curLoc) == null)
                    {
                        floorPlan.AddRoom(curLoc,this.GenericHalls.Pick(rand), this.HallComponents.Clone(), true);
                    }
                    SafeAddHall(new LocRay4(curLoc, Vertical ? Dir4.Left : Dir4.Up), 
                        floorPlan, GenericHalls.Pick(rand), GetDefaultGen(), this.RoomComponents, this.HallComponents,
                        true);
                }
                GenContextDebug.DebugProgress("Connect Side Hallway");
            }
        }
    }
}