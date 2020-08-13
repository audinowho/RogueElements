// <copyright file="AddGridSpecialRoomStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class AddGridSpecialRoomStep<T> : GridPlanStep<T>
        where T : class, IRoomGridGenContext
    {
        public AddGridSpecialRoomStep()
            : base()
        {
            this.Filters = new List<BaseRoomFilter>();
            this.RoomComponents = new ComponentCollection();
        }

        public IRandPicker<RoomGen<T>> Rooms { get; set; }

        public ComponentCollection RoomComponents { get; set; }

        public List<BaseRoomFilter> Filters { get; set; }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            // choose certain rooms in the list to be special rooms
            // special rooms are required; so make sure they don't overlap
            List<int> room_indices = new List<int>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                GridRoomPlan plan = floorPlan.GetRoomPlan(ii);
                if (!BaseRoomFilter.PassesAllFilters(plan, this.Filters))
                    continue;
                if (!plan.PreferHall)
                    room_indices.Add(ii);
            }

            if (room_indices.Count > 0)
            {
                int ind = rand.Next(room_indices.Count);
                GridRoomPlan plan = floorPlan.GetRoomPlan(room_indices[ind]);
                plan.RoomGen = this.Rooms.Pick(rand).Copy();
                plan.Components = this.RoomComponents.Clone();
                room_indices.RemoveAt(ind);
                GenContextDebug.DebugProgress("Set Special Room");
            }
        }
    }
}
