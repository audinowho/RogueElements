// <copyright file="FloorHallPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public class FloorHallPlan : IFloorRoomPlan
    {
        public FloorHallPlan(IPermissiveRoomGen roomGen)
        {
            this.RoomGen = roomGen;
            this.Adjacents = new List<RoomHallIndex>();
        }

        public IPermissiveRoomGen RoomGen { get; set; }

        IRoomGen IFloorRoomPlan.RoomGen => this.RoomGen;

        public List<RoomHallIndex> Adjacents { get; }
    }
}
