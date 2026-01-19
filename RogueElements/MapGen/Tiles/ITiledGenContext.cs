// <copyright file="ITiledGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides context for tile-based map generation operations.
    /// </summary>
    public interface ITiledGenContext : IGenContext
    {
        /// <summary>
        /// Gets the tile type representing walkable room terrain.
        /// </summary>
        ITile RoomTerrain { get; }

        /// <summary>
        /// Gets the tile type representing impassable wall terrain.
        /// </summary>
        ITile WallTerrain { get; }

        /// <summary>
        /// Gets the width of the map in tiles.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the map in tiles.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets a value indicating whether the map wraps around at the edges.
        /// </summary>
        bool Wrap { get; }

        /// <summary>
        /// Gets a value indicating whether the tile array has been initialized.
        /// </summary>
        bool TilesInitialized { get; }

        /// <summary>
        /// Determines whether movement is blocked at the specified location for cardinal directions.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <returns><c>true</c> if the tile blocks movement; otherwise, <c>false</c>.</returns>
        bool TileBlocked(Loc loc);

        /// <summary>
        /// Determines whether movement is blocked at the specified location.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <param name="diagonal">Whether to check for diagonal movement blocking.</param>
        /// <returns><c>true</c> if the tile blocks movement; otherwise, <c>false</c>.</returns>
        bool TileBlocked(Loc loc, bool diagonal);

        /// <summary>
        /// Gets the tile at the specified location.
        /// </summary>
        /// <param name="loc">The location to retrieve the tile from.</param>
        /// <returns>The <see cref="ITile"/> at the specified location.</returns>
        ITile GetTile(Loc loc);

        /// <summary>
        /// Determines whether the specified tile can be placed at the given location.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <param name="tile">The tile to place.</param>
        /// <returns><c>true</c> if the tile can be placed; otherwise, <c>false</c>.</returns>
        bool CanSetTile(Loc loc, ITile tile);

        /// <summary>
        /// Attempts to set the tile at the specified location.
        /// </summary>
        /// <param name="loc">The location to set the tile at.</param>
        /// <param name="tile">The tile to place.</param>
        /// <returns><c>true</c> if the tile was successfully set; otherwise, <c>false</c>.</returns>
        bool TrySetTile(Loc loc, ITile tile);

        /// <summary>
        /// Sets the tile at the specified location.
        /// </summary>
        /// <param name="loc">The location to set the tile at.</param>
        /// <param name="tile">The tile to place.</param>
        void SetTile(Loc loc, ITile tile);

        /// <summary>
        /// Creates a new map with the specified dimensions.
        /// </summary>
        /// <param name="tileWidth">The width of the map in tiles.</param>
        /// <param name="tileHeight">The height of the map in tiles.</param>
        /// <param name="wrap">Whether the map wraps around at the edges.</param>
        void CreateNew(int tileWidth, int tileHeight, bool wrap = false);
    }
}
