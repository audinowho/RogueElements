// <copyright file="DrawGridToFloorStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Converts the grid plan into a floor plan by calculating room and hall bounds.
    /// </summary>
    /// <typeparam name="T">The map context type, which must implement <see cref="IRoomGridGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This step bridges the gap between grid-based layout and tile-based generation.
    /// It creates a new <see cref="FloorPlan"/> and populates it with rooms and halls
    /// from the <see cref="GridPlan"/>.
    /// </para>
    /// <para>
    /// This step should be executed once after the grid plan is fully populated with
    /// rooms and halls. After this step, floor plan-based generation steps can be used
    /// for additional modifications before tile drawing occurs.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridPlan"/>
    /// <seealso cref="FloorPlan"/>
    /// <seealso cref="InitGridPlanStep{T}"/>
    [Serializable]
    public class DrawGridToFloorStep<T> : GenStep<T>
        where T : class, IRoomGridGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawGridToFloorStep{T}"/> class.
        /// </summary>
        public DrawGridToFloorStep()
        {
        }

        /// <summary>
        /// Creates a floor plan from the grid plan and places all rooms and halls.
        /// </summary>
        /// <param name="map">The map context containing the grid plan to convert.</param>
        public override void Apply(T map)
        {
            var floorPlan = new FloorPlan();
            floorPlan.InitSize(map.GridPlan.Size, map.GridPlan.Wrap);
            map.InitPlan(floorPlan);

            map.GridPlan.PlaceRoomsOnFloor(map);
        }

        public override string ToString()
        {
            return string.Format("{0}", this.GetType().GetFormattedTypeName());
        }
    }
}
