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
    public class MapGenContext : BaseMapGenContext<Map>, IRoomGridGenContext
    {
        public MapGenContext()
            : base()
        {
        }

        public FloorPlan RoomPlan { get; private set; }

        public GridPlan GridPlan { get; private set; }

        public void InitPlan(FloorPlan plan)
        {
            this.RoomPlan = plan;
        }

        public void InitGrid(GridPlan plan)
        {
            this.GridPlan = plan;
        }
    }
}
