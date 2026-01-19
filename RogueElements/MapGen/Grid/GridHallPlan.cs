// <copyright file="GridHallPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Represents a single hall segment in a <see cref="GridPlan"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A hall connects two adjacent cells in the grid. When rooms are offset from each other,
    /// a single logical hall may be split into multiple <see cref="GridHallPlan"/> segments
    /// during the <see cref="GridPlan.ChooseHallBounds"/> process.
    /// </para>
    /// <para>
    /// Hall plans use <see cref="IPermissiveRoomGen"/> generators because halls must be able
    /// to connect rooms at various positions and sizes.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridHallGroup"/>
    /// <seealso cref="GridPlan"/>
    public class GridHallPlan : IRoomPlan
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridHallPlan"/> class.
        /// </summary>
        /// <param name="roomGen">The permissive room generator for this hall.</param>
        /// <param name="components">The components to attach to this hall.</param>
        public GridHallPlan(IPermissiveRoomGen roomGen, ComponentCollection components)
        {
            this.RoomGen = roomGen;
            this.Components = components;
        }

        /// <summary>
        /// Gets the permissive room generator for this hall.
        /// </summary>
        public IPermissiveRoomGen RoomGen { get; }

        /// <inheritdoc/>
        IRoomGen IRoomPlan.RoomGen => this.RoomGen;

        /// <summary>
        /// Gets the components attached to this hall.
        /// </summary>
        /// <remarks>
        /// This collection is shared by reference with the corresponding <see cref="FloorHallPlan"/>
        /// and any hall segments created when the hall is split during bounds calculation.
        /// </remarks>
        public ComponentCollection Components { get; }
    }
}
