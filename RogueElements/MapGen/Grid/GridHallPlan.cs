// <copyright file="GridHallPlan.cs" company="Audino">
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
    public class GridHallPlan : IRoomPlan
    {
        public GridHallPlan(IPermissiveRoomGen roomGen, ComponentCollection components)
        {
            this.RoomGen = roomGen;
            this.Components = components;
        }

        public IPermissiveRoomGen RoomGen { get; }

        IRoomGen IRoomPlan.RoomGen => this.RoomGen;

        // This member will be assigned by reference to the Components of FloorHallPlan,
        // as well as to the components of any halls it is split into during bounds calculation
        public ComponentCollection Components { get; }
    }
}
