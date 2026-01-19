// <copyright file="FloorPathStartStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements
{
    /// <summary>
    /// Base class for floor path steps that initialize a floor plan's room layout.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// Path start steps are responsible for creating the initial room layout structure.
    /// They typically populate an empty floor plan with connected rooms following some
    /// algorithmic pattern (branching, grid-based, etc.).
    /// </remarks>
    /// <seealso cref="FloorPathStartStepGeneric{T}"/>
    /// <seealso cref="FloorPathBranch{T}"/>
    [Serializable]
    public abstract class FloorPathStartStep<T> : FloorPlanStep<T>
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Creates a minimal error path when normal path generation fails.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The floor plan to populate with the error path.</param>
        /// <remarks>
        /// This method clears the floor plan and adds a single minimal room, ensuring
        /// that generation can continue even if the main algorithm fails.
        /// </remarks>
        public void CreateErrorPath(IRandom rand, FloorPlan floorPlan)
        {
            floorPlan.Clear();
            RoomGen<T> room = this.GetDefaultGen();
            room.PrepareSize(rand, Loc.One);
            room.SetLoc(Loc.Zero);
            floorPlan.AddRoom(room, new ComponentCollection());
        }

        /// <summary>
        /// Gets the default room generator used for error paths.
        /// </summary>
        /// <returns>A default room generator instance.</returns>
        public virtual RoomGen<T> GetDefaultGen()
        {
            return new RoomGenDefault<T>();
        }
    }
}
