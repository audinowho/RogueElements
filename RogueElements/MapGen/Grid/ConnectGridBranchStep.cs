// <copyright file="ConnectGridBranchStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Takes the current grid plan and connects the ends of its branches to other rooms.
    /// A room is considered the end of a branch when it is connected to only one other room.
    /// ie, a dead end.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ConnectGridBranchStep<T> : GridPlanStep<T>
        where T : class, IRoomGridGenContext
    {
        public ConnectGridBranchStep()
        {
            this.GenericHalls = new SpawnList<PermissiveRoomGen<T>>();
            this.HallComponents = new ComponentCollection();
            this.Filters = new List<BaseRoomFilter>();
        }

        public ConnectGridBranchStep(int connectPercent)
            : this()
        {
            this.ConnectPercent = connectPercent;
        }

        /// <summary>
        /// The percentage of eligible branches to connect.
        /// </summary>
        public int ConnectPercent { get; set; }

        /// <summary>
        /// Determines which rooms are eligible to be connected.
        /// </summary>
        public List<BaseRoomFilter> Filters { get; set; }

        /// <summary>
        /// The room types that can be used as the hall connecting the two base rooms.
        /// </summary>
        public IRandPicker<PermissiveRoomGen<T>> GenericHalls { get; set; }

        /// <summary>
        /// Components that the newly added halls will be labeled with.
        /// </summary>
        public ComponentCollection HallComponents { get; set; }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            List<LocRay4> endBranches = new List<LocRay4>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                GridRoomPlan roomPlan = floorPlan.GetRoomPlan(ii);

                if (!BaseRoomFilter.PassesAllFilters(roomPlan, this.Filters))
                    continue;

                if (roomPlan.Bounds.Size == new Loc(1))
                {
                    List<int> adjacents = floorPlan.GetAdjacentRooms(ii);
                    if (adjacents.Count == 1)
                        endBranches.Add(new LocRay4(roomPlan.Bounds.Start));
                }
            }

            List<List<LocRay4>> candBranchPoints = new List<List<LocRay4>>();
            for (int nn = 0; nn < endBranches.Count; nn++)
            {
                LocRay4 chosenBranch = endBranches[nn];

                while (chosenBranch.Loc != new Loc(-1))
                {
                    List<LocRay4> connectors = new List<LocRay4>();
                    List<LocRay4> candBonds = new List<LocRay4>();
                    foreach (Dir4 dir in DirExt.VALID_DIR4)
                    {
                        if (dir != chosenBranch.Dir)
                        {
                            if (floorPlan.GetHall(new LocRay4(chosenBranch.Loc, dir)) != null)
                            {
                                connectors.Add(new LocRay4(chosenBranch.Loc, dir));
                            }
                            else
                            {
                                Loc loc = chosenBranch.Loc + dir.GetLoc();
                                if (floorPlan.GetRoomIndex(loc) > -1)
                                    candBonds.Add(new LocRay4(chosenBranch.Loc, dir));
                            }
                        }
                    }

                    if (connectors.Count == 1)
                    {
                        if (candBonds.Count > 0)
                        {
                            candBranchPoints.Add(candBonds);
                            chosenBranch = new LocRay4(new Loc(-1));
                        }
                        else
                        {
                            chosenBranch = new LocRay4(connectors[0].Traverse(1), connectors[0].Dir.Reverse());
                        }
                    }
                    else
                    {
                        chosenBranch = new LocRay4(new Loc(-1));
                    }
                }
            }

            // compute a goal amount of terminals to connect
            // this computation ignores the fact that some terminals may be impossible
            var randBin = new RandBinomial(candBranchPoints.Count, this.ConnectPercent);
            int connectionsLeft = randBin.Pick(rand);

            while (candBranchPoints.Count > 0 && connectionsLeft > 0)
            {
                // choose random point to connect
                int randIndex = rand.Next(candBranchPoints.Count);
                List<LocRay4> candBonds = candBranchPoints[randIndex];
                LocRay4 chosenDir = candBonds[rand.Next(candBonds.Count)];

                // connect
                floorPlan.SetHall(chosenDir, this.GenericHalls.Pick(rand), this.HallComponents);
                candBranchPoints.RemoveAt(randIndex);
                GenContextDebug.DebugProgress("Connected Branch");
                connectionsLeft--;

                // check to see if connection destination was also a candidate,
                // counting this as a double if so
                for (int ii = candBranchPoints.Count - 1; ii >= 0; ii--)
                {
                    if (candBranchPoints[ii][0].Loc == chosenDir.Traverse(1))
                    {
                        candBranchPoints.RemoveAt(ii);
                        connectionsLeft--;
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}%", this.GetType().Name, this.ConnectPercent);
        }
    }
}
