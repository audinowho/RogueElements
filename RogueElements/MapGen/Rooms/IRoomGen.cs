// <copyright file="IRoomGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Defines the contract for room generation algorithms.
    /// Implementations generate room shapes and manage border connections to adjacent rooms.
    /// </summary>
    public interface IRoomGen
    {
        /// <summary>
        /// Gets the bounding rectangle where the room is drawn on the map.
        /// </summary>
        Rect Draw { get; }

        /// <summary>
        /// Requests that a given range of tiles be fulfilled by this room.
        /// Adds a side requirement and considers all tiles in the range as eligible for fulfillment.
        /// </summary>
        /// <param name="range">The range of tile positions to request, in absolute coordinates.</param>
        /// <param name="dir">The direction from this room toward the range.</param>
        void AskBorderRange(IntRange range, Dir4 dir);

        /// <summary>
        /// Requests that border tiles be fulfilled based on another room's opened borders.
        /// Creates a side requirement using the edge location of the source room.
        /// </summary>
        /// <param name="sourceDraw">The bounding rectangle of the source room.</param>
        /// <param name="borderQuery">A function to query if a specific border tile is open in the source room.</param>
        /// <param name="dir">The direction from this room to the source room.</param>
        void AskBorderFromRoom(Rect sourceDraw, Func<Dir4, int, bool> borderQuery, Dir4 dir);

        /// <summary>
        /// Gets whether a specific border tile has been opened for connection.
        /// </summary>
        /// <param name="dir">The direction of the border.</param>
        /// <param name="index">The index of the tile along the border.</param>
        /// <returns>True if the border tile is opened; otherwise, false.</returns>
        bool GetOpenedBorder(Dir4 dir, int index);

        /// <summary>
        /// Gets whether a specific border tile can potentially be opened for connection.
        /// </summary>
        /// <param name="dir">The direction of the border.</param>
        /// <param name="index">The index of the tile along the border.</param>
        /// <returns>True if the border tile can be opened; otherwise, false.</returns>
        bool GetFulfillableBorder(Dir4 dir, int index);

        /// <summary>
        /// Returns the preferred dimensions for this room.
        /// </summary>
        /// <param name="rand">The random number generator to use.</param>
        /// <returns>The proposed size as a <see cref="Loc"/>.</returns>
        Loc ProposeSize(IRandom rand);

        /// <summary>
        /// Initializes the room with the specified size.
        /// If the proposed size is not used, the room may draw a default empty square.
        /// </summary>
        /// <param name="rand">The random number generator to use.</param>
        /// <param name="size">The size to initialize the room with.</param>
        void PrepareSize(IRandom rand, Loc size);

        /// <summary>
        /// Sets the location where the room will be drawn on the map.
        /// </summary>
        /// <param name="loc">The top-left corner location of the room.</param>
        void SetLoc(Loc loc);

        /// <summary>
        /// Draws the room onto the specified map context.
        /// </summary>
        /// <param name="map">The map context to draw on.</param>
        void DrawOnMap(ITiledGenContext map);

        /// <summary>
        /// Creates a deep copy of this room generator.
        /// </summary>
        /// <returns>A new instance that is a copy of this room generator.</returns>
        IRoomGen Copy();
    }
}