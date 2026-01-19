// <copyright file="GridRoomPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Represents a room in a <see cref="GridPlan"/>, tracking its cell bounds and generation settings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A grid room plan stores the grid cell bounds (which cells the room occupies),
    /// the room generator, and any attached components. Rooms can span multiple cells
    /// for larger room types.
    /// </para>
    /// <para>
    /// When converted to a <see cref="FloorPlan"/> via <see cref="GridPlan.PlaceRoomsOnFloor"/>,
    /// rooms with <see cref="PreferHall"/> set to true will be added as halls rather than rooms.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridPlan"/>
    /// <seealso cref="FloorRoomPlan"/>
    [Serializable]
    public class GridRoomPlan : IRoomPlan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridRoomPlan"/> class.
        /// </summary>
        /// <param name="bounds">The grid cell rectangle that this room occupies.</param>
        /// <param name="roomGen">The room generator for this room.</param>
        /// <param name="components">The components to attach to this room.</param>
        public GridRoomPlan(Rect bounds, IRoomGen roomGen, ComponentCollection components)
        {
            this.Bounds = bounds;
            this.RoomGen = roomGen;
            this.Components = components;
        }

        /// <summary>
        /// Gets or sets the grid cell rectangle that this room occupies.
        /// </summary>
        public Rect Bounds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this room should be counted as a hall
        /// when translated into floor plan rooms.
        /// </summary>
        /// <remarks>
        /// Hall-preferred rooms are typically single-tile connector rooms or corridors
        /// that serve as passageways rather than destination rooms.
        /// </remarks>
        public bool PreferHall { get; set; }

        /// <summary>
        /// Gets or sets the room generator for this room.
        /// </summary>
        public IRoomGen RoomGen { get; set; }

        /// <summary>
        /// Gets or sets the components attached to this room.
        /// </summary>
        /// <remarks>
        /// This collection is shared by reference with the corresponding <see cref="FloorRoomPlan"/>
        /// when the grid is converted to a floor plan.
        /// </remarks>
        public ComponentCollection Components { get; set; }
    }
}
