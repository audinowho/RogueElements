// <copyright file="GridPathBranch.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class GridPathBranch<T> : GridPathStartStepGeneric<T>
        where T : class, IRoomGridGenContext
    {
        public GridPathBranch()
            : base()
        {
        }

        public RandRange RoomRatio { get; set; }

        public RandRange BranchRatio { get; set; }

        public bool NoForcedBranches { get; set; }

        public static List<LocRay4> GetPossibleExpansions(GridPlan floorPlan, bool branch)
        {
            List<LocRay4> availableRays = new List<LocRay4>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                List<int> adjacents = floorPlan.GetAdjacentRooms(ii);
                if ((adjacents.Count <= 1) != branch)
                {
                    foreach (Dir4 dir in GetRoomExpandDirs(floorPlan, floorPlan.GetRoomPlan(ii).Bounds.Start))
                        availableRays.Add(new LocRay4(floorPlan.GetRoomPlan(ii).Bounds.Start, dir));
                }
            }

            return availableRays;
        }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            for (int ii = 0; ii < 10; ii++)
            {
                // always clear before trying
                floorPlan.Clear();

                int roomsToOpen = floorPlan.GridWidth * floorPlan.GridHeight * this.RoomRatio.Pick(rand) / 100;
                if (roomsToOpen < 1)
                    roomsToOpen = 1;
                int addBranch = this.BranchRatio.Pick(rand);
                int roomsLeft = roomsToOpen;

                Loc sourceRoom = new Loc(rand.Next(floorPlan.GridWidth), rand.Next(floorPlan.GridHeight)); // randomly determine start room
                floorPlan.AddRoom(sourceRoom, this.GenericRooms.Pick(rand), new ComponentCollection(this.RoomComponents));

                GenContextDebug.DebugProgress("Start Room");

                roomsLeft--;
                int pendingBranch = 0;
                while (roomsLeft > 0)
                {
                    bool terminalSuccess = this.ExpandPath(rand, floorPlan, false);
                    bool branchSuccess = false;
                    if (terminalSuccess)
                    {
                        roomsLeft--;
                        if (floorPlan.RoomCount > 2)
                            pendingBranch += addBranch;
                    }
                    else if (this.NoForcedBranches)
                    {
                        break;
                    }
                    else
                    {
                        pendingBranch = 100;
                    }

                    while (pendingBranch >= 100 && roomsLeft > 0)
                    {
                        branchSuccess = this.ExpandPath(rand, floorPlan, true);
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

        public virtual LocRay4 ChooseRoomExpansion(IRandom rand, GridPlan floorPlan, bool branch)
        {
            List<LocRay4> availableRays = GetPossibleExpansions(floorPlan, branch);

            if (availableRays.Count == 0)
                return new LocRay4(Dir4.None);

            LocRay4 chosenRay = availableRays[rand.Next(availableRays.Count)];
            return chosenRay;
        }

        protected bool ExpandPath(IRandom rand, GridPlan floorPlan, bool branch)
        {
            LocRay4 chosenRay = this.ChooseRoomExpansion(rand, floorPlan, branch);
            if (chosenRay.Dir == Dir4.None)
                return false;
            floorPlan.SetHall(chosenRay, this.GenericHalls.Pick(rand), new ComponentCollection(this.HallComponents));
            floorPlan.AddRoom(chosenRay.Traverse(1), this.GenericRooms.Pick(rand), new ComponentCollection(this.RoomComponents));

            GenContextDebug.DebugProgress(branch ? "Branched Path" : "Extended Path");
            return true;
        }

        /// <summary>
        /// Gets the directions a room can expand in.
        /// </summary>
        /// <param name="floorPlan"></param>
        /// <param name="loc"></param>
        /// <returns></returns>
        private static IEnumerable<Dir4> GetRoomExpandDirs(GridPlan floorPlan, Loc loc)
        {
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                Loc endLoc = loc + dir.GetLoc();
                if (Collision.InBounds(floorPlan.GridWidth, floorPlan.GridHeight, endLoc)
                    && floorPlan.GetRoomPlan(endLoc) == null)
                    yield return dir;
            }
        }
    }
}
