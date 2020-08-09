// <copyright file="GridHallGroup.cs" company="Audino">
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
    public class GridHallGroup
    {
        public GridHallGroup()
        {
            this.HallParts = new List<GridHallPlan>();
        }

        public GridHallPlan MainHall => this.HallParts.Count > 0 ? this.HallParts[0] : null;

        public List<GridHallPlan> HallParts { get; }

        public void SetHall(GridHallPlan plan)
        {
            this.HallParts.Clear();
            if (plan != null)
                this.HallParts.Add(plan);
        }
    }
}
