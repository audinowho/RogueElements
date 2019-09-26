// <copyright file="FloorPathBranch.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class FloorPathBranch<T> : FloorPathStartStepGeneric<T>
        where T : class, IFloorPlanGenContext
    {
        // maintains a separate grid based on markovs
        // anything in the actual chain can link to the generic rooms/halls as a last resort
        public FloorPathBranch()
            : base()
        {
        }

        public FloorPathBranch(IRandPicker<RoomGen<T>> genericRooms, IRandPicker<PermissiveRoomGen<T>> genericHalls)
            : base(genericRooms, genericHalls)
        {
        }

        public RandRange FillPercent { get; set; }

        public int HallPercent { get; set; }

        public RandRange BranchRatio { get; set; }

        public bool NoForcedBranches { get; set; }

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
                floorPlan.AddRoom(room, false);
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
        /// Chooses a node to expand the path from based on the specified branch setting.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="floorPlan"></param>
        /// <param name="branch">Chooses to branch from a path instead of extending it.</param>
        /// <returns>A set of instructions on how to expand the path.</returns>
        public virtual ListPathBranchExpansion? ChooseRoomExpansion(IRandom rand, FloorPlan floorPlan, bool branch)
        {
            List<RoomHallIndex> availableExpansions = GetPossibleExpansions(floorPlan, branch);

            if (availableExpansions.Count == 0)
                return null;

            for (int ii = 0; ii < 30; ii++)
            {
                // choose the next room to add to
                RoomHallIndex firstExpandFrom = availableExpansions[rand.Next(availableExpansions.Count)];
                RoomHallIndex expandFrom = firstExpandFrom;
                IRoomGen roomFrom = floorPlan.GetRoomHall(firstExpandFrom).RoomGen;

                // choose the next room to add
                // choose room size/fulfillables
                // note: by allowing halls to be picked as extensions, we run the risk of adding dead-end halls
                // halls should always terminate at rooms?
                // this means... doubling up with hall+room?
                bool addHall = rand.Next(100) < this.HallPercent;
                IRoomGen hall = null;
                if (addHall)
                {
                    hall = this.PrepareRoom(rand, floorPlan, true);

                    // randomly choose a perimeter to assign this to
                    SpawnList<Loc> possibleHallPlacements = new SpawnList<Loc>();
                    foreach (Dir4 dir in DirExt.VALID_DIR4)
                        AddLegalPlacements(possibleHallPlacements, floorPlan, expandFrom, roomFrom, hall, dir);

                    // at this point, all possible factors for whether a placement is legal or not is accounted for
                    // therefor just pick one
                    if (possibleHallPlacements.Count == 0)
                        continue;

                    // randomly choose one
                    Loc hallCandLoc = possibleHallPlacements.Pick(rand);

                    // set location
                    hall.SetLoc(hallCandLoc);

                    // change the roomfrom for the upcoming room
                    expandFrom = new RoomHallIndex(-1, false);
                    roomFrom = hall;
                }

                IRoomGen room = this.PrepareRoom(rand, floorPlan, false);

                // randomly choose a perimeter to assign this to
                SpawnList<Loc> possiblePlacements = new SpawnList<Loc>();
                foreach (Dir4 dir in DirExt.VALID_DIR4)
                    AddLegalPlacements(possiblePlacements, floorPlan, expandFrom, roomFrom, room, dir);

                // at this point, all possible factors for whether a placement is legal or not is accounted for
                // therefore just pick one
                if (possiblePlacements.Count > 0)
                {
                    // randomly choose one
                    Loc candLoc = possiblePlacements.Pick(rand);

                    // set location
                    room.SetLoc(candLoc);
                    return new ListPathBranchExpansion(firstExpandFrom, room, (IPermissiveRoomGen)hall);
                }
            }

            return null;
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

        private protected static void AddLegalPlacements(SpawnList<Loc> possiblePlacements, FloorPlan floorPlan, RoomHallIndex indexFrom, IRoomGen roomFrom, IRoomGen room, Dir4 expandTo)
        {
            bool vertical = expandTo.ToAxis() == Axis4.Vert;

            // this scaling factor equalizes the chances of long sides vs short sides
            int reverseSideMult = vertical ? roomFrom.Draw.Width * room.Draw.Width : roomFrom.Draw.Height * room.Draw.Height;

            Range side = roomFrom.Draw.GetSide(expandTo.ToAxis());

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
                Loc trialLoc = roomFrom.GetEdgeRectLoc(expandTo, room.Draw.Size, currentScalar);
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
                    Loc locTo = roomFrom.GetEdgeRectLoc(expandTo, room.Draw.Size, currentScalar);

                    // must be within the borders of the floor!
                    if (floorPlan.DrawRect.Contains(new Rect(locTo, room.Draw.Size)))
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
        /// Gets all possible places a new path node can be added.
        /// </summary>
        /// <param name="floorPlan"></param>
        /// <param name="branch">Chooses to branch from a path instead of extending it.</param>
        /// <returns>All possible RoomHallIndex that can receive an expansion.</returns>
        private protected static List<RoomHallIndex> GetPossibleExpansions(FloorPlan floorPlan, bool branch)
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
                floorPlan.AddHall(expansion.Hall, from);
                from = new RoomHallIndex(floorPlan.HallCount - 1, true);
                tilesCovered += expansion.Hall.Draw.Area;
                roomsAdded++;
            }

            floorPlan.AddRoom(expansion.Room, false, from);
            tilesCovered += expansion.Room.Draw.Area;
            roomsAdded++;
            GenContextDebug.DebugProgress(branch ? "Branched Path" : "Extended Path");

            // report the added area coverage
            return (tilesCovered, roomsAdded);
        }

        public struct ListPathBranchExpansion
        {
            public RoomHallIndex From;
            public IPermissiveRoomGen Hall;
            public IRoomGen Room;

            public ListPathBranchExpansion(RoomHallIndex from, IRoomGen room, IPermissiveRoomGen hall)
            {
                this.From = from;
                this.Room = room;
                this.Hall = hall;
            }
        }
    }
}
