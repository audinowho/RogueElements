// <copyright file="IFloorPlanGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Defines the interface for generation contexts that support freeform floor plan-based room layouts.
    /// </summary>
    /// <remarks>
    /// Implement this interface to enable the use of <see cref="FloorPlanStep{T}"/> and related steps
    /// that manipulate rooms and halls in a non-grid layout. This extends <see cref="ITiledGenContext"/>
    /// to add floor plan management capabilities.
    /// </remarks>
    /// <seealso cref="FloorPlan"/>
    /// <seealso cref="FloorPlanStep{T}"/>
    public interface IFloorPlanGenContext : ITiledGenContext
    {
        /// <summary>
        /// Gets the floor plan associated with this generation context.
        /// </summary>
        FloorPlan RoomPlan { get; }

        /// <summary>
        /// Initializes this context with the specified floor plan.
        /// </summary>
        /// <param name="plan">The floor plan to associate with this context.</param>
        void InitPlan(FloorPlan plan);
    }
}
