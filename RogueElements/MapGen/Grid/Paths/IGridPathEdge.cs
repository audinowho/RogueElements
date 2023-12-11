using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IGridPathEdge
    {
        Dir4 Edge { get; set; }
        RandRange RoomRatio { get; set; }
        RandRange HallRatio { get; set; }

        RandRange HallBranchRatio { get; set; }
    }

    /// <summary>
    /// Populates the empty grid plan of a map by creating a minimum spanning tree of connected rooms and halls.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class GridPathEdge<T> : GridPathStartStepGeneric<T>, IGridPathEdge
        where T : class, IRoomGridGenContext
    {
        
        /// <summary>
        /// The edge of the map where the rooms will spawn in.
        /// </summary>
        public Dir4 Edge { get; set; }

        /// <summary>
        /// The percentage of total rooms in the edge that the step aims to fill.
        /// </summary>
        public RandRange RoomRatio { get; set; }

        /// <summary>
        /// The percentage of total halls in grid tiles not on the chosen edge that the step aims to fill.
        /// </summary>
        public RandRange HallRatio { get; set; }
        
        /// <summary>
        /// The percent amount of branching paths the hall layout will have in relation to its straight paths.
        /// 0 = A layout without branches. (Worm)
        /// 50 = A layout that branches once for every two extensions. (Tree)
        /// 100 = A layout that branches once for every extension. (Branchier Tree)
        /// 200 = A layout that branches twice for every extension. (Fuzzy Worm)
        /// </summary>
        public RandRange HallBranchRatio { get; set; }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            if (Edge == Dir4.None)
            {
                //Choose a random direction
                SpawnList<Dir4> possibleDirs = new SpawnList<Dir4>();
                possibleDirs.Add(Dir4.Down, 1);
                possibleDirs.Add(Dir4.Up, 1);
                possibleDirs.Add(Dir4.Left, 1);
                possibleDirs.Add(Dir4.Right, 1);

                Edge = possibleDirs.Pick(rand);
            }
            
            for (int ii = 0; ii < 10; ii++)
            {
                // always clear before trying
                floorPlan.Clear();

                int maxNumRooms = this.Edge == Dir4.Down || this.Edge == Dir4.Up
                    ? floorPlan.GridWidth
                    : floorPlan.GridHeight;

                int maxNumHalls = floorPlan.GridWidth * floorPlan.GridHeight - maxNumRooms;
                
                int roomsToOpen = maxNumRooms * this.RoomRatio.Pick(rand) / 100;
                if (roomsToOpen < 1)
                    roomsToOpen = 1;
                
                int hallsToOpen = maxNumHalls * this.HallRatio.Pick(rand) / 100;

                List<Loc> rooms = CreateRoomsOnEdge(rand, floorPlan, Edge, roomsToOpen);
                
                int hallsLeft = hallsToOpen;
                int pendingBranch = 0;
                int addBranch = this.HallBranchRatio.Pick(rand);
                
                List<Loc> terminals = new List<Loc>();
                List<Loc> branchables = new List<Loc>();

                foreach (Loc room in rooms)
                {
                    terminals.Add(room);
                    if (this.GetExpandDirChances(floorPlan, room).Count > 0)
                    {
                        branchables.Add(room);
                    }
                }

                while (hallsLeft > 0 && terminals.Count > 0)
                {
                    // pop a random hall loc from the terminals list
                    Loc newTerminal = this.PopRandomLoc(floorPlan, rand, terminals);

                    // find the directions to extend to
                    SpawnList<LocRay4> availableRays = this.GetExpandDirChances(floorPlan, newTerminal);
                    
                     if (availableRays.Count > 0)
                    {
                        // extend the path a random direction
                        LocRay4 terminalRay = availableRays.Pick(rand);
                        this.ExpandPathWithHall(rand, floorPlan, terminalRay);
                        Loc newRoomLoc = terminalRay.Traverse(1);
                        hallsLeft--;

                        // add the new terminal location to the terminals list
                        terminals.Add(newRoomLoc);
                        if (floorPlan.RoomCount > 2)
                        {
                            if (availableRays.Count > 1)
                                branchables.Add(newTerminal);

                            pendingBranch += addBranch;
                        }
                    }
                    else if (terminals.Count == 0)
                    {
                        pendingBranch = 100;
                    }

                    while (pendingBranch >= 100 && hallsLeft > 0 && branchables.Count > 0)
                    {
                        // pop a random loc from the branchables list
                        Loc newBranch = this.PopRandomLoc(floorPlan, rand, branchables);

                        // find the directions to extend to
                        SpawnList<LocRay4> availableBranchRays = this.GetExpandDirChances(floorPlan, newBranch);

                        if (availableBranchRays.Count > 0)
                        {
                            // extend the path a random direction
                            LocRay4 branchRay = availableBranchRays.Pick(rand);
                            this.ExpandPathWithHall(rand, floorPlan, branchRay);
                            Loc newRoomLoc = branchRay.Traverse(1);
                            hallsLeft--;

                            // add the new terminal location to the terminals list
                            terminals.Add(newRoomLoc);
                            if (availableBranchRays.Count > 1)
                                branchables.Add(newBranch);

                            pendingBranch -= 100;
                        }
                    }

                    if (terminals.Count == 0 && branchables.Count == 0)
                        break;
                }
            }
        }

        /// <summary>
        /// Creates the specified number of rooms along the edge, with a max set as the length of the edge
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="floorPlan"></param>
        /// <param name="edge"></param>
        /// <param name="numRooms"></param>
        /// <returns>A list of all rooms created along the edge</returns>
        protected List<Loc> CreateRoomsOnEdge(IRandom rand, GridPlan floorPlan, Dir4 edge, int numRooms)
        {

            bool isHorizontal = this.Edge == Dir4.Down || this.Edge == Dir4.Up;
            
            int edgeLength = isHorizontal
                ? floorPlan.GridWidth
                : floorPlan.GridHeight;
            
            List<int> roomIndexes = new List<int>();
            List<Loc> rooms = new List<Loc>();

            if (numRooms < 1)
            {
                numRooms = 1;
            }


            if (numRooms > edgeLength)
            {
                numRooms = edgeLength;
            }

            SpawnList<int> roomChoices = new SpawnList<int>();

            for (int i = 0; i < edgeLength; i++)
            {
                roomChoices.Add(i, 1);
            }

            for (int i = 0; i < numRooms; i++)
            {
                roomIndexes.Add(roomChoices.Pick(rand, true));
            }

            for (int i = 0; i < edgeLength; i++)
            {
                int x;
                int y;

                switch (edge)
                {
                    case Dir4.Down:
                        x = i;
                        y = floorPlan.GridHeight - 1;
                        break;
                    case Dir4.Up:
                        x = i;
                        y = 0;
                        break;
                    case Dir4.Left:
                        x = 0;
                        y = i;
                        break;
                    default:
                        //Right
                        x = floorPlan.GridWidth - 1;
                        y = i;
                        break;
                }

                Loc currentRoomLoc = new Loc(x, y);
                if (roomIndexes.Contains(i))
                {
                    floorPlan.AddRoom(currentRoomLoc, this.GenericRooms.Pick(rand), this.RoomComponents.Clone());
                    rooms.Add(currentRoomLoc);
                    
                }
                else
                {
                    floorPlan.AddRoom(currentRoomLoc, this.GetDefaultGen(), this.HallComponents.Clone(), true);
                }

                if (i > 0)
                {
                    floorPlan.SetHall(new LocRay4(currentRoomLoc, isHorizontal ? Dir4.Left : Dir4.Up), this.GenericHalls.Pick(rand), this.HallComponents.Clone());
                }
            }

            return rooms;
        }

        public override string ToString()
        {
            return string.Format("{0}: Edge:{1} Room Fill:{2}% Hall Fill:{3}%", this.GetType().GetFormattedTypeName(), this.Edge, this.RoomRatio, this.HallRatio);
        }

        /// <summary>
        /// Gets the directions a room can expand in.
        /// </summary>
        /// <param name="floorPlan"></param>
        /// <param name="loc"></param>
        /// <returns></returns>
        protected static IEnumerable<Dir4> GetRoomExpandDirs(GridPlan floorPlan, Loc loc)
        {
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                Loc endLoc = loc + dir.GetLoc();
                if ((floorPlan.Wrap || Collision.InBounds(floorPlan.GridWidth, floorPlan.GridHeight, endLoc))
                    && floorPlan.GetRoomIndex(endLoc) == -1)
                    yield return dir;
            }
        }

        /// <summary>
        /// Gets a possible terminal room to expand.  Equal distribution.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="locs"></param>
        /// <returns></returns>
        protected static Loc PopRandomLocEqual(IRandom rand, List<Loc> locs)
        {
            int branchIdx = rand.Next(locs.Count);
            Loc newBranch = locs[branchIdx];
            locs.RemoveAt(branchIdx);
            return newBranch;
        }

        protected virtual Loc PopRandomLoc(GridPlan floorPlan, IRandom rand, List<Loc> locs)
        {
            return PopRandomLocEqual(rand, locs);
        }

        protected bool ExpandPathWithHall(IRandom rand, GridPlan floorPlan, LocRay4 chosenRay)
        {
            floorPlan.SetHall(chosenRay, this.GenericHalls.Pick(rand), this.HallComponents.Clone());
            floorPlan.AddRoom(chosenRay.Traverse(1), this.GetDefaultGen(), this.HallComponents.Clone(), true);

            GenContextDebug.DebugProgress("Added Path");
            return true;
        }

        protected virtual SpawnList<LocRay4> GetExpandDirChances(GridPlan floorPlan, Loc newTerminal)
        {
            SpawnList<LocRay4> availableRays = new SpawnList<LocRay4>();
            foreach (Dir4 dir in GetRoomExpandDirs(floorPlan, newTerminal))
                availableRays.Add(new LocRay4(newTerminal, dir), 1);
            return availableRays;
        }
    }
}