// <copyright file="SpecificGridRoomPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Represents a room specification for use with <see cref="GridPathSpecific{T}"/>.
    /// </summary>
    /// <typeparam name="T">The tile context type, which must implement <see cref="ITiledGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This class defines a room with explicit bounds, generator, and properties for
    /// use in handcrafted grid layouts. It is used by <see cref="GridPathSpecific{T}"/>
    /// to place rooms at specific grid locations.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridPathSpecific{T}"/>
    [Serializable]
    public class SpecificGridRoomPlan<T>
        where T : ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificGridRoomPlan{T}"/> class.
        /// </summary>
        /// <param name="bounds">The grid cell rectangle that this room occupies.</param>
        /// <param name="roomGen">The room generator for this room.</param>
        public SpecificGridRoomPlan(Rect bounds, RoomGen<T> roomGen)
        {
            this.Bounds = bounds;
            this.RoomGen = roomGen;
            this.Components = new ComponentCollection();
        }

        /// <summary>
        /// Gets or sets the grid cell rectangle that this room occupies.
        /// </summary>
        public Rect Bounds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this room should be treated as a hall.
        /// </summary>
        public bool PreferHall { get; set; }

        /// <summary>
        /// Gets or sets the room generator for this room.
        /// </summary>
        public RoomGen<T> RoomGen { get; set; }

        /// <summary>
        /// Gets or sets the components attached to this room.
        /// </summary>
        public ComponentCollection Components { get; set; }
    }
}