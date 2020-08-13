// <copyright file="SpecificGridRoomPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

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
            this.Components = new ComponentCollection();
        }

        public Rect Bounds { get; set; }

        public bool PreferHall { get; set; }

        public RoomGen<T> RoomGen { get; set; }

        public ComponentCollection Components { get; set; }
    }
}