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
    /// Takes the current floor plan and adds new rooms that are connected to existing rooms.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class AddConnectedRoomsRandStep<T> : AddConnectedRoomsBaseStep<T>
        where T : class, IFloorPlanGenContext
    {
        public AddConnectedRoomsRandStep()
            : base()
        {
        }

        public AddConnectedRoomsRandStep(IRandPicker<RoomGen<T>> genericRooms, IRandPicker<PermissiveRoomGen<T>> genericHalls)
            : base(genericRooms, genericHalls)
        {
        }

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

            return FloorPathBranch<T>.ChooseRandRoomExpansion(room, hall, rand, floorPlan, availableExpansions);
        }

    }
}
