// <copyright file="Map.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueElements;

namespace RogueElements.Examples.Ex6_Items
{
    /// <summary>
    /// Map data structure for Example 6 with support for spawned entities.
    /// Extends BaseMap to include collections for items and mobs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unlike tiles (which are stored in a 2D array), spawned entities are stored
    /// in lists. Each entity knows its own location via a Loc property.
    /// </para>
    /// <para>
    /// This separation allows:
    /// - Multiple entities at the same location (if desired)
    /// - Easy iteration over all entities of a type
    /// - Entity properties beyond just position (ID, stats, etc.)
    /// </para>
    /// </remarks>
    public class Map : BaseMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Map"/> class.
        /// Creates empty collections for all entity types.
        /// </summary>
        public Map()
        {
            this.GenEntrances = new List<StairsUp>();
            this.GenExits = new List<StairsDown>();
            this.Items = new List<Item>();
            this.Mobs = new List<Mob>();
        }

        /// <summary>
        /// Gets or sets the list of upward stairs (level entrances).
        /// </summary>
        public List<StairsUp> GenEntrances { get; set; }

        /// <summary>
        /// Gets or sets the list of downward stairs (level exits).
        /// </summary>
        public List<StairsDown> GenExits { get; set; }

        /// <summary>
        /// Gets or sets the list of items spawned on this map.
        /// </summary>
        /// <remarks>
        /// Items are spawned by RandomSpawnStep using the IPlaceableGenContext&lt;Item&gt;
        /// interface. Each Item stores its own position via the Loc property.
        /// </remarks>
        public List<Item> Items { get; set; }

        /// <summary>
        /// Gets or sets the list of mobs (monsters/NPCs) spawned on this map.
        /// </summary>
        /// <remarks>
        /// Mobs use the same spawning system as items but through the
        /// IPlaceableGenContext&lt;Mob&gt; interface.
        /// </remarks>
        public List<Mob> Mobs { get; set; }
    }
}
