// <copyright file="IFloorPathBranch.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Defines the interface for floor path generation with branching layouts.
    /// </summary>
    public interface IFloorPathBranch
    {
        /// <summary>
        /// Gets or sets the target fill percentage for room coverage.
        /// </summary>
        RandRange FillPercent { get; set; }

        /// <summary>
        /// Gets or sets the percentage chance of adding halls between rooms.
        /// </summary>
        int HallPercent { get; set; }

        /// <summary>
        /// Gets or sets the branching ratio for the layout.
        /// </summary>
        RandRange BranchRatio { get; set; }
    }

    /// <summary>
    /// Creates a branching tree layout of rooms connected by halls.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This step generates a floor layout by starting with a single room and repeatedly adding
    /// new rooms adjacent to existing ones. The <see cref="BranchRatio"/> controls how often
    /// the algorithm branches from non-terminal rooms versus extending existing branches.
    /// </para>
    /// <para>
    /// The resulting layout resembles a tree or organic growth pattern, with dead ends that
    /// can optionally be connected using <see cref="ConnectBranchStep{T}"/>.
    /// </para>
    /// </remarks>
    [Serializable]
    public class FloorPathBranch<T> : FloorPathStartStepGeneric<T>, IFloorPathBranch
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloorPathBranch{T}"/> class.
        /// </summary>
        public FloorPathBranch()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FloorPathBranch{T}"/> class with specified generators.
        /// </summary>
        /// <param name="genericRooms">The picker for room generators.</param>
        /// <param name="genericHalls">The picker for hall generators.</param>
        public FloorPathBranch(IRandPicker<RoomGen<T>> genericRooms, IRandPicker<PermissiveRoomGen<T>> genericHalls)
            : base(genericRooms, genericHalls)
        {
        }

        /// <summary>
        /// Delegate for preparing room generators.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The floor plan being built.</param>
        /// <param name="isHall">True if generating a hall; false for a room.</param>
        /// <returns>A room generator configured for the floor plan.</returns>
        public delegate RoomGen<T> RoomPrep(IRandom rand, FloorPlan floorPlan, bool isHall);

        /// <summary>
        /// Gets or sets the target percentage of floor space to fill with rooms.
        /// </summary>
        public RandRange FillPercent { get; set; }

        /// <summary>
        /// Gets or sets the percentage chance of adding an intermediate hall between rooms.
        /// </summary>
        public int HallPercent { get; set; }

        /// <summary>
        /// Gets or sets the branching ratio controlling layout shape.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item><description>0 = Linear layout without branches (worm-like)</description></item>
        /// <item><description>50 = Branches once per two extensions (tree-like)</description></item>
        /// <item><description>100 = Branches once per extension (dense tree)</description></item>
        /// <item><description>200 = Branches twice per extension (fuzzy/organic)</description></item>
        /// </list>
        /// </remarks>
        public RandRange BranchRatio { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to prevent forced branching when fill quota cannot be met.
        /// </summary>
        public bool NoForcedBranches { get; set; }

        /// <summary>
        /// Gets all possible places a new path node can be added.
        /// </summary>
        /// <param name="floorPlan"></param>
        /// <param name="branch">Chooses to branch from a path instead of extending it.</param>
        /// <returns>All possible RoomHallIndex that can receive an expansion.</returns>
        public static List<RoomHallIndex> GetPossibleExpansions(FloorPlan floorPlan, bool branch)
        {
            List<RoomHallIndex> availableExpansions = new List<RoomHallIndex>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                var listHall = new RoomHallIndex(ii, false);
                List<RoomHallIndex> adjacents = floorPlan.GetRoomHall(listHall).Adjacents;
                if ((adjacents.Count <= 1) != branch)
                    availableExpansions.Add(listHall);
            }

            for (int ii = 0; ii < floorPlan.HallCount; ii++)
            {
                var listHall = new RoomHallIndex(ii, true);
                List<RoomHallIndex> adjacents = floorPlan.GetRoomHall(listHall).Adjacents;
                if ((adjacents.Count <= 1) != branch)
                    availableExpansions.Add(listHall);
            }

            return availableExpansions;
        }

        public static void AddLegalPlacements(SpawnList<Loc> possiblePlacements, FloorPlan floorPlan, RoomHallIndex indexFrom, IRoomGen roomFrom, IRoomGen room, Dir4 expandTo)
        {
            bool vertical = expandTo.ToAxis() == Axis4.Vert;

            // this scaling factor equalizes the chances of long sides vs short sides
            int reverseSideMult = vertical ? roomFrom.Draw.Width * room.Draw.Width : roomFrom.Draw.Height * room.Draw.Height;

            IntRange side = roomFrom.Draw.GetSide(expandTo.ToAxis());

            // subtract the room's original size, not the inflated trialrect size
            side.Min -= (vertical ? room.Draw.Size.X : room.Draw.Size.Y) - 1;

            Rect tryRect = room.Draw;

            // expand in every direction
            // this will create a one-tile buffer to check for collisions
            tryRect.Inflate(1, 1);
            int currentScalar = side.Min;
            while (currentScalar < side.Max)
            {
                // compute the location
                Loc trialLoc = roomFrom.Draw.GetEdgeRectLoc(expandTo, room.Draw.Size, currentScalar);
                tryRect.Start = trialLoc + new Loc(-1, -1);

                // check for collisions (not counting the rectangle from)
                List<RoomHallIndex> collisions = floorPlan.CheckCollision(tryRect);

                // find the first tile in which no collisions will be found
                int maxCollideScalar = currentScalar;
                bool collided = false;
                foreach (RoomHallIndex collision in collisions)
                {
                    if (collision != indexFrom)
                    {
                        IRoomGen collideRoom = floorPlan.GetRoomHall(collision).RoomGen;

                        // this is the point at which the new room will barely touch the collided room
                        // the +1 at the end will move it into the safe zone
                        maxCollideScalar = Math.Max(maxCollideScalar, vertical ? collideRoom.Draw.Right : collideRoom.Draw.Bottom);
                        collided = true;
                    }
                }

                // if no collisions were hit, do final checks and add the room
                if (!collided)
                {
                    Loc locTo = roomFrom.Draw.GetEdgeRectLoc(expandTo, room.Draw.Size, currentScalar);

                    // must be within the borders of the floor!
                    if (floorPlan.Wrap || floorPlan.DrawRect.Contains(new Rect(locTo, room.Draw.Size)))
                    {
                        // check the border match and if add to possible placements
                        int chanceTo = FloorPlan.GetBorderMatch(roomFrom, room, locTo, expandTo);
                        if (chanceTo > 0)
                            possiblePlacements.Add(locTo, chanceTo * reverseSideMult);
                    }
                }

                currentScalar = maxCollideScalar + 1;
            }
        }

        /// <summary>
        /// Chooses a node to expand the path from based on the specified branch setting.
        /// Attempts 30 times.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="hall"></param>
        /// <param name="rand"></param>
        /// <param name="floorPlan"></param>
        /// <param name="availableExpansions"></param>
        /// <returns>A set of instructions on how to expand the path.</returns>
        public static ListPathBranchExpansion? ChooseRandRoomExpansion(IRoomGen room, IRoomGen hall, IRandom rand, FloorPlan floorPlan, List<RoomHallIndex> availableExpansions)
        {
            if (availableExpansions.Count == 0)
                return null;

            for (int ii = 0; ii < 30; ii++)
            {
                // choose the next room to add to
                RoomHallIndex firstExpandFrom = availableExpansions[rand.Next(availableExpansions.Count)];
                RoomHallIndex expandFrom = firstExpandFrom;
                IRoomGen roomFrom = floorPlan.GetRoomHall(firstExpandFrom).RoomGen;

                // if a hall is specified, make it the connector
                if (hall != null)
                {
                    if (!ChooseRoomExpansionFromRoom(hall, rand, floorPlan, expandFrom, roomFrom))
                        continue;

                    // change the roomfrom for the upcoming room
                    expandFrom = new RoomHallIndex(-1, false);
                    roomFrom = hall;
                }

                if (ChooseRoomExpansionFromRoom(room, rand, floorPlan, expandFrom, roomFrom))
                    return new ListPathBranchExpansion(firstExpandFrom, room, (IPermissiveRoomGen)hall);
            }

            return null;
        }

        /// <summary>
        /// Chooses a node to expand the path from based on the specified branch setting.
        /// Tries all possible expansions.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="hall"></param>
        /// <param name="rand"></param>
        /// <param name="floorPlan"></param>
        /// <param name="availableExpansions"></param>
        /// <returns></returns>
        public static ListPathBranchExpansion? ChooseRoomExpansion(IRoomGen room, IRoomGen hall, IRandom rand, FloorPlan floorPlan, List<RoomHallIndex> availableExpansions)
        {
            List<RoomHallIndex> expansions = new List<RoomHallIndex>();
            expansions.AddRange(availableExpansions);
            while (expansions.Count > 0)
            {
                int expandIdx = rand.Next(expansions.Count);

                // choose the next room to add to
                RoomHallIndex firstExpandFrom = availableExpansions[expandIdx];
                RoomHallIndex expandFrom = firstExpandFrom;
                IRoomGen roomFrom = floorPlan.GetRoomHall(firstExpandFrom).RoomGen;

                // if a hall is specified, make it the connector
                if (hall != null)
                {
                    // randomly choose a perimeter to assign this to
                    SpawnList<Loc> possibleHallPlacements = new SpawnList<Loc>();
                    foreach (Dir4 dir in DirExt.VALID_DIR4)
                        AddLegalPlacements(possibleHallPlacements, floorPlan, expandFrom, roomFrom, hall, dir);

                    // at this point, all possible factors for whether a placement is legal or not is accounted for
                    // therefor just pick one
                    while (possibleHallPlacements.Count > 0)
                    {
                        // randomly choose one
                        int candIndex = possibleHallPlacements.PickIndex(rand);
                        Loc hallCandLoc = possibleHallPlacements.GetSpawn(candIndex);

                        // set location
                        hall.SetLoc(hallCandLoc);

                        // change the roomfrom for the upcoming room
                        expandFrom = new RoomHallIndex(-1, false);
                        roomFrom = hall;

                        if (ChooseRoomExpansionFromRoom(room, rand, floorPlan, expandFrom, roomFrom))
                            return new ListPathBranchExpansion(firstExpandFrom, room, (IPermissiveRoomGen)hall);

                        possibleHallPlacements.RemoveAt(candIndex);
                    }
                }
                else
                {
                    if (ChooseRoomExpansionFromRoom(room, rand, floorPlan, expandFrom, roomFrom))
                        return new ListPathBranchExpansion(firstExpandFrom, room, (IPermissiveRoomGen)hall);
                }

                expansions.RemoveAt(expandIdx);
            }

            return null;
        }

        public static bool ChooseRoomExpansionFromRoom(IRoomGen room, IRandom rand, FloorPlan floorPlan, RoomHallIndex expandFrom, IRoomGen roomFrom)
        {
            // randomly choose a perimeter to assign this to
            SpawnList<Loc> possiblePlacements = new SpawnList<Loc>();
            foreach (Dir4 dir in DirExt.VALID_DIR4)
                AddLegalPlacements(possiblePlacements, floorPlan, expandFrom, roomFrom, room, dir);

            // at this point, all possible factors for whether a placement is legal or not is accounted for
            // therefore just pick one
            if (possiblePlacements.Count == 0)
                return false;

            // randomly choose one
            Loc candLoc = possiblePlacements.Pick(rand);

            // set location
            room.SetLoc(candLoc);
            return true;
        }

        public override void ApplyToPath(IRandom rand, FloorPlan floorPlan)
        {
            for (int ii = 0; ii < 10; ii++)
            {
                // always clear before trying
                floorPlan.Clear();

                int tilesToOpen = floorPlan.DrawRect.Area * this.FillPercent.Pick(rand) / 100;
                if (tilesToOpen < 1)
                    tilesToOpen = 1;
                int addBranch = this.BranchRatio.Pick(rand);
                int tilesLeft = tilesToOpen;

                // choose a room
                IRoomGen room = this.PrepareRoom(rand, floorPlan, false);

                // place in a random location
                room.SetLoc(new Loc(
                    rand.Next(floorPlan.DrawRect.Left, floorPlan.DrawRect.Right - room.Draw.Width + 1),
                    rand.Next(floorPlan.DrawRect.Top, floorPlan.DrawRect.Bottom - room.Draw.Height + 1)));
                floorPlan.AddRoom(room, this.RoomComponents.Clone());
                GenContextDebug.DebugProgress("Start Room");

                tilesLeft -= room.Draw.Area;

                // repeat this process until the requisite room amount is met
                int pendingBranch = 0;
                while (tilesLeft > 0)
                {
                    (int area, int rooms) terminalResult = this.ExpandPath(rand, floorPlan, false);
                    (int area, int rooms) branchResult = (0, 0);
                    if (terminalResult.area > 0)
                    {
                        tilesLeft -= terminalResult.area;

                        // add branch PER ROOM when we add over the min threshold
                        for (int jj = 0; jj < terminalResult.rooms; jj++)
                        {
                            if (floorPlan.RoomCount + floorPlan.HallCount - terminalResult.rooms + jj + 1 > 2)
                                pendingBranch += addBranch;
                        }
                    }
                    else if (this.NoForcedBranches)
                    {
                        break;
                    }
                    else
                    {
                        pendingBranch = 100;
                    }

                    while (pendingBranch >= 100 && tilesLeft > 0)
                    {
                        branchResult = this.ExpandPath(rand, floorPlan, true);
                        if (branchResult.area == 0)
                            break;
                        pendingBranch -= 100;

                        // if we add any more than one room, that also counts as a branchable node
                        pendingBranch += (branchResult.rooms - 1) * addBranch;
                        tilesLeft -= branchResult.area;
                    }

                    if (terminalResult.area == 0 && branchResult.area == 0)
                        break;
                }

                if (tilesLeft <= 0)
                    break;
            }
        }

        /// <summary>
        /// Returns a random generic room or hall that can fit in the specified floor.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="floorPlan"></param>
        /// <param name="isHall"></param>
        /// <returns></returns>
        public virtual RoomGen<T> PrepareRoom(IRandom rand, FloorPlan floorPlan, bool isHall)
        {
            RoomGen<T> room;
            if (!isHall) // choose a room
                room = this.GenericRooms.Pick(rand).Copy();
            else // chose a hall
                room = this.GenericHalls.Pick(rand).Copy();

            // decide on acceptable border/size/fulfillables
            Loc size = room.ProposeSize(rand);
            if (size.X > floorPlan.DrawRect.Width)
                size.X = floorPlan.DrawRect.Width;
            if (size.Y > floorPlan.DrawRect.Height)
                size.Y = floorPlan.DrawRect.Height;
            room.PrepareSize(rand, size);
            return room;
        }

        public virtual ListPathBranchExpansion? ChooseRoomExpansion(IRandom rand, FloorPlan floorPlan, bool branch)
        {
            List<RoomHallIndex> possibles = GetPossibleExpansions(floorPlan, branch);
            bool addHall = rand.Next(100) < this.HallPercent;
            IRoomGen room, hall;
            room = this.PrepareRoom(rand, floorPlan, false);
            if (addHall)
                hall = this.PrepareRoom(rand, floorPlan, true);
            else
                hall = null;
            return ChooseRandRoomExpansion(room, hall, rand, floorPlan, possibles);
        }

        public override string ToString()
        {
            return string.Format("{0}: Fill:{1}% Hall:{2}% Branch:{3}%", this.GetType().GetFormattedTypeName(), this.FillPercent, this.HallPercent, this.BranchRatio);
        }

        private (int area, int rooms) ExpandPath(IRandom rand, FloorPlan floorPlan, bool branch)
        {
            ListPathBranchExpansion? expansionResult = this.ChooseRoomExpansion(rand, floorPlan, branch);

            if (!expansionResult.HasValue)
                return (0, 0);

            var expansion = expansionResult.Value;

            int tilesCovered = 0;
            int roomsAdded = 0;

            RoomHallIndex from = expansion.From;
            if (expansion.Hall != null)
            {
                floorPlan.AddHall(expansion.Hall, this.HallComponents.Clone(), from);
                from = new RoomHallIndex(floorPlan.HallCount - 1, true);
                tilesCovered += expansion.Hall.Draw.Area;
                roomsAdded++;
            }

            floorPlan.AddRoom(expansion.Room, this.RoomComponents.Clone(), from);
            tilesCovered += expansion.Room.Draw.Area;
            roomsAdded++;
            GenContextDebug.DebugProgress(branch ? "Branched Path" : "Extended Path");

            // report the added area coverage
            return (tilesCovered, roomsAdded);
        }

        /// <summary>
        /// Represents an expansion operation for a branching floor path, containing the source room/hall,
        /// the new room to add, and an optional connecting hall.
        /// </summary>
        public struct ListPathBranchExpansion
        {
            /// <summary>
            /// The index of the room or hall to expand from.
            /// </summary>
            public RoomHallIndex From;

            /// <summary>
            /// The optional intermediate hall connecting the source to the new room. May be null.
            /// </summary>
            public IPermissiveRoomGen Hall;

            /// <summary>
            /// The new room to add to the floor plan.
            /// </summary>
            public IRoomGen Room;

            /// <summary>
            /// Initializes a new instance of the <see cref="ListPathBranchExpansion"/> struct.
            /// </summary>
            /// <param name="from">The index of the room or hall to expand from.</param>
            /// <param name="room">The new room to add.</param>
            /// <param name="hall">The optional intermediate hall, or null for direct connection.</param>
            public ListPathBranchExpansion(RoomHallIndex from, IRoomGen room, IPermissiveRoomGen hall)
            {
                this.From = from;
                this.Room = room;
                this.Hall = hall;
            }
        }
    }
}
