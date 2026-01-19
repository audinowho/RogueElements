// <copyright file="IGridPathBranch.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Defines the configuration interface for branching path generators.
    /// </summary>
    public interface IGridPathBranch
    {
        /// <summary>
        /// Gets or sets the percentage of grid cells to fill with rooms.
        /// </summary>
        RandRange RoomRatio { get; set; }

        /// <summary>
        /// Gets or sets the ratio of branches to straight path extensions.
        /// </summary>
        RandRange BranchRatio { get; set; }
    }

    /// <summary>
    /// Creates a layout using a tree-like algorithm that grows paths and branches.
    /// </summary>
    /// <typeparam name="T">The map context type, which must implement <see cref="IRoomGridGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This path generator creates layouts by starting from a random room and expanding
    /// outward. The main path wanders randomly, and branches are added based on the
    /// <see cref="BranchRatio"/> setting.
    /// </para>
    /// <para>
    /// The algorithm produces tree-like structures where all rooms are connected but
    /// there are no loops. The shape can range from a simple worm (0% branches) to
    /// a heavily branched tree (high branch ratio).
    /// </para>
    /// </remarks>
    /// <seealso cref="GridPathStartStepGeneric{T}"/>
    /// <seealso cref="IGridPathBranch"/>
    [Serializable]
    public class GridPathBranch<T> : GridPathStartStepGeneric<T>, IGridPathBranch
        where T : class, IRoomGridGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridPathBranch{T}"/> class.
        /// </summary>
        public GridPathBranch()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the percentage of grid cells to fill with rooms.
        /// </summary>
        public RandRange RoomRatio { get; set; }

        /// <summary>
        /// Gets or sets the ratio of branches to straight path extensions.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item><description>0 = No branches, creates a worm-like path</description></item>
        /// <item><description>50 = One branch per two extensions, tree-like</description></item>
        /// <item><description>100 = One branch per extension, heavily branched</description></item>
        /// <item><description>200 = Two branches per extension, very dense</description></item>
        /// </list>
        /// </remarks>
        public RandRange BranchRatio { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to stop early rather than force branches.
        /// </summary>
        /// <remarks>
        /// When true, the algorithm stops if it cannot extend without branching.
        /// When false, branches are forced to meet the room quota.
        /// </remarks>
        public bool NoForcedBranches { get; set; }

        /// <summary>
        /// Gets all possible expansion directions from existing rooms.
        /// </summary>
        /// <param name="floorPlan">The grid plan to analyze.</param>
        /// <param name="branch">If true, only returns rooms suitable for branching; if false, returns terminal rooms.</param>
        /// <returns>A list of location-direction pairs for possible expansions.</returns>
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

        /// <summary>
        /// Creates the branching path layout.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The grid plan to populate.</param>
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
                List<Loc> terminals = new List<Loc>();
                List<Loc> branchables = new List<Loc>();

                // place first room
                Loc sourceRoom = new Loc(rand.Next(floorPlan.GridWidth), rand.Next(floorPlan.GridHeight)); // randomly determine start room
                floorPlan.AddRoom(sourceRoom, this.GenericRooms.Pick(rand), this.RoomComponents.Clone());

                // add the room to a terminals list twice
                terminals.Add(sourceRoom);
                terminals.Add(sourceRoom);

                GenContextDebug.DebugProgress("Start Room");

                roomsLeft--;
                int pendingBranch = 0;
                while (roomsLeft > 0)
                {
                    // pop a random loc from the terminals list
                    Loc newTerminal = this.PopRandomLoc(floorPlan, rand, terminals);

                    // find the directions to extend to
                    SpawnList<LocRay4> availableRays = this.GetExpandDirChances(floorPlan, newTerminal);

                    if (availableRays.Count > 0)
                    {
                        // extend the path a random direction
                        LocRay4 terminalRay = availableRays.Pick(rand);
                        this.ExpandPath(rand, floorPlan, terminalRay);
                        Loc newRoomLoc = terminalRay.Traverse(1);
                        roomsLeft--;

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
                        if (this.NoForcedBranches)
                            break;
                        else
                            pendingBranch = 100;
                    }

                    while (pendingBranch >= 100 && roomsLeft > 0 && branchables.Count > 0)
                    {
                        // pop a random loc from the branchables list
                        Loc newBranch = this.PopRandomLoc(floorPlan, rand, branchables);

                        // find the directions to extend to
                        SpawnList<LocRay4> availableBranchRays = this.GetExpandDirChances(floorPlan, newBranch);

                        if (availableBranchRays.Count > 0)
                        {
                            // extend the path a random direction
                            LocRay4 branchRay = availableBranchRays.Pick(rand);
                            this.ExpandPath(rand, floorPlan, branchRay);
                            Loc newRoomLoc = branchRay.Traverse(1);
                            roomsLeft--;

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

                if (roomsLeft <= 0)
                    break;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: Fill:{1}% Branch:{2}%", this.GetType().GetFormattedTypeName(), this.RoomRatio, this.BranchRatio);
        }

        /// <summary>
        /// Gets the directions a room can expand in.
        /// </summary>
        /// <param name="floorPlan">The grid plan to check.</param>
        /// <param name="loc">The location to check expansion from.</param>
        /// <returns>An enumerable of valid expansion directions.</returns>
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
        /// Removes and returns a random location from the list with equal distribution.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="locs">The list of locations to choose from.</param>
        /// <returns>The randomly selected location.</returns>
        protected static Loc PopRandomLocEqual(IRandom rand, List<Loc> locs)
        {
            int branchIdx = rand.Next(locs.Count);
            Loc newBranch = locs[branchIdx];
            locs.RemoveAt(branchIdx);
            return newBranch;
        }

        /// <summary>
        /// Expands the path by adding a hall and room in the specified direction.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The grid plan to modify.</param>
        /// <param name="chosenRay">The location and direction to expand.</param>
        /// <returns>True if the expansion was successful.</returns>
        protected bool ExpandPath(IRandom rand, GridPlan floorPlan, LocRay4 chosenRay)
        {
            floorPlan.SetHall(chosenRay, this.GenericHalls.Pick(rand), this.HallComponents.Clone());
            floorPlan.AddRoom(chosenRay.Traverse(1), this.GenericRooms.Pick(rand), this.RoomComponents.Clone());

            GenContextDebug.DebugProgress("Added Path");
            return true;
        }

        /// <summary>
        /// Removes and returns a random location from the list.
        /// </summary>
        /// <param name="floorPlan">The grid plan for context.</param>
        /// <param name="rand">The random number generator.</param>
        /// <param name="locs">The list of locations to choose from.</param>
        /// <returns>The randomly selected location.</returns>
        protected virtual Loc PopRandomLoc(GridPlan floorPlan, IRandom rand, List<Loc> locs)
        {
            return PopRandomLocEqual(rand, locs);
        }

        /// <summary>
        /// Gets the possible expansion directions from a location with their relative weights.
        /// </summary>
        /// <param name="floorPlan">The grid plan to check.</param>
        /// <param name="newTerminal">The location to expand from.</param>
        /// <returns>A weighted list of possible expansion directions.</returns>
        protected virtual SpawnList<LocRay4> GetExpandDirChances(GridPlan floorPlan, Loc newTerminal)
        {
            SpawnList<LocRay4> availableRays = new SpawnList<LocRay4>();
            foreach (Dir4 dir in GetRoomExpandDirs(floorPlan, newTerminal))
                availableRays.Add(new LocRay4(newTerminal, dir), 1);
            return availableRays;
        }
    }
}
