// <copyright file="InitGridPlanStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Initializes an empty grid plan, which is a grid of rooms and connecting hallways.
    /// Unlike a floor plan, the rooms in a grid plan are rigidly defined to be cells in a grid, instead of being placed in freestyle.
    /// Gen Steps that operate on the grid plan can add, erase, or change rooms/hallways.
    /// Once finished, apply DrawGridToFloorStep to take the grid and create a floor plan.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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

        /// <summary>
        /// The width of all cells in the grid.
        /// </summary>
        public int CellWidth { get; set; }

        /// <summary>
        /// The height of all cells in the grid.
        /// </summary>
        public int CellHeight { get; set; }

        /// <summary>
        /// The number of columns in the grid.
        /// </summary>
        public int CellX { get; set; }

        /// <summary>
        /// The number of rows in the grid.
        /// </summary>
        public int CellY { get; set; }

        /// <summary>
        /// The thickness of the dividers between cells in the grid, in tiles.
        /// </summary>
        public int CellWall { get; set; }

        /// <summary>
        /// Determines if the map is wrapped around.
        /// </summary>
        public bool Wrap { get; set; }

        public override void Apply(T map)
        {
            // initialize grid
            var floorPlan = new GridPlan();
            floorPlan.InitSize(this.CellX, this.CellY, this.CellWidth, this.CellHeight, this.CellWall, this.Wrap);

            map.InitGrid(floorPlan);
        }

        public override string ToString()
        {
            return string.Format("{0}: Cells:{1}x{2} CellSize:{3}x{4}", this.GetType().Name, this.CellX, this.CellY, this.CellWidth, this.CellHeight);
        }
    }
}
