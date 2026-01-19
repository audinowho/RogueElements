// <copyright file="FloorPlanStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements
{
    /// <summary>
    /// Base class for generation steps that operate on a <see cref="FloorPlan"/>.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// This abstract class provides a bridge between the general <see cref="GenStep{T}"/> pattern and
    /// floor plan-specific operations. Subclasses implement <see cref="ApplyToPath"/> to modify
    /// the floor plan directly.
    /// </remarks>
    [Serializable]
    public abstract class FloorPlanStep<T> : GenStep<T>
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Applies the step's logic to the floor plan.
        /// </summary>
        /// <param name="rand">The random number generator for this operation.</param>
        /// <param name="floorPlan">The floor plan to modify.</param>
        public abstract void ApplyToPath(IRandom rand, FloorPlan floorPlan);

        /// <summary>
        /// Applies this generation step to the map context.
        /// </summary>
        /// <param name="map">The generation context containing the floor plan.</param>
        public override void Apply(T map)
        {
            this.ApplyToPath(map.Rand, map.RoomPlan);
        }
    }
}
