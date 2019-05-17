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
        private readonly IPermissiveRoomGen roomGen;

        public FloorHallPlan(IPermissiveRoomGen roomGen)
        {
            this.roomGen = roomGen;
            this.Adjacents = new List<RoomHallIndex>();
        }

        public IPermissiveRoomGen RoomGen => this.roomGen;

        IRoomGen IFloorRoomPlan.RoomGen => this.roomGen;

        public List<RoomHallIndex> Adjacents { get; }
    }
}
