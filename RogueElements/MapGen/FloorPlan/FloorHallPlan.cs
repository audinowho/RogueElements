// <copyright file="FloorHallPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public class FloorHallPlan<TTile> : IFloorRoomPlan<TTile>
        where TTile : ITile<TTile>
    {
        public FloorHallPlan(IPermissiveRoomGen<TTile> roomGen, ComponentCollection components)
        {
            this.RoomGen = roomGen;
            this.Components = components;
            this.Adjacents = new List<RoomHallIndex>();
        }

        public IPermissiveRoomGen<TTile> RoomGen { get; set; }

        IRoomGen<TTile> IRoomPlan<TTile>.RoomGen => this.RoomGen;

        public ComponentCollection Components { get; }

        public List<RoomHallIndex> Adjacents { get; }
    }
}
