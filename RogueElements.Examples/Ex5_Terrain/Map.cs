// <copyright file="Map.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueElements;

namespace RogueElements.Examples.Ex5_Terrain
{
    /// <summary>
    /// Map data structure for Example 5 with support for terrain features.
    /// Extends BaseMap which already supports multiple terrain types via tile IDs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Terrain in RogueElements is represented by tile IDs:
    /// - WALL_TERRAIN_ID (0): Impassable walls
    /// - ROOM_TERRAIN_ID (1): Walkable floor
    /// - WATER_TERRAIN_ID (2): Special terrain (water, lava, etc.)
    /// </para>
    /// <para>
    /// The terrain system is tile-based, not entity-based. Water tiles replace
    /// floor tiles rather than being placed on top of them. This is different
    /// from items or mobs which are entities placed at locations.
    /// </para>
    /// </remarks>
    public class Map : BaseMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Map"/> class.
        /// </summary>
        public Map()
        {
            this.GenEntrances = new List<StairsUp>();
            this.GenExits = new List<StairsDown>();
        }

        /// <summary>
        /// Gets or sets the list of upward stairs (level entrances).
        /// </summary>
        public List<StairsUp> GenEntrances { get; set; }

        /// <summary>
        /// Gets or sets the list of downward stairs (level exits).
        /// </summary>
        public List<StairsDown> GenExits { get; set; }
    }
}
