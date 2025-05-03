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
    /// Takes the current floor plan and adds new rooms that are connected to existing rooms.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class AddConnectedRoomsStep<TGenContext, TTile> : AddConnectedRoomsBaseStep<TGenContext, TTile>
        where TGenContext : class, IFloorPlanGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public AddConnectedRoomsStep()
            : base()
        {
        }

        public AddConnectedRoomsStep(IRandPicker<RoomGen<TGenContext, TTile>> genericRooms, IRandPicker<PermissiveRoomGen<TGenContext, TTile>> genericHalls)
            : base(genericRooms, genericHalls)
        {
        }

        public override FloorPathBranch<TGenContext, TTile>.ListPathBranchExpansion? ChooseRoomExpansion(IRandom rand, FloorPlan<TTile> floorPlan)
        {
            List<RoomHallIndex> availableExpansions = new List<RoomHallIndex>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                if (!BaseRoomFilter<TTile>.PassesAllFilters(floorPlan.GetRoomPlan(ii), this.Filters))
                    continue;
                availableExpansions.Add(new RoomHallIndex(ii, false));
            }

            for (int ii = 0; ii < floorPlan.HallCount; ii++)
            {
                if (!BaseRoomFilter<TTile>.PassesAllFilters(floorPlan.GetHallPlan(ii), this.Filters))
                    continue;
                availableExpansions.Add(new RoomHallIndex(ii, true));
            }

            bool addHall = rand.Next(100) < this.HallPercent;
            IRoomGen<TTile> room, hall;
            room = this.PrepareRoom(rand, floorPlan, false);
            if (addHall)
                hall = this.PrepareRoom(rand, floorPlan, true);
            else
                hall = null;

            return FloorPathBranch<TGenContext, TTile>.ChooseRoomExpansion(room, hall, rand, floorPlan, availableExpansions);
        }
    }
}
