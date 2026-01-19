// <copyright file="FloorHallPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Represents a hall in a <see cref="FloorPlan"/>, storing its generator and connectivity information.
    /// </summary>
    /// <remarks>
    /// Halls are similar to rooms but use <see cref="IPermissiveRoomGen"/> which allows more flexible
    /// placement and connection. Halls typically serve as connectors between rooms.
    /// </remarks>
    /// <seealso cref="FloorRoomPlan"/>
    /// <seealso cref="FloorPlan"/>
    public class FloorHallPlan : IFloorRoomPlan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloorHallPlan"/> class.
        /// </summary>
        /// <param name="roomGen">The permissive room generator for this hall.</param>
        /// <param name="components">The component collection for filtering and identification.</param>
        public FloorHallPlan(IPermissiveRoomGen roomGen, ComponentCollection components)
        {
            this.RoomGen = roomGen;
            this.Components = components;
            this.Adjacents = new List<RoomHallIndex>();
        }

        /// <summary>
        /// Gets or sets the permissive room generator that defines this hall's shape and rendering.
        /// </summary>
        public IPermissiveRoomGen RoomGen { get; set; }

        /// <summary>
        /// Gets the room generator as the base <see cref="IRoomGen"/> interface.
        /// </summary>
        IRoomGen IRoomPlan.RoomGen => this.RoomGen;

        /// <summary>
        /// Gets the component collection for this hall, used for filtering and metadata.
        /// </summary>
        public ComponentCollection Components { get; }

        /// <summary>
        /// Gets the list of adjacent rooms and halls connected to this hall.
        /// </summary>
        public List<RoomHallIndex> Adjacents { get; }
    }
}
