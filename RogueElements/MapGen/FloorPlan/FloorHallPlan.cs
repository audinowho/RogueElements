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
        public FloorHallPlan(IPermissiveRoomGen roomGen, ComponentCollection components)
        {
            this.RoomGen = roomGen;
            this.Components = components;
            this.Adjacents = new List<RoomHallIndex>();
        }

        public IPermissiveRoomGen RoomGen { get; set; }

        IRoomGen IRoomPlan.RoomGen => this.RoomGen;

        public ComponentCollection Components { get; }

        public List<RoomHallIndex> Adjacents { get; }
    }
}
