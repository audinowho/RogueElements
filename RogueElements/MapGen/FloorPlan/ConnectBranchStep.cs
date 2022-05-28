// <copyright file="ConnectBranchStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Takes the current floor plan and connects the ends of its branches to other rooms.
    /// A room is considered the end of a branch when it is connected to only one other room.
    /// ie, a dead end.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ConnectBranchStep<T> : ConnectStep<T>
        where T : class, IFloorPlanGenContext
    {
        public ConnectBranchStep()
            : base()
        {
        }

        public ConnectBranchStep(IRandPicker<PermissiveRoomGen<T>> genericHalls)
            : base(genericHalls)
        {
        }

        /// <summary>
        /// The percentage of eligible branches to connect.
        /// </summary>
        public int ConnectPercent { get; set; }

        public override void ApplyToPath(IRandom rand, FloorPlan floorPlan)
        {
            List<List<RoomHallIndex>> candBranchPoints = GetBranchArms(floorPlan);

            // remove the rooms that do not pass filter
            for (int xx = 0; xx < candBranchPoints.Count; xx++)
            {
                for (int yy = candBranchPoints[xx].Count - 1; yy >= 0; yy--)
                {
                    IFloorRoomPlan plan = floorPlan.GetRoomHall(candBranchPoints[xx][yy]);
                    if (!BaseRoomFilter.PassesAllFilters(plan, this.Filters))
                        candBranchPoints[xx].RemoveAt(yy);
                }
            }

            // compute a goal amount of branches to connect
            // this computation ignores the fact that some terminals may be impossible
            var randBin = new RandBinomial(candBranchPoints.Count, this.ConnectPercent);
            int connectionsLeft = randBin.Pick(rand);

            while (candBranchPoints.Count > 0 && connectionsLeft > 0)
            {
                // choose random point to connect from
                int randIndex = rand.Next(candBranchPoints.Count);
                var chosenDestResult = ChooseConnection(rand, floorPlan, candBranchPoints[randIndex]);

                if (chosenDestResult is ListPathTraversalNode chosenDest)
                {
                    // connect
                    PermissiveRoomGen<T> hall = (PermissiveRoomGen<T>)this.GenericHalls.Pick(rand).Copy();
                    hall.PrepareSize(rand, chosenDest.Connector.Size);
                    hall.SetLoc(chosenDest.Connector.Start);
                    floorPlan.AddHall(hall, this.Components.Clone(), chosenDest.From, chosenDest.To);
                    candBranchPoints.RemoveAt(randIndex);
                    connectionsLeft--;
                    GenContextDebug.DebugProgress("Added Connection");

                    // check to see if connection destination was also a candidate,
                    // counting this as a double if so
                    for (int ii = candBranchPoints.Count - 1; ii >= 0; ii--)
                    {
                        for (int jj = 0; jj < candBranchPoints[ii].Count; jj++)
                        {
                            if (candBranchPoints[ii][jj] == chosenDest.To)
                            {
                                candBranchPoints.RemoveAt(ii);
                                connectionsLeft--;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    // remove the list anyway, but don't call it a success
                    candBranchPoints.RemoveAt(randIndex);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}%", this.GetType().Name, this.ConnectPercent);
        }

        private protected static List<List<RoomHallIndex>> GetBranchArms(FloorPlan floorPlan)
        {
            List<ListPathTraversalNode> endBranches = new List<ListPathTraversalNode>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                FloorRoomPlan roomPlan = floorPlan.GetRoomPlan(ii);
                if (roomPlan.Adjacents.Count == 1)
                    endBranches.Add(new ListPathTraversalNode(new RoomHallIndex(-1, false), new RoomHallIndex(ii, false)));
            }

            List<List<RoomHallIndex>> branchArms = new List<List<RoomHallIndex>>();
            for (int nn = 0; nn < endBranches.Count; nn++)
            {
                ListPathTraversalNode chosenBranch = endBranches[nn];
                List<RoomHallIndex> arm = new List<RoomHallIndex>();

                while (true)
                {
                    List<RoomHallIndex> connectors = new List<RoomHallIndex>();
                    List<RoomHallIndex> adjacents = floorPlan.GetRoomHall(chosenBranch.To).Adjacents;
                    foreach (RoomHallIndex dest in adjacents)
                    {
                        if (dest != chosenBranch.From)
                            connectors.Add(dest);
                    }

                    if (connectors.Count == 1)
                    {
                        arm.Add(chosenBranch.To);
                        chosenBranch = new ListPathTraversalNode(chosenBranch.To, connectors[0]);
                    }
                    else if (connectors.Count == 0)
                    {
                        // we've reached the other side of a single line; add
                        arm.Add(chosenBranch.To);

                        // but also find the other pending arm and remove it
                        for (int ii = endBranches.Count - 1; ii > nn; ii--)
                        {
                            ListPathTraversalNode otherBranch = endBranches[ii];
                            if (chosenBranch.To == otherBranch.To)
                                endBranches.RemoveAt(ii);
                        }

                        // end the loop
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                branchArms.Add(arm);
            }

            return branchArms;
        }
    }
}
