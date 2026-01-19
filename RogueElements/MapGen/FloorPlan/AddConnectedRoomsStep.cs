// <copyright file="AddConnectedRoomsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    /// <summary>
    /// Adds new rooms connected to existing rooms by exhaustively evaluating all possible placements.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This step extends the floor layout by adding rooms adjacent to existing rooms or halls.
    /// Unlike <see cref="AddConnectedRoomsRandStep{T}"/>, this version evaluates all possible
    /// expansion points before selecting one, guaranteeing placement if any valid location exists.
    /// </para>
    /// <para>
    /// Rooms can optionally be connected via an intermediate hallway, controlled by <see cref="AddConnectedRoomsBaseStep{T}.HallPercent"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="AddConnectedRoomsRandStep{T}"/>
    [Serializable]
    public class AddConnectedRoomsStep<T> : AddConnectedRoomsBaseStep<T>
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddConnectedRoomsStep{T}"/> class.
        /// </summary>
        public AddConnectedRoomsStep()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddConnectedRoomsStep{T}"/> class with specified room and hall generators.
        /// </summary>
        /// <param name="genericRooms">The picker for room generators.</param>
        /// <param name="genericHalls">The picker for hall generators.</param>
        public AddConnectedRoomsStep(IRandPicker<RoomGen<T>> genericRooms, IRandPicker<PermissiveRoomGen<T>> genericHalls)
            : base(genericRooms, genericHalls)
        {
        }

        /// <summary>
        /// Chooses a room expansion by evaluating all possible placements.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The floor plan to expand.</param>
        /// <returns>The chosen expansion details, or null if no valid expansion exists.</returns>
        public override FloorPathBranch<T>.ListPathBranchExpansion? ChooseRoomExpansion(IRandom rand, FloorPlan floorPlan)
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

            bool addHall = rand.Next(100) < this.HallPercent;
            IRoomGen room, hall;
            room = this.PrepareRoom(rand, floorPlan, false);
            if (addHall)
                hall = this.PrepareRoom(rand, floorPlan, true);
            else
                hall = null;

            return FloorPathBranch<T>.ChooseRoomExpansion(room, hall, rand, floorPlan, availableExpansions);
        }
    }
}
