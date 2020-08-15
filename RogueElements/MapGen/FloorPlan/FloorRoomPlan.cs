// <copyright file="FloorRoomPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Contains data about which cells a room occupies in a FloorPlan.
    /// </summary>
    public class FloorRoomPlan : IFloorRoomPlan
    {
        public FloorRoomPlan(IRoomGen roomGen, ComponentCollection components)
        {
            this.RoomGen = roomGen;
            this.Components = components;
            this.Adjacents = new List<RoomHallIndex>();
        }

        public IRoomGen RoomGen { get; set; }

        // TODO: needs a better class.  Only one RoomComponent subclass allowed per collection.  Also better lookup.
        public ComponentCollection Components { get; }

        public List<RoomHallIndex> Adjacents { get; }
    }
}
