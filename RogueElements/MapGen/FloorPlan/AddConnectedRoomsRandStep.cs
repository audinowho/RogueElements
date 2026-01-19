// <copyright file="AddConnectedRoomsRandStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace RogueElements
{
    /// <summary>
    /// Adds new rooms connected to existing rooms using random sampling with limited retries.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This step extends the floor layout by adding rooms adjacent to existing rooms or halls.
    /// Unlike <see cref="AddConnectedRoomsStep{T}"/>, this version randomly samples expansion
    /// points with a limited number of retries, providing better performance at the cost of
    /// potentially missing valid placements.
    /// </para>
    /// <para>
    /// Rooms can optionally be connected via an intermediate hallway, controlled by <see cref="AddConnectedRoomsBaseStep{T}.HallPercent"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="AddConnectedRoomsStep{T}"/>
    [Serializable]
    public class AddConnectedRoomsRandStep<T> : AddConnectedRoomsBaseStep<T>
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddConnectedRoomsRandStep{T}"/> class.
        /// </summary>
        public AddConnectedRoomsRandStep()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddConnectedRoomsRandStep{T}"/> class with specified room and hall generators.
        /// </summary>
        /// <param name="genericRooms">The picker for room generators.</param>
        /// <param name="genericHalls">The picker for hall generators.</param>
        public AddConnectedRoomsRandStep(IRandPicker<RoomGen<T>> genericRooms, IRandPicker<PermissiveRoomGen<T>> genericHalls)
            : base(genericRooms, genericHalls)
        {
        }

        /// <summary>
        /// Chooses a room expansion by randomly sampling possible placements.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The floor plan to expand.</param>
        /// <returns>The chosen expansion details, or null if no valid expansion is found within retry limit.</returns>
        public override FloorPathBranch<T>.ListPathBranchExpansion? ChooseRoomExpansion(IRandom rand, FloorPlan floorPlan)
        {
            // TODO: don't go through all rooms, just pick randomly
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

            return FloorPathBranch<T>.ChooseRandRoomExpansion(room, hall, rand, floorPlan, availableExpansions);
        }
    }
}
