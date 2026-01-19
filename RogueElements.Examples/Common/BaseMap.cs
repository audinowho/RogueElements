// <copyright file="BaseMap.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using RogueElements;

namespace RogueElements.Examples
{
    /// <summary>
    /// Abstract base class representing a roguelike dungeon map.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides the fundamental data structure for storing map tiles and
    /// serves as a reference implementation for games using RogueElements. It defines
    /// terrain type constants and manages the 2D tile array that represents the dungeon.
    /// </para>
    /// <para>
    /// Subclasses can extend this to add game-specific data such as entity lists,
    /// lighting information, or additional terrain layers.
    /// </para>
    /// </remarks>
    public abstract class BaseMap
    {
        /// <summary>
        /// Terrain ID representing impassable wall tiles.
        /// </summary>
        /// <remarks>
        /// Walls are the default terrain type when tiles are initialized.
        /// Generation steps carve rooms and hallways by replacing walls with floor tiles.
        /// </remarks>
        public const int WALL_TERRAIN_ID = 0;

        /// <summary>
        /// Terrain ID representing passable floor/room tiles.
        /// </summary>
        /// <remarks>
        /// Floor tiles are walkable areas where entities can be placed.
        /// Rooms and hallways are carved from walls by setting tiles to this ID.
        /// </remarks>
        public const int ROOM_TERRAIN_ID = 1;

        /// <summary>
        /// Terrain ID representing water terrain tiles.
        /// </summary>
        /// <remarks>
        /// Water tiles are typically generated using Perlin noise in terrain generation steps.
        /// Whether water is passable depends on your game's mechanics.
        /// </remarks>
        public const int WATER_TERRAIN_ID = 2;

        /// <summary>
        /// Gets or sets the random number generator used for map generation.
        /// </summary>
        /// <remarks>
        /// This is initialized by <see cref="BaseMapGenContext{TMap}.InitSeed"/> with the
        /// generation seed, ensuring deterministic map generation for the same seed value.
        /// </remarks>
        public ReRandom Rand { get; set; }

        /// <summary>
        /// Gets or sets the 2D array of tiles representing the map.
        /// </summary>
        /// <remarks>
        /// Tiles are stored in column-major order: <c>Tiles[x][y]</c> where x is the
        /// horizontal position and y is the vertical position. Initialize this array
        /// using <see cref="InitializeTiles"/> before generation begins.
        /// </remarks>
        public Tile[][] Tiles { get; set; }

        /// <summary>
        /// Gets the width of the map in tiles.
        /// </summary>
        /// <value>The number of tile columns in the map.</value>
        public int Width => this.Tiles.Length;

        /// <summary>
        /// Gets the height of the map in tiles.
        /// </summary>
        /// <value>The number of tile rows in the map.</value>
        public int Height => this.Tiles[0].Length;

        /// <summary>
        /// Initializes the tile array with the specified dimensions.
        /// </summary>
        /// <param name="width">The width of the map in tiles.</param>
        /// <param name="height">The height of the map in tiles.</param>
        /// <remarks>
        /// All tiles are initialized as wall terrain (<see cref="WALL_TERRAIN_ID"/>).
        /// This is typically called by <see cref="BaseMapGenContext{TMap}.CreateNew"/>
        /// during map generation initialization.
        /// </remarks>
        public void InitializeTiles(int width, int height)
        {
            this.Tiles = new Tile[width][];
            for (int ii = 0; ii < width; ii++)
            {
                this.Tiles[ii] = new Tile[height];
                for (int jj = 0; jj < height; jj++)
                    this.Tiles[ii][jj] = new Tile();
            }
        }
    }
}
