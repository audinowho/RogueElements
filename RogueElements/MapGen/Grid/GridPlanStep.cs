// <copyright file="GridPlanStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements
{
    /// <summary>
    /// Base class for generation steps that operate on a <see cref="GridPlan"/>.
    /// </summary>
    /// <typeparam name="T">The map context type, which must implement <see cref="IRoomGridGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// Grid plan steps modify the grid layout by adding, removing, or modifying rooms and halls.
    /// They operate on the abstract grid structure rather than the actual tile map.
    /// </para>
    /// <para>
    /// Subclasses must implement <see cref="ApplyToPath"/> to define their grid modification logic.
    /// The base <see cref="Apply"/> method automatically extracts the grid plan from the map context.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridPlan"/>
    /// <seealso cref="IRoomGridGenContext"/>
    [Serializable]
    public abstract class GridPlanStep<T> : GenStep<T>
        where T : class, IRoomGridGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridPlanStep{T}"/> class.
        /// </summary>
        protected GridPlanStep()
        {
        }

        /// <summary>
        /// Applies the generation logic to the grid plan.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The grid plan to modify.</param>
        public abstract void ApplyToPath(IRandom rand, GridPlan floorPlan);

        /// <summary>
        /// Applies this generation step to the map by delegating to <see cref="ApplyToPath"/>.
        /// </summary>
        /// <param name="map">The map context containing the grid plan.</param>
        public override void Apply(T map)
        {
            // actual map creation step
            this.ApplyToPath(map.Rand, map.GridPlan);
        }
    }
}
