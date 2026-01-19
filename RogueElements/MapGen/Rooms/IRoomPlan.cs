// <copyright file="IRoomPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RogueElements
{
    /// <summary>
    /// Defines a plan for a room within a floor layout.
    /// Contains the room generator and associated metadata components.
    /// </summary>
    public interface IRoomPlan
    {
        /// <summary>
        /// Gets the room generator that creates the room's physical structure.
        /// </summary>
        IRoomGen RoomGen { get; }

        /// <summary>
        /// Gets the collection of components that provide metadata and behavior for the room.
        /// </summary>
        ComponentCollection Components { get; }
    }
}