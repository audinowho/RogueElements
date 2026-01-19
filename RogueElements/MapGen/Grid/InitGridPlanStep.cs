// <copyright file="InitGridPlanStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Initializes an empty grid plan with the specified dimensions and cell properties.
    /// </summary>
    /// <typeparam name="T">The map context type, which must implement <see cref="IRoomGridGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This step creates a new <see cref="GridPlan"/> with empty cells. Unlike a floor plan where
    /// rooms can be placed freely, a grid plan uses a rigid cell-based structure where rooms
    /// occupy one or more cells and halls connect adjacent cells.
    /// </para>
    /// <para>
    /// After initialization, use <see cref="GridPlanStep{T}"/> subclasses to populate the grid
    /// with rooms and halls. Once the grid is complete, use <see cref="DrawGridToFloorStep{T}"/>
    /// to convert it to a floor plan for tile-level generation.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridPlan"/>
    /// <seealso cref="DrawGridToFloorStep{T}"/>
    [Serializable]
    public class InitGridPlanStep<T> : GenStep<T>
        where T : class, IRoomGridGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitGridPlanStep{T}"/> class.
        /// </summary>
        public InitGridPlanStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitGridPlanStep{T}"/> class with the specified wall thickness.
        /// </summary>
        /// <param name="cellWall">The thickness of dividers between cells, in tiles.</param>
        public InitGridPlanStep(int cellWall)
        {
            this.CellWall = cellWall;
        }

        /// <summary>
        /// Gets or sets the width of all cells in the grid, in tiles.
        /// </summary>
        public int CellWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of all cells in the grid, in tiles.
        /// </summary>
        public int CellHeight { get; set; }

        /// <summary>
        /// Gets or sets the number of columns in the grid.
        /// </summary>
        public int CellX { get; set; }

        /// <summary>
        /// Gets or sets the number of rows in the grid.
        /// </summary>
        public int CellY { get; set; }

        /// <summary>
        /// Gets or sets the thickness of the dividers between cells in the grid, in tiles.
        /// </summary>
        public int CellWall { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the map wraps around at the edges.
        /// </summary>
        /// <remarks>
        /// When enabled, the left edge connects to the right edge, and the top edge connects
        /// to the bottom edge, creating a toroidal topology.
        /// </remarks>
        public bool Wrap { get; set; }

        /// <summary>
        /// Creates and initializes a new grid plan with the configured dimensions.
        /// </summary>
        /// <param name="map">The map context to initialize.</param>
        public override void Apply(T map)
        {
            // initialize grid
            var floorPlan = new GridPlan();
            floorPlan.InitSize(this.CellX, this.CellY, this.CellWidth, this.CellHeight, this.CellWall, this.Wrap);

            map.InitGrid(floorPlan);
        }

        public override string ToString()
        {
            return string.Format("{0}: Cells:{1}x{2} CellSize:{3}x{4}", this.GetType().GetFormattedTypeName(), this.CellX, this.CellY, this.CellWidth, this.CellHeight);
        }
    }
}
