// <copyright file="GridRoomPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Contains data about which cells a room occupies in a GridFloorPlan.
    /// </summary>
    [Serializable]
    public class GridRoomPlan
    {
        public GridRoomPlan(Rect bounds, IRoomGen roomGen)
        {
            this.Bounds = bounds;
            this.RoomGen = roomGen;
        }

        public Rect Bounds { get; set; }

        public bool Immutable { get; set; }

        public bool PreferHall { get; set; }

        public IRoomGen RoomGen { get; set; }

        public bool CountsAsHall()
        {
            if (!this.PreferHall)
                return false;
            return this.RoomGen is IPermissiveRoomGen;
        }
    }
}
