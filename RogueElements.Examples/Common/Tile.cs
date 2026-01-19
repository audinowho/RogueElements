// <copyright file="Tile.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples
{
    /// <summary>
    /// Reference implementation of <see cref="ITile"/> representing a single map tile.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This simple tile implementation uses an integer ID to distinguish terrain types.
    /// It serves as a starting point for games using RogueElements. Extend this class
    /// or create your own <see cref="ITile"/> implementation to add game-specific
    /// tile properties such as:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Visual variants or sprite indices</description></item>
    /// <item><description>Movement costs for pathfinding</description></item>
    /// <item><description>Damage or status effects (lava, poison, etc.)</description></item>
    /// <item><description>Light blocking/transmitting properties</description></item>
    /// <item><description>Interactive elements (doors, traps, switches)</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="BaseMap.WALL_TERRAIN_ID"/>
    /// <seealso cref="BaseMap.ROOM_TERRAIN_ID"/>
    /// <seealso cref="BaseMap.WATER_TERRAIN_ID"/>
    public class Tile : ITile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> class with wall terrain.
        /// </summary>
        /// <remarks>
        /// Default tiles are walls, which are then carved into rooms and hallways
        /// during map generation.
        /// </remarks>
        public Tile()
        {
            this.ID = BaseMap.WALL_TERRAIN_ID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> class with the specified terrain ID.
        /// </summary>
        /// <param name="id">
        /// The terrain type ID. Use constants from <see cref="BaseMap"/>:
        /// <see cref="BaseMap.WALL_TERRAIN_ID"/>, <see cref="BaseMap.ROOM_TERRAIN_ID"/>,
        /// or <see cref="BaseMap.WATER_TERRAIN_ID"/>.
        /// </param>
        public Tile(int id)
        {
            this.ID = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> class by copying another tile.
        /// </summary>
        /// <param name="other">The tile to copy.</param>
        /// <remarks>
        /// Used by <see cref="Copy"/> to create independent tile instances.
        /// </remarks>
        protected Tile(Tile other)
        {
            this.ID = other.ID;
        }

        /// <summary>
        /// Gets or sets the terrain type identifier.
        /// </summary>
        /// <value>
        /// An integer identifying the terrain type. Standard values are defined in <see cref="BaseMap"/>.
        /// </value>
        /// <remarks>
        /// Generation steps use this ID to determine tile behavior:
        /// <list type="bullet">
        /// <item><description><see cref="BaseMap.WALL_TERRAIN_ID"/> (0): Impassable wall</description></item>
        /// <item><description><see cref="BaseMap.ROOM_TERRAIN_ID"/> (1): Walkable floor</description></item>
        /// <item><description><see cref="BaseMap.WATER_TERRAIN_ID"/> (2): Water terrain</description></item>
        /// </list>
        /// </remarks>
        public int ID { get; set; }

        /// <summary>
        /// Creates a deep copy of this tile.
        /// </summary>
        /// <returns>A new <see cref="Tile"/> instance with the same <see cref="ID"/>.</returns>
        /// <remarks>
        /// Required by <see cref="ITile"/> interface. RogueElements uses this to copy
        /// template tiles when setting terrain, ensuring each map location has an
        /// independent tile instance.
        /// </remarks>
        public ITile Copy() => new Tile(this);

        /// <summary>
        /// Determines whether this tile is equivalent to another tile.
        /// </summary>
        /// <param name="other">The tile to compare with.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="other"/> is a <see cref="Tile"/> with the same
        /// <see cref="ID"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Used by generation algorithms to compare terrain types without reference equality.
        /// Two tiles with the same ID are considered equivalent regardless of other properties.
        /// </remarks>
        public bool TileEquivalent(ITile other)
        {
            if (!(other is Tile tile))
                return false;
            return tile.ID == this.ID;
        }
    }
}
