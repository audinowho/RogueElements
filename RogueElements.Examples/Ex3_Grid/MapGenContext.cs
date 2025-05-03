// <copyright file="MapGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements.Examples.Ex3_Grid
{
    public class MapGenContext : BaseMapGenContext<Map>, IRoomGridGenContext<Tile>
    {
        public MapGenContext()
            : base()
        {
        }

        public FloorPlan<Tile> RoomPlan { get; private set; }

        public GridPlan<Tile> GridPlan { get; private set; }

        public void InitPlan(FloorPlan<Tile> plan)
        {
            this.RoomPlan = plan;
        }

        public void InitGrid(GridPlan<Tile> plan)
        {
            this.GridPlan = plan;
        }
    }
}
