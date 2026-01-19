// <copyright file="FloorRoomPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Represents a room in a <see cref="FloorPlan"/>, storing its generator and connectivity information.
    /// </summary>
    /// <remarks>
    /// FloorRoomPlan stores all the data needed to describe a room within a floor layout:
    /// its shape generator, attached components for filtering/identification, and the list
    /// of adjacent rooms and halls.
    /// </remarks>
    /// <seealso cref="FloorHallPlan"/>
    /// <seealso cref="FloorPlan"/>
    public class FloorRoomPlan : IFloorRoomPlan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloorRoomPlan"/> class.
        /// </summary>
        /// <param name="roomGen">The room generator for this room.</param>
        /// <param name="components">The component collection for filtering and identification.</param>
        public FloorRoomPlan(IRoomGen roomGen, ComponentCollection components)
        {
            this.RoomGen = roomGen;
            this.Components = components;
            this.Adjacents = new List<RoomHallIndex>();
        }

        /// <summary>
        /// Gets or sets the room generator that defines this room's shape and rendering.
        /// </summary>
        public IRoomGen RoomGen { get; set; }

        // TODO: needs a better class.  Only one RoomComponent subclass allowed per collection.  Also better lookup.

        /// <summary>
        /// Gets the component collection for this room, used for filtering and metadata.
        /// </summary>
        public ComponentCollection Components { get; }

        /// <summary>
        /// Gets the list of adjacent rooms and halls connected to this room.
        /// </summary>
        public List<RoomHallIndex> Adjacents { get; }
    }
}
