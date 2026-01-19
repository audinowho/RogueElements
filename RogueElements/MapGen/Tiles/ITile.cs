// <copyright file="ITile.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Represents a single tile in the map grid.
    /// </summary>
    public interface ITile
    {
        /// <summary>
        /// Determines whether this tile is equivalent to another tile for comparison purposes.
        /// </summary>
        /// <param name="other">The other tile to compare against.</param>
        /// <returns><c>true</c> if the tiles are equivalent; otherwise, <c>false</c>.</returns>
        bool TileEquivalent(ITile other);

        /// <summary>
        /// Creates a copy of this tile for placement in the generated layout.
        /// </summary>
        /// <returns>A new <see cref="ITile"/> instance that is a copy of this tile.</returns>
        ITile Copy();
    }
}
