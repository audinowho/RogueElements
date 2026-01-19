// <copyright file="IRoomGridGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Defines a map generation context that supports grid-based room layouts.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface extends <see cref="IFloorPlanGenContext"/> to add support for
    /// grid-based dungeon generation using a <see cref="GridPlan"/>.
    /// </para>
    /// <para>
    /// Implementations must provide access to the current grid plan and a method
    /// to initialize it. The grid plan is typically converted to a floor plan
    /// using <see cref="DrawGridToFloorStep{T}"/> before tile-level generation occurs.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridPlan"/>
    /// <seealso cref="IFloorPlanGenContext"/>
    public interface IRoomGridGenContext : IFloorPlanGenContext
    {
        /// <summary>
        /// Gets the current grid plan for this map context.
        /// </summary>
        GridPlan GridPlan { get; }

        /// <summary>
        /// Initializes the map context with a grid plan.
        /// </summary>
        /// <param name="plan">The grid plan to use for this map.</param>
        void InitGrid(GridPlan plan);
    }
}
