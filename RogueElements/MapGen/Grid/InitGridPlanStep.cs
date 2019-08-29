// <copyright file="InitGridPlanStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class InitGridPlanStep<T> : GenStep<T>
        where T : class, IRoomGridGenContext
    {
        public InitGridPlanStep()
        {
        }

        public InitGridPlanStep(int cellWall)
        {
            this.CellWall = cellWall;
        }

        public int CellWall { get; set; }

        public int CellWidth { get; set; }

        public int CellHeight { get; set; }

        public int CellX { get; set; }

        public int CellY { get; set; }

        public override void Apply(T map)
        {
            // initialize grid
            var floorPlan = new GridPlan();
            floorPlan.InitSize(this.CellX, this.CellY, this.CellWidth, this.CellHeight, this.CellWall);

            map.InitGrid(floorPlan);
        }
    }
}
