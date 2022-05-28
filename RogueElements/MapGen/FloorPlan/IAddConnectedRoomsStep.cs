// <copyright file="IAddConnectedRoomsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    public interface IAddConnectedRoomsStep
    {
        int HallPercent { get; set; }

        RandRange Amount { get; set; }
    }

    /// <summary>
    /// Takes the current floor plan and adds new rooms that are connected to existing rooms.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class AddConnectedRoomsStep<T> : FloorPlanStep<T>, IAddConnectedRoomsStep
        where T : class, IFloorPlanGenContext
    {
        public AddConnectedRoomsStep()
            : base()
        {
            this.RoomComponents = new ComponentCollection();
            this.HallComponents = new ComponentCollection();
            this.Filters = new List<BaseRoomFilter>();
        }

        public AddConnectedRoomsStep(IRandPicker<RoomGen<T>> genericRooms, IRandPicker<PermissiveRoomGen<T>> genericHalls)
            : base()
        {
            this.GenericRooms = genericRooms;
            this.GenericHalls = genericHalls;
            this.RoomComponents = new ComponentCollection();
            this.HallComponents = new ComponentCollection();
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// The number of rooms to add.
        /// </summary>
        public RandRange Amount { get; set; }

        /// <summary>
        /// The chance that an added room is attached using an intermediate hallway.
        /// </summary>
        public int HallPercent { get; set; }

        /// <summary>
        /// Determines which rooms are eligible to have the new rooms added on.
        /// </summary>
        public List<BaseRoomFilter> Filters { get; set; }

        /// <summary>
        /// The room types that can be used for the room being added.
        /// </summary>
        public IRandPicker<RoomGen<T>> GenericRooms { get; set; }

        /// <summary>
        /// Components that the newly added rooms will be labeled with.
        /// </summary>
        public ComponentCollection RoomComponents { get; set; }

        /// <summary>
        /// The room types that can be used as the intermediate hall.
        /// </summary>
        public IRandPicker<PermissiveRoomGen<T>> GenericHalls { get; set; }

        /// <summary>
        /// Components that the newly added halls will be labeled with.
        /// </summary>
        public ComponentCollection HallComponents { get; set; }

        public override void ApplyToPath(IRandom rand, FloorPlan floorPlan)
        {
            int amount = this.Amount.Pick(rand);

            for (int kk = 0; kk < amount; kk++)
            {
                FloorPathBranch<T>.ListPathBranchExpansion? expansionResult = this.ChooseRoomExpansion(rand, floorPlan);

                if (!expansionResult.HasValue)
                    continue;

                var expansion = expansionResult.Value;

                RoomHallIndex from = expansion.From;
                if (expansion.Hall != null)
                {
                    floorPlan.AddHall(expansion.Hall, this.HallComponents.Clone(), from);
                    from = new RoomHallIndex(floorPlan.HallCount - 1, true);
                }

                floorPlan.AddRoom(expansion.Room, this.RoomComponents.Clone(), from);

                GenContextDebug.DebugProgress("Extended with Room");
            }
        }

        public virtual FloorPathBranch<T>.ListPathBranchExpansion? ChooseRoomExpansion(IRandom rand, FloorPlan floorPlan)
        {
            List<RoomHallIndex> availableExpansions = new List<RoomHallIndex>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                if (!BaseRoomFilter.PassesAllFilters(floorPlan.GetRoomPlan(ii), this.Filters))
                    continue;
                availableExpansions.Add(new RoomHallIndex(ii, false));
            }

            for (int ii = 0; ii < floorPlan.HallCount; ii++)
            {
                if (!BaseRoomFilter.PassesAllFilters(floorPlan.GetHallPlan(ii), this.Filters))
                    continue;
                availableExpansions.Add(new RoomHallIndex(ii, true));
            }

            return FloorPathBranch<T>.ChooseRoomExpansion(this.PrepareRoom, this.HallPercent, rand, floorPlan, availableExpansions);
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

        public override string ToString()
        {
            return string.Format("{0}: Add:{1} Hall:{2}%", this.GetType().Name, this.Amount, this.HallPercent);
        }
    }
}
