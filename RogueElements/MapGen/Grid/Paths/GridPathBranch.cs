using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class GridPathBranch<T> : GridPathStartStepGeneric<T>
        where T : class, IRoomGridGenContext
    {
        public RandRange RoomRatio;
        public RandRange BranchRatio;
        public bool NoForcedBranches;

        public GridPathBranch()
            : base()
        { }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            for (int ii = 0; ii < 10; ii++)
            {
                //always clear before trying
                floorPlan.Clear();

                int roomsToOpen = floorPlan.GridWidth * floorPlan.GridHeight * RoomRatio.Pick(rand) / 100;
                if (roomsToOpen < 1)
                    roomsToOpen = 1;
                int addBranch = BranchRatio.Pick(rand);
                int roomsLeft = roomsToOpen;

                Loc SourceRoom = new Loc(rand.Next(floorPlan.GridWidth), rand.Next(floorPlan.GridHeight)); // randomly determine start room
                floorPlan.AddRoom(SourceRoom, GenericRooms.Pick(rand));

                GenContextDebug.DebugProgress("Start Room");

                roomsLeft--;
                int pendingBranch = 0;
                while (roomsLeft > 0)
                {
                    bool terminalSuccess = expandPath(rand, floorPlan, false);
                    bool branchSuccess = false;
                    if (terminalSuccess)
                    {
                        roomsLeft--;
                        if (floorPlan.RoomCount > 2)
                            pendingBranch += addBranch;
                    }
                    else if (NoForcedBranches)
                        break;
                    else
                        pendingBranch = 100;
                    while (pendingBranch >= 100 && roomsLeft > 0)
                    {
                        branchSuccess = expandPath(rand, floorPlan, true);
                        if (!branchSuccess)
                            break;
                        pendingBranch -= 100;
                        roomsLeft--;
                    }
                    if (!terminalSuccess && !branchSuccess)
                        break;
                }
                if (roomsLeft <= 0)
                    break;
            }
            
        }


        protected bool expandPath(IRandom rand, GridPlan floorPlan, bool branch)
        {
            LocRay4 chosenRay = ChooseRoomExpansion(rand, floorPlan, branch);
            if (chosenRay.Dir == Dir4.None)
                return false;
            Loc endLoc = chosenRay.Traverse(1);
            floorPlan.SetConnectingHall(chosenRay.Loc, endLoc, GenericHalls.Pick(rand));
            floorPlan.AddRoom(endLoc, GenericRooms.Pick(rand));

            GenContextDebug.DebugProgress(branch ? "Branched Path" : "Extended Path");
            return true;
        }

        public virtual LocRay4 ChooseRoomExpansion(IRandom rand, GridPlan floorPlan, bool branch)
        {
            List<LocRay4> availableRays = GetPossibleExpansions(floorPlan, branch);

            if (availableRays.Count == 0)
                return new LocRay4(Dir4.None);

            LocRay4 chosenRay = availableRays[rand.Next(availableRays.Count)];
            return chosenRay;
        }

        public List<LocRay4> GetPossibleExpansions(GridPlan floorPlan, bool branch)
        {
            List<LocRay4> availableRays = new List<LocRay4>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                List<int> adjacents = floorPlan.GetAdjacentRooms(ii);
                if ((adjacents.Count <= 1) != branch)
                {
                    foreach (Dir4 dir in getRoomExpandDirs(floorPlan, floorPlan.GetRoomPlan(ii).Bounds.Start))
                        availableRays.Add(new LocRay4(floorPlan.GetRoomPlan(ii).Bounds.Start, dir));
                }
            }
            return availableRays;
        }


        /// <summary>
        /// Gets the directions a room can expand in.
        /// </summary>
        /// <param name="floorPlan"></param>
        /// <param name="loc"></param>
        /// <returns></returns>
        private IEnumerable<Dir4> getRoomExpandDirs(GridPlan floorPlan, Loc loc)
        {
            for (int ii = 0; ii < DirExt.DIR4_COUNT; ii++)
            {
                Loc endLoc = loc + ((Dir4)ii).GetLoc();
                if (Collision.InBounds(floorPlan.GridWidth, floorPlan.GridHeight, endLoc)
                    && floorPlan.GetRoomPlan(endLoc) == null)
                    yield return (Dir4)ii;
            }
        }

    }
}
