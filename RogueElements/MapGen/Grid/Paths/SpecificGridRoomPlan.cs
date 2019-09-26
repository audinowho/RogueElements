// <copyright file="SpecificGridRoomPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    [Serializable]
    public class SpecificGridRoomPlan<T>
        where T : ITiledGenContext
    {
        public SpecificGridRoomPlan(Rect bounds, RoomGen<T> roomGen)
        {
            this.Bounds = bounds;
            this.RoomGen = roomGen;
        }

        public Rect Bounds { get; set; }

        public bool Immutable { get; set; }

        public bool PreferHall { get; set; }

        public RoomGen<T> RoomGen { get; set; }

        public bool CountsAsHall()
        {
            if (!this.PreferHall)
                return false;
            return this.RoomGen is IPermissiveRoomGen;
        }
    }
}