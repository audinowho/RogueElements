// <copyright file="IFloorRoomPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Defines the interface for room and hall plans within a <see cref="FloorPlan"/>.
    /// </summary>
    /// <remarks>
    /// This interface extends <see cref="IRoomPlan"/> to add adjacency tracking, which is essential
    /// for floor plan connectivity. Both <see cref="FloorRoomPlan"/> and <see cref="FloorHallPlan"/>
    /// implement this interface.
    /// </remarks>
    public interface IFloorRoomPlan : IRoomPlan
    {
        /// <summary>
        /// Gets the list of rooms and halls that are adjacent to this element.
        /// </summary>
        List<RoomHallIndex> Adjacents { get; }
    }
}
