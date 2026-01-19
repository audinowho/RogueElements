// <copyright file="InitFloorPlanStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Initializes an empty floor plan for freeform room-based map generation.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This step creates a new <see cref="FloorPlan"/> with the specified dimensions and associates it
    /// with the generation context. Subsequent steps can add rooms, delete them, or modify connections.
    /// </para>
    /// <para>
    /// Once floor plan manipulation is complete, use <see cref="DrawFloorToTileStep{T}"/> to render
    /// the rooms and halls to actual tiles.
    /// </para>
    /// </remarks>
    /// <seealso cref="DrawFloorToTileStep{T}"/>
    /// <seealso cref="FloorPlan"/>
    [Serializable]
    public class InitFloorPlanStep<T> : GenStep<T>
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitFloorPlanStep{T}"/> class with default dimensions.
        /// </summary>
        public InitFloorPlanStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitFloorPlanStep{T}"/> class with specified dimensions.
        /// </summary>
        /// <param name="width">The width of the floor plan in tiles.</param>
        /// <param name="height">The height of the floor plan in tiles.</param>
        public InitFloorPlanStep(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets or sets the width of the floor plan in tiles.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the floor plan in tiles.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the map wraps around its edges (toroidal topology).
        /// </summary>
        public bool Wrap { get; set; }

        /// <summary>
        /// Applies this step by creating and initializing a new floor plan.
        /// </summary>
        /// <param name="map">The generation context to initialize.</param>
        public override void Apply(T map)
        {
            var floorPlan = new FloorPlan();
            floorPlan.InitSize(new Loc(this.Width, this.Height), this.Wrap);

            map.InitPlan(floorPlan);
        }

        /// <summary>
        /// Returns a string representation of this step.
        /// </summary>
        /// <returns>A string describing this step's configuration.</returns>
        public override string ToString()
        {
            return string.Format("{0}: Size:{1}x{2}", this.GetType().GetFormattedTypeName(), this.Width, this.Height);
        }
    }
}
