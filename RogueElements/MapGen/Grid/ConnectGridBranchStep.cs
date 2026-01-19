// <copyright file="ConnectGridBranchStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Connects dead-end rooms in the grid plan to create additional paths.
    /// </summary>
    /// <typeparam name="T">The map context type, which must implement <see cref="IRoomGridGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This step identifies rooms that are branch ends (connected to only one other room)
    /// and creates additional hall connections to nearby rooms. This reduces dead ends
    /// and creates more interconnected layouts.
    /// </para>
    /// <para>
    /// The algorithm traces back from each dead end until it finds a room with multiple
    /// possible connections, then randomly selects one to connect.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridPlanStep{T}"/>
    [Serializable]
    public class ConnectGridBranchStep<T> : GridPlanStep<T>
        where T : class, IRoomGridGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectGridBranchStep{T}"/> class.
        /// </summary>
        public ConnectGridBranchStep()
        {
            this.GenericHalls = new SpawnList<PermissiveRoomGen<T>>();
            this.HallComponents = new ComponentCollection();
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectGridBranchStep{T}"/> class
        /// with the specified connection percentage.
        /// </summary>
        /// <param name="connectPercent">The percentage of eligible branches to connect.</param>
        public ConnectGridBranchStep(int connectPercent)
            : this()
        {
            this.ConnectPercent = connectPercent;
        }

        /// <summary>
        /// Gets or sets the percentage of eligible branches to connect.
        /// </summary>
        public int ConnectPercent { get; set; }

        /// <summary>
        /// Gets or sets the filters that determine which rooms are eligible to be connected.
        /// </summary>
        public List<BaseRoomFilter> Filters { get; set; }

        /// <summary>
        /// Gets or sets the hall generators that can be used for connecting halls.
        /// </summary>
        public IRandPicker<PermissiveRoomGen<T>> GenericHalls { get; set; }

        /// <summary>
        /// Gets or sets the components to attach to newly created halls.
        /// </summary>
        public ComponentCollection HallComponents { get; set; }

        /// <summary>
        /// Finds branch ends and connects them to adjacent rooms.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The grid plan to modify.</param>
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
                                int roomIndex = floorPlan.GetRoomIndex(loc);
                                if (roomIndex > -1)
                                {
                                    GridRoomPlan roomPlan = floorPlan.GetRoomPlan(roomIndex);
                                    if (BaseRoomFilter.PassesAllFilters(roomPlan, this.Filters))
                                        candBonds.Add(new LocRay4(chosenBranch.Loc, dir));
                                }
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
            return string.Format("{0}: {1}%", this.GetType().GetFormattedTypeName(), this.ConnectPercent);
        }
    }
}
