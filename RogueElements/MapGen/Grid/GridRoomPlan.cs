// <copyright file="GridRoomPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Contains data about which cells a room occupies in a GridFloorPlan.
    /// </summary>
    [Serializable]
    public class GridRoomPlan : IRoomPlan
    {
        public GridRoomPlan(Rect bounds, IRoomGen roomGen, ComponentCollection components)
        {
            this.Bounds = bounds;
            this.RoomGen = roomGen;
            this.Components = components;
        }

        public Rect Bounds { get; set; }

        public bool PreferHall { get; set; }

        public IRoomGen RoomGen { get; set; }

        // TODO: needs a better class.  Only one RoomComponent subclass allowed per collection.  Also better lookup.
        // This member will be assigned by reference to the Components of FloorRoomPlan
        public ComponentCollection Components { get; set; }
    }
}
