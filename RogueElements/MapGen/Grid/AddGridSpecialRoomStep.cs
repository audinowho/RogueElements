// <copyright file="AddGridSpecialRoomStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class AddGridSpecialRoomStep<T> : GridPlanStep<T> where T : class, IRoomGridGenContext
    {
        public IRandPicker<RoomGen<T>> Rooms;

        public AddGridSpecialRoomStep()
            : base()
        { }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            //choose certain rooms in the list to be special rooms
            //special rooms are required; so make sure they don't overlap
            List<int> room_indices = new List<int>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                GridRoomPlan plan = floorPlan.GetRoomPlan(ii);
                if (!plan.Immutable && !plan.CountsAsHall())
                    room_indices.Add(ii);
            }
            if (room_indices.Count > 0)
            {
                int ind = rand.Next(room_indices.Count);
                GridRoomPlan plan = floorPlan.GetRoomPlan(room_indices[ind]);
                plan.RoomGen = Rooms.Pick(rand).Copy();
                plan.Immutable = true;
                room_indices.RemoveAt(ind);
                GenContextDebug.DebugProgress("Set Special Room");
            }
        }

        
    }
}
