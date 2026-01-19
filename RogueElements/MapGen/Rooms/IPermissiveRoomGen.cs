// <copyright file="IPermissiveRoomGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RogueElements
{
    /// <summary>
    /// Marker interface for room generators that can accept connections from any border tile.
    /// Permissive rooms have all border tiles marked as fulfillable, allowing maximum flexibility
    /// in hallway connections.
    /// </summary>
    public interface IPermissiveRoomGen : IRoomGen
    {
    }

    /// <summary>
    /// Defines a room generator with configurable width and height ranges.
    /// </summary>
    public interface ISizedRoomGen : IRoomGen
    {
        /// <summary>
        /// Gets or sets the range of possible widths for the room.
        /// </summary>
        RandRange Width { get; set; }

        /// <summary>
        /// Gets or sets the range of possible heights for the room.
        /// </summary>
        RandRange Height { get; set; }
    }
}